using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IfeoBootstrap.Win32API;
internal class CommandArg
{

    [DllImport("Shell32.dll", SetLastError = true)]
    private static unsafe extern char** CommandLineToArgvW(
        [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

    public static string[] CommandLineToArgv(string cmdLine)
    {
        unsafe
        {
            List<string> result = new();
            char** args = CommandLineToArgvW(cmdLine, out var count);
            try
            {
                for (int i = 0; i < count; i++)
                {
                    result.Add(new string(args[i]));
                }
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr)args);
            }
            return result.ToArray();
        }
    }

    //空白やダブルクオートが無ければダブルクオートで囲む必要はない
    private static readonly Regex _regexNormal = new("^[^ \"]*$");

    public static string ArgvToCommandLine(string[] args)
    {
        int remain = args.Length;
        var output = new StringBuilder();
        foreach (var arg in args)
        {
            if (_regexNormal.IsMatch(arg))
            {
                output.Append(arg);
            }
            else
            {
                output.Append('"');
                var escapes = 0;
                foreach (var ch in arg)
                {
                    if (ch == '\\')
                    {
                        escapes++;
                    }
                    else if (ch == '"' && 0 < escapes)
                    {
                        output.Append('\\', escapes * 2 + 1);
                        output.Append('"');
                        escapes = 0;
                    }
                    else if (0 < escapes)
                    {
                        output.Append('\\', escapes);
                        output.Append(ch);
                        escapes = 0;
                    }
                    else
                    {
                        output.Append(ch);
                    }
                }
                if (0 < escapes)
                {
                    output.Append('\\', escapes);
                    escapes = 0;
                }
                output.Append('"');
            }
            remain--;
            if (0 < remain)
                output.Append(" ");
        }
        return output.ToString();
    }

}
