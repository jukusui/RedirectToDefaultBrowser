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
            AppRegistry? appRegistry = null;
            string? ifeo = null;
            string? uri = null;
            string? message = null;
            switch (args.Length)
            {
                case int value when value <= 0:
                    message = "No Arguments";
                    break;
                case 1:
                    message = "Missing Arguments";
                    break;
                default:
                    if (args[0] != _ifeo)
                        message = "Unknown Arguments";
                    else if (!File.Exists(args[1]))
                        message = "Argument File not Found";
                    else
                    {
                        using var apps = RegistryRedirect.HKLM.OpenSubKey(Install.EdgeExeLink._appsRegKey);
                        foreach (var keyName in apps?.GetValueNames() ?? Enumerable.Empty<string>())
                        {
                            var reg = AppRegistry.TryCreate(keyName);
                            if (reg != null && reg.ExePath != null)
                            {
                                if (string.Compare(reg.ExePath, args[1], true) == 0)
                                {
                                    appRegistry = reg;
                                    ifeo = reg.ExePath;
                                    break;
                                }
                            }
                        }
                    }
                    break;
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