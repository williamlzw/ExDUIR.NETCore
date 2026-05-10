using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Graphics;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Native;
using static ExDuiR.NET.Native.ExConst;
using System;
using ExDuiR.NET.Frameworks.Layout;

namespace ExDuiRTest
{
    static class SplitterWindow
    {
        static private ExSkin skin;
        // 主分割条、子分割条
        static private ExSplitter splitter;
        static private ExSplitter splitter2;
        // 三个静态面板
        static private ExStatic static1;
        static private ExStatic static2;
        static private ExStatic static3;
        static private ExAbsoluteLayout layout;

        static public void CreateSplitterWindow(ExSkin pOwner)
        {
            skin = new ExSkin(pOwner, null, "测试分隔条", 0, 0, 400, 330,
            WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN | WINDOW_STYLE_MOVEABLE |
            WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON | WINDOW_STYLE_NOSHADOW);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExARGB(80, 80, 90, 255);
                splitter = new ExSplitter(skin, "", 0, 0, 0, 0, OBJECT_STYLE_VISIBLE | OBJECT_STYLE_BORDER);
                layout = new ExAbsoluteLayout(skin);
                layout.SetEdge(splitter, LAYOUT_SUBPROP_ABSOLUTE_LEFT, LAYOUT_SUBPROP_ABSOLUTE_TYPE_PX, 10);
                layout.SetEdge(splitter, LAYOUT_SUBPROP_ABSOLUTE_TOP, LAYOUT_SUBPROP_ABSOLUTE_TYPE_PX, 40);
                layout.SetEdge(splitter, LAYOUT_SUBPROP_ABSOLUTE_RIGHT, LAYOUT_SUBPROP_ABSOLUTE_TYPE_PX, 10);
                layout.SetEdge(splitter, LAYOUT_SUBPROP_ABSOLUTE_BOTTOM, LAYOUT_SUBPROP_ABSOLUTE_TYPE_PX, 10);
                // 设置分割条百分比位置 35%
                splitter.SetPosition(35);
                // 设置分割条颜色
                splitter.SetColor(Util.ExARGB(50, 50, 50, 255));
                // 固定面板1（左/上面板）
                splitter.SetFixedPanel(1);
                // 设置分割条大小为5
                splitter.SetSize(5);

                // 创建面板一（静态文本控件）
                static1 = new ExStatic(splitter, "面板一", 0, 0, 100, 30, -1, -1);
                static1.ColorBackground = Util.ExARGB(50, 150, 150, 55);

                // 创建子分割条（依附于主分割条）
                splitter2 = new ExSplitter(splitter, "", 0, 0, 0, 0, -1);
                // 设置子分割条方向：1=水平
                splitter2.SetDirection(1);
                // 固定子分割条面板2（右/下面板）
                splitter2.SetFixedPanel(2);

                // 为主分割条附加面板：面板1=static1，面板2=splitter2
                splitter.SetPanel(static1.handle, splitter2.handle);

                // 创建面板二、面板三
                static2 = new ExStatic(splitter2, "面板二", 0, 0, 100, 30, -1);
                static2.ColorBackground = Util.ExARGB(150, 150, 50, 55);

                static3 = new ExStatic(splitter2, "面板三", 0, 0, 100, 30, -1);
                static3.ColorBackground = Util.ExARGB(150, 50, 150, 55);

                // 为子分割条附加面板：面板1=static2，面板2=static3
                splitter2.SetPanel(static2.handle, static3.handle);

                // 绑定布局到窗口
                skin.SetLayout(layout);
                skin.Visible = true;
            }
        }
    }
}
