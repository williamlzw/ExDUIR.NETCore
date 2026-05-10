using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Native;
using System;
using System.Runtime.InteropServices;
using static ExDuiR.NET.Native.ExConst;

namespace ExDuiRTest
{
    static class MainWindow
    {
        static private ExApp theApp;
        static private ExSkin skin;
        // 委托&事件（防止GC回收）
        static private ExObjEventProcDelegate buttonClickDelegate;
        static private ExWndProcDelegate mainWndProcDelegate;

        static private ExFlowScrollView flowScrollView;
        static private ExControl containerControl;
        // 完全对标C++的按钮文本数组
        static private readonly List<string> buttonData = new List<string>
        {
            "按钮开关",       "标签",       "单选复选框",    "编辑框",     "列表框",     "菜单",
            "九宫格自定外形", "布局选项卡", "分组框",        "绝对布局",   "相对布局",   "线性布局",
            "流式布局",       "表格布局",   "组合框",        "缓动窗口",   "异型窗口",   "消息框",
            "自定义按钮",     "报表列表",   "图标列表",      "树形列表",   "矩阵",       "扩展按钮",
            "扩展编辑框",     "自定义菜单", "事件分发",      "加载动画",   "滑块条",     "旋转图片框",
            "拖动组件",       "接收拖曳",   "进度条",        "限制通知",   "模态窗口",   "标题框",
            "日期框",         "颜色选择器", "月历",          "CEF浏览框",  "打分按钮",   "轮播",
            "模板列表",       "鼠标绘制板", "调色板",        "属性框",     "原生子窗口", "全屏置顶",
            "路径与区域",     "VLC播放器",  "自定字体和SVG", "卷帘菜单",   "托盘图标",   "蒙板",
            "标注画板",       "效果器",     "打包",          "环形进度条", "水波进度条", "折线图",
            "对话盒",         "流程图",     "分隔条",        "D3D绘制" ,  "表格",       "webview2浏览器",
            "流式滚动容器",    "原型画板",   "K线图"
        };

