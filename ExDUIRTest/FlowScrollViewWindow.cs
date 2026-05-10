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
    static class FlowScrollViewWindow
    {
        static private ExSkin skin;
        static private ExFlowScrollView flowScrollView;
        static private ExControl containerControl;

        static public void CreateFlowScrollViewWindow(ExSkin pOwner)
        {
            skin = new ExSkin(pOwner, null, "测试流式滚动容器", 0, 0, 400, 400,
            WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN | WINDOW_STYLE_MOVEABLE |
            WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON | WINDOW_STYLE_NOSHADOW);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExARGB(80, 80, 90, 255);
                flowScrollView = new ExFlowScrollView(skin, "", 40, 40, 350, 350);

                // 初始化布局配置 + 设置布局(对应C++ 结构体赋值 + 发送消息)
                ExFlowScrollViewLayoutConfig layoutConfig = new ExFlowScrollViewLayoutConfig
                {
                    nHorizontalSpacing = 50,
                    nVerticalSpacing = 50
                };

                // 将结构体转为非托管指针，传递给底层
                IntPtr pConfig = Marshal.AllocHGlobal(Marshal.SizeOf(layoutConfig));
                Marshal.StructureToPtr(layoutConfig, pConfig, false);
                flowScrollView.SetLayoutConfig(pConfig);
                Marshal.FreeHGlobal(pConfig); //释放非托管内存

                // 获取容器内部句柄(对应C++ FLOWSCROLLVIEW_MESSAGE_GET_CONTAINER_HANDLE)
                int hContainer = flowScrollView.GetContainerHandle();
                containerControl = new ExControl(hContainer);

                // 定义组件背景色
                var itemColor = Util.ExARGB(97, 175, 239, 240);

                // 创建第一个静态控件并添加到容器
                ExStatic static1 = new ExStatic(containerControl, "", 22, 22, 250, 200, OBJECT_STYLE_VISIBLE, 1001);
                static1.SetRadius(20, 20, 20, 20, true);
                static1.SetColor(COLOR_EX_BACKGROUND, itemColor, true);
                flowScrollView.AddComponent(static1);

                // 创建第二个静态控件并添加到容器
                ExStatic static2 = new ExStatic(containerControl, "", 22, 22, 250, 200, OBJECT_STYLE_VISIBLE, 1002);
                static2.SetRadius(10, 10, 10, 10, true);
                static2.SetColor(COLOR_EX_BACKGROUND, itemColor, true);
                flowScrollView.AddComponent(static2);

                // 创建第三个静态控件并添加到容器
                ExStatic static3 = new ExStatic(containerControl, "", 22, 22, 250, 200, OBJECT_STYLE_VISIBLE, 1003);
                static3.SetRadius(10, 10, 10, 10, true);
                static3.SetColor(COLOR_EX_BACKGROUND, itemColor, true);
                flowScrollView.AddComponent(static3);
                skin.Visible = true;
            }
        }
    }
}
