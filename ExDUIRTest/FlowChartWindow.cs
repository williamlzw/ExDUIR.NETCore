using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Frameworks.Graphics;
using ExDuiR.NET.Native;
using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Native;
using static ExDuiR.NET.Native.ExConst;
using System.Runtime.InteropServices;
using System;

namespace ExDuiRTest
{
    class FlowChartWindow
    {
        static private ExSkin skin;
        static private ExFlowChart flowChart;
        static private ExObjEventProcDelegate flowChartNotifyProc;
        static private ExObjEventProcDelegate buttonClickProc;

        static public void CreateFlowChartWindow(ExSkin pOwner)
        {
            skin = new ExSkin(pOwner, null, "测试流程图", 0, 0, 1400, 900,
            WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN | WINDOW_STYLE_MOVEABLE |
            WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON | WINDOW_STYLE_NOSHADOW);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExARGB(150, 150, 150, 255);
                flowChart = new ExFlowChart(skin, "", 0, 50, 1380, 780, -1, -1, -1, 200);
                flowChart.SetBackgroundColor(Util.ExARGB(120, 120, 120, 255));
                // ================= 1:1复刻C++ 5个节点 =================
                AddNode1_LoadImage();
                AddNode2_ResizeImage();
                AddNode3_TextConfig();
                AddNode4_SaveImage();
                AddNode5_Preview();

                // ================= 复刻C++ 所有连线 =================
                AddAllConnections();

                // 绑定节点执行回调
                flowChartNotifyProc = new ExObjEventProcDelegate(OnFlowChartNotifyProc);
                flowChart.HandleEvent(FLOWCHART_EVENT_EXECUTE_NODE, flowChartNotifyProc);
                DelayExecuteNode(1000);
                // 创建按钮
                CreateControlButtons();

                // 创建提示文本
                CreateTipsLabel();

                skin.Visible = true;
            }
        }


        static private void AddNode1_LoadImage()
        {
            ExFlowChartNode node = new ExFlowChartNode
            {
                id = 1001,
                x = 50,
                y = 50,
                width = 200,
                height = 260,
                title = Util.StrDupW("Load Image"),
                portCount = 3,
                ports = ExAPI.Ex_MemAlloc(Marshal.SizeOf<ExFlowChartPort>() * 3)
            };

            // 文本端口：路径输入框
            flowChart.WritePortToNode_Text(node, 0, 100, FLOWCHART_PORTTYPE_INPUT, FLOWCHART_DATATYPE_STRING, "path", 180, 25, "Resources/user.png");
            // 图片端口：预览
            flowChart.WritePortToNode_Image(node, 1, 101, FLOWCHART_PORTTYPE_INTERMEDIATE, FLOWCHART_DATATYPE_IMAGE, "preview", 180, 180);
            // 普通端口：图片输出
            flowChart.WritePortToNode(node, 2, 102, FLOWCHART_PORTTYPE_OUTPUT, FLOWCHART_DATATYPE_IMAGE, "IMAGE");

            flowChart.AddNode(node);
        }

        // 2. Resize Image 节点
        static private void AddNode2_ResizeImage()
        {
            ExFlowChartNode node = new ExFlowChartNode
            {
                id = 1002,
                x = 350,
                y = 50,
                width = 200,
                height = 260,
                title = Util.StrDupW("Resize Image"),
                portCount = 3,
                ports = ExAPI.Ex_MemAlloc(Marshal.SizeOf<ExFlowChartPort>() * 3)
            };

            flowChart.WritePortToNode(node, 0, 200, FLOWCHART_PORTTYPE_INPUT, FLOWCHART_DATATYPE_IMAGE, "image");
            flowChart.WritePortToNode_Image(node, 1, 201, FLOWCHART_PORTTYPE_INTERMEDIATE, FLOWCHART_DATATYPE_IMAGE, "preview", 180, 180);
            flowChart.WritePortToNode(node, 2, 202, FLOWCHART_PORTTYPE_OUTPUT, FLOWCHART_DATATYPE_IMAGE, "IMAGE");

            flowChart.AddNode(node);
        }

        // 3. Text Config 节点
        static private void AddNode3_TextConfig()
        {
            ExFlowChartNode node = new ExFlowChartNode
            {
                id = 1003,
                x = 50,
                y = 450,
                width = 200,
                height = 160,
                title = Util.StrDupW("Text Config"),
                portCount = 4,
                ports = ExAPI.Ex_MemAlloc(Marshal.SizeOf<ExFlowChartPort>() * 4)
            };

            flowChart.WritePortToNode_Text(node, 0, 300, FLOWCHART_PORTTYPE_INPUT, FLOWCHART_DATATYPE_STRING, "prefix", 180, 25, "output_");
            flowChart.WritePortToNode_Combo(node, 1, 301, FLOWCHART_PORTTYPE_INPUT, FLOWCHART_DATATYPE_COMBO, "format", 180, 25, CreateComboData(new[] { "PNG", "JPG" }, 0));
            flowChart.WritePortToNode_Combo(node, 2, 302, FLOWCHART_PORTTYPE_INPUT, FLOWCHART_DATATYPE_COMBO, "type", 180, 25, CreateComboData(new[] { "数据1", "数据2", "数据3" }, 0));
            flowChart.WritePortToNode(node, 3, 303, FLOWCHART_PORTTYPE_OUTPUT, FLOWCHART_DATATYPE_STRING, "CONFIG");

            flowChart.AddNode(node);
        }

