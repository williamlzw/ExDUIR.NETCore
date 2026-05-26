using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Frameworks.Graphics;
using ExDuiR.NET.Native;
using static ExDuiR.NET.Native.ExConst;
using System.Runtime.InteropServices;
using System;
using System.Text;

namespace ExDuiRTest
{
    class ChatBoxWindow
    {
        static private ExSkin skin;
        static private ExChatBox chatbox;
        static private ExObjEventProcDelegate objProc;
        static private ExObjEventProcDelegate chatboxProc;
        static private ExEdit edit;
        static private ExButton button1;
        static private ExButton button2;
        static private ExButton button3;
        static private ExButton button4;
        static private ExButton button5;
        static private string markdownText = """
            # 🛑 Markdown 全元素测试文档
            
            ## 1. 标题层级 (Headers)
            这是一级到六级标题的演示：
            # 一级标题 (H1)
            ## 二级标题 (H2)
            ### 三级标题 (H3)
            #### 四级标题 (H4)
            ##### 五级标题 (H5)
            ###### 六级标题 (H6)
                                ---
            
            ## 2. 文本样式 (Inline Styles)
            这是**粗体文本 (Bold)**，这是*斜体文本 (Italic)*。这是***粗斜体文本 (Bold_Italic)***。
            这是`行内代码var a = 2; (Inline Code)`，这是~~删除线 (Strikethrough)~~。
            
            ---
            
            ## 3. 列表 (Lists)
            
            ### 3.1 无序列表 (Unordered)
            - 列表项 A
            - 列表项 B
              - 嵌套子项 B-1
              - 嵌套子项 B-2
            - 列表项 C
            
            ### 3.2 有序列表 (Ordered)
            1. 第一项
            2. 第二项
               1. 嵌套有序项
               2. 嵌套有序项
            3. 第三项
            
            ---
            
            ## 4. 引用块 (Blockquote)
            > **注意**:这是一段引用文本。
            > 可以包含多行。
            > > 这是嵌套引用 (Nested Quote)。
            
            ---
            
            ## 5. 代码块 (Code Blocks)
            
            ### 5.1 普通文本块
            ```plaintext
            这是一个普通的文本代码块，
            ⚠️没有语法高亮。 
            ```
            ### 5.2 C++ 代码块 
            ```cpp
            #include <iostream>
            int main() {
                std::cout << "Hello, ExDUIR!" << std::endl;
                return 0;
            }
            ```
            ### 5.3 Python 代码块  
            ```python
            def calculate_days(year, month):
                import calendar
                return calendar.monthrange(year, month)[1]
            print(calculate_days(2026, 4))
            ```
            ## 6. 图片与链接 (Images & Links)
            ### 6.1 图片 (Image)
            ![测试图片](https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png) 
            
            ### 6.2 链接 (Link)
            - 行内链接：[访问百度](https://www.baidu.com)
            
            ## 7.表格
            | 表头1 | 表头2 | 表头3 |
            |-------|-------|-------|
            | *** 单元格 *** | `单元格` | [访问百度](https://www.baidu.com) |
            | ** 数据1 ** | ~~数据2~~  | 数据3  |
            | * 改动点 * | 说明 | 数据4  |
            """;

