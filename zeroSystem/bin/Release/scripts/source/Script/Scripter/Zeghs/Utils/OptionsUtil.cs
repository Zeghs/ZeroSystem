using System;

namespace Zeghs.Utils {
	public sealed class OptionsUtil {
		/// <summary>
		///   CALL權-風險係數  
		/// </summary>
		/// <param name="S">現貨價格(指數選擇權以期貨價格為現貨價)</param>
		/// <param name="K">履約價格</param>
		/// <param name="R">年利率</param>
		/// <param name="T">年化到期日</param>
		/// <param name="V">年化波動率(歷史波動率)</param>
		/// <returns>返回值: double[](0=Delta, 1=Gamma, 2=Theta, 3=Vega, 4=Rho)</returns>
		public static double[] CALLGreeks(double S, double K, double R, double T, double V) {
			double dD1 = d1(S, K, R, T, V);
			double dND1 = Math.Exp(-0.5 * dD1 * dD1) / Math.Sqrt(2 * Math.PI);
			double dND2 = NormsDist(dD1 - V * Math.Sqrt(T));

			double[] dValues = new double[5];
			dValues[0] = NormsDist(dD1);
			dValues[1] = dND1 / (S * V * Math.Sqrt(T));
			dValues[2] = (-(S * dND1 * V / (2 * Math.Sqrt(T))) - R * K * Math.Exp(-R * T) * dND2) / 365;
			dValues[3] = (S * Math.Sqrt(T) * dND1) / 100;
			dValues[4] = (K * T * Math.Exp(-R * T) * dND2) / 100;
			return dValues;
		}

		/// <summary>
		///   CALL權-隱含波動率
		/// </summary>
		/// <param name="S">現貨價格(指數選擇權以期貨價格為現貨價)</param>
		/// <param name="K">履約價格</param>
		/// <param name="R">年利率</param>
		/// <param name="T">年化到期日</param>
		/// <param name="Target">標的選擇權市價價格</param>
		/// <returns>返回值: 隱含波動率</returns>
		public static double CALL_IV(double S, double K, double R, double T, double Target) {
			double high = 1;
			double low = 0;

			while ((high - low) > 0.00001) {
				if (CALLPrice(S, K, R, T, (high + low) / 2) > Target) {
					high = (high + low) / 2;
				} else {
					low = (high + low) / 2;
				}
			}
			return (high + low) / 2;
		}

		/// <summary>
		///   CALL權-理論價格
		/// </summary>
		/// <param name="S">現貨價格(指數選擇權以期貨價格為現貨價)</param>
		/// <param name="K">履約價格</param>
		/// <param name="R">年利率</param>
		/// <param name="T">年化到期日</param>
		/// <param name="V">年化波動率</param>
		/// <returns>返回值: CALL權-理論價格</returns>
		public static double CALLPrice(double S, double K, double R, double T, double V) {
			double a = Math.Log(S / K);
			double b_call = (R + 0.5 * Math.Pow(V, 2)) * T;
			double b_put = (R - 0.5 * Math.Pow(V, 2)) * T;
			double c = V * Math.Sqrt(T);
			double d1 = (a + b_call) / c;
			double d2 = (a + b_put) / c;
			return S * NormsDist(d1) - K * Math.Exp(-R * T) * NormsDist(d2);
		}

