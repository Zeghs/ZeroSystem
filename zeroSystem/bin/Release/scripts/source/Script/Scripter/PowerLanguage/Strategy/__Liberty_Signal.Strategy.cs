using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PowerLanguage.Function;
using Zeghs.Scripts;

namespace PowerLanguage.Strategy {
    [ScriptProperty(ScriptType = ScriptType.Signal, Version = "1.0.0.150803", Company = "量化策略交易員", Copyright = "Copyright © 2015 量化策略交易員. 保留一切權利。", Comment = "LibertyEA")]
    public sealed class __Liberty_Signal : SignalObject {
        private const string 設定_策略名稱 = "LibertyEA"; // 設定策略名稱
        private const string 設定_策略版本 = "1.0.0.150803"; //設定策略版本
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
        private IOrderMarket 多單 = null, 多停 = null;

        private int 重算日期, 成交日期, 成交時間, 當日作多次數, 當日作空次數;
        private EMarketPositionSide 前次PositionSide;

        private double __d突破價 = 0; // 突破做多價格
        private double __dMaxPrice = 0; //紀錄移動停損最高價位

        /// <summary>
        /// 如果為09:30請填93000
        /// </summary>
        [Input]
        public int 開始交易時間 {
            get;
            set;
        }
        /// <summary>
        /// 12:00 請填 120000 判斷結束時間
        /// </summary>
        [Input]
        public int 結束交易時間 {
            get;
            set;
        }
        /// <summary>
        /// 13:30 請填 133000
        /// </summary>
        [Input]
        public int 當沖全平時間 {
            get;
            set;
        }
        [Input]
        public int 下單口數 {
            get;
            set;
        }
        [Input]
        public double 停損點數 {
            get;
            set;
        }
        [Input]
        public double 停利點數 {
            get;
            set;
        }


        private string 下單名稱;
        private ITradeOrder 最後進場部位 {
            get {
                return CurrentPosition.OpenTrades.Last().EntryOrder;
            }
        }

        public __Liberty_Signal(object _ctx)
            : base(_ctx) {
            開始交易時間 = 90100; //9:30:00
            結束交易時間 = 120000;//13:00:00
            當沖全平時間 = 133000;//13:30:00

            下單口數 = 1;
            停損點數 = 5;
            停利點數 = 8;

            設定_系統磁碟代號 = Path.GetPathRoot(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows));
            設定_程式交易目錄 = 設定_系統磁碟代號 + @"QuantTrader\";
            設定_資料檔目錄 = 設定_程式交易目錄 + @"Databases\";
            設定_記錄檔目錄 = 設定_程式交易目錄 + @"Logs\";
            設定_下單檔目錄 = 設定_程式交易目錄 + @"Orders\";
        }

