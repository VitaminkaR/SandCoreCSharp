using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace SandCoreCSharp.Utils
{
    static public class Logger
    {
        [DllImport("user32.dll")]
        public static extern int MessageBox(int hWnd, string text, string caption, uint type);

        // изменяет заглавление приложения
        public static void LogTitle(string text, Game game) => game.Window.Title = text;
    }
}
