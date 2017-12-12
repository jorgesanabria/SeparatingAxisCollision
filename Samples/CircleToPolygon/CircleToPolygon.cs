using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeparatingAxisCollision;
using RectangleF = System.Drawing.RectangleF;

namespace CircleToPolygon
{
    public class CircleToPolygon : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D WhitePixel;

        DrawableCircle Circle;
        Polygon Triangle;
        DrawableCircle MTVCircle;

        RectangleF CircleBounds;
        RectangleF TriangleBounds;

        bool usingBoundingBoxes = true;
        bool broadColliding = false;
        bool colliding = false;
        Vector2 mtv = Vector2.Zero;

        public CircleToPolygon()
        {
            graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Circle = new DrawableCircle(50, new Vector2(0, 0), pos: new Vector2(150, 150));
            Triangle = new Polygon(new Shape(new Vector2(50, 50), new Vector2(-50, 50), new Vector2(-50, -50)), pos: new Vector2(450, 150));
            MTVCircle = new DrawableCircle(50, new Vector2(0, 0), pos: new Vector2(150, 150));
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
                Circle.Position = mState.Position.ToVector2();
            else if (mState.RightButton == ButtonState.Pressed)
                Triangle.Position = mState.Position.ToVector2();

            if (kState.IsKeyDown(Keys.Q))
                Circle.Rotation += (float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.W))
                Circle.Rotation += -(float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.A))
                Triangle.Rotation += (float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.S))
                Triangle.Rotation += -(float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.E))
                Circle.Scale += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.R))
                Circle.Scale += -(float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kState.IsKeyDown(Keys.D))
                Triangle.Scale += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.F))
                Triangle.Scale += -(float)gameTime.ElapsedGameTime.TotalSeconds;

            broadColliding = false;
            colliding = false;

            if (usingBoundingBoxes)
            {
                CircleBounds = Circle.GetBoundingBox();
                TriangleBounds = Triangle.GetBoundingBox();
            }
            else
            {
                CircleBounds = Circle.GetBoundingSquare();
                TriangleBounds = Triangle.GetBoundingSquare();
            }

            if (CircleBounds.CollidesWith(TriangleBounds))
                broadColliding = true;

            if ((mtv = Circle.CheckCollisionAndRespond(Triangle)) != Vector2.Zero)
            {
                colliding = true;
                MTVCircle.Position = Circle.Position + mtv;
                MTVCircle.Rotation = Circle.Rotation;
                MTVCircle.Scale = Circle.Scale;
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
            Triangle.Draw(WhitePixel, spriteBatch);

            CircleBounds.Draw(WhitePixel, spriteBatch, Color.DarkSlateGray);
            TriangleBounds.Draw(WhitePixel, spriteBatch, Color.DarkSlateGray);

            if (colliding)
                MTVCircle.Draw(GraphicsDevice, spriteBatch, Color.Red);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
