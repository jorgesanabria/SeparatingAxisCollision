#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeparatingAxisCollision;
using SeparatingAxisCollision.polygons;
using Color = Microsoft.Xna.Framework.Color;
using FreeConvex = SeparatingAxisCollision.polygons.FreeConvex;

#endregion

namespace PolygonToCircle {
    public class PolygonToCircle : Game {
        private FreeConvex _box;

        private RectangleF _boxBounds;
        private Boolean _broadColliding;
        private DrawableCircle _circle;
        private RectangleF _circleBounds;
        private Boolean _colliding;
        private Vector2 _mtv = Vector2.Zero;
        private FreeConvex _mtvBox;
        private SpriteBatch _spriteBatch;

        private Boolean _usingBoundingBoxes = true;

        private Texture2D _whitePixel;

        public PolygonToCircle() {
            new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            _box = FreeConvex.CreateBox(50, 50, pos: new Vector2(150, 150));
            _circle = new DrawableCircle(35, new Vector2(0, 0), pos: new Vector2(450, 150));
            _mtvBox = FreeConvex.CreateBox(50, 50, pos: new Vector2(150, 150));
            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _whitePixel = Utils.GeneratePixel(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime) {
            MouseState mState = Mouse.GetState();
            KeyboardState kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.Z))
                _usingBoundingBoxes = true;
            else if (kState.IsKeyDown(Keys.X))
                _usingBoundingBoxes = false;

            if (mState.LeftButton == ButtonState.Pressed)
                _box._position = mState.Position.ToVector2();
            else if (mState.RightButton == ButtonState.Pressed)
                _circle._position = mState.Position.ToVector2();

            if (kState.IsKeyDown(Keys.Q))
                _box.Rotation += (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.W))
                _box.Rotation += -(Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.A))
                _circle.Rotation += (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.S))
                _circle.Rotation += -(Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.E))
                _box._scale += (Single)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.R))
                _box._scale += -(Single)gameTime.ElapsedGameTime.TotalSeconds;

            if (kState.IsKeyDown(Keys.D))
                _circle.Scale += (Single)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.F))
                _circle.Scale += -(Single)gameTime.ElapsedGameTime.TotalSeconds;

            _broadColliding = false;
            _colliding = false;

            if (_usingBoundingBoxes) {
                _boxBounds = _box.GetBoundingBox();
                _circleBounds = _circle.GetBoundingBox();
            } else {
                _boxBounds = _box.GetBoundingSquare();
                _circleBounds = _circle.GetBoundingSquare();
            }

            if (_boxBounds.CollidesWith(_circleBounds))
                _broadColliding = true;

            if ((_mtv = _box.CheckCollisionAndRespond(_circle)) != Vector2.Zero) {
                _colliding = true;
                _mtvBox._position = _box._position + _mtv;
                _mtvBox.Rotation = _box.Rotation;
                _mtvBox._scale = _box._scale;
            }

            if (_colliding && !_broadColliding)
                throw new Exception("Broadphase did not collide when SAT did!");

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            if (_colliding)
                GraphicsDevice.Clear(Color.LightGreen);
            else if (_broadColliding)
                GraphicsDevice.Clear(Color.LightYellow);
            else
                GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();
            _circle.Draw(GraphicsDevice, _spriteBatch);
            _box.Draw(_whitePixel, _spriteBatch);

            _boxBounds.Draw(_whitePixel, _spriteBatch, Color.DarkSlateGray);
            _circleBounds.Draw(_whitePixel, _spriteBatch, Color.DarkSlateGray);

            if (_colliding)
                _mtvBox.Draw(_whitePixel, _spriteBatch, Color.Red);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
