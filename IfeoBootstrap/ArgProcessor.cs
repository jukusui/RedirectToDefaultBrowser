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
        var newArg = TrySimpleArg() ?? TryConfiguredArg();
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
            catch (Win32Exception ex) when (ex.HResult == ERROR_FILE_NOT_FOUND)
            {
                if (MessageBox.Show(
                    "Redirect App is not Exist\nWould you like to open Store Page?",
                    "ERROR", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                    MessageBox.Show("Edge Symbol is not Exist\nPlease ReInstall IFEO", "ERROR");
                }
            }
            else
            {
                MessageBox.Show(
                    "Unknown Path", "ERROR");
            }
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
        {
            Console.WriteLine($"{nameof(TrySimpleArg)}:{InArgs[0]}");
            return InArgs[0];
        }
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
            InArgs.Length == AppRegistry.LaunchCommand.Length - 1)
        {
            string? result = null;
            var len = InArgs.Length;
            for (int i = 0; i < len; i++)
            {
                if (AppRegistry.LaunchCommand[i + 1] == "%1")
                {
                    if (CheckScheme(InArgs[i]))
                    {
                        result = InArgs[i];
                        Console.WriteLine($"{nameof(TryConfiguredArg)}:{i}:{result}");
                    }
                    else
                    {
                        Console.WriteLine($"{nameof(TryConfiguredArg)}:{i}:Not_MS-EDGE");
                        break;
                    }
                }
                else if (AppRegistry.LaunchCommand[i + 1] != InArgs[i])
                {
                    Console.WriteLine($"{nameof(TryConfiguredArg)}:{i}:NotMatch");
                    result = null;
                    break;
                }
            }
            Console.WriteLine($"{nameof(TryConfiguredArg)}:res:{result}");
            return result;
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