        // 对标C++ buttonProc 函数指针数组
        static private Action<ExSkin>[] testFuncArray;
        static public void CreateMainWindow()
        {
            // ========== 1. 初始化引擎（对标C++ Ex_Init） ==========
            string resPath = Path.Combine(Environment.CurrentDirectory, "Resources");
            var themeBytes = File.ReadAllBytes(Path.Combine(resPath, "Default.bin"));
            var cursorBytes = File.ReadAllBytes(Path.Combine(resPath, "cursor.cur"));
            var bkgBytes = File.ReadAllBytes(Path.Combine(resPath, "bkg.jpg"));

            IntPtr hCursor = Util.ExLoadImage(cursorBytes, IMAGE_CURSOR);
            theApp = new ExApp(themeBytes, ENGINE_FLAG_DPI_ENABLE | ENGINE_FLAG_MENU_ALL, hCursor);

            // ========== 2. 初始化测试函数数组 ==========
            InitTestFunctionArray();

            // ========== 3. 创建主窗口（对标C++ Ex_WndCreate+Ex_DUIBindWindowEx） ==========
            mainWndProcDelegate = new ExWndProcDelegate(MainWndProc);
            skin = new ExSkin(null, null,
                "ExDUIR演示,项目地址：https://gitee.com/william_lzw/ExDUIR.NETCore",
                0, 0, 1280, 800,
                WINDOW_STYLE_MAINWINDOW | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN |
                WINDOW_STYLE_BUTTON_MAX | WINDOW_STYLE_MOVEABLE | WINDOW_STYLE_CENTERWINDOW |
                WINDOW_STYLE_ESCEXIT | WINDOW_STYLE_TITLE | WINDOW_STYLE_SIZEABLE | WINDOW_STYLE_HASICON,
                0, 0, IntPtr.Zero, mainWndProcDelegate);

            if (!skin.Validate) return;

            // ========== 4. 窗口样式设置（完全对标C++） ==========
            skin.BackgroundColor = Util.ExARGB(255, 255, 255, 255);
            skin.SetBackgroundImage(bkgBytes, 0, 0, 0, default, 0, 255, true);
            skin.ShadowColor = Util.ExARGB(250, 50, 50, 255);
            skin.Radius = 30;
            
            // 标题栏文字样式
            ExControl title = skin.Caption.GetObjFromID(WINDOW_STYLE_TITLE);
            title.ColorTextNormal = Util.ExARGB(120, 130, 220, 255);
            title.TextFormat = DT_VCENTER | DT_CENTER | DT_SINGLELINE;

            // ========== 5. 创建流式滚动容器（ID=1000，对标C++） ==========
            flowScrollView = new ExFlowScrollView(skin, "", 30, 30, 1220, 740,
                OBJECT_STYLE_VISIBLE | OBJECT_STYLE_VSCROLL, -1, -1, 1000);

            // 设置布局配置（水平24，垂直20）
            ExFlowScrollViewLayoutConfig layoutConfig = new ExFlowScrollViewLayoutConfig
            {
                nHorizontalSpacing = 24,
                nVerticalSpacing = 20
            };
            IntPtr pConfig = Marshal.AllocHGlobal(Marshal.SizeOf(layoutConfig));
            Marshal.StructureToPtr(layoutConfig, pConfig, false);
            flowScrollView.SetLayoutConfig(pConfig);
            Marshal.FreeHGlobal(pConfig);

            // 获取容器内部句柄
            int hContainer = flowScrollView.GetContainerHandle();
            containerControl = new ExControl(hContainer);

            // ========== 6. 循环创建按钮（对标C++ for循环） ==========
            buttonClickDelegate = new ExObjEventProcDelegate(ButtonClickProc);
            for (int i = 0; i < buttonData.Count; i++)
            {
                // 创建ButtonEx按钮
                ExButtonEx btn = new ExButtonEx(containerControl, buttonData[i],
                    0, 0, 100, 70, -1, -1, DT_VCENTER | DT_CENTER, 101 + i);

                // 设置按钮样式（对标C++ SetButtonStyle）
                SetButtonStyle(btn);

                // 加载图标（res\\button_icon\\{i}.png）
                LoadButtonIcon(btn, i, resPath);

                // 绑定点击事件
                btn.HandleEvent(NM_CLICK, buttonClickDelegate);

                // 添加到流式容器
                flowScrollView.AddComponent(btn);
            }

            // 更新滚动范围
            flowScrollView.UpdateScrollRange();

            // 显示窗口
            skin.Visible = true;
            // 引擎消息循环
            theApp.Run();
        }


