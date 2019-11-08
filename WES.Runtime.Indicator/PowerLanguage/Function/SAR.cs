namespace PowerLanguage.Function {
	/// <summary>
	///   SAR指標(Stop And Reverse)
	/// </summary>
	public sealed class SAR : FunctionObject<double> {
		private double __dAF = 0d;
		private int __iPrevious = 0, __iBuyOrSell = 0, __iUP = 0, __iDOWN = 0;
		private VariableSeries<double> __cSAR = null;
		private VariableSeries<double> __cLimit = null;

		/// <summary>
		///   [取得/設定] 週期長度
		/// </summary>
		public int Length {
			get;
			set;
		}

		/// <summary>
		///   [取得] 極值
		/// </summary>
		public VariableSeries<double> Limit {
			get {
				return __cLimit;
			}
		}

		/// <summary>
		///   [取得] SAR 值
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值: 從索引值獲得的所需資料</returns>
		public override double this[int barsAgo] {
			get {
				return __cSAR[barsAgo];
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="master">CStudyAbstract 類別</param>
		/// <param name="dataStream">資料串流編號</param>
		/// <param name="manageFromStudy">由系統自動管理與釋放資源(預設:true)</param>
		public SAR(CStudyAbstract master, int dataStream = 1, bool manageFromStudy = true)
			: base(master, dataStream, manageFromStudy) {

			this.Length = 4;
		}

		/// <summary>
		///   建立模型時所呼叫的方法
		/// </summary>
		protected override void Create() {
			__cSAR = new VariableSeries<double>(this, 0, this.MasterDataStream);
			__cLimit = new VariableSeries<double>(this, 0, this.MasterDataStream);
		}

		/// <summary>
		///   計算 Bars 時所呼叫的方法
		/// </summary>
		protected override void CalcBar() {
			int iCurrent = Bars.CurrentBar;
			bool bChange = Bars.Status == EBarState.Close;
			if (bChange && iCurrent > __iPrevious) {  //只計算收線狀態
				__iPrevious = iCurrent;

				double dLimit1 = __cLimit[1];
				if (dLimit1 == 0) {
					CheckTrend();  //檢查區間波段(判斷是上升波段或是下降波段, 初始需要檢查一個完整波段)
				} else {
					__cLimit.Value = dLimit1;

					bool bHighest = __iBuyOrSell == 1 && Bars.High.Value > dLimit1, bLowest = __iBuyOrSell == -1 && dLimit1 > Bars.Low.Value;
					if (__dAF == 0d || bHighest || bLowest) {
						__dAF += 0.02d;
						__dAF = (__dAF > 0.2d) ? 0.2d : __dAF;

						if (bHighest) {
							__cLimit.Value = Bars.High.Value;
						} else if (bLowest) {
							__cLimit.Value = Bars.Low.Value;
						}
					}

					//SAR(今) = SAR(昨) + AF* [ 區間極值(昨) – SAR(昨) ]
					double dSAR1 = __cSAR[1];
					__cSAR.Value = dSAR1 + __dAF * (dLimit1 - dSAR1);

					double dClose0 = Bars.Close[0];
					if (__iBuyOrSell == 1 && __cSAR.Value > dClose0) {
						__dAF = 0d;
						__iBuyOrSell = -1;
						__cSAR.Value = dLimit1;
						__cLimit.Value = Bars.Low[1];
					} else if (__iBuyOrSell == -1 && dClose0 > __cSAR.Value) {
						__dAF = 0d;
						__iBuyOrSell = 1;
						__cSAR.Value = dLimit1;
						__cLimit.Value = Bars.High[1];
					}
				}
			}
		}

		private void CheckTrend() {
			double dClose0 = Bars.Close[0];
			double dClose1 = Bars.Close[1];
			if (dClose0 > dClose1) {
				++__iUP;
				__iDOWN = 0;

				if (__iUP == this.Length) {
					__iBuyOrSell = 1;
				}
			} else if (dClose1 > dClose0) {
				__iUP = 0;
				++__iDOWN;

				if (__iDOWN == this.Length) {
					__iBuyOrSell = -1;
				}
			}

			if (__iBuyOrSell != 0) {
				if (__iBuyOrSell == 1) {
					__cSAR.Value = Bars.Low.Lowest(this.Length);
					__cLimit.Value = Bars.High.Highest(this.Length);
				} else {
					__cSAR.Value = Bars.High.Highest(this.Length);
					__cLimit.Value = Bars.Low.Lowest(this.Length);
				}
			}
		}
	}
}