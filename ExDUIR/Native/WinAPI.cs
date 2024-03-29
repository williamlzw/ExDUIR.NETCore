﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ExDuiR.NET.Native
{
    public class WinAPI
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct LogFont
        {
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct DLGTEMPLATE
        {
            public int style;
            public int dwExtendedStyle;
            public short cdit;
            public short x;
            public short y;
            public short cx;
            public short cy;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MENUITEMINFO
        {
            public Int32 cbSize;
            public UInt32 fMask;
            public UInt32 fType;
            public UInt32 fState;
            public UInt32 wID;
            public IntPtr hSubMenu;
            public IntPtr hbmpChecked;
            public IntPtr hbmpUnchecked;
            public IntPtr dwItemData;
            public string dwTypeData;
            public UInt32 cch; // length of dwTypeData
            public IntPtr hbmpItem;
        }

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetAsyncKeyState")]
        public static extern short GetAsyncKeyState(int nVirtKey);
        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetKeyState")]
        public static extern short GetKeyState(int nVirtKey);
        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLongPtrW(IntPtr hWnd, int nIndex);
        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetWindowLong")]
        public static extern IntPtr SetWindowLongPtrW(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetParent")]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateMenu")]
        public static extern IntPtr CreateMenu();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "CreatePopupMenu")]
        public static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "AppendMenuW")]
        public static extern bool AppendMenu(IntPtr menuHandle, uint flag, IntPtr newID, string item);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "EnableMenuItem")]
        public static extern bool EnableMenuItem(IntPtr menuHandle, uint uEnableItem, uint uEnable);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetMenuItemID")]
        public static extern IntPtr GetMenuItemID(IntPtr hMenu, int nPos);

        public delegate bool DlgWndProcDelegate(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "DialogBoxIndirectParamW")]
        public static extern int DialogBoxIndirectParam(IntPtr hInstance, ref DLGTEMPLATE hDialogTemplate, IntPtr hwndParent, DlgWndProcDelegate lpDialogFunc, int dwInitParam);
        
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "DragQueryFileW")]
        public static extern uint DragQueryFile(IntPtr hDrop, uint iFile, string lpszFile, uint cch);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "EndMenu")]
        public static extern bool EndMenu();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetPropW")]
        public static extern bool SetProp(IntPtr hWnd, string text, IntPtr data);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetPropW")]
        public static extern IntPtr GetProp(IntPtr hWnd, string text);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "ReleaseDC")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetDeviceCaps")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetModuleHandleW")]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string wzModule);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "IsWindow")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "IsWindowVisible")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "IsWindowEnabled")]
        public static extern bool IsWindowEnabled(IntPtr hWnd);
        
        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetWindowRect")]
        public static extern bool GetWindowRect(IntPtr hWnd, out ExRect rect);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetCursorPos")]
        public static extern bool GetCursorPos(out ExPoint point);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "WindowFromPoint")]
        public static extern IntPtr WindowFromPoint(ExPoint point);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "DeleteObject")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetMenuItemInfo")]
        public static extern bool GetMenuItemInfo(IntPtr hMenu, uint uItem, [MarshalAs(UnmanagedType.Bool)] bool fByPosition, ref MENUITEMINFO lpmii);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetMenuItemInfo")]
        public static extern bool SetMenuItemInfo(IntPtr hMenu, uint uItem, [MarshalAs(UnmanagedType.Bool)]  bool fByPosition, ref MENUITEMINFO lpmii);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "ScreenToClient")]
        public static extern IntPtr ScreenToClient(IntPtr hWnd, ref ExPoint point);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "EnableWindow")]
        public static extern bool EnableWindow(IntPtr hWnd, bool fEnable);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool fRedraw);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "SendMessageW")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "PostMessageW")]
        public static extern bool PostMessage(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int nWidth, int nHeight, int dwFlags);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetWindowTextW")]
        public static extern bool SetWindowText(IntPtr hWnd, string sText);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetWindowTextW")]
        public static extern bool GetWindowText(IntPtr hWnd, StringBuilder sText, int cchText);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetWindowTextLengthW")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetLastError")]
        public static extern uint GetLastError();
    }
}
