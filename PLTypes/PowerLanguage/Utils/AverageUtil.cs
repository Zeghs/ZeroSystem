using System;

namespace PowerLanguage.Utils {
	internal sealed class AverageUtil {
		/// <summary>
		///   取得移動平均值
		/// </summary>
		/// <param name="series">ISeries 類別</param>
		/// <param name="length">欲計算平均值的個數</param>
		/// <param name="averageMode">移動平均模式</param>
		/// <param name="previous">前一筆平均結果值(如果有前一筆平均結果值則可以加快計算速度)</param>
		/// <returns>返回值: 移動平均結果值</returns>
		internal static double GetAverage(ISeries<double> series, int length, EAverageMode averageMode = EAverageMode.SMA, double previous = 0) {
			switch (averageMode) {
				case EAverageMode.EMA:
					return GetEMA(series, length, previous);
				case EAverageMode.SMA:
					return GetSMA(series, length, previous);
				case EAverageMode.WMA:
					return GetWMA(series, length, previous);
			}
			return 0;
		}

		/// <summary>
		///   指數移動平均值
		/// </summary>
		/// <param name="series">ISeries 類別</param>
		/// <param name="length">欲計算平均值的個數</param>
		/// <param name="previous">前一筆平均結果值(如果有前一筆平均結果值則可以加快計算速度)</param>
		/// <returns>返回值:平均值</returns>
		private static double GetEMA(ISeries<double> series, int length, double previous = 0) {
			double dValue = 0, a = 2d / (length + 1);
			if (previous == 0) {
				double dAvg = 1;
				dValue += series[0];
				for (int i = length; i >= 1; i--) {
					double dExp = Math.Pow(1 - a, i);
					dAvg += dExp;
					dValue += series[i] * dExp;
				}
				dValue /= dAvg;
			} else {
				dValue = previous + a * (series.Value - previous);
			}
			return dValue;
		}

		/// <summary>
		///   簡易移動平均值
		/// </summary>
		/// <param name="series">ISeries 類別</param>
		/// <param name="length">欲計算平均值的個數</param>
		/// <param name="previous">前一筆平均結果值(如果有前一筆平均結果值則可以加快計算速度)</param>
		/// <returns>返回值:平均值</returns>
		private static double GetSMA(ISeries<double> series, int length, double previous = 0) {
			double dValue = 0;
			if (previous == 0) {
				for (int i = length - 1; i >= 0; i--) {
					dValue += series[i];
				}
				dValue /= ((length == 0) ? 1 : length);
			} else {
				previous *= length;
				dValue = (previous - series[length] + series.Value) / ((length == 0) ? 1 : length);
			}
			return dValue;
		}

		/// <summary>
		///   加權移動平均值
		/// </summary>
		/// <param name="series">ISeries 類別</param>
		/// <param name="length">欲計算平均值的個數</param>
		/// <param name="previous">前一筆平均結果值(如果有前一筆平均結果值則可以加快計算速度)</param>
		/// <returns>返回值:平均值</returns>
		private static double GetWMA(ISeries<double> series, int length, double previous = 0) {
			double dValue = 0, dSum = 0, dWeight = 1;
			if (previous == 0) {
				for (int i = length - 1; i >= 0; i--) {
					dSum += dWeight;
					dValue += series[i] * dWeight++;
				}
				dValue /= ((dSum == 0) ? 1 : dSum);
			} else {
				dSum = series.Summation(length);

				//計算出加權移動平均
				dWeight = (length * (length + 1)) / 2;
				previous *= dWeight;
				dValue = (previous + series.Value * length) - dSum;
				dValue /= ((dWeight == 0) ? 1 : dWeight);
			}
			return dValue;
		}
	}
}