using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PowerLanguage.Function;
using Zeghs.Scripts;

namespace PowerLanguage.Strategy {
    [ScriptProperty(ScriptType = ScriptType.Signal, Version = "1.0.150707", Company = "量化策略交易員", Copyright = "Copyright © 2015 量化策略交易員. 保留一切權利。", Comment = "Martingale")]
    // 2007/10/08 之後，才有 13:40 至 13:45 的跳動資料

    public class __Martingale_Signal : SignalObject {
        private const string 設定_策略名稱 = "Martingale"; // 設定策略名稱
        private const string 設定_策略版本 = "1.0.150707"; //設定策略版本
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
        private int 當沖出場時間, 重算日期, 成交日期, 成交時間, 當日作多次數, 當日作空次數;
        private double 作多區間, 作空區間, 開盤區間高點, 開盤區間低點, ATR;
        private double 今最高, 今最低, 今開盤, 開高價, 開低價, 開收盤, 開漲跌, 開漲跌幅, 昨最高, 昨最低, 昨收盤;
        private bool 今日停單;
        private EMarketPositionSide 前次PositionSide;
        private DayOfWeek 週間日;
        private IInstrument Data2;

        #region 宣告指標策略內部變數
        /// <summary>
        /// 用來記錄每次下單的次數
        /// </summary>
        private int 單次下單次數 = 0;
        private int 今日成功交易次數 = 0;
        private int 上筆成交時間 = 0;
        private bool Is多單獲利 = false;
        private bool Is空單獲利 = false;
        #endregion

        private ITradeOrder 最後進場部位 {
            get {
                return CurrentPosition.OpenTrades.Last().EntryOrder;
            }
        }

        #region 宣告指標策略外部變數
        /// <summary>
        /// 如果為09:30請填93000
        /// </summary>
        [Input]
        public int 開始交易時間 {
            get;
            set;
        }
        /// <summary>
        /// 13:00 請填 130000 時間到時全部平倉
        /// </summary>
        [Input]
        public int 結束交易時間 {
            get;
            set;
        }
        [Input]
        public int 今日交易獲利次數 {
            get;
            set;
        }
        /// <summary>
        /// 最多4次
        /// </summary>
        [Input]
        public int 今日交易連續虧損最多次數 {
            get;
            set;
        }
        /// <summary>
        /// 完全未交易:0 , 虧損一次:1 ,虧損兩次:2 , 虧損三次:3 , 虧損四次:4
        /// </summary>
        [Input]
        public int 期初狀態 {
            get;
            set;
        }

        [Input]
        public int 交易多空1 {
            get;
            set;
        }
        /// <summary>
        /// 延遲秒數1
        /// </summary>
        [Input]
        public int 間隔秒數1 {
            get;
            set;
        }
        [Input]
        public int 自設停利點數1 {
            get;
            set;
        }

        [Input]
        public int 自設停損點數1 {
            get;
            set;
        }

        [Input]
        public int 下單口數1 {
            get;
            set;
        }

        [Input]
        public int 交易多空2 {
            get;
            set;
        }
        /// <summary>
        /// 延遲秒數2
        /// </summary>
        [Input]
        public int 間隔秒數2 {
            get;
            set;
        }
        [Input]
        public int 自設停利點數2 {
            get;
            set;
        }

        [Input]
        public int 自設停損點數2 {
            get;
            set;
        }

        [Input]
        public int 下單口數2 {
            get;
            set;
        }

        [Input]
        public int 交易多空3 {
            get;
            set;
        }
        /// <summary>
        /// 延遲秒數3
        /// </summary>
        [Input]
        public int 間隔秒數3 {
            get;
            set;
        }
        [Input]
        public int 自設停利點數3 {
            get;
            set;
        }

        [Input]
        public int 自設停損點數3 {
            get;
            set;
        }

        [Input]
        public int 下單口數3 {
            get;
            set;
        }

