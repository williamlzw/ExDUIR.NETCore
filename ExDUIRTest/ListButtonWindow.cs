using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Graphics;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Native;
using static ExDuiR.NET.Native.ExConst;
using System.Runtime.InteropServices;

namespace ExDuiRTest
{
    static class ListButtonWindow
    {
        static private ExSkin skin;
        static private ExMenuBar menubar;
        static private ExMenuBar menubar2;
        static private ExToolBar toolbar;
        static private ExStatusBar statusbar;
        static private ExImageList imglist;
        static private ExObjProcDelegate listButtonProc;
        static private ExWndProcDelegate wndProc;
        static private ExObjEventProcDelegate listButtonEventProc;
        static private ExObjProcDelegate objProc;
        static private List<int> menuItems;
        static private List<string> menuItemsName;
        static private int mainMenu;
        static public void CreateListButtonWindow(ExSkin pOwner)
        {
            wndProc = new ExWndProcDelegate(OnListButtonWndProc);
            skin = new ExSkin(pOwner, null, "测试列表按钮", 0, 0, 480, 200,
            WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN | WINDOW_STYLE_MOVEABLE |
            WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON | WINDOW_STYLE_NOSHADOW, 0, 0, IntPtr.Zero, wndProc);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExARGB(150, 150, 150, 255);
                objProc = new ExObjProcDelegate(OnMenuItemMsgProc);
                Dictionary<string, List<string>> itemInfo = new Dictionary<string, List<string>>();
                //分隔条用"-"
                itemInfo.Add("文件(&F)", new List<string> { "新建(&N)", "打开(&O)", "保存(&S)", "-", "退出(&E)" });
                itemInfo.Add("编辑(&E)", new List<string> { "剪切(&T)", "复制(&C)", "粘贴(&P)", "-", "撤销(&U)", "重做(&R)" });
                //快捷键和标题之间用\t
                itemInfo.Add("选项(&O)", new List<string> { "设置(&S)\tCtrl+S" });
                itemInfo.Add("帮助(&H)", new List<string> { "关于(&A)", "文档(&D)", "Git" });

                List<string> subItem2Str = new List<string> { "https://gitee.com/william_lzw/ExDUIR", "https://github.com/williamlzw/ExDUIR" };
                List<string> enableItemKey = new List<string> { "粘贴(&P)", "撤销(&U)", "重做(&R)" };
                List<string> subItemKey = new List<string> { "Git" };
                mainMenu = ExAPI.Ex_MenuCreateMenu();
                menuItems = new List<int>();
                menuItemsName = new List<string>();
                uint menuIndex = 0;
                foreach (var index in itemInfo)
                {
                    //创建一级弹出菜单
                    var menuItem = ExAPI.Ex_MenuCreatePopupMenu();
                    uint i = 0;
                    foreach (var itemIndex in index.Value)
                    {
                        if (itemIndex == "-")
                        {
                            ExAPI.Ex_MenuAppendMenuW(menuItem, MF_SEPARATOR, i, "");
                        }
                        else
                        {
                            if (subItemKey.Contains(itemIndex))
                            {
                                var subItem = ExAPI.Ex_MenuCreatePopupMenu();
                                nuint jj = 0;
                                foreach (var subItem2Index in subItem2Str)
                                {
                                    ExAPI.Ex_MenuAppendMenuW(subItem, MF_STRING, jj, subItem2Index);
                                    jj++;
                                }
                                // 挂载二级弹出菜单
                                ExAPI.Ex_MenuAppendMenuW(menuItem, MF_POPUP, (nuint)subItem, itemIndex);
                            }
                            else
                            {
                                ExAPI.Ex_MenuAppendMenuW(menuItem, MF_STRING, i, itemIndex);
                            }
                        }

                        if (enableItemKey.Contains(itemIndex))
                        {
                            ExAPI.Ex_MenuEnableItem(menuItem, i, MF_DISABLED | MF_BYPOSITION);
                        }
                        i++;
                    }

                    // ✅【核心修复1】主菜单添加一级弹出菜单，必须用 MF_POPUP + 菜单句柄
                    ExAPI.Ex_MenuAppendMenuW(mainMenu, MF_POPUP, (nuint)menuItem, index.Key);
                    menuItems.Add(menuItem);
                    menuItemsName.Add(index.Key);
                    menuIndex++;
                }

                menubar = new ExMenuBar(skin, "", 0, 30, 250, 24);
                menubar.ColorTextNormal = Util.ExRGB2ARGB(0, 255);
                menubar.ColorTextHover = Util.ExRGB2ARGB(16774117, 255);
                menubar.ColorTextDown = Util.ExRGB2ARGB(16765337, 255);
         
                listButtonProc = new ExObjProcDelegate(OnListButtonMsgProc);
                
                menubar2 = new ExMenuBar(skin, "", 0, 60, 250, 24, -1, -1, -1, 0, IntPtr.Zero, listButtonProc);
                menubar2.ColorBackground = Util.ExARGB(110, 120, 55, 255);//改变菜单按钮背景色
                menubar2.ColorTextNormal = Util.ExARGB(255, 255, 255, 255);//改变菜单按钮字体正常色
                menubar2.ColorTextHover = Util.ExARGB(255, 255, 255, 55);//改变菜单按钮字体悬浮色
                menubar2.ColorTextDown = Util.ExARGB(255, 255, 255, 100);//改变菜单按钮字体按下色
                
                //列表按钮插入一级菜单句柄
                var j = 0;
                foreach (var index in menuItems)
                {
                    ExListButtonItemInfo item1info = new ExListButtonItemInfo()
                    {
                        wzText = Marshal.StringToHGlobalUni(menuItemsName[j]),
                        nMenu = index
                    };
                    
                    //把一级菜单句柄加入列表按钮
                    menubar.InsertItem(item1info);
                    menubar2.InsertItem(item1info);
                    j++;
                }

                var bitmap = File.ReadAllBytes("Resources/nav1.png");
                ExImage image = new ExImage(bitmap, bitmap.Length);
                image.Scale(24, 24, out var smallImg);
    
                ExMENUITEMINFOW minfo = new ExMENUITEMINFOW();
                minfo.cbSize = (uint)Marshal.SizeOf(minfo);

                minfo.fMask = 128;
                var hSubMenu = ExAPI.Ex_MenuGetSubMenu(mainMenu, 0);
    
                var ret = ExAPI.Ex_MenuGetItemInfoW(hSubMenu, 1, true, ref minfo);
                if (minfo.hbmpItem != 0)
                {
                    ExAPI._img_destroy(minfo.hbmpItem);
                }
                minfo.hbmpItem = smallImg.handle;
                ret = ExAPI.Ex_MenuSetItemInfoW(hSubMenu, 1, true,ref minfo);
                image.Dispose();
                skin.LParam = (IntPtr)smallImg.handle;//把图片句柄保存到窗口的LParam，窗口销毁时销毁图片

                toolbar = new ExToolBar(skin, "", 0, 90, 400, 24);
                toolbar.ColorTextNormal = Util.ExRGB2ARGB(0, 255);
                toolbar.ColorTextHover = Util.ExRGB2ARGB(16774117, 255);
                toolbar.ColorTextDown = Util.ExRGB2ARGB(16765337, 255);
                bitmap = File.ReadAllBytes("Resources/icon.png");
                imglist = new ExImageList(18, 18);
                ExImage image2 = new ExImage(bitmap, bitmap.Length);
                var nImageIndex = imglist.AddImage(image2, 0);
                toolbar.SetImageList(imglist);
                ExListButtonItemInfo item1info2 = new ExListButtonItemInfo()
                {
                    nType = 1,
                    nImage = nImageIndex,
                    wzText = IntPtr.Zero,
                    wzTips = Marshal.StringToHGlobalUni("普通按钮1")
                };
                toolbar.InsertItem(item1info2);
                item1info2.nType = 1;
                item1info2.nImage = 0;
                item1info2.wzText = Marshal.StringToHGlobalUni("普通按钮不带图标");
                item1info2.wzTips = Marshal.StringToHGlobalUni("普通按钮不带图标");
                toolbar.InsertItem(item1info2);
                item1info2.nType = 2;
                item1info2.nImage = nImageIndex;
                item1info2.wzText = IntPtr.Zero;
                item1info2.wzTips = Marshal.StringToHGlobalUni("选择按钮");
                toolbar.InsertItem(item1info2);
                item1info2.nType = 2;
                item1info2.nImage = 0;
                item1info2.wzText = Marshal.StringToHGlobalUni("选择按钮不带图标");
                item1info2.wzTips = Marshal.StringToHGlobalUni("选择按钮不带图标");
                toolbar.InsertItem(item1info2);
                item1info2.nType = 0;
                item1info2.nImage = 0;
                item1info2.wzText = IntPtr.Zero;
                item1info2.wzTips = IntPtr.Zero;
                item1info2.dwState = STATE_NORMAL;
                toolbar.InsertItem(item1info2);
                item1info2.nType = 1;
                item1info2.nImage = nImageIndex;
                item1info2.wzText = Marshal.StringToHGlobalUni("禁用按钮带图标");
                item1info2.wzTips = Marshal.StringToHGlobalUni("禁用按钮带图标");
                item1info2.dwState = STATE_DISABLE;
                toolbar.InsertItem(item1info2);

                listButtonEventProc = new ExObjEventProcDelegate(OnListButtonEvent);
                toolbar.HandleEvent(LISTBUTTON_EVENT_CLICK, listButtonEventProc);
                toolbar.HandleEvent(LISTBUTTON_EVENT_CHECK, listButtonEventProc);

                statusbar = new ExStatusBar(skin, "", 0, 120, 300, 24);
                statusbar.ColorBackground = Util.ExRGB2ARGB(12557930, 255);
                statusbar.ColorBorder = Util.ExARGB(255, 255, 255, 255);
                statusbar.ColorTextNormal = Util.ExARGB(255, 255, 255, 255);

                ExListButtonItemInfo item1info3 = new ExListButtonItemInfo();
                item1info3.nWidth = 100;
                item1info3.iTextFormat = DT_LEFT;
                item1info3.wzText = Marshal.StringToHGlobalUni("左对齐");
                statusbar.InsertItem(item1info3);
                item1info3.nWidth = 100;
                item1info3.iTextFormat = DT_CENTER;
                item1info3.wzText = Marshal.StringToHGlobalUni("居中");
                statusbar.InsertItem(item1info3);
                item1info3.nWidth = 100;
                item1info3.iTextFormat = DT_RIGHT;
                item1info3.nIndex = 2;
                item1info3.wzText = Marshal.StringToHGlobalUni("右对齐");
                statusbar.InsertItem(item1info3);



                skin.Visible = true;
            }
        }

