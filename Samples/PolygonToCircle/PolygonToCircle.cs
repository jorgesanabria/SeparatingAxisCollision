using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeparatingAxisCollision;
using RectangleF = System.Drawing.RectangleF;

namespace PolygonToCircle
{
    public class PolygonToCircle : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D WhitePixel;
        
        Polygon Box;
        DrawableCircle Circle;
        Polygon MTVBox;

        RectangleF BoxBounds;
        RectangleF CircleBounds;

        bool usingBoundingBoxes = true;
        bool broadColliding = false;
        bool colliding = false;
        Vector2 mtv = Vector2.Zero;

        public PolygonToCircle()
        {
            graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Box = Polygon.CreateBox(50, 50, pos: new Vector2(150, 150));
            Circle = new DrawableCircle(35, new Vector2(0, 0), pos: new Vector2(450, 150));
            MTVBox = Polygon.CreateBox(50, 50, pos: new Vector2(150, 150));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            WhitePixel = Utils.GeneratePixel(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            var mState = Mouse.GetState();
            var kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.Z))
                usingBoundingBoxes = true;
            else if (kState.IsKeyDown(Keys.X))
                usingBoundingBoxes = false;

            if (mState.LeftButton == ButtonState.Pressed)
                Box.Position = mState.Position.ToVector2();
            else if (mState.RightButton == ButtonState.Pressed)
                Circle.Position = mState.Position.ToVector2();

            if (kState.IsKeyDown(Keys.Q))
                Box.Rotation += (float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.W))
                Box.Rotation += -(float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.A))
                Circle.Rotation += (float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.S))
                Circle.Rotation += -(float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.E))
                Box.Scale += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.R))
                Box.Scale += -(float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kState.IsKeyDown(Keys.D))
                Circle.Scale += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.F))
                Circle.Scale += -(float)gameTime.ElapsedGameTime.TotalSeconds;

            broadColliding = false;
            colliding = false;

            if (usingBoundingBoxes)
            {
                BoxBounds = Box.GetBoundingBox();
                CircleBounds = Circle.GetBoundingBox();
            }
            else
            {
                BoxBounds = Box.GetBoundingSquare();
                CircleBounds = Circle.GetBoundingSquare();
            }

            if (BoxBounds.CollidesWith(CircleBounds))
                broadColliding = true;

            if ((mtv = Box.CheckCollisionAndRespond(Circle)) != Vector2.Zero)
            {
                colliding = true;
                MTVBox.Position = Box.Position + mtv;
                MTVBox.Rotation = Box.Rotation;
                MTVBox.Scale = Box.Scale;
            }

            if (colliding && !broadColliding)
                throw new System.Exception("Broadphase did not collide when SAT did!");

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (colliding)
                GraphicsDevice.Clear(Color.LightGreen);
            else if (broadColliding)
                GraphicsDevice.Clear(Color.LightYellow);
            else
                GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            Circle.Draw(GraphicsDevice, spriteBatch);
            Box.Draw(WhitePixel, spriteBatch);

            BoxBounds.Draw(WhitePixel, spriteBatch, Color.DarkSlateGray);
            CircleBounds.Draw(WhitePixel, spriteBatch, Color.DarkSlateGray);

            if (colliding)
                MTVBox.Draw(WhitePixel, spriteBatch, Color.Red);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
