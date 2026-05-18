using ExDuiR.NET.Frameworks.Controls;
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
            skin = new ExSkin(pOwner, null, "自定义字体和SVG测试", 0, 0, 800, 800,
            WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_NOSHADOW | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_TITLE, 0, 0, default, wndProc);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExARGB(150, 150, 150, 255);

                skin.Visible = true;
            }
        }

        static private IntPtr OnWndMsgProc(IntPtr hWnd, int hExDui, int uMsg, IntPtr wParam, IntPtr lParam, IntPtr pResult)
        {
            if (uMsg == WM_ERASEBKGND)
            {
                var canvas = new ExCanvas((int)wParam);
                canvas.Clear(Util.ExARGB(150, 150, 150, 255));
                var font = new ExFont("Resources/文道灵飞小楷.ttf", 48);
                canvas.DrawText(font, Util.ExARGB(200, 0, 200, 200), "我是可以选中文本(DT_SELECTABLE风格)，选中ctrl+c复制到剪贴板", -1, DT_SELECTABLE, 20, 450, 1050, 730);

                canvas.DrawText(font, Util.ExARGB(200, 0, 200, 200), "双击或者按住拖动选择试试，支持多个文本选择", -1, DT_SELECTABLE, 20, 750, 850, 930);

                font.Dispose();
                ExSvg svg1 = new ExSvg("Resources/niu1.svg");
                canvas.DrawSvg(svg1.handle, 50, 50, 150, 150);
                var data = File.ReadAllBytes("Resources/niu1.svg");
                var ptr = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, ptr, data.Length);
                ExSvg svg2 = new ExSvg(ptr);
                canvas.DrawSvg(svg2.handle, 250, 50, 150, 150);

                ExSvg svg3 = new ExSvg("Resources/niu.svg");
                canvas.DrawSvg(svg3.handle, 250, 250, 150, 200);
                svg1.Dispose();
                svg2.Dispose();
                svg3.Dispose();
                return (IntPtr)1;
            }
            else if (uMsg == WM_LBUTTONDOWN || uMsg == WM_MOUSEMOVE || uMsg == WM_LBUTTONUP || uMsg == WM_LBUTTONDBLCLK)
            {
                var hCanvas = skin.hCanvas;
                var canvas = new ExCanvas((int)hCanvas);
                var x = Util.GET_X_LPARAM(lParam);
                var y = Util.GET_Y_LPARAM(lParam);
                canvas.HandleMouseEventForText(uMsg, x, y);
            }
            else if (uMsg == WM_KEYDOWN)
            {
                if(((int)wParam == 0x43) && (WinAPI.GetAsyncKeyState(VK_CONTROL) & 0x8000) > 0)
                {
                    var hCanvas = skin.hCanvas;
                    var canvas = new ExCanvas((int)hCanvas);
                    canvas.CopySelectedText();
                }
            }
            return IntPtr.Zero;
        }
    }
}
