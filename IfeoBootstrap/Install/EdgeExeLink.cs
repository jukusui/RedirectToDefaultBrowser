using IfeoBootstrap.Win32API;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Text;
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


    private readonly Regex FirstDirRegex = new("^/?(.+?)/");
    private readonly string APPS_REG_KEY = @"Software\Jukusui\R2DB\Apps\";
    private readonly string DIR_EXT_NAME = "_R2DB";

    private readonly string IFEO_NAME_HEADER = "Jukusui.R2DB_";
    private readonly string DEBUGGER_NAME = "Debugger";
    private readonly string FILTER_FULLPATH_NAME = "FilterFullPath";
    private readonly string DEBUGGER_FORMAT = "{0} /IFEO";
    private readonly string IFEO_REG_KEY = @"Software\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\msedge.exe\";

    internal List<AppRegistry> CopiedApps { get; } = new List<AppRegistry>();

    public override void Install(IDictionary stateSaver)
    {
        try
        {
            var thread = new Thread(InstallTask);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
        catch (ThreadInterruptedException ex)
        {
            throw ex.InnerException;
        }
        base.Install(stateSaver);
    }

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

            using var appKey = Registry.LocalMachine.CreateSubKey(APPS_REG_KEY);
            using var ifeoKey = Registry.LocalMachine.CreateSubKey(IFEO_REG_KEY);

            foreach (var edge in selector.SelectedItems)
            {
                var root = edge.MatchedRoot;
                if (root == null || edge.ExePath == null)
                    throw new NullReferenceException("Root Directory is not Match");

                var absUri = new Uri(edge.ExePath);
                var rootUri = new Uri(root);
                var relativeUri = rootUri.MakeRelativeUri(absUri).ToString();
                var firstDirName = FirstDirRegex.Match(relativeUri).Result("$1");

                try
                {
                    {
                        var appDir = root + firstDirName;
                        SymbolicLink.CreateFolderSymbolicLink(appDir, appDir + DIR_EXT_NAME);

                        appKey.SetValue(edge.AppRegRoot, appDir);
                    }
                    {
                        using var ifeoSubKey = ifeoKey.CreateSubKey(IFEO_NAME_HEADER + edge.AppRegRoot);
                        ifeoSubKey.SetValue(DEBUGGER_NAME, string.Format(DEBUGGER_FORMAT, exePath));
                        ifeoSubKey.SetValue(FILTER_FULLPATH_NAME, edge.ExePath);
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
        using var appsKey = Registry.LocalMachine.OpenSubKey(APPS_REG_KEY);
        foreach (var value in appsKey.GetValueNames())
        {
            if (appsKey.GetValue(value) is string valueStr)
            {
                Directory.Delete(valueStr + DIR_EXT_NAME);
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
        base.Rollback(stateSaver);
    }
}
