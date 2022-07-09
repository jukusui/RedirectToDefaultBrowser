using IfeoBootstrap.Win32API;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        var command = GetRawLaunchCommand(appRegRoot);
        if (command == null)
            return null;
        return new AppRegistry(appRegRoot, command);
    }

    private AppRegistry(string appRegRoot, string command)
    {
        AppRegRoot = appRegRoot;
        RawLaunchCommand = command;
    }

    private static object? TryGetRegistry(RegistryKey root, string key, string value)
    {
        try
        {
            using var appKey = root.OpenSubKey(key);
            return appKey?.GetValue(value);
        }
        catch (SecurityException) { return null; }
    }

    private static string? GetRawLaunchCommand(string appRegRoot)
    {
        var cmd = TryGetRegistry(
            Registry.ClassesRoot,
            $@"{appRegRoot}\{_regKeyShellOpenCommand}",
            "");
        if (cmd is string res)
            return res;
        return null;
    }

    public string? GetLinkExe()
    {
        using var apps = RegistryRedirect.HKLM.OpenSubKey(Install.EdgeExeLink._appsRegKey);
        var edgeDirObj = apps?.GetValue(AppRegRoot);
        if (edgeDirObj is not string edgeDir)
            return null;
        var linkDir = edgeDir + Install.EdgeExeLink._dirExtName;
        var relative = PathAPI.GetRelativePath(ExePath, edgeDir, true);
        return PathAPI.CombinePath(linkDir, relative);
    }

    public string AppRegRoot { get; }


    public string RawLaunchCommand { get; }

    public string[] LaunchCommand => CommandArg.CommandLineToArgv(RawLaunchCommand);

    public string ExePath
    {
        get => LaunchCommand[0];
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
