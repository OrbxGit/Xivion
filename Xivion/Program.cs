using System;
using System.IO;
using System.Windows;

namespace Xivion
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length > 0)
                throw new ArgumentException("This program does not expect any args!");

            string[] dirs = { "Workspace", "AutoExec", "Scripts", "Bin" };

            for (int i = 0; i < dirs.Length; i++)
                if (!Directory.Exists(dirs[i])) Directory.CreateDirectory(dirs[i]);

            var _ = new Application();
            Application.Current.Run(new MainWindow());
        }
    }
}
