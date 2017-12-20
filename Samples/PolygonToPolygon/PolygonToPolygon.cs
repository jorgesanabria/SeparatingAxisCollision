#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeparatingAxisCollision;
using Color = Microsoft.Xna.Framework.Color;
using FreeConvex = SeparatingAxisCollision.polygons.FreeConvex;

#endregion

namespace PolygonToPolygon {
    public class PolygonToPolygon : Game {
        private FreeConvex _box;

        private RectangleF _boxBounds;
        private Boolean _broadColliding;
        private Boolean _colliding;
        
        private Vector2 _mtv = Vector2.Zero;
        private FreeConvex _mtvBox;
        private SpriteBatch _spriteBatch;
        private FreeConvex _triangle;
        private RectangleF _triangleBounds;

        private Boolean _usingBoundingBoxes = true;

        private Texture2D _whitePixel;

        public PolygonToPolygon() {
            new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            _box = FreeConvex.CreateBox(50, 50, pos: new Vector2(150, 150));
            _triangle = new FreeConvex(new Shape(new Vector2(50, 50), new Vector2(-50, 50), new Vector2(-50, -50)),
                pos: new Vector2(450, 150));
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
                _triangle._position = mState.Position.ToVector2();

            if (kState.IsKeyDown(Keys.Q))
                _box.Rotation += (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.W))
                _box.Rotation += -(Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.A))
                _triangle.Rotation += (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.S))
                _triangle.Rotation += -(Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.E))
                _box._scale += (Single)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.R))
                _box._scale += -(Single)gameTime.ElapsedGameTime.TotalSeconds;

            if (kState.IsKeyDown(Keys.D))
                _triangle._scale += (Single)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.F))
                _triangle._scale += -(Single)gameTime.ElapsedGameTime.TotalSeconds;

            _broadColliding = false;
            _colliding = false;

            if (_usingBoundingBoxes) {
                _boxBounds = _box.GetBoundingBox();
                _triangleBounds = _triangle.GetBoundingBox();
            } else {
                _boxBounds = _box.GetBoundingSquare();
                _triangleBounds = _triangle.GetBoundingSquare();
            }
            
            if (_boxBounds.CollidesWith(_triangleBounds))
                _broadColliding = true;

            if ((_mtv = _box.CheckCollisionAndRespond(_triangle)) != Vector2.Zero) {
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
            _box.Draw(_whitePixel, _spriteBatch);
            _triangle.Draw(_whitePixel, _spriteBatch);

            _boxBounds.Draw(_whitePixel, _spriteBatch, Color.DarkSlateGray);
            _triangleBounds.Draw(_whitePixel, _spriteBatch, Color.DarkSlateGray);

            if (_colliding)
                _mtvBox.Draw(_whitePixel, _spriteBatch, Color.Red);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
