using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Graphics;
using ExDuiR.NET.Frameworks.Layout;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Native;
using System;
using System.Runtime.InteropServices;
using static ExDuiR.NET.Native.ExConst;

namespace ExDuiRTest
{
    static class PrototypeBoardWindow
    {
        static private ExSkin skin;
        static private ExPrototypeBoard prototypeBoard;
        static private ExObjEventProcDelegate objEvent;

        // 按钮ID定义 (与C++端完全一致)
        private const int BTN_ID_MOVE = 100;
        private const int BTN_ID_SELECT = 101;
        private const int BTN_ID_DRAW = 102;
        private const int BTN_ID_DRAW_RECT = 103;
        private const int BTN_ID_DRAW_LINE = 104;
        private const int BTN_ID_DRAW_ELLIPSE = 105;
        private const int BTN_ID_DRAW_IMAGE = 106;
        private const int BTN_ID_SET_IMAGE = 107;
        private const int BTN_ID_DRAW_TEXT = 108;
        private const int BTN_ID_SET_TEXT = 109;

        static public void CreatePrototypeBoardWindow(ExSkin pOwner)
        {
            skin = new ExSkin(pOwner, null, "测试原型画板", 0, 0, 1400, 900,
            WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN | WINDOW_STYLE_MOVEABLE |
            WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON | WINDOW_STYLE_NOSHADOW);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExARGB(80, 80, 90, 255);
                // 创建原型画板控件 
                prototypeBoard = new ExPrototypeBoard(skin, "", 50, 50, 1200, 800);

                // 创建提示标签 
                string tipText = "1.按住CTRL+鼠标滚轮可缩放画布\r\n" +
                                 "2.选择模式按住Shift可以拖动选中的图形\r\n" +
                                 "3.选择模式可以拖动红色控制点改变形状大小\r\n" +
                                 "4.绘制模式鼠标点选多点绘制路径，可以闭合路径，右键可以取消上一次绘制\r\n" +
                                 "5.选择模式选中图片或文本形状可以插入图片或文本";
                objEvent = new ExObjEventProcDelegate(OnPrototypeBoardButtonEvent);
                ExStatic label = new ExStatic(skin, tipText, 1280, 30, 100, 300, -1);
                label.TextFormat = DT_WORDBREAK; // 自动换行

                // 创建10个功能按钮 + 绑定点击事件
                CreateButton("拖动模式", 350, BTN_ID_MOVE);
                CreateButton("选择模式", 390, BTN_ID_SELECT);
                CreateButton("绘制模式", 430, BTN_ID_DRAW);
                CreateButton("添加矩形形状", 470, BTN_ID_DRAW_RECT);
                CreateButton("添加直线形状", 510, BTN_ID_DRAW_LINE);
                CreateButton("添加椭圆形状", 550, BTN_ID_DRAW_ELLIPSE);
                CreateButton("添加图片形状", 590, BTN_ID_DRAW_IMAGE);
                CreateButton("插入图片", 630, BTN_ID_SET_IMAGE);
                CreateButton("添加文本形状", 670, BTN_ID_DRAW_TEXT);
                CreateButton("插入文本", 710, BTN_ID_SET_TEXT);
                skin.Visible = true;
            }
        }

        /// <summary>
        /// 统一创建按钮（简化代码）
        /// </summary>
        static private ExButton CreateButton(string text, int y, int id)
        {
            ExButton btn = new ExButton(skin, text, 1280, y, 100, 30, -1, -1, -1, id);
            btn.HandleEvent(NM_CLICK, objEvent); // 绑定点击事件
            return btn;
        }

        /// <summary>
        /// 按钮点击事件处理（对应C++ OnPrototypeBoardButtonEvent回调）
        /// </summary>
        static private IntPtr OnPrototypeBoardButtonEvent(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            ExButton btn = new ExButton(hObj);
      
            switch (btn.ID)
            {
                // 拖动模式
                case BTN_ID_MOVE:
                    prototypeBoard.SetMode(PROTOTYPEBOARD_MODE_MOVE);
                    break;
                // 选择模式
                case BTN_ID_SELECT:
                    prototypeBoard.SetMode(PROTOTYPEBOARD_MODE_SELECT);
                    break;
                // 绘制模式
                case BTN_ID_DRAW:
                    prototypeBoard.SetMode(PROTOTYPEBOARD_MODE_DRAW);
                    break;
                // 绘制矩形
                case BTN_ID_DRAW_RECT:
                    prototypeBoard.DrawRect();
                    break;
                // 绘制直线
                case BTN_ID_DRAW_LINE:
                    prototypeBoard.DrawLine();
                    break;
                // 绘制椭圆
                case BTN_ID_DRAW_ELLIPSE:
                    prototypeBoard.DrawEllipse();
                    break;
                // 添加图片形状
                case BTN_ID_DRAW_IMAGE:
                    prototypeBoard.DrawImage();
                    break;
                // 插入图片（加载本地图片+设置到画板）
                case BTN_ID_SET_IMAGE:
                    {
                        ExImage image = new ExImage("Resources/user.png");
                        // FALSE=拉伸图片，对应C++ (LPARAM)FALSE
                        prototypeBoard.SetImage(image, false);
                        break;
                    }
                   
                // 添加文本形状
                case BTN_ID_DRAW_TEXT:
                    prototypeBoard.DrawText();
                    break;
                // 插入文本（创建字体+设置文本）
                case BTN_ID_SET_TEXT:
                    {
                        ExFont font = new ExFont("微软雅黑", 16, 0);
                        prototypeBoard.SetText(font, "这是文本内容");
                        break;
                    }
            }
            return IntPtr.Zero;
        }
    }
}
