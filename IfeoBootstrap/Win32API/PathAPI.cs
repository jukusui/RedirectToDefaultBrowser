using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IfeoBootstrap.Win32API;
internal unsafe class PathAPI
{

    public const int MaxPath = 260;

    private enum ItemType : int
    {
        File = 0x80,
        Directory = 0x10,
    }

    [DllImport("Shlwapi.dll")]
    private static extern bool PathRelativePathToW(char* pszPath,
        [MarshalAs(UnmanagedType.LPWStr)] string pszFrom, [MarshalAs(UnmanagedType.I4)] ItemType dwAttrFrom,
        [MarshalAs(UnmanagedType.LPWStr)] string pszTo, [MarshalAs(UnmanagedType.I4)] ItemType dwAttrTo);
    [DllImport("Shlwapi.dll")]
    private static extern char* PathCombineW(char* lpszDest,
        [MarshalAs(UnmanagedType.LPWStr)] string lpszDir,
        [MarshalAs(UnmanagedType.LPWStr)] string lpszFile);

    public static string GetRelativePath(string abstractPath, string basePath, bool abstractIsFile = false, bool baseIsFile = false)
    {
        char* output = stackalloc char[MaxPath];
        if (PathRelativePathToW(output,
            basePath, baseIsFile ? ItemType.File : ItemType.Directory,
            abstractPath, abstractIsFile ? ItemType.File : ItemType.Directory))
            return new string(output);
        else
            throw new IOException();
    }

    public static string CombinePath(string baseDir, string relativeFile)
    {

        char* buffer = stackalloc char[MaxPath];
        char* result = PathCombineW(buffer, baseDir, relativeFile);
        if (result != null)
            return new string(result);
        else
            throw new IOException();
    }


}