        // 4. Save Image 节点
        static private void AddNode4_SaveImage()
        {
            ExFlowChartNode node = new ExFlowChartNode
            {
                id = 1004,
                x = 350,
                y = 450,
                width = 200,
                height = 160,
                title = Util.StrDupW("Save Image"),
                portCount = 3,
                ports = ExAPI.Ex_MemAlloc(Marshal.SizeOf<ExFlowChartPort>() * 3)
            };

            flowChart.WritePortToNode(node, 0, 400, FLOWCHART_PORTTYPE_INPUT, FLOWCHART_DATATYPE_IMAGE, "image");
            flowChart.WritePortToNode(node, 1, 401, FLOWCHART_PORTTYPE_INPUT, FLOWCHART_DATATYPE_STRING, "config");
            flowChart.WritePortToNode_Text(node, 2, 402, FLOWCHART_PORTTYPE_INTERMEDIATE, FLOWCHART_DATATYPE_STRING, "result", 300, 100, "等待保存...");

            flowChart.AddNode(node);
        }

        // 5. Preview 节点
        static private void AddNode5_Preview()
        {
            ExFlowChartNode node = new ExFlowChartNode
            {
                id = 1005,
                x = 650,
                y = 50,
                width = 280,
                height = 320,
                title = Util.StrDupW("Preview"),
                portCount = 2,
                ports = ExAPI.Ex_MemAlloc(Marshal.SizeOf<ExFlowChartPort>() * 2)
            };

            flowChart.WritePortToNode(node, 0, 500, FLOWCHART_PORTTYPE_INPUT, FLOWCHART_DATATYPE_IMAGE, "image");
            flowChart.WritePortToNode_Image(node, 1, 501, FLOWCHART_PORTTYPE_INTERMEDIATE, FLOWCHART_DATATYPE_IMAGE, "preview", 250, 250);

            flowChart.AddNode(node);
        }

        /// <summary>
        /// 添加所有连线 (与C++一致)
        /// </summary>
        static private void AddAllConnections()
        {
            flowChart.AddConnection(new ExFlowChartConnection { fromNode = 1001, fromSlot = 2, toNode = 1002, toSlot = 0 });
            flowChart.AddConnection(new ExFlowChartConnection { fromNode = 1002, fromSlot = 2, toNode = 1005, toSlot = 0 });
            flowChart.AddConnection(new ExFlowChartConnection { fromNode = 1002, fromSlot = 2, toNode = 1004, toSlot = 0 });
            flowChart.AddConnection(new ExFlowChartConnection { fromNode = 1003, fromSlot = 3, toNode = 1004, toSlot = 1 });
        }

        /// <summary>
        /// 创建按钮
        /// </summary>
        static private void CreateControlButtons()
        {
            ExButton btnExport = new ExButton(skin, "导出流程图到YAML", 10, 850, 200, 30);
            ExButton btnImport = new ExButton(skin, "从YAML加载流程图", 240, 850, 200, 30);

            btnExport.ID = 10001;
            btnImport.ID = 10002;

            buttonClickProc = new ExObjEventProcDelegate(OnFlowChartButtonEvent);
            btnExport.HandleEvent(NM_CLICK, buttonClickProc);
            btnImport.HandleEvent(NM_CLICK, buttonClickProc);
        }

        /// <summary>
        /// C#异步延时执行节点（替代原生Win32定时器）
        /// </summary>
        private static async void DelayExecuteNode(int milliseconds)
        {
            // 纯C#延时1秒
            await Task.Delay(milliseconds);
            // UI线程安全执行流程图事件

            flowChart.ExecuteNode(1001); // 新版：执行节点
            flowChart.ExecuteNode(1003);

        }

