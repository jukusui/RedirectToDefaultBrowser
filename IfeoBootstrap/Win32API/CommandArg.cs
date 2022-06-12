using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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


}
