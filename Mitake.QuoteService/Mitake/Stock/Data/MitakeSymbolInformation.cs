namespace Mitake.Stock.Data {
	/// <summary>
	///   報價資訊的基本資料類別(商品代號與商品名稱之類的資訊) @三竹報價需要有股票對照表才能找到股票流水號，如果全部都使用MitakeQuote實體化則記憶體會非常龐大，因此改變原來的寫法
	/// </summary>
	internal sealed class MitakeSymbolInformation {
		/// <summary>
		///   [取得] 期貨擴充:1=一般, 2=現月, 3=次月 (股票無使用此變數)
		/// </summary>
		public byte FutureMark {
			get;
			set;
		}

		/// <summary>
		///   [取得] 商品代號
		/// </summary>
		public string SymbolId {
			get;
			set;
		}

		/// <summary>
		///   [取得] 商品名稱
		/// </summary>
		public string SymbolName {
			get;
			set;
		}

		/// <summary>
		///   [取得] 市場別:0=集中市場, 1=上櫃, 2=期貨, 3=興櫃
		/// </summary>
		public byte 市場別 {
			get;
			set;
		}

		/// <summary>
		///   [取得] 個股是否為警示股
		/// </summary>
		public bool 警示 {
			get;
			set;
		}

		/// <summary>
		///   [取得] 下市(五個交易日後清除，會再分配給新上市使用)
		/// </summary>
		public bool 下市 {
			get;
			set;
		}

		public byte 市場分類 {
			get;
			set;
		}

		public string 產業別 {
			get;
			set;
		}

		public string 證券別 {
			get;
			set;
		}
	}
}