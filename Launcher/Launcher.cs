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
            try
            {
                switch (args.Length)
                {
                    case 0:
                        var win = new MainWindow();
                        win.ShowDialog();
                        break;
                    case 1:
                        Launch(args[0]);
                        break;
                    default:
                        MessageBox.Show("引数の数は1つでなければなりません", AppName);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"予期せぬエラー\n{ex}", AppName);
            }
        }

        static void Launch(string arg)
        {
            {
                if (!Uri.IsWellFormedUriString(arg, UriKind.Absolute))
                    MessageBox.Show("引数がURIとして認識できませんでした", AppName);
                else
                {
                    Uri source = null;
                    IEnumerable<Uri> targets = null;
                    try { source = new Uri(arg); }
                    catch
                    {
                        MessageBox.Show("引数がURIとして認識できませんでした", AppName);
                        return;
                    }
                    switch (source.Scheme)
                    {
                        case "microsoft-edge":
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
                            break;
                        default:
                            MessageBox.Show($"引数のURI Scheme({source.Scheme})が認識できませんでした", AppName);
                            return;
                    }
                    if (targets == null || !targets.Any())
                        MessageBox.Show("URLがありませんでした", AppName);
                    else
                        foreach (var target in targets)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(target.ToString());
                            }
                            catch
                            {
                                MessageBox.Show("ブラウザの起動に失敗しました", AppName);
                            }
                        }
                }
            }

        }
    }
}
