﻿using IfeoBootstrap.Win32API;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IfeoBootstrap.Install;

/// <summary>
/// シンボリックリンクの作成関連
/// </summary>
[RunInstaller(true)]
public class EdgeExeLink : Installer
{

    public EdgeExeLink()
    {

    }


    internal static readonly Regex _firstDirRegex = new(@"^(\.\\)?(.+?)\\");
    internal const string _appsRegKey = @"Software\Jukusui\R2DB\Apps\";
    internal const string _dirExtName = "_R2DB";

    internal const string _ifeoNameHeader = "Jukusui.R2DB_";
    internal const string _debuggerName = "Debugger";
    internal const string _filterFullPathName = "FilterFullPath";
    internal const string _debuggerFormat = "{0} --IFEO";
    internal const string _ifeoRegKey = @"Software\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\msedge.exe\";

    /// <summary>
    /// UIを使用するため、STAでスレッドを作成する
    /// </summary>
    /// <param name="stateSaver"></param>
    public override void Install(IDictionary stateSaver)
    {
        try
        {
            var taskCmp = new TaskCompletionSource<bool>();
            var thread = new Thread(() =>
            {
                try
                {
                    InstallTask();
                    taskCmp.SetResult(true);
                }
                catch (Exception ex)
                {
                    taskCmp.SetException(ex);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            taskCmp.Task.Wait();
            base.Install(stateSaver);
        }
        catch (Exception ex)
        {
            Dispose();
            throw new InstallException(ex.ToString(), ex);
        }
    }

    /// <summary>
    /// STAで実行される実際のインストール処理
    /// </summary>
    private void InstallTask()
    {
        var exePath = Context.Parameters["assemblypath"];
        if (exePath == null)
            throw new NullReferenceException();

        var infos = RegistryInfo.GetInfo();
        var selector = new EdgeSelectWindow();
        foreach (var edge in infos.MsEdgeApps)
            selector.Apps.Add(edge);
        selector.SelectAll();
        var dialogRes = selector.ShowDialog();

        if (dialogRes == true)
        {
            using var appKey = RegistryRedirect.HKLM.CreateSubKey(_appsRegKey);
            using var ifeoKey = RegistryRedirect.HKLM.CreateSubKey(_ifeoRegKey);

            foreach (var edge in selector.SelectedItems)
            {
                var root = edge.MatchedRoot;
                if (root == null || edge.ExePath == null)
                    throw new NullReferenceException("Root Directory is not Match");

                var relative = PathAPI.GetRelativePath(edge.ExePath, root, true);
                var groups = _firstDirRegex.Match(relative).Groups;
                var firstDirName = groups[groups.Count - 1];
                try
                {
                    {
                        var appDir = root + firstDirName;
                        SymbolicLink.CreateFolderSymbolicLink(appDir, appDir + _dirExtName);

                        appKey.SetValue(edge.AppRegRoot, appDir);
                    }
                    {
                        using var ifeoSubKey = ifeoKey.CreateSubKey(_ifeoNameHeader + edge.AppRegRoot);
                        ifeoSubKey.SetValue(_debuggerName, string.Format(_debuggerFormat, exePath));
                        ifeoSubKey.SetValue(_filterFullPathName, edge.ExePath);
                    }
                }
                catch (Exception ex) when (ex is IOException | ex is Win32Exception)
                {
                    throw ex;
                }
            }
        }
        else
        {
            throw new OperationCanceledException("Install Cancelled");
        }
    }

    public override void Commit(IDictionary stateSaver)
    {
        base.Commit(stateSaver);
    }

    private void RemoveLinks()
    {
        if (_appsRegKey != null)
        {
            using var appsKey = RegistryRedirect.HKLM?.OpenSubKey(_appsRegKey, true);
            if (appsKey != null)
            {
                foreach (var value in appsKey.GetValueNames())
                {
                    if (appsKey.GetValue(value) is string valueStr)
                    {
                        Directory.Delete(valueStr + _dirExtName);
                    }
                    appsKey.DeleteValue(value);
                }
            }
        }
    }

    /// <summary>
    /// 登録したシンボリックリンクを全て消す
    /// </summary>
    public override void Rollback(IDictionary stateSaver)
    {
        RemoveLinks();
        base.Rollback(stateSaver);
    }

    public override void Uninstall(IDictionary stateSaver)
    {
        RemoveLinks();
        base.Uninstall(stateSaver);
    }
}
