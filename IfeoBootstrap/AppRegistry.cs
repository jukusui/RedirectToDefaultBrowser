using IfeoBootstrap.Win32API;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace IfeoBootstrap;
public class AppRegistry
{

    //開くためのコマンドを既定の値として含むキー
    private const string REG_KEY_SHELL_OPEN_COMMAND = @"shell\open\command";

    private const string REG_KEY_APP_INFO = "Application";
    private const string REG_VAL_APP_NAME = "ApplicationName";
    private const string REG_VAL_APP_ICON = "ApplicationIcon";

    private static readonly string USER_ROOT = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    private static readonly IReadOnlyList<string> EDGE_INSTALL_ROOTS = new string[]
    {
        @"C:\Program Files\Microsoft\",
        @"C:\Program Files (x86)\Microsoft\",
        @"C:\Program Files (ARM)\Microsoft\",
        @$"{USER_ROOT}\AppData\Local\Microsoft\",
    };

    public static AppRegistry? TryCreate(string appRegRoot)
    {
        var res = new AppRegistry(appRegRoot);
        if (res.RawLaunchCommand != null)
            return res;
        return null;
    }

    private AppRegistry(string appRegRoot)
    {
        AppRegRoot = appRegRoot;
        RawLaunchCommand = GetRawLaunchCommand();
    }

    private object? TryGetRegistry(RegistryKey root, string key, string value)
    {
        try
        {
            using var appKey = root.OpenSubKey(key);
            return appKey?.GetValue(value);
        }
        catch (SecurityException) { return null; }
    }

    public string AppRegRoot { get; }

    private string? GetRawLaunchCommand()
    {
        var cmd = TryGetRegistry(
            Registry.ClassesRoot,
            $@"{AppRegRoot}\{REG_KEY_SHELL_OPEN_COMMAND}",
            "");
        if (cmd is string res)
            return res;
        return null;
    }

    public string? RawLaunchCommand { get; }

    public string[]? LaunchCommand
    {
        get
        {
            var arg = RawLaunchCommand;
            if (arg == null)
                return null;
            return CommandArg.CommandLineToArgv(arg);
        }
    }

    public string? ExePath
    {
        get => LaunchCommand?[0];
    }

    public bool IsMatchRoot
    {
        get => EDGE_INSTALL_ROOTS.Any(root => ExePath?.StartsWith(root, StringComparison.OrdinalIgnoreCase) == true);
    }

    public string? MatchedRoot
    {
        get => EDGE_INSTALL_ROOTS.FirstOrDefault(root => ExePath?.StartsWith(root, StringComparison.Ordinal) == true);
    }

    public string? AppName
    {
        get
        {
            var reg = TryGetRegistry(
                Registry.ClassesRoot,
                $@"{AppRegRoot}\{REG_KEY_APP_INFO}",
                REG_VAL_APP_NAME);
            if (reg is string res)
                return res;
            return null;
        }
    }

    public string? AppIcon
    {
        get
        {
            var reg = TryGetRegistry(
                Registry.ClassesRoot,
                $@"{AppRegRoot}\{REG_KEY_APP_INFO}",
                REG_VAL_APP_ICON);
            if (reg is string res)
                return res;
            return null;
        }
    }
}