        [Input]
        public int 交易多空4 {
            get;
            set;
        }
        /// <summary>
        /// 延遲秒數4
        /// </summary>
        [Input]
        public int 間隔秒數4 {
            get;
            set;
        }
        [Input]
        public int 自設停利點數4 {
            get;
            set;
        }

        [Input]
        public int 自設停損點數4 {
            get;
            set;
        }

        [Input]
        public int 下單口數4 {
            get;
            set;
        }

        #endregion



        [Input]
        public string 順勢進場週間日 {
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
        public double 資金_風險比例 {
            get;
            set;
        }
        [Input]
        public int 資金_基本規模 {
            get;
            set;
        }


        /// <summary>
        /// 1:啟用當沖
        /// </summary>
        [Input]
        public int 濾網_啟用當沖 {
            get;
            set;
        }



        public __Martingale_Signal(object _ctx)
            : base(_ctx) {
            #region 指標策略參數預設值

            開始交易時間 = 90000; //9:30:00
            結束交易時間 = 132900;//13:00:00
            今日交易獲利次數 = 10;
            今日交易連續虧損最多次數 = 4;
            期初狀態 = 0;

            // 第1次
            交易多空1 = 1;//1:多單 0:空單
            間隔秒數1 = 3;
            自設停利點數1 = 10; //10點
            自設停損點數1 = 10;
            下單口數1 = 1;
            // 第2次
            交易多空2 = 1;//1:多單 0:空單
            間隔秒數2 = 3; //10秒
            自設停利點數2 = 10; //10點
            自設停損點數2 = 10;
            下單口數2 = 2;
            // 第3次
            交易多空3 = 1;//1:多單 0:空單
            間隔秒數3 = 3;
            自設停利點數3 = 10; //10點
            自設停損點數3 = 10;
            下單口數3 = 4;
            // 第4次
            交易多空4 = 1;//1:多單 0:空單
            間隔秒數4 = 3;
            自設停利點數4 = 10; //10點
            自設停損點數4 = 10;
            下單口數4 = 8;
            #endregion


            順勢進場週間日 = "0,1,2,3,4,5,6"; // 0 為週日，1~6 為週一至週六          
            逆勢進場週間日 = "0,1,2,3,4,5,6"; // 0 為週日，1~6 為週一至週六 (逆勢績效一、二佳，二、四差)
            結算隔日進場月份 = "1,2,3,4,5,10,11,12";


            資金_風險比例 = 0.2; // 保證金建議值：0.2；波動度建議值：0.06
            資金_基本規模 = 1; //預設1口
            濾網_啟用當沖 = 1;

            結束交易時間 = 134500;


            設定_系統磁碟代號 = Path.GetPathRoot(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows));
            設定_程式交易目錄 = 設定_系統磁碟代號 + @"QuantTrader\";
            設定_資料檔目錄 = 設定_程式交易目錄 + @"Databases\";
            設定_記錄檔目錄 = 設定_程式交易目錄 + @"Logs\";
            設定_下單檔目錄 = 設定_程式交易目錄 + @"Orders\";
        }

