using Launcher.Ex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Launcher
{
    public static class Launcher
    {
        internal const string AppName = "RedirectToDefaultBrowser";

        [STAThread()]
        public static void Main(string[] args)
        {
            var showWin = true;
            try
            {
                var task = MainTask(args);
                task.Wait();
                showWin = task.Result;
            }
            catch (Exception ex)
            {
                ShowErrorMsg(ex.Message);
            }
            if (showWin)
            {
                var win = new MainWindow();
                win.ShowDialog();
            }
        }

        public static async Task<bool> MainTask(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    return true;
                case 1:
                    Properties.Settings.Default.LastURL = args[0];
                    Properties.Settings.Default.Save();
                    await Opener.Open(args[0]);
                    return false;
                default:
                    throw new TooManyArgException();
            }
        }

        public static void ShowErrorMsg(string message)
        {
            MessageBox.Show(message, AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
