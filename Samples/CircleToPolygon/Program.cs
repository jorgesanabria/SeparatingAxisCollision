namespace CircleToPolygon
{
    public static class Program
    {
        public static CircleToPolygon Core;

        static void Main()
        {
            try
            {
                Core = new CircleToPolygon();
                Core.Run();
            }
            finally
            {
                Core.Dispose();
            }
        }
    }
}
