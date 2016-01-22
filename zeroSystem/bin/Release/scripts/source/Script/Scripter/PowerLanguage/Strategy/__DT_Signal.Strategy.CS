using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PowerLanguage.Function;
using Zeghs.Scripts;

namespace PowerLanguage.Strategy {
    [ScriptProperty(ScriptType = ScriptType.Signal, Version = "1.0.150420", Company = "量化策略交易員", Copyright = "Copyright © 2015 量化策略交易員. 保留一切權利。", Comment = "DT")]
    public class __DT_Signal : SignalObject {
        private const string 設定_策略名稱 = "DT"; // 設定策略名稱
        private const string 設定_策略版本 = "1.0.150420"; //設定策略版本
        private const bool 設定_啟用詳細訊息 = true;
        private const bool 設定_啟用記錄輸出 = true;
        private const bool 設定_啟用下單輸出 = true;
        private string 設定_系統磁碟代號 = "";
        private string 設定_程式交易目錄 = "";
        private string 設定_資料檔目錄 = "";
        private string 設定_記錄檔目錄 = "";
        private string 設定_下單檔目錄 = "";
        private Dictionary<string, string> 設定_輸出集合 = null;

        // 宣告下單物件
        private IOrderMarket 多單, 多停, 多當沖, 多隔沖, 空單, 空停, 空當沖, 空隔沖;
        private string 下單名稱;
        // 宣告各種價量、時間、資料等功能性變數
        private VariableSeries<Double> 漲跌, 漲跌幅, 上影線, 下影線, 實體線;
        private Dictionary<DateTime, bool> 結算日集合;
        private Dictionary<DayOfWeek, bool> 順勢進場週間日集合, 逆勢進場週間日集合;
        private Dictionary<int, bool> 結算隔日進場月份集合;
        private int 當沖出場時間, 重算日期, 成交日期, 成交時間, 下單規模, 當日作多次數, 當日作空次數;
        private double 作多區間, 作空區間, 開盤區間高點, 開盤區間低點, ATR;
        private double 停損點數, 今最高, 今最低, 今開盤, 開高價, 開低價, 開收盤, 開漲跌, 開漲跌幅, 昨最高, 昨最低, 昨收盤;
        private bool 今日停單;
        private EMarketPositionSide 前次PositionSide;
        private DayOfWeek 週間日;
        private IInstrument Data2;

        #region 宣告指標策略內部變數
        private double m_HH, m_LL, m_HC, m_LC;
        #endregion

        private ITradeOrder 最後進場部位 {
            get {
                return CurrentPosition.OpenTrades.Last().EntryOrder;
            }
        }

        #region 宣告指標策略外部變數
        [Input]
        public double 作多參數 {
            get;
            set;
        }

        [Input]
        public double 作空參數 {
            get;
            set;
        }

        [Input]
        public int 作多天數 {
            get;
            set;
        }

        [Input]
        public int 作空天數 {
            get;
            set;
        }

        [Input]
        public int 啟用指標停損 {
            get;
            set;
        }
        #endregion

        [Input]
        public int 啟用順勢進場 {
            get;
            set;
        }

        [Input]
        public string 順勢進場週間日 {
            get;
            set;
        }

        [Input]
        public int 啟用逆勢進場 {
            get;
            set;
        }

        [Input]
        public string 逆勢進場週間日 {
            get;
            set;
        }

        [Input]
        public string 結算隔日進場月份 {
            get;
            set;
        }

        [Input]
        public int 資金_管理模式 {
            get;
            set;
        }

        [Input]
        public double 資金_風險比例 {
            get;
            set;
        }

        [Input]
        public int 資金_基本規模 {
            get;
            set;
        }

        [Input]
        public double 資金_跳空倍注 {
            get;
            set;
        }

        [Input]
        public int 資金_均多倍注 {
            get;
            set;
        }

        [Input]
        public int 資金_均空倍注 {
            get;
            set;
        }

        [Input]
        public int 資金_逆勢倍注 {
            get;
            set;
        }

        [Input]
        public int 濾網_啟用多單 {
            get;
            set;
        }

        [Input]
        public int 濾網_啟用空單 {
            get;
            set;
        }