        #region 核心功能实现
        /// <summary>
        /// 初始化测试函数数组（对标C++ buttonProc）
        /// </summary>
        static private void InitTestFunctionArray()
        {
            testFuncArray = new Action<ExSkin>[buttonData.Count];
            // 按顺序绑定所有测试窗口
            testFuncArray[0] = ButtonWindow.CreateButtonWindow;
            testFuncArray[1] = LabelWindow.CreateLabelWindow;
            testFuncArray[2] = CheckButtonWindow.CreateCheckButtonWindow;
            testFuncArray[3] = EditWindow.CreateEditWindow;
            testFuncArray[4] = ListViewWindow.CreateListViewWindow;
            testFuncArray[5] = ListButtonWindow.CreateListButtonWindow;
            testFuncArray[6] = CustomBackgroundWindow.CreateCustomBackgroundWindow;
            testFuncArray[7] = NavButtonWindow.CreateNavButtonWindow;
            testFuncArray[8] = GroupBoxWindow.CreateGroupBoxWindow;
            testFuncArray[9] = AbsoluteLayoutWindow.CreateAbsoluteLayoutWindow;
            testFuncArray[10] = RelativeLayoutWindow.CreateRelativeLayoutWindow;
            testFuncArray[11] = LinearLayoutWindow.CreateLinearLayoutWindow;
            testFuncArray[12] = FlowLayoutWindow.CreateFlowLayoutWindow;
            testFuncArray[13] = TableLayoutWindow.CreateTableLayoutWindow;
            testFuncArray[14] = ComboBoxWindow.CreateComboBoxWindow;
            testFuncArray[15] = EasingWindow.CreateEasingWindow;
            testFuncArray[16] = CustomRedrawWindow.CreateCustomRedrawWindow;
            testFuncArray[17] = MessageBoxWindow.CreateMessageBoxWindow;
            testFuncArray[18] = CustomCtrlWindow.CreateCustomCtrlWindow;
            testFuncArray[19] = ReportListViewWindow.CreateReportListViewWindow;
            testFuncArray[20] = IconListViewWindow.CreateIconListViewWindow;
            testFuncArray[21] = TreeViewWindow.CreateTreeViewWindow;
            testFuncArray[22] = MatrixWindow.CreateMatrixWindow;
            testFuncArray[23] = ButtonExWindow.CreateButtonExWindow;
            testFuncArray[24] = EditExWindow.CreateEditExWindow;
            testFuncArray[25] = CustomMenuWindow.CreateCustomMenuWindow;
            testFuncArray[26] = DispatchMessageWindow.CreateDispatchMessageWindow;
            testFuncArray[27] = LoadingWindow.CreateLoadingWindow;
            testFuncArray[28] = SliderBarWindow.CreateSliderBarWindow;
            testFuncArray[29] = RotateImageWindow.CreateRotateImageWindow;
            testFuncArray[30] = DragObjWindow.CreateDragObjWindow;
            testFuncArray[31] = DropWindow.CreateDropWindow;
            testFuncArray[32] = ProgressBarWindow.CreateProgressBarWindow;
            testFuncArray[33] = NchitTestWindow.CreateNchitTestWindow;
            testFuncArray[34] = ModalWindow.CreateModalWindow;
            testFuncArray[35] = TitleBarWindow.CreateTitleBarWindow;
            testFuncArray[36] = DateBoxWindow.CreateDateBoxWindow;
            testFuncArray[37] = ColorPickerWindow.CreateColorPickerWindow;
            testFuncArray[38] = CalendarWindow.CreateCalendarWindow;
            testFuncArray[39] = (p) => { };
            testFuncArray[40] = ScoreButtonWindow.CreateScoreButtonWindow;
            testFuncArray[41] = CarouselWindow.CreateCarouselWindow;
            testFuncArray[42] = TemplateListView.CreateTemplateListView;
            testFuncArray[43] = DrawingBoardWindow.CreateDrawingBoardWindow;
            testFuncArray[44] = PaletteWindow.CreatePaletteWindow;
            testFuncArray[45] = PropertygridWindow.CreatePropertygridWindow;
            testFuncArray[46] = (p) => { }; // 原生窗口
            testFuncArray[47] = FullScreenWindow.CreateFullScreenWindow;
            testFuncArray[48] = PathAndRegionWindow.CreatePathAndRegionWindow;
            testFuncArray[49] = MediaPlayWindow.CreateMediaPlayWindow;
            testFuncArray[50] = SVGWindow.CreateSVGWindow;
            testFuncArray[51] = RollMenuWindow.CreateRollMenuWindow;
            testFuncArray[52] = TrayWindow.CreateTrayWindow;
            testFuncArray[53] = ImageMaskWindow.CreateImageMaskWindow;
            testFuncArray[54] = TaggingBoardWindow.CreateTaggingBoardWindow;
            testFuncArray[55] = (p) => { }; // 效果器
            testFuncArray[56] = ResPackWindow.CreateResPackWindow;
            testFuncArray[57] = CircleProgressBarWindow.CreateCircleProgressBarWindow;
            testFuncArray[58] = WaveProgressBarWindow.CreateWaveProgressBarWindow;
            testFuncArray[59] = LineChartWindow.CreateLineChartWindow;
            testFuncArray[60] = ChatBoxWindow.CreateChatBoxWindow;
            testFuncArray[61] = FlowChartWindow.CreateFlowChartWindow;
            testFuncArray[62] = SplitterWindow.CreateSplitterWindow;
            testFuncArray[63] = (p) => { }; // D3D
            testFuncArray[64] = GridWindow.CreateGridWindow;
            testFuncArray[65] = (p) => { }; // WebView2
            testFuncArray[66] = FlowScrollViewWindow.CreateFlowScrollViewWindow;
            testFuncArray[67] = PrototypeBoardWindow.CreatePrototypeBoardWindow;
            testFuncArray[68] = CandlestickChartWindow.CreateCandlestickChartWindow;
        }

