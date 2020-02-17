using Shared;
using Shared.Ex;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace AppLauncher
{
    public static class Launcher
    {
        internal const string AppName = "RedirectToDefaultBrowser";

        internal const string PackageName = "14065Jukusui.RedirectToDefaultBrowser_m5qwzdwnyj6rw";

        internal const string SchemeUi = "redirecttodefaultbrowser-ui";

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
                bool modernMode = true;
                if (modernMode)
                {
                    var task = UiTask();
                    task.Wait();
                    modernMode = task.Result;
                }
                if (!modernMode)
                {
                    //モダンUIの起動不良
                    var win = new MainWindow();
                    win.ShowDialog();
                    Config.Save();

                }
            }
        }

        public static async Task<bool> MainTask(string[] args)
        {
            //var opt = new Windows.System.LauncherOptions()
            //{
            //    TargetApplicationPackageFamilyName = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe"
            //};
            //var items = await Windows.System.Launcher.FindUriSchemeHandlersAsync("microsoft-edge");
            //foreach (var item in items)
            //{
            //    Debug.WriteLine(item.DisplayInfo.DisplayName);
            //}
            //var regApps = Microsoft.Win32.Registry.LocalMachine.
            //    OpenSubKey("SOFTWARE", false).
            //    OpenSubKey("RegisteredApplications", false);
            //foreach (var regApp in regApps.GetValueNames())
            //{
            //    var value = regApps.GetValue(regApp).ToString();
            //    var capabilities = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(value, false) ??
            //        Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(value, false);

            //    var urls = capabilities?.OpenSubKey("URLAssociations", false);
            //    foreach (var url in urls?.GetValueNames() ?? new string[] { })
            //    {
            //        if (url.ToLower() == "microsoft-edge")
            //            Debug.WriteLine(regApp);
            //    }
            //}

            switch (args.Length)
            {
                case 0:
                    return true;
                case 1:
                    if (args[0].ToLower() == "microsoft-edge:")
                        return true;
                    else
                    {
                        Config.LastUrl = args[0];
                        Config.Save();
                        await Opener.Open(args[0]);
                        return false;
                    }
                default:
                    throw new TooManyArgException();
            }
        }

        public static async Task<bool> UiTask()
        {
            var uri = new Uri($"{SchemeUi}:");
            var opt = new Windows.System.LauncherOptions()
            {
                TargetApplicationPackageFamilyName = PackageName
            };
            return await Windows.System.Launcher.LaunchUriAsync(uri, opt);
        }

        public static void ShowErrorMsg(Exception ex)
        {
            if (ex is AggregateException aEx && aEx.InnerException != null)
                ex = aEx.InnerException;
            IUri? uri =
                ex as IUri ?? ex.InnerException as IUri;
            IText? text =
                ex as IText ?? ex.InnerException as IText;
            var value = uri?.Uri?.AbsoluteUri ?? text?.Text;
            if (value != null)
            {
                if (MessageBox.Show(ex.Message + "\n\n" + Shared.Properties.Resources.CopyURL, AppName,
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
