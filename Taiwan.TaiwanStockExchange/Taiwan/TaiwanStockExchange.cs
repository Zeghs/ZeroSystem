using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using PowerLanguage;
using Zeghs.Rules;
using Zeghs.Products;
using Taiwan.Products;
using Taiwan.Rules.Contracts;
using Taiwan.Rules.PriceScales;

namespace Taiwan {
	/* -- 台灣證交所商品代號定義如下 --------------------------------------------------------------
	   商品代號
	    a. 大盤指數=TWI.tw, 上櫃指數=OTC.tw
	    b. 台指期=TXF?.tw, 小台期=MXF?.tw, 電子期=EXF?.tw, 金融期=FXF?.tw, ?=近遠月(0=現月)
	    c. 選擇權=TXO?C8800.tw @前3碼為商品代號, 8800=合約價值, ?=近遠月(0=現月)
	*/
	public sealed class TaiwanStockExchange : AbstractExchange {
		internal const double TIME_ZONE = 8.0d;

		private static ProductPropertyList CreateProductPropertyList(string jsonText) {
			ProductPropertyList cBasePropertyList = new ProductPropertyList();

			Dictionary<string, TaiwanProductProperty> cPropertys = JsonConvert.DeserializeObject<Dictionary<string, TaiwanProductProperty>>(jsonText);
			foreach (TaiwanProductProperty cProperty in cPropertys.Values) {
				//設定交易時間列表給規則使用
				RuleBase cProductRule = cProperty.ContractRule;
				if (cProductRule is IContractParameters) {
					IContractParameters cRule = cProductRule as IContractParameters;
					cRule.SetParameters(cProperty.Sessions);
				}
				cBasePropertyList.AddProperty(cProperty);
			}
			return cBasePropertyList;
		}

		private Dictionary<ERuleType, List<RulePropertyAttribute>> __cRuleItems = null;

		[JsonIgnore]
		public override double TimeZone {
			get {
				return TIME_ZONE;
			}
		}

		public override List<RulePropertyAttribute> GetRuleItems(ERuleType ruleType) {
			List<RulePropertyAttribute> cRuleItems = null;
			__cRuleItems.TryGetValue(ruleType, out cRuleItems);
			return cRuleItems;
		}

		public override void Load() {
			Assembly cAssembly = Assembly.GetExecutingAssembly();
			__cRuleItems = RulePropertyAttribute.GetRules(cAssembly);

			string sLocation = cAssembly.Location;
			string sPath = Path.GetDirectoryName(sLocation);
			string sTargetName = Path.GetFileNameWithoutExtension(sLocation) + ".set";
			string sFileName = Path.Combine(sPath, sTargetName);

			string[] sDatas = File.ReadAllLines(sFileName, Encoding.UTF8);

			JObject cExchange = JsonConvert.DeserializeObject<JObject>(sDatas[0]);
			this.FullName = cExchange["FullName"].Value<string>();
			this.ShortName = cExchange["ShortName"].Value<string>();
			this.UpdateTime = cExchange["UpdateTime"].Value<DateTime>();
			
			this.Products = JsonConvert.DeserializeObject<Dictionary<string, Product>>(sDatas[1]);

			//讀取基礎商品屬性表
			this.BasePropertys = CreateProductPropertyList(sDatas[2]);

			//讀取使用者自訂商品屬性表
			this.CustomPropertys = new Dictionary<string, ProductPropertyList>(16);
			Dictionary<string, JToken> sCustoms = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(sDatas[3]);

			foreach (string sDataSource in sCustoms.Keys) {
				JToken cToken = sCustoms[sDataSource];
				ProductPropertyList cPropertyList = CreateProductPropertyList(cToken.ToString(Formatting.None));
				this.CustomPropertys.Add(sDataSource, cPropertyList);
			}
		}

		public override void Save() {
			string sLocation = Assembly.GetExecutingAssembly().Location;
			string sPath = Path.GetDirectoryName(sLocation);
			string sTargetName = Path.GetFileNameWithoutExtension(sLocation) + ".set";
			string sFileName = Path.Combine(sPath, sTargetName);

			this.UpdateTime = DateTime.Now;  //更新寫入時間

			string sCustomPropertys = "{}";
			string sExchange = JsonConvert.SerializeObject(this);
			string sProducts = JsonConvert.SerializeObject(this.Products);
			string sBasePropertys = JsonConvert.SerializeObject(this.BasePropertys.Propertys);

			int iStringCount = sExchange.Length + sProducts.Length + sBasePropertys.Length;
			int iCustomCount = this.CustomPropertys.Count;
			if (iCustomCount > 0) {
				Dictionary<string, Dictionary<string, AbstractProductProperty>> cCustoms = new Dictionary<string, Dictionary<string, AbstractProductProperty>>(this.CustomPropertys.Count);
				foreach (string sKey in this.CustomPropertys.Keys) {
					ProductPropertyList cPropertyList = this.CustomPropertys[sKey];
					cCustoms.Add(sKey, cPropertyList.Propertys);
				}

				sCustomPropertys = JsonConvert.SerializeObject(cCustoms);
				iStringCount += sCustomPropertys.Length;
			}

			StringBuilder cBuilder = new StringBuilder(iStringCount + 128);
			cBuilder.AppendLine(sExchange);        //交易所資訊
			cBuilder.AppendLine(sProducts);        //產品資訊
			cBuilder.AppendLine(sBasePropertys);   //基礎商品屬性資訊
			cBuilder.AppendLine(sCustomPropertys); //自訂商品屬性資訊

			File.WriteAllText(sFileName, cBuilder.ToString(), Encoding.UTF8);
		}

		protected override AbstractProductProperty CreateProperty(string commodityId) {
			TaiwanProductProperty cProperty = new TaiwanProductProperty() {
				CommodityId = commodityId,
				Description = string.Empty,
				CautionMoneys = new List<CautionMoney>(4),
				Sessions = new List<SessionObject>(8)
			};
			
			cProperty.Sessions.Add(new SessionObject() {
				EndDay = DayOfWeek.Monday,
				EndTime = new TimeSpan(23, 59, 0),
				StartDay = DayOfWeek.Monday,
				StartTime = TimeSpan.Zero
			});
			return cProperty;
		}
	}
}