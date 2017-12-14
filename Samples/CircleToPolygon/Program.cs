namespace CircleToPolygon {
    public static class Program {
        public static CircleToPolygon Core;

        private static void Main() {
            try {
                Core = new CircleToPolygon();
                Core.Run();
            }
            finally {
                Core.Dispose();
            }
        }
    }
}
