using IfeoBootstrap.Win32API;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IfeoBootstrap
{
    public static class Program
    {

        private static readonly string _ifeo = "--IFEO";

        public static void Main(string[] args)
        {
            var option = new Windows.System.LauncherOptions();
            option.PreferredApplicationDisplayName = "14065Jukusui.RedirectToDefaultBrowser";
            option.PreferredApplicationPackageFamilyName = "14065Jukusui.RedirectToDefaultBrowser_m5qwzdwnyj6rw";


            var task = Windows.System.Launcher.LaunchUriAsync(new Uri(Shared.Properties.UniversalResources.StoreUri)).AsTask();
            task.Wait();

            AppRegistry? appRegistry = null;
            string? ifeo = null;
            string? message = null;
            if (args.Length <= 0)
            {
                message = "No Arguments";
            }
            else if (args.Length == 1)
            {
                message = "Missing Arguments";
            }
            else
            {
                if (args[0] != _ifeo)
                    message = "Unknown Arguments";
                else if (!File.Exists(args[1]))
                    message = "Argument File not Found";
                else
                {
                    string exePath = args[1];
                    using var apps = RegistryRedirect.HKLM.OpenSubKey(Install.EdgeExeLink._appsRegKey);
                    foreach (var keyName in apps?.GetValueNames() ?? Enumerable.Empty<string>())
                    {
                        var reg = AppRegistry.TryCreate(keyName);
                        if (reg != null && reg.ExePath != null)
                        {

                            if (string.Compare(reg.ExePath, exePath, true) == 0)
                            {
                                appRegistry = reg;
                                ifeo = reg.ExePath;
                                break;
                            }
                        }
                    }

                    if (ifeo == null || appRegistry == null)
                    {
                        message = "Unknown IFEO Source";
                    }
                }
            }
            if (message != null)
            {
                MessageBox.Show(message, "ERROR");
            }
            else
            {
                MessageBox.Show(string.Join("\n", args.Skip(2).ToArray()), ifeo);
            }
        }

    }
}