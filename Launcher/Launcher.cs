using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;

namespace Launcher
{
    public static class Launcher
    {
        const string AppName = "RedirectToDefaultBrowser";

        [STAThread()]
        public static void Main(string[] args)
        {
            string message = null;
            try
            {
                switch (args.Length)
                {
                    case 0:
                        var win = new MainWindow();
                        win.ShowDialog();
                        break;
                    case 1:
                        message = Launch(args[0]);
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
                if (MessageBox.Show("ブラウザを開けませんでした\n\n" + message + "\nURIをクリップボードに保存しますか?", AppName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    Clipboard.SetText(string.Join("\n", args));
        }

        static string Launch(string arg)
        {
            if (!Uri.IsWellFormedUriString(arg, UriKind.Absolute))
                MessageBox.Show("引数がURIとして認識できませんでした", AppName);
            else
            {
                IEnumerable<Uri> targets = null;
                if (!Uri.TryCreate(arg, UriKind.Absolute, out Uri source))
                {
                    return "引数がURIとして認識できませんでした";
                }
                switch (source.Scheme)
                {
                    case "microsoft-edge":
                        if (Uri.TryCreate(arg.Substring("microsoft-edge:".Length), UriKind.Absolute, out Uri oneUri))
                        {
                            targets = new Uri[] { oneUri };
                        }
                        else
                        {
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
                        break;
                    default:
                        return $"引数のURI Scheme({source.Scheme})が認識できませんでした";
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
                            System.Diagnostics.Process.Start(target.ToString());
                        }
                        catch
                        {
                            hasError = true;
                        }
                    }
                    if (hasError)
                        return "ブラウザの起動に失敗しました";
                }
                else
                    return "http、https以外のプロトコルが検知されました";
            }
            return null;

        }
    }
}
