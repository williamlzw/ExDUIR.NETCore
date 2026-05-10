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
    static class CandlestickChartWindow
    {
        static private ExSkin skin;
        static private ExCandlestickChart candlestickChart;
        static private ExObjEventProcDelegate objEvent;
        static private ExObjEventProcDelegate objEventHover;
        static private Random random = new Random();

        static public void CreateCandlestickChartWindow(ExSkin pOwner)
        {
            skin = new ExSkin(pOwner, null, "测试K线图", 0, 0, 900, 600,
            WINDOW_STYLE_NOINHERITBKG | WINDOW_STYLE_BUTTON_CLOSE | WINDOW_STYLE_BUTTON_MIN | WINDOW_STYLE_MOVEABLE |
            WINDOW_STYLE_CENTERWINDOW | WINDOW_STYLE_TITLE | WINDOW_STYLE_HASICON | WINDOW_STYLE_NOSHADOW);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExARGB(80, 80, 90, 255);
                // 创建K线图控件（对应C++ Ex_ObjCreate）
                candlestickChart = new ExCandlestickChart(skin, "K线图", 50, 50, 800, 500,
                    OBJECT_STYLE_VISIBLE | OBJECT_STYLE_BORDER | OBJECT_STYLE_HSCROLL);
                objEvent = new ExObjEventProcDelegate(OnCandleClicked);
                objEventHover = new ExObjEventProcDelegate(OnCandleHover);
                // 生成测试数据
                GenerateTestData();

                // 设置均线天数 5/10/20/30
                candlestickChart.SetMADays(new int[] { 5, 10, 20, 30 });

                // 显示均线
                candlestickChart.ShowMA(true);

                // 绑定事件
                candlestickChart.HandleEvent(CANDLESTICKCHART_EVENT_ITEM_CLICKED, objEvent);
                candlestickChart.HandleEvent(CANDLESTICKCHART_EVENT_ITEM_HOVER, objEventHover);
                skin.Visible = true;
            }
        }

        /// <summary>
        /// 生成100条测试K线数据（完全复刻C++逻辑）
        /// </summary>
        static void GenerateTestData()
        {
            // 清空数据
            candlestickChart.ClearData();

            double prevClose = 100.0;
            // 2021-01-01 时间戳
            long baseTimestamp = 1609459200;

            for (int i = 0; i < 100; i++)
            {
                ExCandlestickData candle = new ExCandlestickData();
                candle.timestamp = baseTimestamp + i * 86400;

                // 开盘价：小幅波动
                candle.open = prevClose * (1.0 + (random.Next(31) - 15) / 1000.0);

                // 最高价/最低价 （严格遵循C++逻辑）
                double riseRate = (0.1 + random.Next(24)) / 1000.0;
                candle.high = candle.open * (1.0 + riseRate);

                double dropRate = (0.1 + random.Next(24)) / 1000.0;
                candle.low = candle.open * (1.0 - dropRate);

                // 兜底防护
                double priceRange = candle.high - candle.low;
                if (priceRange <= 0.00001)
                {
                    candle.high = candle.open + 0.005;
                    candle.low = candle.open - 0.005;
                    priceRange = 0.01;
                }

                // 收盘价（合法区间）
                candle.close = candle.low + random.NextDouble() * priceRange;

                // 成交量
                candle.volume = 5000000 + random.NextDouble() * 7500000;

                // 均线赋值0
                candle.ma5 = candle.ma10 = candle.ma20 = candle.ma30 = 0;

                // 添加数据
                candlestickChart.AddData(candle);

                // 更新收盘价
                prevClose = candle.close;
            }
        }

        /// <summary>
        /// 时间戳转日期时间字符串（完全匹配C++ Ex_TimestampToDatetime）
        /// </summary>
        /// <param name="timestamp">时间戳（支持10位秒级/13位毫秒级）</param>
        /// <returns>格式化时间：yyyy-MM-dd HH:mm:ss，异常返回--</returns>
        public static string TimestampToDatetime(long timestamp)
        {
            try
            {
                DateTimeOffset dateTimeOffset;

                // 完全复刻C++逻辑：大于1000000000000 判定为毫秒级，否则秒级
                if (timestamp > 1000000000000)
                {
                    dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                }
                else
                {
                    dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                }

                // 转换为【本地时间】（对应C++ localtime_s）
                DateTime localTime = dateTimeOffset.LocalDateTime;

                // 格式化输出：年-月-日 时:分:秒（与C++ swprintf_s格式完全一致）
                return localTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                // 异常兜底，返回--（对应C++默认值）
                return "--";
            }
        }

        /// <summary>
        /// K线点击事件（复刻C++弹窗逻辑）
        /// </summary>
        static public IntPtr OnCandleClicked(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            int index = wParam.ToInt32();
            var chart =  new ExCandlestickChart(hObj);
            ExCandlestickData data = chart.GetSelectedData();

            if (data.timestamp > 0)
            {
                // 时间戳转日期
                string time = TimestampToDatetime(data.timestamp);
                // 弹窗显示K线信息
                string info = $"选中K线 #{index}\n时间: {time}\n开盘: {data.open:F2}\n收盘: {data.close:F2}\n最高: {data.high:F2}\n最低: {data.low:F2}\n成交量: {data.volume:F0}";
                ExAPI.Ex_MessageBox(hObj, info, "K线信息", MB_OK | MB_ICONINFORMATION, MESSAGEBOX_FLAG_CENTEWINDOW);
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// K线悬停事件
        /// </summary>
        static IntPtr OnCandleHover(int hObj, int nID, int nCode, IntPtr wParam, IntPtr lParam)
        {
            int index = wParam.ToInt32();
            // 可在此处添加悬停逻辑
            return IntPtr.Zero;
        }

        #region 辅助方法（完全移植C++）
        /// <summary>
        /// 添加实时K线数据
        /// </summary>
        static public void AddRealtimeData(double open, double high, double low, double close, double volume)
        {
            ExCandlestickData candle = new ExCandlestickData
            {
                timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                open = open,
                high = high,
                low = low,
                close = close,
                volume = volume
            };
            candlestickChart.AddData(candle);
        }

        /// <summary>
        /// 设置价格范围
        /// </summary>
        static public void SetPriceRange(double minPrice, double maxPrice)
        {
            candlestickChart.SetPriceRange(minPrice, maxPrice);
        }

        /// <summary>
        /// 切换均线显示
        /// </summary>
        static public void ToggleMA(bool show)
        {
            candlestickChart.ShowMA(show);
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        static public void ClearChartData()
        {
            candlestickChart.ClearData();
        }
        #endregion
    }
}
