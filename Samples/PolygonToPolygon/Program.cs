namespace PolygonToPolygon {
    public static class Program {
        public static PolygonToPolygon Core;

        private static void Main() {
            try {
                Core = new PolygonToPolygon();
                Core.Run();
            }
            finally {
                Core.Dispose();
            }
        }
    }
}