        static private IntPtr OnListButtonWndProc(IntPtr hWnd, int hExDui, int uMsg, IntPtr wParam, IntPtr lParam, IntPtr pResult)
        {
            if (uMsg == WM_DESTROY)
            {
                ExAPI.Ex_MenuDestroy(mainMenu);
                mainMenu = 0;
                var hImg = (int)skin.LParam;
                if(hImg != 0)
                {
                    ExAPI._img_destroy(hImg);
                    skin.LParam = IntPtr.Zero;
                }
            }
            return IntPtr.Zero;
        }

        static private IntPtr OnListButtonEvent(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == LISTBUTTON_EVENT_CLICK)
            {
                Console.WriteLine($"点击项目索引:{wParam}");
            }
            else if (nCode == LISTBUTTON_EVENT_CHECK)
            {
                Console.WriteLine($"选择项目索引:{wParam},状态:{lParam}");
            }
            return IntPtr.Zero;
        }

        static private IntPtr OnMenuItemMsgProc(IntPtr hWnd, int hObj, int uMsg, IntPtr wParam, IntPtr lParam, IntPtr pResult)
        {
            if (uMsg == WM_EX_LCLICK)
            {
                ExControl obj = new ExControl(hObj);
                var text = obj.Text;
                Console.WriteLine($"菜单项目text:{text}");
            }
            return IntPtr.Zero;
        }

        static private IntPtr OnListButtonMsgProc(IntPtr hWnd, int hObj, int uMsg, IntPtr wParam, IntPtr lParam, IntPtr lpResult)
        {
            if (uMsg == LISTBUTTON_MESSAGE_DOWNITEM)
            {
                var rcWindow = skin.WindowRect;
                var itemInfo = Util.IntPtrToStructure<ExListButtonItemInfo>(lParam);
                int hMenu = (int)itemInfo.nMenu;
                if(ExAPI.Ex_MenuIsMenu(hMenu))
                {
                    ExMenuBar button = new ExMenuBar(hObj);
                    var rcObj = button.WindowRect;
                    int posX = rcWindow.nLeft + rcObj.nLeft + itemInfo.nLeft;
                    button.TrackPopupMenu(hMenu, 1, posX, rcWindow.nTop + (int)ExAPI.Ex_Scale(rcObj.nBottom), 0);
                }
                
                Marshal.WriteInt32(lpResult, 1);
                return (IntPtr)1;
            }
            return IntPtr.Zero;
        }
    }
}
