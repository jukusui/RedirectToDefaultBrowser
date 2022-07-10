using IfeoBootstrap.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    const int ERROR_FILE_NOT_FOUND = 2;

    public ArgProcessor(AppRegistry appRegistry, string ifeoFile, string[] args)
    {
        AppRegistry = appRegistry;
        IfeoFile = ifeoFile;
        InArgs = args;
    }

    public async Task Launch()
    {
        var newArg = TrySimpleArg();
        if (newArg != null)
        {
            //引数はURIでmicrosoft-edgeスキーム
            //リダイレクト対象
            try
            {
                var info = new ProcessStartInfo(TargetExe,
                    Win32API.CommandArg.ArgvToCommandLine(new[] { newArg }));
                //info.UseShellExecute = true;
                //MessageBox.Show(TargetExe + " " + info.Arguments, "Target Exe");
                using var process = Process.Start(info);
                //process.Close();
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == ERROR_FILE_NOT_FOUND)
            {
                if (MessageBox.Show(ExResources.AppMissing, ExResources.ErrorCaption + Program._caption,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    await Windows.System.Launcher.LaunchUriAsync(new Uri(Shared.Properties.UniversalResources.StoreUri));

                }
            }
        }
        else
        {
            var linkExe = AppRegistry.GetLinkExe();
            if (linkExe != null)
            {
                try
                {
                    var info = new ProcessStartInfo(linkExe,
                        Win32API.CommandArg.ArgvToCommandLine(InArgs));
                    using var process = Process.Start(info);
                }
                catch (Win32Exception ex) when (ex.HResult == ERROR_FILE_NOT_FOUND)
                {
                    MessageBox.Show(ExResources.EdgeMissing, ExResources.ErrorCaption + Program._caption);
                }
            }
            else
            {
                MessageBox.Show(ExResources.EdgeMissing, ExResources.ErrorCaption + Program._caption);
            }
        }
    }

    /// <summary>
    /// hoge.exe microsoft-edge://の形式についてチェックする
    /// </summary>
    /// <returns>最初に一致したもののみ、引数を設定して返す</returns>
    private string? TrySimpleArg()
    {
        //引数は一つのみ && 例のURI
        foreach (var item in InArgs)
        {
            if (CheckScheme(item))
                return item;
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