        static public void CreateChatBoxWindow(ExSkin pOwner)
        {
            skin = new ExSkin(pOwner, null, "测试对话盒", 0, 0, 1100, 1000,
            WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN | WINDOW_STYLE_MOVEABLE |
            WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON | WINDOW_STYLE_NOSHADOW);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExARGB(80, 80, 90, 255);
                objProc = new ExObjEventProcDelegate(OnChatButtonEvent);
                chatboxProc = new ExObjEventProcDelegate(OnChatBoxEvent);
                edit = new ExEdit(skin, "测试多行编辑框\n测试多行编辑框测试多行编辑框\n测试多行编辑框测试多行编辑框测试多行编辑框\n测试多行编辑框测试多行编辑框测试多行编辑框测试多行编辑框",
                    50, 830, 600, 100, OBJECT_STYLE_VISIBLE | OBJECT_STYLE_VSCROLL, OBJECT_STYLE_EX_FOCUSABLE | OBJECT_STYLE_EX_COMPOSITED, DT_VCENTER);
                
                button1 = new ExButton(skin, "发送助手", 50, 950, 100, 30, -1, -1, DT_CENTER | DT_VCENTER, 101);
                button2 = new ExButton(skin, "发送用户", 180, 950, 100, 30, -1, -1, DT_CENTER | DT_VCENTER, 102);
                button3 = new ExButton(skin, "流式更新文本", 310, 950, 100, 30, -1, -1, DT_CENTER | DT_VCENTER, 103);
                button4 = new ExButton(skin, "更新卡片", 440, 950, 100, 30, -1, -1, DT_CENTER | DT_VCENTER, 104);
                button5 = new ExButton(skin, "取内容", 570, 950, 100, 30, -1, -1, DT_CENTER | DT_VCENTER, 105);
                button1.HandleEvent(NM_CLICK, objProc);
                button2.HandleEvent(NM_CLICK, objProc);
                button3.HandleEvent(NM_CLICK, objProc);
                button4.HandleEvent(NM_CLICK, objProc);
                button5.HandleEvent(NM_CLICK, objProc);

                chatbox = new ExChatBox(skin, "", 50, 50, 1000, 750);
                chatbox.HandleEvent(CHATBOX_EVENT_CLICKBUTTON, chatboxProc);
                chatbox.HandleEvent(CHATBOX_EVENT_CLICKLINK, chatboxProc);

                chatbox.AssistantImg = new ExImage("Resources/ai.png");
                chatbox.UserImg = new ExImage("Resources/user.png");
                chatbox.AddItemText("输出更多样式", true);

                //============================
                chatbox.AddItemCard(new ExImage("Resources/user.png"), 
                    "测试卡片标题!",
                    "测试卡片内容测试卡片内容测试卡片内容测试卡片内容测试卡片内容测试卡片内容测试卡片内容测试卡片内容测试卡片内容",
                    "测试卡片子标题",
                    "测试卡片子内容测试卡片子内容测试卡片子内容测试卡片子内容测试卡片子内容测试卡片子内容测试卡片子内容测试卡片子内容.",
                    "测试按钮");

                //============================
                chatbox.AddItemMode(new ExImage("Resources/user.png"),
                   "完成标题",
                   "完成内容!");

                //============================
                List<ExChatBoxItemInfoErrorListUnit> units = new List<ExChatBoxItemInfoErrorListUnit>();
                ExChatBoxItemInfoErrorListUnit unit1 = new ExChatBoxItemInfoErrorListUnit
                {
                    errorCode = Marshal.StringToHGlobalUni("错误ID"),
                    errorCodeText = Marshal.StringToHGlobalUni("20"),
                    description = Marshal.StringToHGlobalUni("错误描述"),
                    descriptionText = Marshal.StringToHGlobalUni("错误详情\r\n[来源]Microsoft-Windows-WindowsUpdateClient\r\n[创建时间]2025-06-24 23:25:18\r\n[记录ID]26994"),
                };
                units.Add(unit1);
                ExChatBoxItemInfoErrorListUnit unit2 = new ExChatBoxItemInfoErrorListUnit
                {
                    errorCode = Marshal.StringToHGlobalUni("错误ID"),
                    errorCodeText = Marshal.StringToHGlobalUni("20"),
                    description = Marshal.StringToHGlobalUni("错误描述"),
                    descriptionText = Marshal.StringToHGlobalUni("错误详情\r\n[来源]Microsoft-Windows-WindowsUpdateClient\r\n[创建时间]2025-06-24 23:25:18\r\n[记录ID]26994\r\n错误详情\r\n[来源]Microsoft-Windows-WindowsUpdateClient\r\n[创建时间]2025-06-24 23:25:18\r\n[记录ID]26994"),
                };
                units.Add(unit2);
                ExChatBoxItemInfoErrorListUnit unit3 = new ExChatBoxItemInfoErrorListUnit
                {
                    errorCode = Marshal.StringToHGlobalUni("错误ID"),
                    errorCodeText = Marshal.StringToHGlobalUni("20"),
                    description = Marshal.StringToHGlobalUni("错误描述"),
                    descriptionText = Marshal.StringToHGlobalUni("错误详情\r\n[来源]Microsoft-Windows-WindowsUpdateClient\r\n[创建时间]2025-06-24 23:25:18\r\n[记录ID]26994"),
                };
                units.Add(unit3);
                chatbox.AddItemErrorList(new ExImage("Resources/user.png"),
                    "一些错误被捕捉",
                    units);

                //============================
                List<ExChatBoxItemInfoInfoListUnit> unitsInfo = new List<ExChatBoxItemInfoInfoListUnit>();
                ExChatBoxItemInfoInfoListUnit unitInfo1 = new ExChatBoxItemInfoInfoListUnit
                {
                    title = Marshal.StringToHGlobalUni("CPU"),
                    description = Marshal.StringToHGlobalUni("Intel i9 14900k")
                };
                unitsInfo.Add(unitInfo1);
                ExChatBoxItemInfoInfoListUnit unitInfo2 = new ExChatBoxItemInfoInfoListUnit
                {
                    title = Marshal.StringToHGlobalUni("网络"),
                    description = Marshal.StringToHGlobalUni("●Intel Ethernet Connection 1219-LM\n●TP-LINK Wireless N Adapter\n●Marvell AQtion 10Gbit Network Adapter")
                };
                unitsInfo.Add(unitInfo2);
                ExChatBoxItemInfoInfoListUnit unitInfo3 = new ExChatBoxItemInfoInfoListUnit
                {
                    title = Marshal.StringToHGlobalUni("GPU"),
                    description = Marshal.StringToHGlobalUni("NVIDIA RTX 4090")
                };
                unitsInfo.Add(unitInfo3);
                chatbox.AddItemInfoList(new ExImage("Resources/user.png"),
                    "以下是信息列表",
                    unitsInfo);

             

                //============================
                var links = new List<string>
                {
                     "测试条目一",
                    "测试条目二测试条目二测试条目二", 
                    "测试条目三\n测试条目三"
                };
                chatbox.AddItemInfoLink(
                    content: "测试标题",
                    title: "副标题",
                    linkItems: links
                );
                //============================
                var tableData = new List<string[]>
                {
                    new string[] { "Header1", "Header2", "Header3", "Header4" },
                    new string[] { "Row1Col1", "Row1Col2", "行1列3\n多行文本", "Row1Col4" },
                    new string[] { "Row2Col1", "行2列2\n多行文本", "Row2Col3", "Row2Col4" },
                    new string[] { "Row3Col1", "Row3Col2", "Row3Col3", "行3列4\n多行文本\n多行文本" }
                };

                // 调用安全方法
                chatbox.AddItemTableList(
                    content: "表格测试",
                    rows: tableData,
                    totalColumnCount: 4
                );

                chatbox.AddItemMarkdown(markdownText);
                ExAPI.Ex_Sleep(1000);
               
                skin.Visible = true;
            }
        }

