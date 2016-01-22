using System;

namespace PowerLanguage.Function {
	/// <summary>
	///   [AvgTrueRange] 移動平均真實區間函式
	/// </summary>
	public static class AvgTrueRange {
		/// <summary>
		///   計算真實區間移動平均的移動平均值
		/// </summary>
		/// <param name="_this">IStudyControl 類別</param>
		/// <param name="length">移動平均長度</param>
		/// <returns>返回值: 真實區間移動平均值</returns>
		public static double AverageTrueRange(IStudyControl _this, int length) {
			return AverageTrueRange(_this, length, 0, 1, 0d);
		}

		/// <summary>
		///   計算真實區間移動平均的移動平均值
		/// </summary>
		/// <param name="_this">IStudyControl 類別</param>
		/// <param name="length">移動平均長度</param>
		/// <param name="barsBack">Bars 索引值(0=目前索引位置)</param>
		/// <returns>返回值: 真實區間移動平均值</returns>
		public static double AverageTrueRange(IStudyControl _this, int length, int barsBack) {
			return AverageTrueRange(_this, length, barsBack, 1, 0d);
		}

		/// <summary>
		///   計算真實區間移動平均的移動平均值
		/// </summary>
		/// <param name="_this">IStudyControl 類別</param>
		/// <param name="length">移動平均長度</param>
		/// <param name="barsBack">Bars 索引值(0=目前索引位置)</param>
		/// <param name="dataStream">資料串流編號</param>
		/// <returns>返回值: 真實區間移動平均值</returns>
		public static double AverageTrueRange(IStudyControl _this, int length, int barsBack, int dataStream) {
			return AverageTrueRange(_this, length, barsBack, dataStream, 0d);
		}

		/// <summary>
		///   計算真實區間移動平均的移動平均值
		/// </summary>
		/// <param name="_this">IStudyControl 類別</param>
		/// <param name="length">移動平均長度</param>
		/// <param name="previous">前一筆真實區間移動平均值</param>
		/// <returns>返回值: 真實區間移動平均值</returns>
		public static double AverageTrueRange(IStudyControl _this, int length, double previous) {
			return AverageTrueRange(_this, length, 0, 1, previous);
		}

		/// <summary>
		///   計算真實區間移動平均的移動平均值
		/// </summary>
		/// <param name="_this">IStudyControl 類別</param>
		/// <param name="length">移動平均長度</param>
		/// <param name="dataStream">資料串流編號</param>
		/// <param name="previous">前一筆真實區間移動平均值</param>
		/// <returns>返回值: 真實區間移動平均值</returns>
		public static double AverageTrueRange(IStudyControl _this, int length, int dataStream, double previous) {
			return AverageTrueRange(_this, length, 0, dataStream, previous);
		}

		/// <summary>
		///   計算真實區間移動平均的移動平均值
		/// </summary>
		/// <param name="_this">IStudyControl 類別</param>
		/// <param name="length">移動平均長度</param>
		/// <param name="barsBack">Bars 索引值(0=目前索引位置)</param>
		/// <param name="dataStream">資料串流編號</param>
		/// <param name="previous">前一筆真實區間移動平均值</param>
		/// <returns>返回值: 真實區間移動平均值</returns>
		public static double AverageTrueRange(IStudyControl _this, int length, int barsBack, int dataStream, double previous) {
			double dValue = 0;
			
			IInstrument cBars = _this.BarsOfData(dataStream);
			if (previous == 0d) {
				double dSum = 0;
				for (int i = length - 1; i >= 0; i--) {
					dSum += GetTrueRange(cBars, barsBack + i);
				}
				dValue = dSum / length;
			} else {
				previous *= length;
				dValue = (previous - GetTrueRange(cBars, barsBack + length) + GetTrueRange(cBars, barsBack)) / ((length == 0) ? 1 : length);
			}
			return dValue;
		}

		private static double GetTrueRange(IInstrument bars, int barsBack) {
			double dPrevClose = bars.Close[barsBack + 1];
			double dHigh = Math.Max(bars.High[barsBack], dPrevClose);
			double dLow = Math.Min(bars.Low[barsBack], dPrevClose);
			return dHigh - dLow;
		}
	}
}