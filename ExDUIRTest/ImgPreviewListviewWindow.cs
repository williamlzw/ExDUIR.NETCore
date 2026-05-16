using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Native;
using System;
using static ExDuiR.NET.Native.ExConst;

namespace ExDuiRTest
{
    static class ImgPreviewListviewWindow
    {
        // 控件对象
        static private ExSkin skin;
        static private ExImagePreviewListView listView;
        static private ExButton btnAdd, btnDelete, btnGetInfo, btnGetAllInfo;

        // 事件委托
        static private ExObjEventProcDelegate btnEvent;

        static public void CreateImgPreviewListviewWindow(ExSkin pOwner)
        {
            // 创建主窗口（完全匹配C++窗口样式）
            skin = new ExSkin(pOwner, null, "测试图片预览列表", 0, 0, 450, 300,
                WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN |
                WINDOW_STYLE_MOVEABLE | WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON);

            if (skin.Validate)
            {
                // 背景色（匹配C++ ExARGB(80,80,90,255)）
                skin.BackgroundColor = Util.ExARGB(80, 80, 90, 255);

                // 1. 创建图片预览列表（匹配C++坐标、尺寸）
                listView = new ExImagePreviewListView(skin, "", 10, 30, 300, 210);

                // 2. 初始插入3张图片（C++：nIndex=0 插入头部）
                InsertTestImage(@"Resources/carousel1.jpeg");
                InsertTestImage(@"Resources/carousel2.jpeg");
                InsertTestImage(@"Resources/carousel3.jpeg");

                // 3. 设置项尺寸（180x180 匹配C++）
                listView.SetItemSize(180, 180);
                listView.Update();

                // 4. 创建三个按钮（匹配C++坐标、文本）
                btnAdd = new ExButton(skin, "添加图片", 320, 30, 100, 30);
                btnDelete = new ExButton(skin, "删除选中图片", 320, 70, 100, 30);
                btnGetInfo = new ExButton(skin, "获取选中信息", 320, 110, 100, 30);
                btnGetAllInfo = new ExButton(skin, "获取全部信息", 320, 150, 100, 30); // 新增按钮
                // 5. 绑定按钮点击事件（和C#按钮例子一致）
                btnEvent = new ExObjEventProcDelegate(OnButtonClick);
                btnAdd.HandleEvent(NM_CLICK, btnEvent);
                btnDelete.HandleEvent(NM_CLICK, btnEvent);
                btnGetInfo.HandleEvent(NM_CLICK, btnEvent);
                btnGetAllInfo.HandleEvent(NM_CLICK, btnEvent); // 绑定事件
                skin.Visible = true;
            }
        }

        #region 工具方法：插入测试图片
        static private void InsertTestImage(string path)
        {
            listView.InsertItem(0, path);
        }
        #endregion

        #region 按钮点击事件（完全复刻C++逻辑）
        static public IntPtr OnButtonClick(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            // 1. 添加图片按钮
            if (hObj == btnAdd.handle)
            {
                InsertTestImage(@"Resources/carousel1.jpeg");
                listView.Update();
            }
            // 2. 删除选中图片按钮
            else if (hObj == btnDelete.handle)
            {
                listView.DeleteSelectedItem();
                listView.Update();
            }
            // 3. 获取选中信息按钮
            else if (hObj == btnGetInfo.handle)
            {
                if (listView.GetSelectedItemInfo(out int index, out string path))
                {
                    Console.WriteLine($"图片信息：索引={index}，路径={path}");

                }
            }
            else if (hObj == btnGetAllInfo.handle)
            {
                var allItems = listView.GetAllItems();
                if (allItems.Count == 0)
                {
                    ExMessageBox.Show(skin, "列表中无任何图片", "提示", MB_ICONWARNING);
                    return IntPtr.Zero;
                }

                string msg = $"总数量：{allItems.Count}\r\n\r\n";
                foreach (var item in allItems)
                {
                    msg += $"索引：{item.Index} | 路径：{item.FilePath}\r\n";
                }
                Console.WriteLine(msg);
            }
            return IntPtr.Zero;
        }
        #endregion
    }
}