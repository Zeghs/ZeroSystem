namespace PowerLanguage.Function {
	/// <summary>
	///   [MACD] 指數平滑異同平均線
	/// </summary>
	public sealed class MACD : FunctionObject<double> {
		private _Variable<double> __cPrevFEMA;  //儲存前一筆的快速週期移動平均值(可以加快計算速度)
		private _Variable<double> __cPrevSEMA;  //儲存前一筆的慢速週期移動平均值(可以加快計算速度)
		private _Variable<double> __cPrevMACD;  //儲存前一筆的 DIF 移動平均值也可以稱作 MACD(可以加快計算速度)
		private VariableSeries<double> __cDIF = null;
		private VariableSeries<double> __cOSD = null;
		private VariableSeries<double> __cMACD = null;
		private VariableSeries<double> __cWeight = null;

		/// <summary>
		///   [取得] DIF 值
		/// </summary>
		public ISeries<double> DIF {
			get {
				return __cDIF;
			}
		}

		/// <summary>
		///   [取得/設定] 快速平均週期(預設值:12)
		/// </summary>
		public int FastPeriod {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] MACD 平均週期(預設值:9)
		/// </summary>
		public int MACDPeriod {
			get;
			set;
		}

		/// <summary>
		///   [取得] OSD 值
		/// </summary>
		public ISeries<double> OSD {
			get {
				return __cOSD;
			}
		}

		/// <summary>
		///   [取得/設定] 慢速平均週期(預設值:26)
		/// </summary>
		public int SlowPeriod {
			get;
			set;
		}

		/// <summary>
		///   [取得] MACD 值
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值: 從索引值獲得的所需資料</returns>
		public override double this[int barsAgo] {
			get {
				return __cMACD[barsAgo];
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyAbstract 類別</param>
		/// <param name="dataStream">資料串流編號</param>
		public MACD(CStudyAbstract master, int dataStream = 1) 
			: base(master, dataStream) {

			this.MACDPeriod = 9;   //預設 MACD 周期長度為9
			this.FastPeriod = 12;  //預設快速周期長度為12
			this.SlowPeriod = 26;  //預設慢速周期長度為26
		}

		/// <summary>
		///   建立模型時所呼叫的方法
		/// </summary>
		protected override void Create() {
			__cPrevSEMA = new _Variable<double>(this, 0, this.MasterDataStream);
			__cPrevFEMA = new _Variable<double>(this, 0, this.MasterDataStream);
			__cPrevMACD = new _Variable<double>(this, 0, this.MasterDataStream);
			__cDIF = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cOSD = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cMACD = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cWeight = new VariableSeries<double>(this, 0, this.MasterDataStream);
		}

		/// <summary>
		///   計算 Bars 時所呼叫的方法
		/// </summary>
		protected override void CalcBar() {
			int iCurrent = Bars.CurrentBar;
			bool bChange = Bars.Status == EBarState.Close;
			if (bChange) {  //只計算收線狀態
				__cWeight.Value = (Bars.Close.Value * 2 + Bars.High.Value + Bars.Low.Value) / 4;
				
				double dSEMA = __cWeight.Average(this.SlowPeriod, EAverageMode.EMA, __cPrevSEMA.Previous);
				double dFEMA = __cWeight.Average(this.FastPeriod, EAverageMode.EMA, __cPrevFEMA.Previous);
				double dDIF = dFEMA - dSEMA;
				__cDIF.Value = dDIF;

				double dMACD = __cDIF.Average(this.MACDPeriod, EAverageMode.EMA, __cPrevMACD.Previous);
				__cMACD.Value = dMACD;
				__cOSD.Value = dDIF - dMACD;

				__cPrevFEMA.Value = dFEMA;
				__cPrevSEMA.Value = dSEMA;
				__cPrevMACD.Value = dMACD;
			}
		}
	}
}