		/// <summary>
		///   PUT權-風險係數  
		/// </summary>
		/// <param name="S">現貨價格(指數選擇權以期貨價格為現貨價)</param>
		/// <param name="K">履約價格</param>
		/// <param name="R">年利率</param>
		/// <param name="T">年化到期日</param>
		/// <param name="V">年化波動率(歷史波動率)</param>
		/// <returns>返回值: double[](0=Delta, 1=Gamma, 2=Theta, 3=Vega, 4=Rho)</returns>
		public static double[] PUTGreeks(double S, double K, double R, double T, double V) {
			double dD1 = d1(S, K, R, T, V);
			double dND1 = Math.Exp(-0.5 * dD1 * dD1) / Math.Sqrt(2 * Math.PI);
			double dND2 = NormsDist(dD1 - V * Math.Sqrt(T));

			double[] dValues = new double[5];
			dValues[0] = NormsDist(dD1) - 1;
			dValues[1] = dND1 / (S * V * Math.Sqrt(T));
			dValues[2] = (-(S * dND1 * V / (2 * Math.Sqrt(T))) + R * K * Math.Exp(-R * T) * (1 - dND2)) / 365;
			dValues[3] = (S * Math.Sqrt(T) * dND1) / 100;
			dValues[4] = (K * T * Math.Exp(-R * T) * (1 - dND2)) / -100;
			return dValues;
		}

		/// <summary>
		///   PUT權-隱含波動率
		/// </summary>
		/// <param name="S">現貨價格(指數選擇權以期貨價格為現貨價)</param>
		/// <param name="K">履約價格</param>
		/// <param name="R">年利率</param>
		/// <param name="T">年化到期日</param>
		/// <param name="Target">標的選擇權市價價格</param>
		/// <returns>返回值: 隱含波動率</returns>
		public static double PUT_IV(double S, double K, double R, double T, double Target) {
			double high = 1;
			double low = 0;

			while ((high - low) > 0.00001) {
				if (PUTPrice(S, K, R, T, (high + low) / 2) > Target) {
					high = (high + low) / 2;
				} else {
					low = (high + low) / 2;
				}
			}
			return (high + low) / 2;
		}

		/// <summary>
		///   PUT權-理論價格
		/// </summary>
		/// <param name="S">現貨價格(指數選擇權以期貨價格為現貨價)</param>
		/// <param name="K">履約價格</param>
		/// <param name="R">年利率</param>
		/// <param name="T">年化到期日</param>
		/// <param name="V">年化波動率</param>
		/// <returns>返回值: PUT權-理論價格</returns>
		public static double PUTPrice(double S, double K, double R, double T, double V) {
			double a = Math.Log(S / K);
			double b_call = (R + 0.5 * Math.Pow(V, 2)) * T;
			double b_put = (R - 0.5 * Math.Pow(V, 2)) * T;
			double c = V * Math.Sqrt(T);
			double d1 = (a + b_call) / c;
			double d2 = (a + b_put) / c;
			return K * Math.Exp(-R * T) * NormsDist(-d2) - S * NormsDist(-d1);
		}

		/// <summary>
		///   d1數值(不清楚是甚麼數值)
		/// </summary>
		/// <param name="S">現貨價格(指數選擇權以期貨價格為現貨價)</param>
		/// <param name="K">履約價格</param>
		/// <param name="R">年利率</param>
		/// <param name="T">年化到期日</param>
		/// <param name="V">年化波動率(歷史波動率)</param>
		/// <returns>返回值: d1數值</returns>
		private static double d1(double S, double K, double R, double T, double V) {
			double a = Math.Log(S / K);
			double b_call = (R + 0.5 * Math.Pow(V, 2)) * T;
			double c = V * Math.Sqrt(T);
			return (a + b_call) / c;
		}

		private static double NormsDist(double d) {
			const double a1 = 0.31938153;
			const double a2 = -0.356563782;
			const double a3 = 1.781477937;
			const double a4 = -1.821255978;
			const double a5 = 1.330274429;
			double L = Math.Abs(d);
			double K = 1.0 / (1.0 + 0.2316419 * L);
			double dD = 1.0 - 1.0 / Math.Sqrt(2 * Convert.ToDouble(Math.PI)) * Math.Exp(-L * L / 2.0) * (a1 * K + a2 * K * K + a3 * Math.Pow(K, 3.0) + a4 * Math.Pow(K, 4.0) + a5 * Math.Pow(K, 5.0));

			return (d < 0) ? 1.0d - dD : dD;
		}
	}
}