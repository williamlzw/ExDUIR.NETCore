﻿using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Frameworks.Graphics;
using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Native;
using static ExDuiR.NET.Native.ExConst;
using System;

namespace ExDuiRTest
{
    static class EditExWindow
    {
        static private ExSkin skin;
        static private ExEditEx edit1;
        static private ExEditEx edit2;
        static private ExEditEx edit3;
        static private ExEditEx edit4;
        static private ExObjEventProcDelegate editProc;
        static public void CreateEditExWindow(ExSkin pOwner)
        {
            skin = new ExSkin(pOwner, null, "测试扩展编辑框", 0, 0, 400, 200,
            WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN | WINDOW_STYLE_MOVEABLE |
            WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON | WINDOW_STYLE_NOSHADOW);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExARGB(150, 150, 150, 255);
                edit1 = new ExEditEx(skin, "", 30, 50, 150, 30, OBJECT_STYLE_VISIBLE | EDIT_STYLE_UNDERLINE, -1, -1);
                edit1.SetBanner("请输入账户名称", Util.ExARGB(255, 255, 255, 100));
                edit1.ColorTextNormal = Util.ExARGB(255, 255, 255, 200);
                var icon_contacts_normal = File.ReadAllBytes("Resources/icon_contacts_normal.png");
                var psw_normal = File.ReadAllBytes("Resources/psw_normal.png");
                var search_normal = File.ReadAllBytes("Resources/search_normal.png");
                edit1.Icon = new ExImage(icon_contacts_normal, icon_contacts_normal.Length);

                edit2 = new ExEditEx(skin, "", 30, 100, 150, 30, OBJECT_STYLE_VISIBLE | EDIT_STYLE_UNDERLINE | EDIT_STYLE_USEPASSWORD, -1, -1);
                edit2.SetBanner("请输入账户密码", Util.ExARGB(255, 255, 255, 100));
                edit2.ColorTextNormal = Util.ExARGB(255, 255, 255, 200);
                edit2.Icon = new ExImage(psw_normal, psw_normal.Length);

                edit3 = new ExEditEx(skin, "", 200, 50, 150, 30, -1, -1, -1);
                edit3.SetBanner("搜索一下", Util.ExARGB(255, 255, 255, 100));
                edit3.ColorTextNormal = Util.ExARGB(255, 255, 255, 200);
                edit3.Props = new ExObjProps
                {
                    crBkgNormal = Util.ExARGB(20, 20, 120, 150),
                    crBorderHover = Util.ExARGB(255, 77, 77, 150),
                    crBorderDownOrChecked = Util.ExARGB(255, 77, 77, 200),
                    radius = 15,
                    strokeWidth = 1,
                    nIconPosition = 1
                };
                edit3.Icon = new ExImage(search_normal, search_normal.Length);

                edit4 = new ExEditEx(skin, "", 200, 100, 150, 30, -1, -1, -1);
                edit4.SetBanner("搜索一下", Util.ExARGB(255, 255, 255, 100));
                edit4.ColorTextNormal = Util.ExARGB(255, 255, 255, 200);
                edit4.Icon = new ExImage(search_normal, search_normal.Length);
                editProc = new ExObjEventProcDelegate(OnEditChangeEvent);
                edit4.HandleEvent(NM_CHAR, editProc);

                skin.Visible = true;
            }
        }

        static private IntPtr OnEditChangeEvent(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode == NM_CHAR)
            {
                Console.WriteLine($"编辑框输入字符:{wParam}");
            }
            return IntPtr.Zero;
        }
    }
}
