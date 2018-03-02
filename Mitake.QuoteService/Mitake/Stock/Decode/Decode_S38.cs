using System;
using System.Text;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Products;
using Zeghs.Managers;
using Mitake.Sockets;
using Mitake.Stock.Data;
using Mitake.Stock.Util;
using Mitake.Sockets.Data;

namespace Mitake.Stock.Decode {
        /*
         *   個股基本資料封包(<type:1> = ' 8 '，[38]16)
         *     [data:N] = [class:1] [SID:5] [name:6] {[mark:1] [SIDx:10~13]}擴充選項
         */
        /// <summary>
        ///   個股基本資料
        /// </summary>
        internal sealed class Decode_S38 {
                internal static void Decode(int serial, PacketBuffer buffer) {
                        byte bSize = 0, bMark = 0, bFlag = 0, bTemp = 0;
			MitakeSymbolInformation cSymbolInfo = new MitakeSymbolInformation();

                        bSize = buffer.Data[3];
                        buffer.Position = 7; //移動至資料結構

                        //判斷股票別(如果為0 表示無此資料)
                        bFlag = buffer[0];

                        //取得市場別(0=集中市場　1=上櫃　2=期貨　3=興櫃)
                        cSymbolInfo.市場別 = BitConvert.GetValue(bFlag, 6, 2);
  
                        //取得是否為警示股
                        bTemp = BitConvert.GetValue(bFlag, 5, 1);
                        cSymbolInfo.警示 = ((bTemp == 1) ? true : false);

                        //取得是否為下市股票(五個交易日後移除)
                        bTemp = BitConvert.GetValue(bFlag, 4, 1);
                        cSymbolInfo.下市 = ((bTemp == 1) ? true : false);

                        //取得市場分類(參閱解碼表代號)
                        cSymbolInfo.市場分類 = BitConvert.GetValue(bFlag, 0, 4);
                        ++buffer.Position;

                        //取得股票代號(舊格式股票代號)
			cSymbolInfo.SymbolId = Encoding.UTF8.GetString(buffer.Data, buffer.Position, 5).Trim();
			buffer.Position += 5;

                        //取得股票名稱
                        cSymbolInfo.SymbolName = Encoding.UTF8.GetString(buffer.Data, buffer.Position, 9).Replace("\0", string.Empty);
                        buffer.Position += 9;

                        //期貨擴充(1:一般, 2:現月, 3:次月  PS:股票無使用)
                        bMark = buffer[0];
                        cSymbolInfo.FutureMark = bMark;
                        ++buffer.Position;

			string sSymbolId = string.Empty;
			if (cSymbolInfo.市場別 == 2) { //判斷是否為期貨
				if (cSymbolInfo.市場分類 == 2) {
					//如果市場分類 == 2，則可能是選擇權(SID2 = 10Bytes)
					sSymbolId = Encoding.UTF8.GetString(buffer.Data, buffer.Position, 10).Trim();
					buffer.Position += 10;
				} else {
					sSymbolId = Encoding.UTF8.GetString(buffer.Data, buffer.Position, 10).Trim();
					buffer.Position += 13;
				}
			} else {
				//取得SID2
				sSymbolId = Encoding.UTF8.GetString(buffer.Data, buffer.Position, 6).Trim();
				buffer.Position += 6;

				//取得個股產業類別
				cSymbolInfo.產業別 = Encoding.UTF8.GetString(buffer.Data, buffer.Position, 2);
				buffer.Position += 2;

				//取得個股證券類別
				cSymbolInfo.證券別 = Encoding.UTF8.GetString(buffer.Data, buffer.Position, 2);
				buffer.Position += 2;
			}

			if (sSymbolId.Length > 0) {
				//轉換為標準台股商品代號格式
				string sProductId = MitakeSymbolManager.Convert(cSymbolInfo.SymbolId, sSymbolId, (cSymbolInfo.市場別 == 2) ? cSymbolInfo.市場分類 : 0);
				if (sProductId != null) {
					cSymbolInfo.SymbolId = sProductId;

					if (!MitakeSymbolManager.IsExist(sProductId)) {
						AddProductToExchange(cSymbolInfo);  //將股票代號更新至交易所內
					}
					MitakeSymbolManager.AddQuoteSymbolInformation(serial, cSymbolInfo);  //將基本資訊加入代號管理員內
				}
			}
                }
		
		private static void AddProductToExchange(MitakeSymbolInformation symbolInformation) {
			string sCommodityId = "UNKNOWN";
			ESymbolCategory cCategory = ESymbolCategory.Stock;
			switch (symbolInformation.市場別) {
				case 0:  //集中市場
				case 1:  //上櫃市場
					string sType = ((symbolInformation.市場別 == 0) ? "TSE" : "OTC");
					sCommodityId = string.Format("{0}_STOCK", sType);

					switch (symbolInformation.市場分類) {
						case 3:  //基金
							cCategory = ESymbolCategory.Spread;
							sCommodityId = string.Format("{0}_{1}", sType, "SPREAD");
							break;
						case 4:  //認股權證
							cCategory = ESymbolCategory.Warrant;
							sCommodityId = string.Format("{0}_{1}", sType, "WARRANT");
							break;
						case 9:  //中央政府公債
							cCategory = ESymbolCategory.Bond;
							sCommodityId = string.Format("{0}_{1}", sType, "BOND");
							break;
						case 10:  //富時指數(TW50)
							cCategory = ESymbolCategory.Index;
							sCommodityId = "INDEX";
							break;
					}
					break;
				case 2:  //期貨市場
					sCommodityId = symbolInformation.SymbolId.Substring(0, 3);

					switch (symbolInformation.市場分類) {
						case 1: //一般
							cCategory = ESymbolCategory.Future;
							break;
						case 2:  //選擇權
							if (MitakeSymbolManager.IsIndexOption(sCommodityId)) {
								cCategory = ESymbolCategory.IndexOption;
							} else {
								cCategory = ESymbolCategory.StockOption;
							}
							break;
					}
					break;
			}

			string sExchangeName = MitakeSymbolManager.ExchangeName;
			AbstractExchange cExchange = ProductManager.Manager.GetExchange(sExchangeName);
			cExchange.AddProduct(new Product() {
				CommodityId = sCommodityId,
				Category = cCategory,
				SymbolId = symbolInformation.SymbolId,
				SymbolName = symbolInformation.SymbolName
			});
		}
	}
}