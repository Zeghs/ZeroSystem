namespace PowerLanguage.Function {
	/// <summary>
	///   [BIAS] 乖離率指標
	/// </summary>
	public sealed class BIAS : FunctionObject<double> {
		private _Variable<double> __cPrevMA;            //保存前一筆的移動平均值(可以增快計算速度)
		private VariableSeries<double> __cBIAS = null;

		/// <summary>
		///   [取得/設定] 移動平均線模式(預設:SMA)
		/// </summary>
		public EAverageMode AverageMode {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 週期長度
		/// </summary>
		public int Length {
			get;
			set;
		}

		/// <summary>
		///   [取得] 乖離率
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值: 從索引值獲得的所需資料</returns>
		public override double this[int barsAgo] {
			get {
				return __cBIAS[barsAgo];
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyAbstract 類別</param>
		/// <param name="dataStream">資料串流編號</param>
		public BIAS(CStudyAbstract master, int dataStream = 1) 
			: base(master, dataStream) {

			this.Length = 6;  //預設周期長度為6
			this.AverageMode = EAverageMode.SMA;  //預設移動平均模式為簡單移動平均
		}

		/// <summary>
		///   建立模型時所呼叫的方法
		/// </summary>
		protected override void Create() {
			__cPrevMA = new _Variable<double>(this, 0, this.MasterDataStream);
			__cBIAS = new VariableSeries<double>(this, 0, this.MasterDataStream);
		}

		/// <summary>
		///   計算 Bars 時所呼叫的方法
		/// </summary>
		protected override void CalcBar() {
			bool bChange = Bars.Status == EBarState.Close;
			if (bChange) {  //只計算收線狀態
				double dMA = Bars.Close.Average(this.Length, this.AverageMode, __cPrevMA.Previous);
				__cBIAS.Value = (Bars.Close[0] - dMA) / dMA * 100;
				__cPrevMA.Value = dMA;
			}
		}
	}
}