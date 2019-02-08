using Launcher.Ex;
using System;
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
            //System.Globalization.CultureInfo.CurrentCulture =
            //    System.Globalization.CultureInfo.CurrentUICulture =
            //    new System.Globalization.CultureInfo("en-us");
            var showWin = true;
            try
            {
                var task = MainTask(args);
                task.Wait();
                showWin = task.Result;
            }
            catch (Exception ex)
            {
                ShowErrorMsg(ex);
            }
            if (showWin)
            {
                var win = new MainWindow();
                win.ShowDialog();
                Config.Save();
            }
        }

        public static async Task<bool> MainTask(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    return true;
                case 1:
                    Config.LastUrl = args[0];
                    Config.Save();
                    await Opener.Open(args[0]);
                    return false;
                default:
                    throw new TooManyArgException();
            }
        }

        public static void ShowErrorMsg(Exception ex)
        {
            if (ex is AggregateException aEx && aEx.InnerException != null)
                ex = aEx.InnerException;
            IUri uri =
                ex as IUri ?? ex.InnerException as IUri;
            IText text =
                ex as IText  ?? ex.InnerException as IText;
            var value = uri?.Uri?.AbsoluteUri ?? text?.Text;
            if (value != null)
            {
                if (MessageBox.Show(ex.Message + "\n\n" + Properties.Resources.CopyURL, AppName,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error) == MessageBoxResult.Yes)
                    Clipboard.SetText(value);
            }
            else
                MessageBox.Show(ex.Message, AppName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }
    }
}
