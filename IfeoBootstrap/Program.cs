using IfeoBootstrap.Properties;
using IfeoBootstrap.Win32API;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IfeoBootstrap
{
    public static class Program
    {

        private const string _ifeo = "--IFEO";
        internal const string _caption = " - R2DB IFEO";

        public static void Main(string[] args)
        {
            try
            {
                MainTask(args).Wait();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ExResources.UnhandledCaption + _caption);
            }
        }

        public static async Task MainTask(string[] args)
        {
            AppRegistry? appRegistry = null;
            string? ifeo = null;
            string? message = null;
            if (args.Length <= 0)
            {
                message = ExResources.NoArgs;
            }
            else if (args.Length == 1)
            {
                message = ExResources.MissingArgs;
            }
            else
            {
                if (args[0] != _ifeo)
                    message = ExResources.UnknownArgs;
                else if (!File.Exists(args[1]))
                    message = ExResources.ArgFileNotFound;
                else
                {
                    Debug.Write("IFEO Source Check:");
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
                                Debug.Write(keyName);
                                break;
                            }
                        }
                    }
                    Debug.WriteLine("");

                    if (ifeo == null || appRegistry == null)
                    {
                        message = ExResources.UnknownIfeoSource;
                    }
                }
            }
            if (message != null)
            {
                MessageBox.Show(message, ExResources.ErrorCaption + _caption);
            }
            else
            {

                if (ifeo == null || appRegistry == null)
                {
                    message = ExResources.UnknownIfeoSource;
                    MessageBox.Show(message, ExResources.ErrorCaption + _caption);
                }
                else
                {
                    var argProcessor = new ArgProcessor(appRegistry, ifeo, args.Skip(2).ToArray());
                    await argProcessor.Launch();
                }
            }
        }

    }
}