#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeparatingAxisCollision;
using Color = Microsoft.Xna.Framework.Color;

#endregion

namespace CircleToPolygon {
    public class CircleToPolygon : Game {
        private Boolean _broadColliding;

        private DrawableCircle _circle;

        private RectangleF _circleBounds;
        private Boolean _colliding;
        private Vector2 _mtv = Vector2.Zero;
        private DrawableCircle _mtvCircle;
        private SpriteBatch _spriteBatch;
        private Polygon _triangle;
        private RectangleF _triangleBounds;

        private Boolean _usingBoundingBoxes = true;

        private Texture2D _whitePixel;

        public CircleToPolygon() {
            new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            _circle = new DrawableCircle(50, new Vector2(0, 0), pos: new Vector2(150, 150));
            _triangle = new Polygon(new Shape(new Vector2(50, 50), new Vector2(-50, 50), new Vector2(-50, -50)),
                pos: new Vector2(450, 150));
            _mtvCircle = new DrawableCircle(50, new Vector2(0, 0), pos: new Vector2(150, 150));
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
                _circle.Position = mState.Position.ToVector2();
            else if (mState.RightButton == ButtonState.Pressed)
                _triangle.Position = mState.Position.ToVector2();

            if (kState.IsKeyDown(Keys.Q))
                _circle.Rotation += (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.W))
                _circle.Rotation += -(Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.A))
                _triangle.Rotation += (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.S))
                _triangle.Rotation += -(Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.E))
                _circle.Scale += (Single)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.R))
                _circle.Scale += -(Single)gameTime.ElapsedGameTime.TotalSeconds;

            if (kState.IsKeyDown(Keys.D))
                _triangle.Scale += (Single)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.F))
                _triangle.Scale += -(Single)gameTime.ElapsedGameTime.TotalSeconds;

            _broadColliding = false;
            _colliding = false;

            if (_usingBoundingBoxes) {
                _circleBounds = _circle.GetBoundingBox();
                _triangleBounds = _triangle.GetBoundingBox();
            } else {
                _circleBounds = _circle.GetBoundingSquare();
                _triangleBounds = _triangle.GetBoundingSquare();
            }

            if (_circleBounds.CollidesWith(_triangleBounds))
                _broadColliding = true;

            if ((_mtv = _circle.CheckCollisionAndRespond(_triangle)) != Vector2.Zero) {
                _colliding = true;
                _mtvCircle.Position = _circle.Position + _mtv;
                _mtvCircle.Rotation = _circle.Rotation;
                _mtvCircle.Scale = _circle.Scale;
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
            _triangle.Draw(_whitePixel, _spriteBatch);

            _circleBounds.Draw(_whitePixel, _spriteBatch, Color.DarkSlateGray);
            _triangleBounds.Draw(_whitePixel, _spriteBatch, Color.DarkSlateGray);

            if (_colliding)
                _mtvCircle.Draw(GraphicsDevice, _spriteBatch, Color.Red);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