        static public IntPtr OnChatBoxEvent(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == CHATBOX_EVENT_CLICKLINK)
            {
                Console.WriteLine($"链接点击 {wParam}, {lParam}");
            }
            else if (nCode == CHATBOX_EVENT_CLICKBUTTON)
            {
                Console.WriteLine($"卡片按钮点击 {wParam}, {lParam}");
            }
            return IntPtr.Zero;
        }

        static public IntPtr OnChatButtonEvent(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nID == 101)
            {
                var text = edit.Text;
                chatbox.AddItemText(text, false);
            }
            else if(nID == 102)
            {
                var text = edit.Text;
                chatbox.AddItemText(text, true);
            }
            else if (nID == 103)
            {
                //添加一个空文本项
                chatbox.AddItemMarkdown("");
                var itemCount = chatbox.GetItemCount();
                var nUpdateIndex = itemCount - 1;
                string streamText = markdownText;
                StringBuilder currentText = new StringBuilder();
                foreach(var ch in streamText)
                {
                    currentText.Append(ch);
                    chatbox.UpdateItemMarkdown(nUpdateIndex, currentText.ToString());
                    ExAPI.Ex_Sleep(100);
                }
            }
            else if (nID == 104)
            {
                chatbox.UpdateItemCard(1, new ExImage("Resources/user.png"),
                    "测试卡片标题2!",
                    "测试卡片内容2测试卡片内容2",
                    "测试卡片子标题2",
                    "测试卡片子内容2测试卡片子内容2.",
                    "测试按钮2");
            }
            else if (nID == 105)
            {
                var type = chatbox.GetItemType(1);
                var dataCount = chatbox.GetItemCount();
                var str = chatbox.GetItemText(0);
                var card = chatbox.GetItemCard(1);
                Console.WriteLine($"类型{type}, 总数{dataCount}, {str}, {Marshal.PtrToStringUni(card.buttonText)}");
            }
            return IntPtr.Zero;
        }
    }
}
