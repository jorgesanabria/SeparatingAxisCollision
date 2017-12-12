namespace CircleToCircle
{
    public static class Program
    {
        public static CircleToCircle Core;

        static void Main()
        {
            try
            {
                Core = new CircleToCircle();
                Core.Run();
            }
            finally
            {
                Core.Dispose();
            }
        }
    }
}
