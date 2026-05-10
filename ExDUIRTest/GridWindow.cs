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
    static class GridWindow
    {
        static private ExSkin skin;
        static private ExGrid grid;
        static private ExButton btnPrint;
        static private ExObjEventProcDelegate objEvent;

        static public void CreateGridWindow(ExSkin pOwner)
        {
            skin = new ExSkin(pOwner, null, "测试表格", 0, 0, 800, 500,
            WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN | WINDOW_STYLE_MOVEABLE |
            WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON | WINDOW_STYLE_NOSHADOW);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExARGB(80, 80, 90, 255);
                // ===================== 核心：移植C++ testgrid 逻辑 =====================
                // 1. 创建表格控件（坐标、样式完全对齐）
                int gridStyle = OBJECT_STYLE_VISIBLE | OBJECT_STYLE_VSCROLL | OBJECT_STYLE_HSCROLL;
                grid = new ExGrid(skin, "", 15, 35, 760, 410, gridStyle);

                // 2. 创建打印按钮（ID=101，对应C++）
                btnPrint = new ExButton(skin, "表格打印测试", 15, 455, 100, 30, -1, 101, DT_VCENTER | DT_CENTER);
                objEvent = new ExObjEventProcDelegate(OnGridButtonEvent);
                btnPrint.HandleEvent(NM_CLICK, objEvent);

                // 3. 表格基础配置（101行，16列，固定1行1列）
                const int n_row = 101;
                const int n_col = 16;
                const int m_nFixRows = 1;
                const int m_nFixCols = 1;

                grid.SetRowCount(n_row);
                grid.SetColumnCount(n_col);
                grid.SetFixedRowCount(m_nFixRows);
                grid.SetFixedColumnCount(m_nFixCols);

                // 4. 设置所有行高31，所有列宽80
                for (int i = 0; i < n_row; i++)
                    grid.SetRowHeight(i, 31);
                for (int i = 0; i < n_col; i++)
                    grid.SetColumnWidth(i, 80);

                // 5. 填充单元格文本+颜色（完全对应C++逻辑）
                for (int row = 0; row < n_row; row++)
                {
                    for (int col = 0; col < n_col; col++)
                    {
                        string text = string.Empty;
                        // 列头（红色字体）
                        if (row < m_nFixRows)
                        {
                            text = $"Column {col}";
                            grid.SetItemForeColor(row, col, Util.ExARGB(255, 0, 0, 255));
                        }
                        // 行头（蓝色字体）
                        else if (col < m_nFixCols)
                        {
                            text = $"Row {row}";
                            grid.SetItemForeColor(row, col, Util.ExARGB(0, 0, 255, 255));
                        }
                        // 普通单元格（显示乘积）
                        else
                        {
                            text = (row * col).ToString();
                        }

                        grid.SetItemText(row, col, text);
                    }
                }

                // 6. 设置下拉框单元格（第4行第1列）
                grid.SetCellType(4, 1, GRID_CELL_COMBO);
                string[] comboOptions = { "Option 1", "Option 2", "Option 3" };
                var comboParam = ExGrid.ConvertToComboOptions(comboOptions);
                IntPtr pComboParam = Marshal.AllocHGlobal(Marshal.SizeOf<ExGridComboOptionsParam>());
                try
                {
                    Marshal.StructureToPtr(comboParam, pComboParam, false);
                    grid.SetComboOptions(4, 1, pComboParam);
                    // 设置默认文本
                    grid.SetItemText(4, 1, "Option 1");
                    // 设置背景/前景色
                    grid.SetItemBackColor(4, 1, Util.ExARGB(240, 133, 0, 160));
                    grid.SetItemForeColor(4, 1, Util.ExARGB(120, 120, 120, 255));
                }
                finally
                {
                    // 释放内存
                    ExGrid.FreeComboOptions(ref comboParam);
                    Marshal.FreeHGlobal(pComboParam);
                }

                // 7. 设置日期框单元格（第1行第2列）
                grid.SetCellType(1, 2, GRID_CELL_DATE);
                // C# DateTime 转 C++ time_t (Unix时间戳)
                long unixTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                IntPtr pDate = Marshal.AllocHGlobal(sizeof(long));
                try
                {
                    Marshal.WriteInt64(pDate, unixTime);
                    grid.SetItemDate(1, 2, pDate);
                    // 设置背景/前景色
                    grid.SetItemBackColor(1, 2, Util.ExARGB(80, 154, 120, 255));
                    grid.SetItemForeColor(1, 2, Util.ExARGB(0, 255, 0, 255));
                }
                finally
                {
                    Marshal.FreeHGlobal(pDate);
                }

                // 8. 刷新表格（对应C++ Ex_ObjInvalidateRect）
                grid.Invalidate();


                skin.Visible = true;
            }
        }

        static public IntPtr OnGridButtonEvent(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            grid?.Print();
            return IntPtr.Zero;
        }
    }
}