        /// <summary>
        /// 按钮点击回调
        /// </summary>
        static public IntPtr OnFlowChartButtonEvent(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nID == 10001)
            {
                flowChart.ExportYaml("Resources/flowchart.yaml");
            }
            else if (nID == 10002)
            {
                flowChart.ImportYaml("Resources/flowchart.yaml"); 
                DelayExecuteNode(1000);
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 节点执行回调 (核心业务逻辑，与C++一致)
        /// </summary>
        static public IntPtr OnFlowChartNotifyProc(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (lParam == IntPtr.Zero) return IntPtr.Zero;
            try
            {
                var param = Marshal.PtrToStructure<ExFlowChartExecuteParams>(lParam);
                int ioSize = Marshal.SizeOf<ExFlowChartNodeIoData>();
                switch (param.nodeId)
                {
                    case 1001:
                        var input1 = Marshal.PtrToStructure<ExFlowChartNodeIoData>(param.inputs);
                        if (ExAPI._img_createfromfile(Marshal.PtrToStringUni(input1.data), out int hImg))
                        {
                            Marshal.StructureToPtr(new ExFlowChartNodeIoData { data = (IntPtr)hImg }, param.outputs, false);
                            flowChart.UpdateNodeImage(1001, 101, hImg);
                        }
                        break;
                    case 1002:
                        var input2 = Marshal.PtrToStructure<ExFlowChartNodeIoData>(param.inputs);
                        if (ExAPI._img_scale((int)input2.data, 512, 512, out int hDst))
                        {
                            Marshal.StructureToPtr(new ExFlowChartNodeIoData { data = (IntPtr)hDst }, param.outputs, false);
                            flowChart.UpdateNodeImage(1002, 201, hDst);
                        }
                        break;
                    case 1003:
                        var i0 = Marshal.PtrToStructure<ExFlowChartNodeIoData>(IntPtr.Add(param.inputs, 0 * ioSize));
                        var i1 = Marshal.PtrToStructure<ExFlowChartNodeIoData>(IntPtr.Add(param.inputs, 1 * ioSize));
                        var i2 = Marshal.PtrToStructure<ExFlowChartNodeIoData>(IntPtr.Add(param.inputs, 2 * ioSize));

                        string prefix = Marshal.PtrToStringUni(i0.data) ?? "";
                        var fmt = Marshal.PtrToStructure<ExFlowChartNodeComboData>(i1.data);
                        var type = Marshal.PtrToStructure<ExFlowChartNodeComboData>(i2.data);
                        string res = $"{prefix}_{Marshal.PtrToStringUni(Marshal.ReadIntPtr(fmt.options, fmt.current * IntPtr.Size))}_{Marshal.PtrToStringUni(Marshal.ReadIntPtr(type.options, type.current * IntPtr.Size))}";

                        Marshal.StructureToPtr(new ExFlowChartNodeIoData { data = Util.StrDupW(res) }, param.outputs, false);
                        break;
                    case 1004:
                        var imgInput = Marshal.PtrToStructure<ExFlowChartNodeIoData>(IntPtr.Add(param.inputs, 0 * ioSize));
                        var cfgInput = Marshal.PtrToStructure<ExFlowChartNodeIoData>(IntPtr.Add(param.inputs, 1 * ioSize));
                        string cfg = Marshal.PtrToStringUni(cfgInput.data);

                        if (!string.IsNullOrEmpty(cfg)) ExAPI._img_savetofile((int)imgInput.data, $"Resources/{cfg}.png");
                        flowChart.UpdateNodeText(1004, 402, $"保存成功：{cfg}"); // 新版：文本专用方法
                        ExAPI.Ex_MemFree(cfgInput.data);
                        break;
                    case 1005:
                        var i5 = Marshal.PtrToStructure<ExFlowChartNodeIoData>(param.inputs);
                        if (ExAPI._img_copy((int)i5.data, out int hCopy))
                            flowChart.UpdateNodeImage(1005, 501, hCopy);
                        break;
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 创建提示文本
        /// </summary>
        static private void CreateTipsLabel()
        {
            ExStatic label = new ExStatic(skin, "1.选中连接线按下【DELETE】键删除\n2.按住【SHIFT】键可拉多条连接线", 500, 850, 480, 60, -1);
            label.SetColor(COLOR_EX_TEXT_NORMAL, Util.ExARGB(133, 33, 53, 255), true);
        }

        /// <summary>
        /// 统一添加节点 + 释放内存 (复刻C++ AddTestNode)
        /// </summary>
        static private void AddTestNode(ExFlowChartNode node)
        {
            // 发送消息添加节点
            int nodeSize = Marshal.SizeOf<ExFlowChartNode>();
            IntPtr pNode = ExAPI.Ex_MemAlloc(nodeSize);
            Marshal.StructureToPtr(node, pNode, false);
            flowChart.SendMessage(FLOWCHART_MESSAGE_ADD_NODE, IntPtr.Zero, pNode);
            ExAPI.Ex_MemFree(pNode);
        }

        /// <summary>
        /// 创建下拉框数据
        /// </summary>
        static private IntPtr CreateComboData(string[] opt, int cur)
        {
            IntPtr p = ExAPI.Ex_MemAlloc(Marshal.SizeOf<ExFlowChartNodeComboData>());
            ExFlowChartNodeComboData data = new ExFlowChartNodeComboData { count = opt.Length, current = cur, options = ExAPI.Ex_MemAlloc(IntPtr.Size * opt.Length) };
            for (int i = 0; i < opt.Length; i++) Marshal.WriteIntPtr(data.options, i * IntPtr.Size, Util.StrDupW(opt[i]));
            Marshal.StructureToPtr(data, p, false);
            return p;
        }

    }
}
