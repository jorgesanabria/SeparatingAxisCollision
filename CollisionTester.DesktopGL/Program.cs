#region using

using System;

#endregion

namespace CollisionTester.DesktopGL {
    public static class Program {
        [STAThread]
        private static void Main() {
            using (CollisionTester game = new CollisionTester()) {
                game.Run();
            }
        }
    }
}