        [Input]
        public int 濾網_啟用當沖 {
            get;
            set;
        }

        [Input]
        public int 濾網_開盤區間時間 {
            get;
            set;
        }

        [Input]
        public int 濾網_停單時間 {
            get;
            set;
        }

        [Input]
        public int 加減碼_停損金額 {
            get;
            set;
        }

        [Input]
        public double 加減碼_加碼點數 {
            get;
            set;
        }

        [Input]
        public int 加減碼_加碼次數 {
            get;
            set;
        }

        public __DT_Signal(object _ctx)
            : base(_ctx) {
            #region 指標策略參數預設值
            作多參數 = 0.5;
            作空參數 = 0.5;
            作多天數 = 1;
            作空天數 = 1;
            啟用指標停損 = 0;
            作多參數 = 0.5;
            作空參數 = 0.5;
            作多天數 = 1;
            作空天數 = 1;
            啟用指標停損 = 0;
            #endregion

            啟用順勢進場 = 1;
            順勢進場週間日 = "0,1,2,3,4,5,6"; // 0 為週日，1~6 為週一至週六
            啟用逆勢進場 = 1;
            逆勢進場週間日 = "0,1,2,3,4,5,6"; // 0 為週日，1~6 為週一至週六 (逆勢績效一、二佳，二、四差)
            結算隔日進場月份 = "1,2,3,4,5,10,11,12";

            資金_管理模式 = 1; // 模式 1 (保證金)；模式 2 (波動度)
            資金_風險比例 = 0.2; // 保證金建議值：0.2；波動度建議值：0.06
            資金_基本規模 = 1;
            資金_跳空倍注 = 0; // 建議值：1.0%
            資金_均多倍注 = 0; // 建議值：60，即 MA60
            資金_均空倍注 = 0; // 建議值：60，即 MA60
            資金_逆勢倍注 = 0;

            濾網_啟用多單 = 1;
            濾網_啟用空單 = 0;
            濾網_啟用當沖 = 1;
            濾網_開盤區間時間 = 90559;
            濾網_停單時間 = 134500;

            加減碼_停損金額 = -1;//變更預設值，-1為自動判定15點
            加減碼_加碼點數 = 0;
            加減碼_加碼次數 = 0;

            設定_系統磁碟代號 = Path.GetPathRoot(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows));
            設定_程式交易目錄 = 設定_系統磁碟代號 + @"QuantTrader\";
            設定_資料檔目錄 = 設定_程式交易目錄 + @"Databases\";
            設定_記錄檔目錄 = 設定_程式交易目錄 + @"Logs\";
            設定_下單檔目錄 = 設定_程式交易目錄 + @"Orders\";
        }

