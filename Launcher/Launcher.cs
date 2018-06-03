using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
            var task = MainAsync(args);
            task.Wait();
            if (task.Result)
            {
                var win = new MainWindow();
                win.ShowDialog();
            }
        }

        public static async Task<bool> MainAsync(string[] args)
        {
            bool? showWin = false;
            string message = null;
            try
            {
                switch (args.Length)
                {
                    case 0:
                        showWin = true;
                        break;
                    case 1:
                        message = await Launch(args[0]);
                        Properties.Settings.Default.LastURL = args[0];
                        Properties.Settings.Default.Save();
                        showWin = null;
                        break;
                    default:
                        message = "起動時の引数の数は1つでなければなりません";
                        break;
                }
            }
            catch (Exception ex)
            {
                message = $"予期せぬエラー\n{ex}";
            }
            if (message != null)
                MessageBox.Show("ブラウザを開けませんでした\n\n" + message, AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
            return (showWin == true) || (message != null && showWin == null);
        }

        internal static async Task<string> Launch(string arg)
        {
            if (arg.StartsWith("microsoft-edge:"))
            {
                IEnumerable<Uri> targets = null;
                if (Uri.TryCreate(arg.Substring("microsoft-edge:".Length), UriKind.Absolute, out Uri oneUri))
                {
                    targets = new Uri[] { oneUri };
                }
                else
                {
                    if (!Uri.TryCreate(arg, UriKind.Absolute, out Uri source))
                    {
                        return "引数がURIとして認識できませんでした";
                    }
                    var q = source.Query;
                    if (q != null && 1 < q.Length)
                    {
                        q = q.Substring(1);
                        targets =
                            from l in q.Split('&')
                            where l.StartsWith("url=", StringComparison.CurrentCultureIgnoreCase)
                            let url = Uri.UnescapeDataString(l.Substring(4))
                            where Uri.IsWellFormedUriString(url, UriKind.Absolute)
                            select new Uri(url);
                    }
                }
                if (targets == null || !targets.Any())
                    return "URLがありませんでした";
                else if (targets.All(uri => uri.Scheme == "http" || uri.Scheme == "https"))
                {
                    bool hasError = false;
                    foreach (var target in targets)
                    {
                        try
                        {
                            hasError = !await Windows.System.Launcher.LaunchUriAsync(target);
                        }
                        catch
                        {
                            hasError = true;
                        }
                    }
                    if (hasError)
                        return "ブラウザの起動に失敗しました";
                    return null;
                }
                else
                    return "http、https以外のプロトコルが検知されました";
            }
            else
                return "引数がURIとして認識できませんでした";

        }
    }
}
