using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Graphics;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Native;
using System;
using static ExDuiR.NET.Native.ExConst;

namespace ExDuiRTest
{
    class EditMaterialWindow
    {
        static private ExSkin skin;                  // 主窗口
        static private ExEditMaterial editMaterial; // 素材编辑框
        static private ExButton btnAdd, btnClear, btnRemove; // 三个功能按钮

        static private ExObjEventProcDelegate btnEvent;     // 按钮点击事件
        static private ExObjEventProcDelegate materialEvent;// 素材选中事件

        // 初始素材列表（匹配C++）
        static private readonly (string name, string path)[] initMaterials = new[]
        {
            (name: "头像", path: "Resources/rotateimgbox.jpg"),
            (name: "菜单背景", path: "Resources/custombkg.png")
        };

        static public void CreateEditMaterialWindow(ExSkin pOwner)
        {
            // 创建窗口：尺寸/样式/背景色 完全与C++一致
            skin = new ExSkin(pOwner, null, "测试素材编辑框", 0, 0, 700, 400,
                WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN |
                WINDOW_STYLE_MOVEABLE | WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE |
                WINDOW_STYLE_HASICON | WINDOW_STYLE_NOSHADOW);

            if (skin.Validate)
            {
                // 窗口背景色：C++原版 45,45,55,255
                skin.BackgroundColor = Util.ExARGB(45, 45, 55, 255);

                // ===================== 创建素材编辑框 =====================
                int dwStyleEx = OBJECT_STYLE_EX_FOCUSABLE | OBJECT_STYLE_EX_COMPOSITED | OBJECT_STYLE_EX_TABSTOP;
                int dwStyle = OBJECT_STYLE_VISIBLE | OBJECT_STYLE_VSCROLL | EDIT_STYLE_RICHTEXT |
                              EDIT_STYLE_NEWLINE | EDIT_STYLE_ALLOWTAB;
                int dwTextFormat = DT_LEFT | DT_TOP;

                editMaterial = new ExEditMaterial(skin, "", 20, 40, 500, 300,
                    dwStyle, dwStyleEx, dwTextFormat, 301);

                // 绑定【素材选中】通知事件
                materialEvent = new ExObjEventProcDelegate(OnMaterialSelected);
                editMaterial.HandleEvent(EDITMATERIAL_EVENT_SELECTED, materialEvent);

                // ===================== 加载初始素材 =====================
                LoadInitMaterials();

                // ===================== 设置初始文本 =====================
                editMaterial.SetInitText("测试 @{头像} 素材编辑框，这是@{菜单背景}");

                // ===================== 创建功能按钮 =====================
                btnAdd = new ExButton(skin, "添加素材", 530, 40, 150, 30, OBJECT_STYLE_VISIBLE, -1, 0, 402);
                btnClear = new ExButton(skin, "清空素材", 530, 80, 150, 30, OBJECT_STYLE_VISIBLE, -1, 0, 403);
                btnRemove = new ExButton(skin, "删除'头像'素材", 530, 120, 150, 30, OBJECT_STYLE_VISIBLE, -1, 0, 404);

                // 绑定按钮点击事件
                btnEvent = new ExObjEventProcDelegate(OnButtonClick);
                btnAdd.HandleEvent(NM_CLICK, btnEvent);
                btnClear.HandleEvent(NM_CLICK, btnEvent);
                btnRemove.HandleEvent(NM_CLICK, btnEvent);

           

                // 显示窗口
                skin.Visible = true;
            }
        }

        /// <summary>
        /// 加载初始素材
        /// </summary>
        static private void LoadInitMaterials()
        {
            foreach (var mat in initMaterials)
            {
                ExImage img = new ExImage(mat.path);
                ExEditMaterialItem item = new ExEditMaterialItem
                {
                    szName = mat.name,
                    hImage = img.handle
                };
                editMaterial.AddMaterial(item);
            }
        }

        /// <summary>
        /// 按钮点击事件（匹配C++逻辑）
        /// </summary>
        static private IntPtr OnButtonClick(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            switch (nID)
            {
                case 402:
                    // 动态添加素材：409.dds + Loading.png
                    AddDynamicMaterial();
                    break;
                case 403:
                    // 清空所有素材
                    editMaterial.ClearMaterials();
                    break;
                case 404:
                    // 删除名称为"头像"的素材
                    editMaterial.RemoveMaterial("头像");
                    break;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 动态添加素材（C++原版逻辑）
        /// </summary>
        static private void AddDynamicMaterial()
        {
            // 添加素材1：头像头像
            ExImage img1 = new ExImage("Resources/409.dds");
            editMaterial.AddMaterial(new ExEditMaterialItem
            {
                szName = "头像头像",
                hImage = img1.handle
            });

            // 添加素材2：加载
            ExImage img2 = new ExImage("Resources/Loading.png");
            editMaterial.AddMaterial(new ExEditMaterialItem
            {
                szName = "加载",
                hImage = img2.handle
            });
        }

        /// <summary>
        /// 素材选中通知事件（输出日志，匹配C++）
        /// </summary>
        static private IntPtr OnMaterialSelected(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == EDITMATERIAL_EVENT_SELECTED)
            {
                int index = wParam.ToInt32();
                Console.WriteLine($"素材被选中, 索引: {index}");
            }
            return IntPtr.Zero;
        }
    }
}