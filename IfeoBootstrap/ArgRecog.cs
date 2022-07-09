using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IfeoBootstrap;
public class ArgProcessor
{

    const string TargetScheme = "microsoft-edge";

    const string TargetExe = "JukusuiR2DB.exe";

    public ArgProcessor(AppRegistry appRegistry, string ifeoFile, string[] args)
    {
        AppRegistry = appRegistry;
        IfeoFile = ifeoFile;
        InArgs = args;
    }

    public void Launch()
    {
        var newArg = TrySimpleArg() ?? TryConfiguredArg();
        if (newArg != null)
        {
            //引数はURIでmicrosoft-edgeスキーム
            //リダイレクト対象
            try
            {
                var info = new ProcessStartInfo(TargetExe, newArg);
                using var process = Process.Start(info);
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show(
                    "Redirect App is not Exist\nWould you like to open Store Page?",
                    "ERROR", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var task = Windows.System.Launcher.LaunchUriAsync(new Uri(Shared.Properties.UniversalResources.StoreUri)).AsTask();
                    task.Wait();
                }
            }
        }
        else
        {
            //それ以外の場合
            //元のアプリで開く

        }
    }

    /// <summary>
    /// hoge.exe microsoft-edge://の形式についてチェックする
    /// </summary>
    /// <returns>一致した場合のみ、引数を設定して返す</returns>
    private string? TrySimpleArg()
    {
        //引数は一つのみ && 例のURI
        if (InArgs.Length == 1 && CheckScheme(InArgs[0]))
            return InArgs[0];
        else
            return null;
    }

    /// <summary>
    /// レジストリに指定された形式についてチェックする
    /// </summary>
    /// <returns>一致した場合のみ、引数を設定して返す</returns>

    private string? TryConfiguredArg()
    {
        if (AppRegistry.LaunchCommand != null &&
            InArgs.Length == AppRegistry.LaunchCommand.Length)
        {
            var len = InArgs.Length;
            for (int i = 0; i < len; i++)
            {
                if (AppRegistry.LaunchCommand[i] == "%1")
                {
                    if (CheckScheme(InArgs[i]))
                        return InArgs[i];
                    else
                        break;
                }
            }
        }
        return null;
    }

    private bool CheckScheme(string uriString)
    {
        try
        {
            var uri = new Uri(uriString);
            return uri.Scheme.ToLower() == TargetScheme;
        }
        catch (UriFormatException) { return false; }
    }


    public AppRegistry AppRegistry { get; }
    public string IfeoFile { get; }
    /// <summary>
    /// プログラムを起動する際に受け取ったコマンド引数のうち実際に関係のある部分
    /// </summary>
    public string[] InArgs { get; }

}
