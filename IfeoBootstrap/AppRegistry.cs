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
    private const string _regKeyShellOpenCommand = @"shell\open\command";

    private const string _regKeyAppInfo = "Application";
    private const string _regValAppName = "ApplicationName";
    private const string _regValAppIcon = "ApplicationIcon";

    private static readonly string _userRoot = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    private static readonly IReadOnlyList<string> _edgeInstallRoots = new string[]
    {
        @"C:\Program Files\Microsoft\",
        @"C:\Program Files (x86)\Microsoft\",
        @"C:\Program Files (ARM)\Microsoft\",
        @$"{_userRoot}\AppData\Local\Microsoft\",
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
            $@"{AppRegRoot}\{_regKeyShellOpenCommand}",
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
        get => _edgeInstallRoots.Any(root => ExePath?.StartsWith(root, StringComparison.OrdinalIgnoreCase) == true);
    }

    public string? MatchedRoot
    {
        get => _edgeInstallRoots.FirstOrDefault(root => ExePath?.StartsWith(root, StringComparison.Ordinal) == true);
    }

    public string? AppName
    {
        get
        {
            var reg = TryGetRegistry(
                Registry.ClassesRoot,
                $@"{AppRegRoot}\{_regKeyAppInfo}",
                _regValAppName);
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
                $@"{AppRegRoot}\{_regKeyAppInfo}",
                _regValAppIcon);
            if (reg is string res)
                return res;
            return null;
        }
    }
}
