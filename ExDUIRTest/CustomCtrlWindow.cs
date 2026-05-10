using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Frameworks.Graphics;
using ExDuiR.NET.Frameworks.Layout;
using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Native;
using static ExDuiR.NET.Native.ExConst;
using System;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace ExDuiRTest
{
    /// <summary>
    /// 扩展组件演示,继承自ExControl,演示c#类成员属性与底层库通过Prop交互数据
    /// </summary>
    public class TestCustomCtrl : ExControl
    {
        public class TestStruct
        {
            public string str;
            public object obj;
        }

        private const int CustomCtrl_MESSAGE_GET_STRUCT = 10000;
        private const int CustomCtrl_MESSAGE_SET_STRUCT = 10001;
        private const int CustomCtrl_MESSAGE_GET_PARAM = 10002;
        private const int CustomCtrl_MESSAGE_SET_PARAM = 10003;

        public TestStruct Struct
        {
            get
            {
                var ptr = this.SendMessage(CustomCtrl_MESSAGE_GET_STRUCT);
                GCHandle gcHandle = GCHandle.FromIntPtr(ptr);
                TestStruct obj = (TestStruct)gcHandle.Target;
                return obj;
            }
            set
            {
                GCHandle gcHandle = GCHandle.Alloc(value);
                IntPtr ptr = GCHandle.ToIntPtr(gcHandle);
                this.SendMessage(CustomCtrl_MESSAGE_SET_STRUCT, IntPtr.Zero, ptr);
            }
        }

        /// <summary>
        /// c#类成员属性
        /// </summary>
        public int Param
        {
            get
            {
                return (int)this.SendMessage(CustomCtrl_MESSAGE_GET_PARAM);
            }
            set
            {
                this.SendMessage(CustomCtrl_MESSAGE_SET_PARAM, IntPtr.Zero, value);
            }
        }

        private static ExObjClassProcDelegate s_pfnObjClassProc = new ExObjClassProcDelegate(
            (IntPtr hWnd, int hObj, int uMsg, IntPtr wParam, IntPtr lParam, IntPtr pvData) =>
        {
            ExControl Obj = new ExControl(hObj);
            switch (uMsg)
            {
                case WM_CREATE:
                    {
                        Obj.InitPropList(2);
                        break;
                    }
                case WM_PAINT:
                    {
                        //开始绘制与EndPaint配套调用
                        Obj.BeginPaint(out var ps);
                        ExCanvas canvas = new ExCanvas(ps.hCanvas);
                        var brush = new ExBrush(Util.ExARGB(255, 0, 0, 255));
                        canvas.Clear(Util.ExARGB(80, 80, 80, 255));
                        canvas.DrawText(Obj.Font, brush, Obj.Text, -1, DT_CENTER | DT_VCENTER, ps.rcPaint.nLeft, ps.rcPaint.nTop, ps.rcPaint.nRight, ps.rcPaint.nBottom);
                        //结束绘制
                        Obj.EndPaint(ref ps);
                        brush.Dispose();
                        break;
                    }
                case CustomCtrl_MESSAGE_SET_STRUCT:
                    {
                        Obj.SetProp(0, lParam);
                        break;
                    }
                case CustomCtrl_MESSAGE_GET_STRUCT:
                    {
                        return Obj.GetProp(0);
                    }
                case CustomCtrl_MESSAGE_SET_PARAM:
                    {
                        Obj.SetProp(1, lParam);
                        break;
                    }
                case CustomCtrl_MESSAGE_GET_PARAM:
                    {
                        return Obj.GetProp(1);
                    }
                default:
                    break;
            }
            //组件默认回调
            return CallDefProc(hWnd, hObj, uMsg, wParam, lParam);
        });

        public static int RegisterControl()
        {
            return ExAPI.Ex_ObjRegister("TestCustomCtrl", OBJECT_STYLE_VISIBLE, OBJECT_STYLE_EX_FOCUSABLE, 0, 0, IntPtr.Zero, 0, s_pfnObjClassProc);
        }

        public TestCustomCtrl(IExBaseUIEle pOwner, string sText, int x, int y, int nWidth, int nHeight)
           : base(pOwner, "TestCustomCtrl", sText, x, y, nWidth, nHeight)
        {

        }
        public new string ClassName => "TestCustomCtrl";
    }

    static class CustomCtrlWindow
    {
        static private ExSkin skin;
        static private ExObjEventProcDelegate objEvent;
        static private TestCustomCtrl custom1;
        static private TestCustomCtrl custom2;

        static public void CreateCustomCtrlWindow(ExSkin pOwner)
        {
            skin = new ExSkin(pOwner, null, "测试自定义组件", 0, 0, 200, 250,
            WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN | WINDOW_STYLE_MOVEABLE |
            WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON | WINDOW_STYLE_NOSHADOW);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExARGB(150, 150, 150, 255);

                objEvent = new ExObjEventProcDelegate(OnButtonEventProc);
                //注册自定义组件
                TestCustomCtrl.RegisterControl();
                //创建自定义组件
                custom1 = new TestCustomCtrl(skin, "点击我看输出", 50, 50, 100, 50);
                custom1.Param = 1;
                custom1.Struct = new TestCustomCtrl.TestStruct
                {
                    str = "结构1",
                    obj = JsonConvert.DeserializeObject("{value:'我是结构1'}")
                };
                custom2 = new TestCustomCtrl(skin, "点击我看输出", 50, 150, 100, 50);
                custom2.Param = 2;
                custom2.Struct = new TestCustomCtrl.TestStruct
                {
                    str = "结构2",
                    obj = JsonConvert.DeserializeObject("{value:'我是结构2'}")
                };
                custom1.HandleEvent(NM_CLICK, objEvent);
                custom2.HandleEvent(NM_CLICK, objEvent);
                skin.Visible = true;
            }
        }

        static public IntPtr OnButtonEventProc(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (hObj == custom1.handle)
            {
                Console.WriteLine(custom1.Param);
                Console.WriteLine($"{custom1.Struct.str}, {JsonConvert.SerializeObject(custom1.Struct.obj)}");
            }
            else if (hObj == custom2.handle)
            {
                Console.WriteLine(custom2.Param);
                Console.WriteLine($"{custom2.Struct.str}, {JsonConvert.SerializeObject(custom2.Struct.obj)}");
            }
            return IntPtr.Zero;
        }
    }
}