        protected override void Create() {
            // 初始化下單物件，Contracts.UserSpecified 可指定規模，OrderExit.FromAll 可一次全平
            多單 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
            多停 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
        }
        protected override void StartCalc() {

            前次PositionSide = EMarketPositionSide.Flat;
            重算日期 = 0;
        }
        protected override void CalcBar() {
            成交日期 = int.Parse(Bars.Time[0].Date.ToString("yyyyMMdd"));
            成交時間 = int.Parse(Bars.BarUpdateTime.ToString("HHmmss"));
            string m_資料時間 = Bars.Time[0].ToString("HH:mm:ss");

            if (設定_啟用詳細訊息 && 設定_輸出集合 == null) {
                設定_輸出集合 = new Dictionary<string, string>();
                設定_輸出集合.Add("OutputLogs", "參數設定");
                設定_輸出集合.Add("策略名稱", 設定_策略名稱);
                設定_輸出集合.Add("策略版本", 設定_策略版本);

                設定_輸出集合.Add("開始交易時間", Convert時間(開始交易時間));
                設定_輸出集合.Add("結束交易時間", Convert時間(結束交易時間));
                設定_輸出集合.Add("當沖全平時間", Convert時間(當沖全平時間));

                設定_輸出集合.Add("下單口數", 下單口數.ToString());
                設定_輸出集合.Add("停損點數", 停損點數.ToString());
                設定_輸出集合.Add("停利點數", 停利點數.ToString());

                輸出詳細內容();
            }
            // 換日後需重算關鍵數值
            if (Bars.Time[0].Date > Bars.Time[1].Date && 重算日期 < 成交日期) {
                if (設定_啟用詳細訊息) {
                    設定_輸出集合.Clear();
                    設定_輸出集合.Add("OutputLogs", "新交易日");
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

                __d突破價 = 0;
                __dMaxPrice = 0;

                當日作多次數 = 0;
                當日作空次數 = 0;
                前次PositionSide = EMarketPositionSide.Flat;


                重算日期 = 成交日期;
            }
            // 若仍無重算日期記錄就跳出
            if (重算日期 == 0) {
                return;
            }

            if (CurrentPosition.Side == EMarketPositionSide.Long && 前次PositionSide == EMarketPositionSide.Flat) {
                當日作多次數++;
            }

            if (CurrentPosition.Side == EMarketPositionSide.Short && 前次PositionSide == EMarketPositionSide.Flat) {
                當日作空次數++;
            }
            if (CurrentPosition.Side == EMarketPositionSide.Flat && 前次PositionSide == EMarketPositionSide.Long) {
                //多單平倉
            }

            double dOpen = Bars.Open[0];
            double dHigh = Bars.High[0];
            double dLow = Bars.Low[0];
            double dClose = Bars.Close[0];
            double dVolume = Bars.Volume[0];

            //Console.WriteLine(string.Format("量_({0}){1}", Convert時間(成交時間), dVolume));

            //--- 觸發多單進場 (目前版本無倉且符合條件才下單)
            if (CurrentPosition.Side == EMarketPositionSide.Flat && CurrentPosition.OpenTrades.Count == 0) { //無倉狀態

                if (成交時間 >= 開始交易時間 && 成交時間 <= 結束交易時間) {
                    // 判斷突破條件:當該跟黑K 且 量>1000 且 當跟最低價<  前15跟最低價最小值
                    if (dVolume > 1000) { //量>1000，回測系統沒有一分K的量
                        //Console.WriteLine(string.Format("量大1000_({0}){1}>1000", Convert時間(成交時間), dVolume));
                        double dLow_ = Bars.Low[1];
                        if (成交時間 > 90100) { //取前15跟最低價
                            for (int i = 2; i <= 15; i++) {
                                dLow_ = Math.Min(dLow_, Bars.Low[i]);
                            }
                        }
                        double dLow15 = dLow_;// Bars.Low.Lowest(15);
                        //double dLow15 = Bars.Low.Lowest(15);
                        if (dLow <= dLow15) {
                            //Console.WriteLine(string.Format("跌破前15跟新低_({0}){1}<{2}", Convert時間(成交時間), dLow, dLow15));
                            if (dClose < dOpen) { // 黑K
                                //Console.WriteLine(string.Format("黑K_({0}){1}<{2}", Convert時間(成交時間), dClose, dOpen));

                                __d突破價 = dHigh;

                                下單名稱 = "偵測突破";
                                if (設定_啟用詳細訊息) {
                                    下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                                    設定_輸出集合.Clear();
                                    設定_輸出集合.Add("OutputLogs", 下單名稱);
                                    設定_輸出集合.Add("時間", m_資料時間);
                                    設定_輸出集合.Add("突破價", __d突破價.ToString());
                                    輸出詳細內容();
                                }
                            }
                        }

                    }

                    if (dClose > __d突破價 && __d突破價 > 0) {


                        下單名稱 = "突破作多";
                        if (設定_啟用詳細訊息) {
                            下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("OutputOrders", 下單名稱);
                            設定_輸出集合.Add("時間", m_資料時間);
                            設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                            設定_輸出集合.Add("突破價", __d突破價.ToString());
                            設定_輸出集合.Add("下單口數", 下單口數.ToString());
                            輸出詳細內容();
                        }

                        __d突破價 = 0; //進場後後歸零，重新找新的

                        多單.Send(下單名稱, 下單口數);
                    }


                }
            }
            //--- 多單平倉，過下單時間後也要執行平倉動作
            if (CurrentPosition.Side == EMarketPositionSide.Long) {

                double dOpenPrice = 最後進場部位.Price;

                // --- 1. 多單停利
                if (Bars.Close[0] >= dOpenPrice + 停利點數) {
                    //__d突破價 = 0; // 1.停利須歸零
                    下單名稱 = "多單停利";
                    if (設定_啟用詳細訊息) {
                        下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                        設定_輸出集合.Clear();
                        設定_輸出集合.Add("OutputOrders", 下單名稱 + "_停利");
                        設定_輸出集合.Add("時間", m_資料時間);
                        設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                        設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                        設定_輸出集合.Add("停利點位", (最後進場部位.Price + 停利點數).ToString());
                        輸出詳細內容();
                    }
                    多停.Send(下單名稱);

                }
                // --- 2. 移動停損               

                if (__dMaxPrice == 0) { //成交後初值須填入
                    __dMaxPrice = dOpenPrice;
                }
                if (dClose > dOpenPrice && dClose > __dMaxPrice) {
                    __dMaxPrice = dClose; //更新買進後最高價
                } else {
                    double dDiff = __dMaxPrice - dClose;
                    if (dDiff >= 停損點數) { //移動停損
                        //__d突破價 = 0; // 2.停損須歸零
                        下單名稱 = "移動停損";
                        if (設定_啟用詳細訊息) {
                            下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                            設定_輸出集合.Clear();
                            設定_輸出集合.Add("OutputOrders", 下單名稱 + "_多單");
                            設定_輸出集合.Add("時間", m_資料時間);
                            設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                            設定_輸出集合.Add("最後進場點位", 最後進場部位.Price.ToString());
                            設定_輸出集合.Add("高點", __dMaxPrice.ToString());
                            設定_輸出集合.Add("平倉點位", (最後進場部位.Price - 停損點數).ToString());
                            輸出詳細內容();
                        }

                        __dMaxPrice = 0;

                        多停.Send(下單名稱);
                    }
                }

                // --- 3.時間到時平倉
                if (成交時間 >= 當沖全平時間) {  //全部平倉
                    下單名稱 = "多單當沖全平";
                    //__d突破價 = 0; // 3.平倉須歸零
                    if (設定_啟用詳細訊息) {
                        下單名稱 = 設定_策略名稱 + "_" + 下單名稱;
                        設定_輸出集合.Clear();
                        設定_輸出集合.Add("OutputOrders", 下單名稱);
                        設定_輸出集合.Add("時間", m_資料時間);
                        設定_輸出集合.Add("成交價", Bars.Close[0].ToString());
                        設定_輸出集合.Add("平倉口數", CurrentPosition.OpenLots.ToString());
                        輸出詳細內容();
                    }
                    多停.Send(下單名稱);
                }
            }

            前次PositionSide = CurrentPosition.Side;
        }
        #region 輸出內容
        private void 輸出詳細內容() {
            List<string> 輸出內容集合 = new List<string>();
            foreach (string key in 設定_輸出集合.Keys) {
                輸出內容集合.Add(key + " = " + 設定_輸出集合[key]);
            }

            if (設定_啟用記錄輸出 && 設定_輸出集合.ContainsKey("OutputLogs")) {
                輸出記錄內容(輸出內容集合);
            }

            if (設定_啟用下單輸出 && 設定_輸出集合.ContainsKey("OutputOrders")) {
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

        #endregion
        /// <summary>
        /// 時間轉換
        /// </summary>
        /// <param name="時間">HHmmss ex: 90500</param>
        /// <returns>09:05:00</returns>
        private string Convert時間(int 時間) {
            return (時間 / 10000).ToString("00") + ":" + ((時間 % 10000) / 100).ToString("00") + ":" + (時間 % 100).ToString("00");
        }

    }
}