        protected override void Create() {
            // 初始化下單物件，Contracts.UserSpecified 可指定規模，OrderExit.FromAll 可一次全平，OrderExit.Total指定口數平倉
            多單 = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
            空單 = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));
            多停 = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
            空停 = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover, OrderExit.FromAll));
            多當沖 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
            空當沖 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover, OrderExit.FromAll));
            多隔沖 = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
            空隔沖 = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover, OrderExit.FromAll));

            漲跌 = new VariableSeries<double>(this);
            漲跌幅 = new VariableSeries<double>(this);
            上影線 = new VariableSeries<double>(this);
            下影線 = new VariableSeries<double>(this);
            實體線 = new VariableSeries<double>(this);
            結算日集合 = new Dictionary<DateTime, bool>();
            順勢進場週間日集合 = new Dictionary<DayOfWeek, bool>();
            逆勢進場週間日集合 = new Dictionary<DayOfWeek, bool>();
            結算隔日進場月份集合 = new Dictionary<int, bool>();

            #region 初始化指標策略內部變數

            #endregion
        }

        protected override void StartCalc() {
            Data2 = BarsOfData(2);
            當沖出場時間 = 132955;
            前次PositionSide = EMarketPositionSide.Flat;

            // 啟用當沖時，若停單時間比 13:30 晚，就提前至 13:30
            if (濾網_啟用當沖 == 1) {
                濾網_停單時間 = Math.Min(濾網_停單時間, 當沖出場時間);
            }

            // 建立結算日資料集合，結算當日強制於當沖出場時間平倉
            string 結算日檔案路徑 = 設定_資料檔目錄 + "ExpireDate.TW.csv";
            結算日集合.Clear();
            if (File.Exists(結算日檔案路徑)) {
                string[] 結算日陣列 = File.ReadAllLines(結算日檔案路徑);
                for (int i = 0; i < 結算日陣列.Length; i++) {
                    結算日集合.Add(DateTime.ParseExact(結算日陣列[i], "yyyy/MM/dd", CultureInfo.InvariantCulture), true);
                }
            }

            // 建立順勢/逆勢進場週間日集合，只有允許週間日才能進場
            順勢進場週間日集合.Clear();
            逆勢進場週間日集合.Clear();
            string[] 順勢進場週間日陣列 = 順勢進場週間日.Split(new char[] { ',' });
            string[] 逆勢進場週間日陣列 = 逆勢進場週間日.Split(new char[] { ',' });

            for (int i = 0; i <= 6; i++) {
                bool 啟用 = 順勢進場週間日陣列.Count(x => x == i.ToString()) > 0;
                順勢進場週間日集合.Add((DayOfWeek)i, 啟用);

                啟用 = 逆勢進場週間日陣列.Count(x => x == i.ToString()) > 0;
                逆勢進場週間日集合.Add((DayOfWeek)i, 啟用);
            }

            // 建立結算隔日進場集合，只有允許月份才能進場
            結算隔日進場月份集合.Clear();
            string[] 結算隔日進場月份陣列 = 結算隔日進場月份.Split(new char[] { ',' });

            for (int i = 1; i <= 12; i++) {
                bool 啟用 = 結算隔日進場月份陣列.Count(x => x == i.ToString()) > 0;
                結算隔日進場月份集合.Add(i, 啟用);
            }

            // 計算停損金額在不同商品間換算出的停損點數，如 $3,000 在台指期的停損點數為 15 點
            double 每跳動點價值 = Bars.Info.MinMove / Bars.Info.PriceScale * Bars.Info.BigPointValue;
            if (加減碼_停損金額 > 0) {
                停損點數 = 加減碼_停損金額 / 每跳動點價值;
            }

            if (加減碼_停損金額 == -1) { //預設15點
                停損點數 = 15;  // 固定15點
                加減碼_停損金額 = (int)(停損點數 * 每跳動點價值);
            }

            設定_輸出集合 = null;
            重算日期 = 0;
        }

        protected override void CalcBar() {
            // 只在 Data2 有資料後才處理分線
            if (Data2.CurrentBar <= 1 || Bars.Info.Resolution.Type != EResolution.Minute) {
                return;
            }

            漲跌.Value = Bars.Close[0] - Bars.Open[0];
            漲跌幅.Value = 100 * ((Bars.Close[0] - Bars.Close[1]) / Bars.Close[1]);
            上影線.Value = Bars.High[0] - Math.Max(Bars.Open[0], Bars.Close[0]);
            下影線.Value = Math.Min(Bars.Open[0], Bars.Close[0]) - Bars.Low[0];
            實體線.Value = Math.Abs(Bars.Open[0] - Bars.Close[0]);
            成交日期 = int.Parse(Bars.Time[0].Date.ToString("yyyyMMdd"));
            成交時間 = int.Parse(Bars.BarUpdateTime.ToString("HHmmss"));
            string m_資料時間 = Bars.BarUpdateTime.ToString("HH:mm:ss");

            if (設定_啟用詳細訊息 && 設定_輸出集合 == null) {
                設定_輸出集合 = new Dictionary<string, string>();
                設定_輸出集合.Add("輸出記錄", "參數設定");
                設定_輸出集合.Add("策略名稱", 設定_策略名稱);
                設定_輸出集合.Add("策略版本", 設定_策略版本);
                #region 輸出指標策略參數
                設定_輸出集合.Add("作多參數", 作多參數.ToString());
                設定_輸出集合.Add("作空參數", 作空參數.ToString());
                設定_輸出集合.Add("作多天數", 作多天數.ToString());
                設定_輸出集合.Add("作空天數", 作空天數.ToString());
                設定_輸出集合.Add("啟用指標停損", 啟用指標停損.ToString());
                #endregion
                設定_輸出集合.Add("啟用順勢進場", 啟用順勢進場.ToString());
                設定_輸出集合.Add("順勢進場週間日", 順勢進場週間日);
                設定_輸出集合.Add("啟用逆勢進場", 啟用逆勢進場.ToString());
                設定_輸出集合.Add("逆勢進場週間日", 逆勢進場週間日);
                設定_輸出集合.Add("結算隔日進場月份", 結算隔日進場月份);
                設定_輸出集合.Add("資金_管理模式", 資金_管理模式.ToString());
                設定_輸出集合.Add("資金_風險比例", 資金_風險比例.ToString());
                設定_輸出集合.Add("資金_基本規模", 資金_基本規模.ToString());
                設定_輸出集合.Add("資金_跳空倍注", 資金_跳空倍注.ToString());
                設定_輸出集合.Add("資金_均多倍注", 資金_均多倍注.ToString());
                設定_輸出集合.Add("資金_均空倍注", 資金_均空倍注.ToString());
                設定_輸出集合.Add("資金_逆勢倍注", 資金_逆勢倍注.ToString());
                設定_輸出集合.Add("濾網_啟用多單", 濾網_啟用多單.ToString());
                設定_輸出集合.Add("濾網_啟用空單", 濾網_啟用空單.ToString());
                設定_輸出集合.Add("濾網_啟用當沖", 濾網_啟用當沖.ToString());
                設定_輸出集合.Add("濾網_開盤區間時間", 濾網_開盤區間時間.ToString());
                設定_輸出集合.Add("濾網_停單時間", 濾網_停單時間.ToString());
                設定_輸出集合.Add("加減碼_停損金額", 加減碼_停損金額.ToString());
                設定_輸出集合.Add("加減碼_停損點數", 停損點數.ToString());
                設定_輸出集合.Add("加減碼_加碼點數", 加減碼_加碼點數.ToString());
                設定_輸出集合.Add("加減碼_加碼次數", 加減碼_加碼次數.ToString());
                輸出詳細內容();
            }

            // 換日後需重算關鍵數值
            if (Bars.Time[0].Date > Bars.Time[1].Date && 重算日期 < 成交日期) {
                if (設定_啟用詳細訊息) {
                    設定_輸出集合.Clear();
                    設定_輸出集合.Add("輸出記錄", "新交易日");
                    設定_輸出集合.Add("原交易日", Bars.Time[1].Date.ToString("yyyy-MM-dd"));
                    設定_輸出集合.Add("新交易日", Bars.Time[0].Date.ToString("yyyy-MM-dd"));
                    設定_輸出集合.Add("資料時間", m_資料時間);
                    輸出詳細內容();
                }

                重算日期 = 0;
            }

            if (重算日期 == 0) {
                // 如非換日所引起的重算動作就跳出
                if (Bars.Time[0].Date < Bars.Time[1].Date) {
                    return;
                }

                昨最高 = Data2.High[0];
                昨最低 = Data2.Low[0];
                昨收盤 = Data2.Close[0];
                今開盤 = Bars.Open[0];
                開高價 = Bars.High[0];
                開低價 = Bars.Low[0];
                開收盤 = Bars.Close[0];
                開漲跌 = Bars.Open[0] - Data2.Close[0];
                開漲跌幅 = 100 * (開漲跌 / Data2.Close[0]);
                開盤區間高點 = 0;
                開盤區間低點 = int.MaxValue;
                ATR = AvgTrueRange.AverageTrueRange(this, 14, 0, 2);
                當日作多次數 = 0;
                當日作空次數 = 0;
                前次PositionSide = EMarketPositionSide.Flat;
                今最高 = Bars.High[0];
                今最低 = Bars.Low[0];
                今日停單 = false;
                週間日 = Bars.Time[0].DayOfWeek;

                // 資金管理：依模式計算下單規模
                double m_可用金額 = InitialCapital + NetProfit;
                double m_冒險金額 = m_可用金額 * 資金_風險比例;
                double m_市場風險 = 0;
                下單規模 = 資金_基本規模;

                switch (資金_管理模式) {
                    case 0: // 固定規模
                        下單規模 = 資金_基本規模;
                        break;
                    case 1: // 以保證金為基礎計算下單規模
                        m_市場風險 = Bars.Info.Margin;
                        下單規模 = Math.Max((int)Math.Floor(m_冒險金額 / m_市場風險), 資金_基本規模);
                        break;
                    case 2: // 以波動度為基礎計算下單規模
                        m_市場風險 = ATR * Bars.Info.BigPointValue;
                        下單規模 = Math.Max((int)Math.Floor(m_冒險金額 / m_市場風險), 資金_基本規模);
                        break;
                }

                if (資金_跳空倍注 > 0 && 開漲跌幅 >= 資金_跳空倍注) {
                    下單規模 *= 2;
                }

                if (資金_均多倍注 > 0 && 開收盤 > Data2.Close.Average(資金_均多倍注)) {
                    下單規模 *= 2;
                }

                if (資金_均空倍注 > 0 && 開收盤 < Data2.Close.Average(資金_均空倍注)) {
                    下單規模 *= 2;
                }

                #region 計算指標策略數值1
                // 原版 Dual Thrust 公式
                計算DualThrust(作多天數);
                作多區間 = 開收盤 + 作多參數 * Math.Max(m_HH - m_LC, m_HC - m_LL);
                計算DualThrust(作空天數);
                作空區間 = 開收盤 - 作空參數 * Math.Max(m_HH - m_LC, m_HC - m_LL);

                // 若 作多區間 - 作空區間 小於商品的 0.2% 幅度，今日就停單
                if (作多區間 - 作空區間 < 今開盤 * 0.002) {
                    今日停單 = true;
                }

                if (設定_啟用詳細訊息) {
                    設定_輸出集合.Clear();
                    設定_輸出集合.Add("輸出記錄", "指標數值");
                    設定_輸出集合.Add("時間", m_資料時間);
                    設定_輸出集合.Add("作多區間", 作多區間.ToString());
                    設定_輸出集合.Add("作空區間", 作空區間.ToString());
                    輸出詳細內容();
                }
                #endregion

                // 本交易日開倉，但不在進場月份中就停單
                if (結算日集合.ContainsKey(Bars.Time[1].Date) && !結算隔日進場月份集合[Bars.Time[0].Date.Month]) {
                    今日停單 = true;
                }

                if (設定_啟用詳細訊息 && 今日停單) {
                    設定_輸出集合.Clear();
                    設定_輸出集合.Add("輸出記錄", "今日停單");
                    設定_輸出集合.Add("日期", Bars.Time[0].ToString("yyyy-MM-dd"));
                    設定_輸出集合.Add("停單", 今日停單.ToString());
                    輸出詳細內容();
                }

                if (設定_啟用詳細訊息) {
                    設定_輸出集合.Clear();
                    設定_輸出集合.Add("輸出記錄", "下單規模");
                    設定_輸出集合.Add("口數", 下單規模.ToString());
                    輸出詳細內容();
                }

                重算日期 = 成交日期;
            }

            // 若仍無重算日期記錄就跳出
            if (重算日期 == 0) {
                return;
            }

            今最高 = Math.Max(今最高, Bars.High[0]);
            今最低 = Math.Min(今最低, Bars.Low[0]);

            #region 計算指標策略數值2

            #endregion

            // 開盤區間內只記錄高低價，不判斷訊號
            if (濾網_開盤區間時間 > 0 && 成交時間 <= 濾網_開盤區間時間) {
                開盤區間高點 = Math.Max(開盤區間高點, Bars.High[0]);
                開盤區間低點 = Math.Min(開盤區間低點, Bars.Low[0]);
                return;
            }

            if (CurrentPosition.Side == EMarketPositionSide.Long && 前次PositionSide == EMarketPositionSide.Flat) {
                當日作多次數++;
            }

            if (CurrentPosition.Side == EMarketPositionSide.Short && 前次PositionSide == EMarketPositionSide.Flat) {
                當日作空次數++;
            }

            // 判斷順勢進場新單訊號
            if (啟用順勢進場 == 1) {
                #region 順勢進場新單訊號
                if (CurrentPosition.Side == EMarketPositionSide.Flat && CurrentPosition.OpenTrades.Count == 0 && 成交時間 <= 濾網_停單時間 && !今日停單) {
                    if (濾網_啟用多單 == 1 && 當日作多次數 == 0 && Bars.Close[0] >= 作多區間 && 順勢進場週間日集合[週間日]) {
                        下單名稱 = "突破作多";

                        if (設定_啟用詳細訊息) {
                            下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("輸出下單", 下單名稱);
                            設定_輸出集合.Add("時間", m_資料時間);
                            設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                            設定_輸出集合.Add("作多區間", 作多區間.ToString());
                            設定_輸出集合.Add("下單規模", 下單規模.ToString());
                            輸出詳細內容();
                        }

                        多單.Send(下單名稱, 下單規模);
                    }

                    if (濾網_啟用空單 == 1 && 當日作空次數 == 0 && Bars.Close[0] <= 作空區間 && 順勢進場週間日集合[週間日]) {
                        下單名稱 = "跌破作空";

                        if (設定_啟用詳細訊息) {
                            下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("輸出下單", 下單名稱);
                            設定_輸出集合.Add("時間", m_資料時間);
                            設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                            設定_輸出集合.Add("作多區間", 作多區間.ToString());
                            設定_輸出集合.Add("下單規模", 下單規模.ToString());
                            輸出詳細內容();
                        }

                        空單.Send(下單名稱, 下單規模);
                    }
                }
                #endregion
            }

            // 判斷逆勢進場新單訊號
            if (啟用逆勢進場 == 1) {
                #region 逆勢進場新單訊號
                #endregion
            }

            // 判斷多單是否需要加碼
            if (CurrentPosition.Side == EMarketPositionSide.Long && CurrentPosition.OpenTrades.Count <= 加減碼_加碼次數) {
                if (Bars.Close[0] >= 最後進場部位.Price + 加減碼_加碼點數) {
                    下單名稱 = "多單加碼";

                    if (設定_啟用詳細訊息) {
                        下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                        設定_輸出集合.Clear();
                        設定_輸出集合.Add("輸出下單", 下單名稱);
                        設定_輸出集合.Add("時間", m_資料時間);
                        設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                        設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                        設定_輸出集合.Add("加碼點位", (最後進場部位.Price + 加減碼_加碼點數).ToString());
                        設定_輸出集合.Add("下單規模", ((資金_逆勢倍注 == 1) ? 下單規模 * 2 : 下單規模).ToString());
                        輸出詳細內容();
                    }

                    多單.Send(下單名稱, 最後進場部位.Contracts);
                }
            }

            // 判斷空單是否需要加碼
            if (CurrentPosition.Side == EMarketPositionSide.Short && CurrentPosition.OpenTrades.Count <= 加減碼_加碼次數) {
                if (Bars.Close[0] <= 最後進場部位.Price - 加減碼_加碼點數) {
                    下單名稱 = "空單加碼";

                    if (設定_啟用詳細訊息) {
                        下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                        設定_輸出集合.Clear();
                        設定_輸出集合.Add("輸出下單", 下單名稱);
                        設定_輸出集合.Add("時間", m_資料時間);
                        設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                        設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                        設定_輸出集合.Add("加碼點位", (最後進場部位.Price - 加減碼_加碼點數).ToString());
                        設定_輸出集合.Add("下單規模", ((資金_逆勢倍注 == 1) ? 下單規模 * 2 : 下單規模).ToString());
                        輸出詳細內容();
                    }

                    空單.Send(下單名稱, 最後進場部位.Contracts);
                }
            }

            // 判斷多單是否需要停損
            if (CurrentPosition.Side == EMarketPositionSide.Long && CurrentPosition.OpenTrades.Count > 0) {
                if (Bars.Close[0] <= 最後進場部位.Price - 停損點數) {
                    下單名稱 = "多單停損";

                    if (設定_啟用詳細訊息) {
                        下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                        設定_輸出集合.Clear();
                        設定_輸出集合.Add("輸出下單", 下單名稱 + "_虧損");
                        設定_輸出集合.Add("時間", m_資料時間);
                        設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                        設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                        設定_輸出集合.Add("停損點位", (最後進場部位.Price - 停損點數).ToString());
                        輸出詳細內容();
                    }

                    多停.Send(下單名稱, 最後進場部位.Contracts);
                }

                #region 多單指標停損訊號
                if (啟用指標停損 == 1 && Bars.Close[0] <= 作空區間) {
                    下單名稱 = "多單停損";

                    if (設定_啟用詳細訊息) {
                        下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                        設定_輸出集合.Clear();
                        設定_輸出集合.Add("輸出下單", 下單名稱 + "_作空區間");
                        設定_輸出集合.Add("時間", m_資料時間);
                        設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                        設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                        設定_輸出集合.Add("作空區間", 作空區間.ToString());
                        輸出詳細內容();
                    }

                    多停.Send(下單名稱, 最後進場部位.Contracts);
                }
                #endregion
            }

            // 判斷空單是否需要停損
            if (CurrentPosition.Side == EMarketPositionSide.Short && CurrentPosition.OpenTrades.Count > 0) {
                if (Bars.Close[0] >= 最後進場部位.Price + 停損點數) {
                    下單名稱 = "空單停損";

                    if (設定_啟用詳細訊息) {
                        下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                        設定_輸出集合.Clear();
                        設定_輸出集合.Add("輸出下單", 下單名稱 + "_虧損");
                        設定_輸出集合.Add("時間", m_資料時間);
                        設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                        設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                        設定_輸出集合.Add("停損點位", (最後進場部位.Price + 停損點數).ToString());
                        輸出詳細內容();
                    }

                    空停.Send(下單名稱, 最後進場部位.Contracts);
                }

                #region 空單指標停損訊號
                if (啟用指標停損 == 1 && Bars.Close[0] >= 作多區間) {
                    下單名稱 = "空單停損";

                    if (設定_啟用詳細訊息) {
                        下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                        設定_輸出集合.Clear();
                        設定_輸出集合.Add("輸出下單", 下單名稱 + "_作多區間");
                        設定_輸出集合.Add("時間", m_資料時間);
                        設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                        設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                        設定_輸出集合.Add("作多區間", 作多區間.ToString());
                        輸出詳細內容();
                    }

                    空停.Send(下單名稱, 最後進場部位.Contracts);
                }
                #endregion
            }

            // 當沖/隔日沖平倉
            if (CurrentPosition.Side != EMarketPositionSide.Flat && CurrentPosition.OpenTrades.Count > 0) {
                // 當沖時於指定出場時間全平
                if (濾網_啟用當沖 == 1 && 成交時間 >= 當沖出場時間) {
                    if (CurrentPosition.Side == EMarketPositionSide.Long) {
                        下單名稱 = "多單當沖全平";

                        if (設定_啟用詳細訊息) {
                            下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("輸出下單", 下單名稱);
                            設定_輸出集合.Add("時間", m_資料時間);
                            設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                            設定_輸出集合.Add("下單規模", CurrentPosition.OpenLots.ToString());
                            輸出詳細內容();
                        }

                        多當沖.Send(下單名稱);
                    }

                    if (CurrentPosition.Side == EMarketPositionSide.Short) {
                        下單名稱 = "空單當沖全平";

                        if (設定_啟用詳細訊息) {
                            下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("輸出下單", 下單名稱);
                            設定_輸出集合.Add("時間", m_資料時間);
                            設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                            設定_輸出集合.Add("下單規模", CurrentPosition.OpenLots.ToString());
                            輸出詳細內容();
                        }

                        空當沖.Send(下單名稱);
                    }
                } else {
                    // 結算當日強制於當沖出場時間平倉
                    if (結算日集合.ContainsKey(Bars.Time[0].Date) && 成交時間 >= 當沖出場時間) {
                        if (CurrentPosition.Side == EMarketPositionSide.Long) {
                            下單名稱 = "多單結算日全平";

                            if (設定_啟用詳細訊息) {
                                下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                                設定_輸出集合.Clear();
                                設定_輸出集合.Add("輸出下單", 下單名稱);
                                設定_輸出集合.Add("時間", m_資料時間);
                                設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                                設定_輸出集合.Add("下單規模", CurrentPosition.OpenLots.ToString());
                                輸出詳細內容();
                            }

                            多當沖.Send(下單名稱);
                        }

                        if (CurrentPosition.Side == EMarketPositionSide.Short) {
                            下單名稱 = "空單結算日全平";

                            if (設定_啟用詳細訊息) {
                                下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                                設定_輸出集合.Clear();
                                設定_輸出集合.Add("輸出下單", 下單名稱);
                                設定_輸出集合.Add("時間", m_資料時間);
                                設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                                設定_輸出集合.Add("下單規模", CurrentPosition.OpenLots.ToString());
                                輸出詳細內容();
                            }

                            空當沖.Send(下單名稱);
                        }
                    }

                    // 隔日沖時於隔日開盤市價平倉
                    if (!結算日集合.ContainsKey(Bars.Time[0].Date) && 成交時間 >= 濾網_停單時間) {
                        if (CurrentPosition.Side == EMarketPositionSide.Long) {
                            下單名稱 = "多單隔日沖全平";

                            if (設定_啟用詳細訊息) {
                                下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                                設定_輸出集合.Clear();
                                設定_輸出集合.Add("輸出下單", 下單名稱);
                                設定_輸出集合.Add("時間", m_資料時間);
                                設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                                設定_輸出集合.Add("下單規模", CurrentPosition.OpenLots.ToString());
                                輸出詳細內容();
                            }

                            多隔沖.Send(下單名稱);
                        }

                        if (CurrentPosition.Side == EMarketPositionSide.Short) {
                            下單名稱 = "空單隔日沖全平";

                            if (設定_啟用詳細訊息) {
                                下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                                設定_輸出集合.Clear();
                                設定_輸出集合.Add("輸出下單", 下單名稱);
                                設定_輸出集合.Add("時間", m_資料時間);
                                設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                                設定_輸出集合.Add("下單規模", CurrentPosition.OpenLots.ToString());
                                輸出詳細內容();
                            }

                            空隔沖.Send(下單名稱);
                        }
                    }
                }
            }

            前次PositionSide = CurrentPosition.Side;
        }

        private void 輸出詳細內容() {
            List<string> 輸出內容集合 = new List<string>();
            foreach (string key in 設定_輸出集合.Keys) {
                輸出內容集合.Add(key + " = " + 設定_輸出集合[key]);
            }

            if (設定_啟用記錄輸出 && 設定_輸出集合.ContainsKey("輸出記錄")) {
                輸出記錄內容(輸出內容集合);
            }

            if (設定_啟用下單輸出 && 設定_輸出集合.ContainsKey("輸出下單")) {
                輸出下單內容(輸出內容集合);
            }
        }

        private void 輸出記錄內容(List<string> 輸出內容集合) {
            string 記錄別 = 輸出內容集合[0].Split(new char[] { '=' })[1].Trim();
            Console.WriteLine(記錄別 + "：" + string.Join(", ", 輸出內容集合.Skip(1).ToArray()));
            Output.WriteLine(記錄別 + "：" + string.Join(", ", 輸出內容集合.Skip(1).ToArray()));
            File.WriteAllText(設定_記錄檔目錄 + 成交日期.ToString() + "_" + 成交時間.ToString() + "_" + 設定_策略名稱 + "_" + 記錄別 + ".log", string.Join(System.Environment.NewLine, 輸出內容集合.Skip(1).ToArray()));
        }

        private void 輸出下單內容(List<string> 輸出內容集合) {
            string 下單名稱 = 輸出內容集合[0].Split(new char[] { '=' })[1].Trim();
            Console.WriteLine(下單名稱 + "：" + string.Join(", ", 輸出內容集合.Skip(1).ToArray()));
            Output.WriteLine(下單名稱 + "：" + string.Join(", ", 輸出內容集合.Skip(1).ToArray()));
            File.WriteAllText(設定_下單檔目錄 + 成交日期.ToString() + "_" + 成交時間.ToString() + "_" + 下單名稱 + ".log", string.Join(System.Environment.NewLine, 輸出內容集合.Skip(1).ToArray()));
        }

        #region 計算指標策略數值3
        private void 計算DualThrust(int 天數) {
            m_HH = Data2.High.Highest(天數);
            m_HC = Data2.Close.Highest(天數);
            m_LL = Data2.Low.Lowest(天數);
            m_LC = Data2.Close.Lowest(天數);
        }
        #endregion
    }
}