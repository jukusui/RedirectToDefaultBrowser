﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IfeoBootstrap.Win32API;
internal class WindowApi
{

    public enum SystemCommand : int
    {
        CS_CLOSE = 0xF060,
        SC_CONTEXTHELP = 0xF180,
        SC_DEFAULT = 0xF160,
        SC_HOTKEY = 0xF150,
        SC_HSCROLL = 0xF080,
        SCF_ISSECURE = 0x00000001,
        SC_KEYMENU = 0xF100,
        SC_MAXIMIZE = 0xF030,
        SC_MINIMIZE = 0xF020,
        SC_MONITORPOWER = 0xF170,
        SC_MOUSEMENU = 0xF090,
        SC_MOVE = 0xF010,
        SC_NEXTWINDOW = 0xF040,
        SC_PREVWINDOW = 0xF050,
        SC_RESTORE = 0xF120,
        SC_SCREENSAVE = 0xF140,
        SC_SIZE = 0xF000,
        SC_TASKLIST = 0xF130,
        SC_VSCROLL = 0xF070,
    }

    [DllImport("User32.dll")]
    static extern nint GetSystemMenu(nint hWnd, nint bRevert);
    [DllImport("User32.dll", SetLastError = true)]
    static extern bool RemoveMenu(nint hMenu, int uPosition, bool uFlags);


    public static void RemoveMenuItem(nint hWnd, SystemCommand position, bool flag)
    {
        nint windowMenu = GetSystemMenu(hWnd, 0);
        RemoveMenu(windowMenu, (int)position, flag);
    }


}
