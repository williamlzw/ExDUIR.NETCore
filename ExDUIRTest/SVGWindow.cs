﻿using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Native;
using static ExDuiR.NET.Native.ExConst;
using System;
using ExDuiR.NET.Frameworks.Graphics;
using System.Runtime.InteropServices;

namespace ExDuiRTest
{
    static class SVGWindow
    {
        static private ExSkin skin;
        static private ExWndProcDelegate wndProc;
        static public void CreateSVGWindow(ExSkin pOwner)
        {
            wndProc = new ExWndProcDelegate(OnWndMsgProc);
            skin = new ExSkin(pOwner, null, "自定义字体和SVG测试", 0, 0, 500, 600,
            EWS_MOVEABLE | EWS_CENTERWINDOW | EWS_NOSHADOW | EWS_BUTTON_CLOSE | EWS_TITLE, 0, 0, default, wndProc);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExRGBA(150, 150, 150, 255);
 
                skin.Visible = true;
            }
        }

        static private IntPtr OnWndMsgProc(IntPtr hWnd, int hExDui, int uMsg, IntPtr wParam, IntPtr lParam, IntPtr pResult)
        {
            if(uMsg == WM_ERASEBKGND)
            {
                var canvas = new ExCanvas((int)wParam);
                canvas.Clear(Util.ExRGBA(150, 150, 150, 255));
                var font = new ExFont("Resources/文道灵飞小楷.ttf", 64);
                var ret = canvas.DrawText(font, Util.ExRGBA(200, 0, 200, 200), "我是测试文本", -1, -1, 20, 450, 450, 530);
                Console.WriteLine(ret);
                font.Dispose();

                canvas.DrawSvgFromFile("Resources/niu1.svg", 0, 50, 50, 200, 200);
                var data = File.ReadAllBytes("Resources/niu1.svg");
                var ptr = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, ptr, data.Length);
                canvas.DrawSvg(ptr, Util.ExRGBA(55, 250, 20, 255), 250, 50, 400, 200);
                canvas.DrawSvgFromFile("Resources/niu1.svg", Util.ExRGBA(55, 0, 250, 255), 50, 250, 200, 400);
                canvas.DrawSvgFromFile("Resources/niu.svg", 0, 250, 250, 400, 450);
                return (IntPtr)1;
            }
            return IntPtr.Zero;
        }
    }
}
