#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeparatingAxisCollision;
using Color = Microsoft.Xna.Framework.Color;

#endregion

namespace PolygonToPolygon {
    public class PolygonToPolygon : Game {
        private Polygon _box;

        private RectangleF _boxBounds;
        private Boolean _broadColliding;
        private Boolean _colliding;
        
        private Vector2 _mtv = Vector2.Zero;
        private Polygon _mtvBox;
        private SpriteBatch _spriteBatch;
        private Polygon _triangle;
        private RectangleF _triangleBounds;

        private Boolean _usingBoundingBoxes = true;

        private Texture2D _whitePixel;

        public PolygonToPolygon() {
            new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            _box = Polygon.CreateBox(50, 50, pos: new Vector2(150, 150));
            _triangle = new Polygon(new Shape(new Vector2(50, 50), new Vector2(-50, 50), new Vector2(-50, -50)),
                pos: new Vector2(450, 150));
            _mtvBox = Polygon.CreateBox(50, 50, pos: new Vector2(150, 150));
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
                _box.Position = mState.Position.ToVector2();
            else if (mState.RightButton == ButtonState.Pressed)
                _triangle.Position = mState.Position.ToVector2();

            if (kState.IsKeyDown(Keys.Q))
                _box.Rotation += (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.W))
                _box.Rotation += -(Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.A))
                _triangle.Rotation += (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.S))
                _triangle.Rotation += -(Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.E))
                _box.Scale += (Single)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.R))
                _box.Scale += -(Single)gameTime.ElapsedGameTime.TotalSeconds;

            if (kState.IsKeyDown(Keys.D))
                _triangle.Scale += (Single)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.F))
                _triangle.Scale += -(Single)gameTime.ElapsedGameTime.TotalSeconds;

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
                _mtvBox.Position = _box.Position + _mtv;
                _mtvBox.Rotation = _box.Rotation;
                _mtvBox.Scale = _box.Scale;
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
