#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeparatingAxisCollision;
using Color = Microsoft.Xna.Framework.Color;

#endregion

namespace CollisionTester.DesktopGL {
    public class CollisionTester : Game {
        private RectangleF _alphaBounds;
        private PolyDrawer _alphaDrawer;

        // Colliders
        private Byte _alphaPolyType;
        private PolyDrawer _alphaProjectionDrawer;
        private RectangleF _betaBounds;
        private PolyDrawer _betaDrawer;

        private Byte _betaPolyType = 1;

        // Collision Data
        private Boolean _broadphaseColliding; // If the bounds collide, this is true.
        private Boolean _colliding; // If the actual polygons collide, this is true.
        // Graphics stuff
        private GraphicsDeviceManager _graphics;
        // Else, use traditional bounding boxes.
        private KeyboardState _lastKeyState;
        private Vector2 _minimumTranslationVector = Vector2.Zero
            ; // The min. translation needed to move Alpha from Beta. Is Zero if they do not collide.
        private Texture2D _pixel;
        private IPolygon _polygonAlpha;

        private IPolygon _polygonAlphaProjection;
        private IPolygon _polygonBeta;
        private SpriteBatch _spriteBatch;

        // Settings
        public Boolean UsesBoundingSquares
            ; // Bounding Squares are rotation-agnostic; they will cover all rotations of a polygon.

        public CollisionTester() {
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] {Color.White});
            _polygonAlpha = PolyTypes.CreateDefaultPolygon(_alphaPolyType);
            _polygonBeta = PolyTypes.CreateDefaultPolygon(_betaPolyType);
            _polygonAlphaProjection = PolyTypes.CreateDefaultPolygon(_alphaPolyType);

            _polygonAlpha.SetPosition(new Vector2(250, 250));
            _polygonBeta.SetPosition(new Vector2(625, 250));

            _polygonAlpha.SetScale(200);
            _polygonBeta.SetScale(200);

            _alphaDrawer = new PolyDrawer(GraphicsDevice, _spriteBatch, _pixel, _polygonAlpha);
            _betaDrawer = new PolyDrawer(GraphicsDevice, _spriteBatch, _pixel, _polygonBeta);
            _alphaProjectionDrawer = new PolyDrawer(GraphicsDevice, _spriteBatch, _pixel, _polygonAlphaProjection);

            _lastKeyState = Keyboard.GetState();
        }

        protected override void Update(GameTime gameTime) {
            MouseState mState = Mouse.GetState();
            KeyboardState kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.C) && _lastKeyState.IsKeyUp(Keys.C)) {
                _alphaPolyType++;
                if (_alphaPolyType == 4)
                    _alphaPolyType = 0;
                _polygonAlpha = PolyTypes.CreateDefaultPolygon(_alphaPolyType, pos: _polygonAlpha.GetPosition(),
                    rotation: _polygonAlpha.GetRotation(), scale: _polygonAlpha.GetScale());
                _alphaDrawer = new PolyDrawer(GraphicsDevice, _spriteBatch, _pixel, _polygonAlpha);
                _polygonAlphaProjection = PolyTypes.CreateDefaultPolygon(_alphaPolyType,
                    pos: _polygonAlpha.GetPosition(), rotation: _polygonAlpha.GetRotation(),
                    scale: _polygonAlpha.GetScale());
                _alphaProjectionDrawer = new PolyDrawer(GraphicsDevice, _spriteBatch, _pixel, _polygonAlphaProjection);
            }

            if (kState.IsKeyDown(Keys.V) && _lastKeyState.IsKeyUp(Keys.V)) {
                _betaPolyType++;
                if (_betaPolyType == 4)
                    _betaPolyType = 0;
                _polygonBeta = PolyTypes.CreateDefaultPolygon(_betaPolyType, pos: _polygonBeta.GetPosition(),
                    rotation: _polygonBeta.GetRotation(), scale: _polygonBeta.GetScale());
                _betaDrawer = new PolyDrawer(GraphicsDevice, _spriteBatch, _pixel, _polygonBeta);
            }

            if (kState.IsKeyDown(Keys.Z))
                UsesBoundingSquares = false;
            else if (kState.IsKeyDown(Keys.X))
                UsesBoundingSquares = true;

            if (mState.LeftButton == ButtonState.Pressed)
                _polygonAlpha.SetPosition(mState.Position.ToVector2());
            else if (mState.RightButton == ButtonState.Pressed)
                _polygonBeta.SetPosition(mState.Position.ToVector2());

            if (kState.IsKeyDown(Keys.Q))
                _polygonAlpha.SetRotation(_polygonAlpha.GetRotation()
                                          + (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds));
            else if (kState.IsKeyDown(Keys.W))
                _polygonAlpha.SetRotation(_polygonAlpha.GetRotation()
                                          - (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds));

            if (kState.IsKeyDown(Keys.A))
                _polygonBeta.SetRotation(_polygonBeta.GetRotation()
                                         + (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds));
            else if (kState.IsKeyDown(Keys.S))
                _polygonBeta.SetRotation(_polygonBeta.GetRotation()
                                         - (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds));

            if (kState.IsKeyDown(Keys.E))
                _polygonAlpha.SetScale(_polygonAlpha.GetScale() + (Single)gameTime.ElapsedGameTime.TotalSeconds * 100);
            else if (kState.IsKeyDown(Keys.R))
                _polygonAlpha.SetScale(_polygonAlpha.GetScale() - (Single)gameTime.ElapsedGameTime.TotalSeconds * 100);

            if (kState.IsKeyDown(Keys.D))
                _polygonBeta.SetScale(_polygonBeta.GetScale() + (Single)gameTime.ElapsedGameTime.TotalSeconds * 100);
            else if (kState.IsKeyDown(Keys.F))
                _polygonBeta.SetScale(_polygonBeta.GetScale() - (Single)gameTime.ElapsedGameTime.TotalSeconds * 100);

            _broadphaseColliding = false;
            _colliding = false;

            if (!UsesBoundingSquares) {
                _alphaBounds = _polygonAlpha.GetBoundingBox();
                _betaBounds = _polygonBeta.GetBoundingBox();
            } else {
                _alphaBounds = _polygonAlpha.GetBoundingSquare();
                _betaBounds = _polygonBeta.GetBoundingSquare();
            }

            if (_alphaBounds.CollidesWith(_betaBounds))
                _broadphaseColliding = true;

            if ((_minimumTranslationVector = Collision.CheckCollisionAndRespond(_polygonAlpha, _polygonBeta))
                != Vector2.Zero) {
                _colliding = true;
                _polygonAlphaProjection.SetPosition(_polygonAlpha.GetPosition() + _minimumTranslationVector);
                _polygonAlphaProjection.SetRotation(_polygonAlpha.GetRotation());
                _polygonAlphaProjection.SetScale(_polygonAlpha.GetScale());
            }

            if (_colliding && !_broadphaseColliding)
                throw new Exception("Broadphase did not collide when SAT did!");

            _lastKeyState = kState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            if (_colliding)
                GraphicsDevice.Clear(Color.LightGreen);
            else if (_broadphaseColliding)
                GraphicsDevice.Clear(Color.LightYellow);
            else
                GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();

            _alphaDrawer.Draw();
            _betaDrawer.Draw();

            PolyDrawer.Draw(_alphaBounds, _pixel, _spriteBatch, Color.DarkSlateGray);
            PolyDrawer.Draw(_betaBounds, _pixel, _spriteBatch, Color.DarkSlateGray);

            if (_colliding)
                _alphaProjectionDrawer.Draw(Color.Red);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
