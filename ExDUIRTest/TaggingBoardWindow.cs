﻿using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Frameworks.Graphics;
using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Native;
using static ExDuiR.NET.Native.ExConst;
using System.Reflection.Emit;

namespace ExDuiRTest
{
    static class TaggingBoardWindow
    {
        static private ExSkin skin;
        static private ExTaggingBoard taggingboard;
        static private ExButton button1;
        static private ExButton button2;
        static private ExButton button3;
        static private ExButton button4;
        static private ExButton button5;
        static private ExObjEventProcDelegate objEvent;
        static private ExStatic label;

        static public void CreateTaggingBoardWindow(ExSkin pOwner)
        {
            skin = new ExSkin(pOwner, null, "测试标注画板", 0, 0, 1200, 850,
            EWS_NOINHERITBKG | EWS_BUTTON_CLOSE | EWS_BUTTON_MIN | EWS_MOVEABLE |
            EWS_CENTERWINDOW | EWS_TITLE | EWS_HASICON | EWS_NOSHADOW);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExRGBA(150, 150, 150, 255);
                taggingboard = new ExTaggingBoard(skin, "", 30, 30, 1000, 800);
                button1 = new ExButton(skin, "开始绘图", 1050, 30, 100, 30, -1, -1, DT_VCENTER | DT_CENTER);
                button2 = new ExButton(skin, "结束绘图", 1050, 70, 100, 30, -1, -1, DT_VCENTER | DT_CENTER);
                button3 = new ExButton(skin, "清空绘图", 1050, 110, 100, 30, -1, -1, DT_VCENTER | DT_CENTER);
                button4 = new ExButton(skin, "取出数据", 1050, 150, 100, 30, -1, -1, DT_VCENTER | DT_CENTER);
                button5 = new ExButton(skin, "设置数据", 1050, 190, 100, 30, -1, -1, DT_VCENTER | DT_CENTER);
                objEvent = new ExObjEventProcDelegate(OnButtonEventProc);
                button1.HandleEvent(NM_CLICK, objEvent);
                button2.HandleEvent(NM_CLICK, objEvent);
                button3.HandleEvent(NM_CLICK, objEvent);
                button4.HandleEvent(NM_CLICK, objEvent);
                button5.HandleEvent(NM_CLICK, objEvent);

                label = new ExStatic(skin, "操作提示：\r\n1.点击【开始绘图】，鼠标在画板左键单击，开始绘制路径点，右键可以撤销点，达到3个点及以上可以闭合路径。 闭合路径后会自动调用【结束绘图】。此时再次点击【开始绘图】继续绘制下一条路径。\r\n2.绘制过程中点击【结束绘图】清空临时点。变为选中模式，可以选择画板上闭合的路径。\r\n3.点击【清空绘图】清空画板全部临时点和闭合路径。\r\n4.点击【取出数据】演示打印原图点坐标", 1050, 230, 130, 550, -1, -1, DT_WORDBREAK);
                label.SetFont("微软雅黑", 16, FS_BOLD);
                label.ColorTextNormal = Util.ExRGBA(133, 33, 53, 255);

                taggingboard.PenColor = Util.ExRGBA(0, 255, 0, 255);
                var bitmap = File.ReadAllBytes("Resources/carousel3.jpeg");
                taggingboard.TaggingImage = new ExImage(bitmap, bitmap.Length);

                skin.Visible = true;
            }
        }

        static public IntPtr OnButtonEventProc(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(hObj == button1.handle)
            {
                taggingboard.StartTagging = true;
            }
            else if (hObj == button2.handle)
            {
                taggingboard.StopTagging = true;
            }
            else if (hObj == button3.handle)
            {
                taggingboard.ClearTagging = true;
            }
            else if (hObj == button4.handle)
            {
                var polygons = taggingboard.TaggingData;
                int index = 0;
                foreach(var polygon in polygons)
                {
                    foreach(var point in polygon)
                    {
                        Console.WriteLine($"原图坐标 路径:{index} x:{point.x}, y:{point.y}");
                    }
                    index++;
                }
            }
            else if (hObj == button5.handle)
            {
                List<ExPoint> polygon = new List<ExPoint>();
                polygon.Add(new ExPoint(356, 377));
                polygon.Add(new ExPoint(329, 398));
                polygon.Add(new ExPoint(331, 419));
                polygon.Add(new ExPoint(388, 419));
                polygon.Add(new ExPoint(388, 392));
                List<List<ExPoint>> polygons = new List<List<ExPoint>>();
                polygons.Add(polygon);
                taggingboard.TaggingData = polygons;
            }
            return IntPtr.Zero;
        }
    }
}