using System;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using PowerLanguage;
using Zeghs.Orders;
using Netwings.Forms;

namespace Netwings.Rules.Fees {
	/// <summary>
	///   經紀商自訂手續費用規則
	/// </summary>
	[RuleProperty(ERuleType.Fee, "CustomBrokerFeeRule", "Netwings.Rules.Fees.CustomBrokerFeeRule", "(經紀商自訂手續費用規則)\r\n\r\n經紀商(證券商)所制定之手續費用基準，費用會依各使用者條件與狀況而有所不同，可自行設定其交易手續費用。", true)]
	public sealed class CustomBrokerFeeRule : RuleBase, ICommission, IRuleSetting {
		private double __dFee = 0;

		/// <summary>
		///   [取得] 交易佣金類型
		/// </summary>
		public ERuleType RuleType {
			get {
				return ERuleType.Fee;
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		public CustomBrokerFeeRule() {
			ShowSetting();
		}

		/// <summary>
		///   建構子
		/// </summary>
		public CustomBrokerFeeRule(JToken args) {
			this.args = args;
		}

		/// <summary>
		///   計算佣金
		/// </summary>
		/// <param name="order">交易訂單</param>
		/// <returns>返回值: 佣金</returns>
		public double Calculate(ITradeOrder order) {
			return __dFee;
		}

		/// <summary>
		///   顯示設定表單視窗
		/// </summary>
		/// <returns>返回值: 0=設定完成, 其他=設計者自訂</returns>
		public int ShowSetting() {
			frmCustomBrokerFeeSetting cSetting = new frmCustomBrokerFeeSetting();
			DialogResult cResult = cSetting.ShowDialog();
			if (cResult == DialogResult.OK) {
				__dFee = cSetting.Fee;
				return 0;  //成功
			}
			return -1;  //取消
		}
	}
}