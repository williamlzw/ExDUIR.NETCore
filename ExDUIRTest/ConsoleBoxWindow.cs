using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Graphics;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Native;
using System;
using System.Threading.Tasks;
using static ExDuiR.NET.Native.ExConst;

namespace ExDuiRTest
{
    class ConsoleBoxWindow
    {
        static private ExSkin skin;                  // 主窗口
        static private ExConsoleBox consoleBox;      // 控制台控件
        static private ExButton btnAdd;              // 模拟添加按钮
        static private ExButton btnClear;            // 清空按钮
        static private ExObjEventProcDelegate btnEvent; // 按钮事件委托
        static private Random random = new Random(); // 随机数（避免重复）

        // C++原版9条日志文本
        static private readonly string[] LogTexts = new string[]
        {
             "[Info] 系统初始化完成，所有核心服务已正常启动。",
             "[Warn] 检测到配置文件版本过旧，已自动迁移至新格式，请检查设置项是否丢失。",
             "[Error] 数据库连接超时：无法连接到 192.168.1.100:3306，请检查网络或防火墙设置。重试次数：3/3。",
             "[Debug] 内存池分配详情 -> 总大小: 256MB | 已用: 142MB | 碎片率: 2.3% | 峰值: 198MB",
             "[Net] 收到客户端请求 [ID:8842] POST /api/v2/data/sync?token=xxx&ts=1716720000 耗时: 45ms",
             "[Render] 帧率稳定在 60FPS，GPU占用率 34%，显存使用 1.2GB / 8GB，渲染管线无瓶颈。",
             "[Auth] 用户 admin@system.local 登录成功，会话令牌已签发，有效期 7200 秒，IP: 10.0.0.55",
             "[IO] 日志文件滚动触发：当前文件大小达到 50MB 阈值，已归档为 app.log.20260526.bak",
             "[Perf] ⚠️ 主线程阻塞警告：消息处理耗时 320ms，超过 16ms 流畅阈值，建议将耗时操作移至工作线程执行以避免界面卡顿。"
        };

        static public void CreateConsoleBoxWindow(ExSkin pOwner)
        {
            // 创建窗口（和C++尺寸/样式完全一致）
            skin = new ExSkin(pOwner, null, "测试日志框", 0, 0, 600, 400,
            WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN |
            WINDOW_STYLE_MOVEABLE | WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON);

            if (skin.Validate)
            {
                // 窗口背景色（C++原版：80,80,90,255）
                skin.BackgroundColor = Util.ExARGB(80, 80, 90, 255);

                // ===================== 创建控制台框 =====================
                consoleBox = new ExConsoleBox(skin, "", 10, 30, 570, 300);
                consoleBox.SetAutoScroll(true); // 开启自动滚动

                // ===================== 初始化：添加第一条消息 =====================
                int initColor = Util.ExARGB(255, 255, 255, 255);
                consoleBox.AddItem("这是第一条消息，Hello, ExDUIR!", initColor);

                // ===================== 初始化：批量添加30条随机日志 =====================
                AddRandomLogs(30);

                // ===================== 创建按钮（和C++ID/位置一致） =====================
                btnAdd = new ExButton(skin, "模拟添加", 10, 350, 120, 30, -1, -1, DT_VCENTER | DT_CENTER, 300);
                btnClear = new ExButton(skin, "清空", 140, 350, 120, 30, -1, -1, DT_VCENTER | DT_CENTER, 301);

                // 绑定按钮点击事件
                btnEvent = new ExObjEventProcDelegate(OnButtonClick);
                btnAdd.HandleEvent(NM_CLICK, btnEvent);
                btnClear.HandleEvent(NM_CLICK, btnEvent);

                // ===================== 设置字体/背景色（C++原版） =====================
                ExFont font = new ExFont("Consolas", 14, 0); // Consolas 14号字体
                consoleBox.SetFont(font);
                consoleBox.SetBackColor(Util.ExARGB(80, 80, 80, 255)); // 控制台背景色

                // 显示窗口
                skin.Visible = true;
            }
        }

        /// <summary>
        /// 按钮点击事件（完全还原C++逻辑）
        /// </summary>
        static private IntPtr OnButtonClick(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nID == 300) // 模拟添加按钮
            {
                // 异步添加30条日志（延迟300ms/条，不阻塞UI）
                _ = AddLogsAsync(30, 300);
            }
            else if (nID == 301) // 清空按钮
            {
                consoleBox.ClearAll();
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 同步添加N条随机日志（初始化用）
        /// </summary>
        static private void AddRandomLogs(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int color = GetRandomColor();
                string log = LogTexts[random.Next(LogTexts.Length)];
                consoleBox.AddItem(log, color);
            }
        }

        /// <summary>
        /// 异步添加日志（按钮点击用，延迟不阻塞UI）
        /// </summary>
        static private async Task AddLogsAsync(int count, int delayMs)
        {
            for (int i = 0; i < count; i++)
            {
                int color = GetRandomColor();
                string log = LogTexts[random.Next(LogTexts.Length)];
                consoleBox.AddItem(log, color);
                await Task.Delay(delayMs); // 异步延迟，UI不卡死
            }
        }

        /// <summary>
        /// 生成随机颜色（C++原版：rand()%200+55）
        /// </summary>
        static private int GetRandomColor()
        {
            int r = random.Next(55, 255);
            int g = random.Next(55, 255);
            int b = random.Next(55, 255);
            return Util.ExARGB(r, g, b, 255);
        }
    }
}