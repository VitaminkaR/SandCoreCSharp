using System;

namespace SandCoreCSharp
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SandCore())
                game.Run();
        }
    }
}