        /// <summary>
        /// 设置按钮样式（完全对标C++ SetButtonStyle）
        /// </summary>
        static private void SetButtonStyle(ExButtonEx btn)
        {
            ExObjProps props = new ExObjProps
            {
                crBkgNormal = Util.ExARGB(253, 253, 253, 255),
                crBkgHover = Util.ExARGB(164, 204, 253, 255),
                crBkgDownOrChecked = Util.ExARGB(142, 176, 217, 255),
                crBorderNormal = Util.ExARGB(189, 189, 191, 255),
                crBorderHover = Util.ExARGB(0, 108, 190, 255),
                crBorderDownOrChecked = Util.ExARGB(20, 126, 255, 255),
                nIconPosition = 2,
                radius = 8
            };

            IntPtr pProps = Marshal.AllocHGlobal(Marshal.SizeOf(props));
            Marshal.StructureToPtr(props, pProps, false);
            btn.SendMessage(WM_EX_PROPS, IntPtr.Zero, pProps);
            Marshal.FreeHGlobal(pProps);

            // 设置文字颜色
            btn.SetColor(COLOR_EX_TEXT_NORMAL, Util.ExARGB(89, 89, 91, 255), false);
            btn.SetColor(COLOR_EX_TEXT_HOVER, Util.ExARGB(20, 126, 255, 255), false);
            btn.SetColor(COLOR_EX_TEXT_DOWN, Util.ExARGB(19, 116, 234, 255), false);
        }

        /// <summary>
        /// 加载按钮图标（对标C++）
        /// </summary>
        static private void LoadButtonIcon(ExButtonEx btn, int index, string resPath)
        {
            string iconPath = Path.Combine(resPath, "button_icon", $"{index}.png");
            if (!File.Exists(iconPath)) return;

            ExAPI._img_createfromfile(iconPath, out var hImg);
            if (hImg == IntPtr.Zero) return;

            // 缩放为30x30
            ExAPI._img_scale(hImg, 30, 30, out var hImgSmall);
            ExAPI._img_destroy(hImg);

            // 设置图标
            btn.SendMessage(WM_SETICON, IntPtr.Zero, hImgSmall);
        }

        /// <summary>
        /// 按钮点击事件（对标C++ button_click）
        /// </summary>
        static private IntPtr ButtonClickProc(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            int index = nID - 101;
            if (index >= 0 && index < testFuncArray.Length)
            {
                testFuncArray[index]?.Invoke(skin);
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 主窗口过程（实现WM_SIZE自适应，对标C++ OnMainWndMsgProc）
        /// </summary>
        static private IntPtr MainWndProc(IntPtr hWnd, int hExDui, int uMsg, IntPtr wParam, IntPtr lParam, IntPtr pResult)
        {
            if (uMsg == WM_SIZE)
            {
                if (flowScrollView == null || !flowScrollView.Validate) return IntPtr.Zero;

                // 获取窗口宽高
                int width = Util.LOWORD((uint)lParam);
                int height = Util.HIWORD((uint)lParam);
                int margin = 30;

                // 调整流式容器大小
                flowScrollView.SetPos(margin, margin, width - 2 * margin, height - 2 * margin, 0, SWP_NOZORDER);
                // 更新滚动范围
                flowScrollView.UpdateScrollRange();
            }
            return IntPtr.Zero;
        }
        #endregion
    }
}