        protected override void Create() {
            // 初始化下單物件，Contracts.UserSpecified 可指定規模，OrderExit.FromAll 可一次全平
            多單 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
            空單 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));
            多停 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
            空停 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover, OrderExit.FromAll));
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

            單次下單次數 = 期初狀態;
            #endregion
        }

        protected override void StartCalc() {
            Data2 = BarsOfData(2);
            當沖出場時間 = 132955;
            前次PositionSide = EMarketPositionSide.Flat;

            // 啟用當沖時，若停單時間比 13:30 晚，就提前至 13:30
            if (濾網_啟用當沖 == 1) {
                結束交易時間 = Math.Min(結束交易時間, 當沖出場時間);
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
            //停損點數 = 加減碼_停損金額 / 每跳動點價值;

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
                設定_輸出集合.Add("開始交易時間", 開始交易時間.ToString());
                設定_輸出集合.Add("結束交易時間", 結束交易時間.ToString());
                設定_輸出集合.Add("今日交易獲利次數", 今日交易獲利次數.ToString());
                設定_輸出集合.Add("今日交易連續虧損最多次數", 今日交易連續虧損最多次數.ToString());
                設定_輸出集合.Add("期初狀態", 期初狀態.ToString());

                設定_輸出集合.Add("交易多空1", 交易多空1.ToString());
                設定_輸出集合.Add("間隔秒數1", 間隔秒數1.ToString());
                設定_輸出集合.Add("自設停利點數1", 自設停利點數1.ToString());
                設定_輸出集合.Add("自設停損點數1", 自設停損點數1.ToString());
                設定_輸出集合.Add("下單口數1", 下單口數1.ToString());

                設定_輸出集合.Add("交易多空2", 交易多空2.ToString());
                設定_輸出集合.Add("間隔秒數2", 間隔秒數2.ToString());
                設定_輸出集合.Add("自設停利點數2", 自設停利點數2.ToString());
                設定_輸出集合.Add("自設停損點數2", 自設停損點數2.ToString());
                設定_輸出集合.Add("下單口數2", 下單口數2.ToString());

                設定_輸出集合.Add("交易多空3", 交易多空3.ToString());
                設定_輸出集合.Add("間隔秒數3", 間隔秒數3.ToString());
                設定_輸出集合.Add("自設停利點數3", 自設停利點數3.ToString());
                設定_輸出集合.Add("自設停損點數3", 自設停損點數3.ToString());
                設定_輸出集合.Add("下單口數3", 下單口數3.ToString());

                設定_輸出集合.Add("交易多空4", 交易多空4.ToString());
                設定_輸出集合.Add("間隔秒數4", 間隔秒數4.ToString());
                設定_輸出集合.Add("自設停利點數4", 自設停利點數4.ToString());
                設定_輸出集合.Add("自設停損點數4", 自設停損點數4.ToString());
                設定_輸出集合.Add("下單口數4", 下單口數4.ToString());
                #endregion
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
                #region 計算指標策略數值1
                單次下單次數 = 期初狀態; //換日需重算，需在此填入初始值
                今日成功交易次數 = 0;//換日需重算
                #endregion



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



            if (CurrentPosition.Side == EMarketPositionSide.Long && 前次PositionSide == EMarketPositionSide.Flat) {
                
                if (設定_啟用詳細訊息) {
                    設定_輸出集合.Clear();
                    設定_輸出集合.Add("輸出下單", 設定_策略名稱 + "_狀態");
                    設定_輸出集合.Add("目前狀態", string.Format("失敗:{0} /成功:{1}", 單次下單次數, 今日成功交易次數));
                    輸出詳細內容();
                }
                單次下單次數++;
            }

            if (CurrentPosition.Side == EMarketPositionSide.Short && 前次PositionSide == EMarketPositionSide.Flat) {
               
                if (設定_啟用詳細訊息) {
                    設定_輸出集合.Clear();
                    設定_輸出集合.Add("輸出下單", 設定_策略名稱 + "_狀態");
                    設定_輸出集合.Add("目前狀態", string.Format("失敗:{0} /成功:{1}", 單次下單次數, 今日成功交易次數));
                    輸出詳細內容();
                }
                單次下單次數++;
            }

            if (CurrentPosition.Side == EMarketPositionSide.Flat && 前次PositionSide == EMarketPositionSide.Long) {
                上筆成交時間 = int.Parse(Bars.BarUpdateTime.ToString("HHmmss"));
                if (Is多單獲利) {
                    Is多單獲利 = false;
                    當日作多次數++;
                    今日成功交易次數++;
                    單次下單次數 = 0; //停利重頭開始後繼續玩
                    if (今日成功交易次數 >= 今日交易獲利次數) {
                        // 獲利了結今日停單
                        今日停單 = true;

                        if (設定_啟用詳細訊息) {
                            下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("輸出下單", 下單名稱 + "_停單");
                            設定_輸出集合.Add("已達今日成功交易次數", 今日成功交易次數.ToString());
                            設定_輸出集合.Add("目前狀態", string.Format("失敗:{0} /成功:{1}", 單次下單次數, 今日成功交易次數));
                            輸出詳細內容();
                        }
                    }
                }
            }
            if (CurrentPosition.Side == EMarketPositionSide.Flat && 前次PositionSide == EMarketPositionSide.Short) {
                上筆成交時間 = int.Parse(Bars.BarUpdateTime.ToString("HHmmss"));
                if (Is空單獲利) {
                    Is空單獲利 = false;
                    當日作空次數++;
                    今日成功交易次數++;
                    單次下單次數 = 0; //停利重頭開始後繼續玩
                    if (今日成功交易次數 >= 今日交易獲利次數) {
                        // 獲利了結今日停單
                        今日停單 = true;

                        if (設定_啟用詳細訊息) {
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("輸出下單", 設定_策略名稱 + "_停單");
                            設定_輸出集合.Add("已達今日成功交易次數", 今日成功交易次數.ToString());
                            設定_輸出集合.Add("目前狀態", string.Format("失敗:{0} /成功:{1}", 單次下單次數, 今日成功交易次數));
                            輸出詳細內容();
                        }
                    }
                }
            }

            //--- 下單  ---//
            if (成交時間 <= 結束交易時間 && CurrentPosition.Side == EMarketPositionSide.Flat && CurrentPosition.OpenTrades.Count == 0 && !今日停單) {

                //第一次 時間到多單進場
                switch (單次下單次數) {
                    case 0:
                        int rt0 = CalDelay((上筆成交時間 == 0) ? 開始交易時間 : 上筆成交時間, 間隔秒數1);
                        if (成交時間 >=rt0) {
                            if (交易多空1 == 1) {
                                下單名稱 = "做多1";
                                多單.Send(下單名稱, 下單口數1);
                            } else {
                                下單名稱 = "做空1";
                                空單.Send(下單名稱, 下單口數1);
                            }
                        }
                        break;
                    case 1:
                        int rt1 = CalDelay(上筆成交時間, 間隔秒數2);
                        if (成交時間 >= rt1) {
                            if (交易多空2 == 1) {
                                下單名稱 = "做多2";
                                多單.Send(下單名稱, 下單口數2);
                            } else {
                                下單名稱 = "做空2";
                                空單.Send(下單名稱, 下單口數2);
                            }
                        }
                        break;
                    case 2:
                        int rt2 = CalDelay(上筆成交時間, 間隔秒數3);
                        if (成交時間 >= rt2) {
                            if (交易多空3 == 1) {
                                下單名稱 = "做多3";
                                多單.Send(下單名稱, 下單口數3);
                            } else {
                                下單名稱 = "做空3";
                                空單.Send(下單名稱, 下單口數3);
                            }
                        }
                        break;
                    case 3:
                        int rt3 = CalDelay(上筆成交時間, 間隔秒數4);
                        if (成交時間 >= rt3) {
                            if (交易多空4 == 1) {
                                下單名稱 = "做多4";
                                多單.Send(下單名稱, 下單口數4);
                            } else {
                                下單名稱 = "做空4";
                                空單.Send(下單名稱, 下單口數4);
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (今日交易連續虧損最多次數 <= 單次下單次數) { // 已達連續虧損次數立即停單
                    今日停單 = true;

                    if (設定_啟用詳細訊息) {
                        設定_輸出集合.Clear();
                        設定_輸出集合.Add("輸出下單", 設定_策略名稱 + "_停單");
                        設定_輸出集合.Add("已達連續虧損次數", 今日交易連續虧損最多次數.ToString());
                        設定_輸出集合.Add("目前狀態", string.Format("失敗:{0} /成功:{1}", 單次下單次數, 今日成功交易次數));
                        輸出詳細內容();
                    }
                }
            }



            //--- 平倉 ---//
            if (CurrentPosition.OpenTrades.Count > 0) {
                int i停利點數 = 0, i停損點數 = 0;
                switch (單次下單次數) {
                    case 1:
                        i停利點數 = 自設停利點數1;
                        i停損點數 = 自設停損點數1;
                        break;
                    case 2:
                        i停利點數 = 自設停利點數2;
                        i停損點數 = 自設停損點數2;
                        break;
                    case 3:
                        i停利點數 = 自設停利點數3;
                        i停損點數 = 自設停損點數3;
                        break;
                    case 4:
                        i停利點數 = 自設停利點數4;
                        i停損點數 = 自設停損點數4;
                        break;
                    default:
                        break;
                }
                // 判斷多單是否需要停損
                if (CurrentPosition.Side == EMarketPositionSide.Long) {

                    if (Bars.Close[0] >= 最後進場部位.Price + i停利點數) {
                        下單名稱 = "多單停利";

                        if (設定_啟用詳細訊息) {
                            下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("輸出下單", 下單名稱 + "_獲利");
                            設定_輸出集合.Add("時間", m_資料時間);
                            設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                            設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                            設定_輸出集合.Add("自設停利點數", (最後進場部位.Price + i停利點數).ToString());
                            輸出詳細內容();
                        }

                        多停.Send(下單名稱);

                        Is多單獲利 = true;

                    }

                    if (Bars.Close[0] <= 最後進場部位.Price - i停損點數) {
                        下單名稱 = "多單停損";

                        if (設定_啟用詳細訊息) {
                            下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("輸出下單", 下單名稱 + "_虧損");
                            設定_輸出集合.Add("時間", m_資料時間);
                            設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                            設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                            設定_輸出集合.Add("自設停損點數", (最後進場部位.Price - i停損點數).ToString());
                            輸出詳細內容();
                        }

                        多停.Send(下單名稱);
                    }

                }

                // 判斷空單是否需要停損
                if (CurrentPosition.Side == EMarketPositionSide.Short) {
                    if (Bars.Close[0] <= 最後進場部位.Price - i停利點數) {
                        下單名稱 = "空單停利";

                        if (設定_啟用詳細訊息) {
                            下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("輸出下單", 下單名稱 + "_獲利");
                            設定_輸出集合.Add("時間", m_資料時間);
                            設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                            設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                            設定_輸出集合.Add("停利點位", (最後進場部位.Price - i停利點數).ToString());
                            輸出詳細內容();
                        }

                        空停.Send(下單名稱);

                        Is空單獲利 = true;
                    }
                    if (Bars.Close[0] >= 最後進場部位.Price + i停損點數) {
                        下單名稱 = "空單停損";

                        if (設定_啟用詳細訊息) {
                            下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("輸出下單", 下單名稱 + "_虧損");
                            設定_輸出集合.Add("時間", m_資料時間);
                            設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                            設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                            設定_輸出集合.Add("停損點位", (最後進場部位.Price + i停損點數).ToString());
                            輸出詳細內容();
                        }

                        空停.Send(下單名稱);
                    }

                }

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
                    if (!結算日集合.ContainsKey(Bars.Time[0].Date) && 成交時間 >= 結束交易時間) {
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
        /// <summary>
        /// 回傳延遲秒數時間
        /// </summary>
        /// <param name="OriginTime">原來時間HHmmss</param>
        /// <param name="DelaySec">延遲秒數(sec)</param>
        /// <returns>延遲後時間</returns>
        private int CalDelay(int OriginTime, int DelaySec) {
            if (DelaySec > 0) {
                //DateTime OT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (OriginTime / 10000), ((OriginTime / 100) % 100), (OriginTime % 100));
                DateTime OT = new DateTime(2015, 7, 1, (OriginTime / 10000), ((OriginTime / 100) % 100), (OriginTime % 100));
                OT = OT.AddSeconds(DelaySec);
                int RParse = int.Parse(OT.ToString("HHmmss"));
                return RParse;
            } else {
                return OriginTime;
            }
        }
        #endregion
    }
}