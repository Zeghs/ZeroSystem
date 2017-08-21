namespace PowerLanguage.Function {
	/// <summary>
	///   [KDJ] 隨機指標
	/// </summary>
	public sealed class KD : FunctionObject<double> {
		private VariableSeries<double> __cKDK = null;
		private VariableSeries<double> __cKDD = null;
		private VariableSeries<double> __cKDJ = null;

		/// <summary>
		///   [取得] KDD 值
		/// </summary>
		public ISeries<double> D {
			get {
				return __cKDD;
			}
		}

		/// <summary>
		///   [取得] KDK 值
		/// </summary>
		public ISeries<double> K {
			get {
				return __cKDK;
			}
		}

		/// <summary>
		///   [取得/設定] 週期長度
		/// </summary>
		public int Length {
			get;
			set;
		}

		/// <summary>
		///   [取得] KDJ 值
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值: 從索引值獲得的所需資料</returns>
		public override double this[int barsAgo] {
			get {
				return __cKDJ[barsAgo];
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyAbstract 類別</param>
		/// <param name="dataStream">資料串流編號</param>
		/// <param name="manageFromStudy">由系統自動管理與釋放資源(預設:true)</param>
		public KD(CStudyAbstract master, int dataStream = 1, bool manageFromStudy = true) 
			: base(master, dataStream) {

			this.Length = 9;  //預設周期長度為9
		}

		/// <summary>
		///   建立模型時所呼叫的方法
		/// </summary>
		protected override void Create() {
			__cKDK = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cKDD = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cKDJ = new VariableSeries<double>(this, 0, this.MasterDataStream);
		}

		/// <summary>
		///   計算 Bars 時所呼叫的方法
		/// </summary>
		protected override void CalcBar() {
			if (Bars.CurrentBar < 9) {
				__cKDK.Value = 50;
				__cKDD.Value = 50;
			} else {
				double dHighest = Bars.High.Highest(this.Length);
				double dLowest = Bars.Low.Lowest(this.Length);
				double dHLDiff = dHighest - dLowest;
				double dRSV = (dHLDiff == 0) ? 0 : (Bars.Close.Value - dLowest) / dHLDiff * 100;
				__cKDK.Value = (__cKDK[1] * 2 + dRSV) / 3;
				__cKDD.Value = (__cKDD[1] * 2 + __cKDK.Value) / 3;
				__cKDJ.Value = 3 * __cKDK.Value - 2 * __cKDD.Value;
			}
		}
	}
}