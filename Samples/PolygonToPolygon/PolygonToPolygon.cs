using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeparatingAxisCollision;
using SeparatingAxisCollision.Broadphase;
using RectangleF = System.Drawing.RectangleF;

namespace PolygonToPolygon
{
    public class PolygonToPolygon : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D WhitePixel;

        SpatialGrid Grid;
        Point[] BoxCells;
        RectangleF[] BoxCellZones;
        Point[] TriangleCells;
        RectangleF[] TriangleCellZones;

        Polygon Box;
        Polygon Triangle;
        Polygon MTVBox;

        RectangleF BoxBounds;
        RectangleF TriangleBounds;

        bool usingBoundingBoxes = true;
        bool broadColliding = false;
        bool colliding = false;
        Vector2 mtv = Vector2.Zero;

        public PolygonToPolygon()
        {
            graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Grid = new SpatialGrid(200, capacity: 1000);
            Box = Polygon.CreateBox(50, 50, pos: new Vector2(150, 150));
            Triangle = new Polygon(new Shape(new Vector2(50, 50), new Vector2(-50, 50), new Vector2(-50, -50)), pos: new Vector2(450, 150));
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
                Triangle.Position = mState.Position.ToVector2();

            if (kState.IsKeyDown(Keys.Q))
                Box.Rotation += (float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.W))
                Box.Rotation += -(float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.A))
                Triangle.Rotation += (float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);
            else if (kState.IsKeyDown(Keys.S))
                Triangle.Rotation += -(float)(MathHelper.PiOver2 * gameTime.ElapsedGameTime.TotalSeconds);

            if (kState.IsKeyDown(Keys.E))
                Box.Scale += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.R))
                Box.Scale += -(float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kState.IsKeyDown(Keys.D))
                Triangle.Scale += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (kState.IsKeyDown(Keys.F))
                Triangle.Scale += -(float)gameTime.ElapsedGameTime.TotalSeconds;

            broadColliding = false;
            colliding = false;

            if (usingBoundingBoxes)
            {
                BoxBounds = Box.GetBoundingBox();
                TriangleBounds = Triangle.GetBoundingBox();
            }
            else
            {
                BoxBounds = Box.GetBoundingSquare();
                TriangleBounds = Triangle.GetBoundingSquare();
            }

            BoxCells = Grid.GetCells(BoxBounds);
            BoxCellZones = Grid.GetCellZones(BoxCells);
            TriangleCells = Grid.GetCells(TriangleBounds);
            TriangleCellZones = Grid.GetCellZones(TriangleCells);

            if (BoxBounds.CollidesWith(TriangleBounds))
                broadColliding = true;

            if ((mtv = Box.CheckCollisionAndRespond(Triangle)) != Vector2.Zero)
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
            Box.Draw(WhitePixel, spriteBatch);
            Triangle.Draw(WhitePixel, spriteBatch);

            BoxBounds.Draw(WhitePixel, spriteBatch, Color.DarkSlateGray);
            TriangleBounds.Draw(WhitePixel, spriteBatch, Color.DarkSlateGray);

            for (int a = 0; a < BoxCellZones.Length; a++)
                BoxCellZones[a].Draw(WhitePixel, spriteBatch, Color.Pink * 0.5f);

            for (int b = 0; b < TriangleCellZones.Length; b++)
                TriangleCellZones[b].Draw(WhitePixel, spriteBatch, Color.Purple * 0.5f);

            if (colliding)
                MTVBox.Draw(WhitePixel, spriteBatch, Color.Red);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
