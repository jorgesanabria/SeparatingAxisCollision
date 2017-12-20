#region using

using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeparatingAxisCollision;
using SeparatingAxisCollision.polygons;
using Color = Microsoft.Xna.Framework.Color;

#endregion

namespace CircleToCircle {
    public class CircleToCircle : Game {
        private Boolean _broadColliding;

        private DrawableCircle _circle;

        private RectangleF _circleBounds;
        private Boolean _colliding;
        private Vector2 _mtv = Vector2.Zero;
        private DrawableCircle _mtvCircle;
        private DrawableCircle _otherCircle;
        private RectangleF _otherCircleBounds;
        private SpriteBatch _spriteBatch;

        private Boolean _usingBoundingBoxes = true;

        private Texture2D _whitePixel;

        public CircleToCircle() {
            new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            _circle = new DrawableCircle(25, new Vector2(0, 0), pos: new Vector2(150, 150));
            _otherCircle = new DrawableCircle(30, new Vector2(15, 0), pos: new Vector2(450, 150));
            _mtvCircle = new DrawableCircle(25, new Vector2(0, 0), pos: new Vector2(150, 150));
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
                _circle.SetPosition(mState.Position.ToVector2());
            else if (mState.RightButton == ButtonState.Pressed)
                _otherCircle.SetPosition(mState.Position.ToVector2());

            if (kState.IsKeyDown(Keys.Q))
                _circle.SetRotation(_circle.GetRotation() + (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds));
            else if (kState.IsKeyDown(Keys.W))
                _circle.SetRotation(_circle.GetRotation() - (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds));

            if (kState.IsKeyDown(Keys.A))
                _otherCircle.SetRotation(_circle.GetRotation() + (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds));
            else if (kState.IsKeyDown(Keys.S))
                _otherCircle.SetRotation(_circle.GetRotation() - (Single)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds));

            if (kState.IsKeyDown(Keys.E))
                _circle.SetScale(_circle.GetScale() + (Single)gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.R))
                _circle.SetScale(_circle.GetScale() - (Single)gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.D))
                _otherCircle.SetScale(_circle.GetScale() + (Single)gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.F))
                _otherCircle.SetScale(_circle.GetScale() - (Single)gameTime.ElapsedGameTime.TotalSeconds);

            _broadColliding = false;
            _colliding = false;

            if (_usingBoundingBoxes) {
                _circleBounds = _circle.GetBoundingBox();
                _otherCircleBounds = _otherCircle.GetBoundingBox();
            } else {
                _circleBounds = _circle.GetBoundingSquare();
                _otherCircleBounds = _otherCircle.GetBoundingSquare();
            }

            if (_circleBounds.CollidesWith(_otherCircleBounds))
                _broadColliding = true;

            if ((_mtv = Collision.CheckCollisionAndRespond(_circle, _otherCircle)) != Vector2.Zero) {
                _colliding = true;
                _mtvCircle.SetPosition(_circle.GetPosition() + _mtv);
                _mtvCircle.SetRotation(_circle.GetRotation());
                _mtvCircle.SetScale(_circle.GetScale());
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
            _otherCircle.Draw(GraphicsDevice, _spriteBatch);

            _circleBounds.Draw(_whitePixel, _spriteBatch, Color.DarkSlateGray);
            _otherCircleBounds.Draw(_whitePixel, _spriteBatch, Color.DarkSlateGray);

            if (_colliding)
                _mtvCircle.Draw(GraphicsDevice, _spriteBatch, Color.Red);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
