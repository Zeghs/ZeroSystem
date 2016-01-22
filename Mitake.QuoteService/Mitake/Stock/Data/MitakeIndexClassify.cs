namespace Mitake.Stock.Data {
        /// <summary>
        ///    類股成交資訊
        /// </summary>
        public sealed class MitakeIndexClassify {
		/// <summary>
		///   [取得] 類股指數值
		/// </summary>
		public float IndexValue {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 類股成交總額
		/// </summary>
		public uint Amount {
			get;
			internal set;
		}

		/// <summary>
		///   [取得] 類股成交張數
		/// </summary>
		public uint Totals {
			get;
			internal set;
		}

		internal void Clone(MitakeIndexClassify classify) {
			if (this.Amount == 0) {
				this.Amount = classify.Amount;
			}

			if (this.IndexValue == 0) {
				this.IndexValue = classify.IndexValue;
			}

			if (this.Totals == 0) {
				this.Totals = classify.Totals;
			}
		}
        }
}