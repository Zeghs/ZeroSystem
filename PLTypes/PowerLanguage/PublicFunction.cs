using System;
using PowerLanguage.Utils;

namespace PowerLanguage {
	/// <summary>
	///   公用工具方法
	/// </summary>
	public static class PublicFunction {
		/// <summary>
		///   取得移動平均值
		/// </summary>
		/// <param name="series">ISeries 類別</param>
		/// <param name="length">搜尋的個數</param>
		/// <param name="averageMode">移動平均模式</param>
		/// <param name="previous">前一筆平均結果值(如果有前一筆平均結果值則可以加快計算速度)</param>
		/// <returns>返回值: 移動平均結果值</returns>
		public static double Average(this ISeries<double> series, int length, EAverageMode averageMode = EAverageMode.SMA, double previous = 0) {
			return AverageUtil.GetAverage(series, length, averageMode, previous);
		}

		/// <summary>
		///   取得區間內的最大值
		/// </summary>
		/// <param name="series">序列資料</param>
		/// <param name="length">區間個數</param>
		/// <param name="barsBack">Bars 之前的索引(0=預設值)</param>
		/// <returns>返回值: 區間內的最大值</returns>
		/// <returns></returns>
		public static double Highest(this ISeries<double> series, int length, int barsBack = 0) {
			double dHighest = series[barsBack];
			length += barsBack;

			for (int i = length - 1; i > barsBack; i--) {
				if (dHighest < series[i]) {
					dHighest = series[i];
				}
			}
			return dHighest;
		}

		/// <summary>
		///   取得區間內的最小值
		/// </summary>
		/// <param name="series">序列資料</param>
		/// <param name="length">區間個數</param>
		/// <param name="barsBack">Bars 之前的索引(0=預設值)</param>
		/// <returns>返回值: 區間內的最小值</returns>
		public static double Lowest(this ISeries<double> series, int length, int barsBack = 0) {
			double dLowest = series[barsBack];
			length += barsBack;

			for (int i = length - 1; i > 0; i--) {
				if (dLowest > series[i]) {
					dLowest = series[i];
				}
			}
			return dLowest;
		}
		
		/// <summary>
		///   取得區間的加總總合
		/// </summary>
		/// <param name="series">序列資料</param>
		/// <param name="length">區間個數</param>
		/// <param name="barsBack">Bars 之前的索引(0=預設值)</param>
		/// <returns>返回值: 區間內的加總總和</returns>
		public static double Summation(this ISeries<double> series, int length, int barsBack = 0) {
			double dSumm = 0;
			length += barsBack;

			for (int i = length - 1; i >= barsBack; i--) {
				dSumm += series[i];
			}
			return dSumm;
		}
	}
}