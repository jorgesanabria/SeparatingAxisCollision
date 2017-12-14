namespace PolygonToCircle {
    public static class Program {
        public static PolygonToCircle Core;

        private static void Main() {
            try {
                Core = new PolygonToCircle();
                Core.Run();
            }
            finally {
                Core.Dispose();
            }
        }
    }
}
