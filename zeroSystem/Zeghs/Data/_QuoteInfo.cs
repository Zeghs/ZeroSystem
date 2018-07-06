using System;
using System.Collections.Generic;
using Zeghs.Rules;
using Zeghs.Products;
using Zeghs.Managers;

namespace Zeghs.Data {
	internal sealed class _QuoteInfo {
		private int __iDecimals = 0;
		private double __dPrice = 0;
		private double __dVolume = 0;
		private TimeSpan __cTime = TimeSpan.Zero;

		public double Price {
			get {
				return Math.Round(__dPrice, __iDecimals);
			}
		}

		public string SymbolName {
			get;
			internal set;
		}

		public TimeSpan Time {
			get {
				return __cTime;
			}
		}

		public double Volume {
			get {
				return __dVolume;
			}
		}

		internal _QuoteInfo(string exchangeName, string dataSource, string symbolId, double price) {
			AbstractExchange cExchange = ProductManager.Manager.GetExchange(exchangeName);
			AbstractProductProperty cProperty = cExchange.GetProperty(symbolId, dataSource);
			
			IPriceScale cPriceScale = null;
			if (cProperty != null) {
				cPriceScale = cProperty.PriceScaleRule as IPriceScale;
			}
			
			double[] dScales = new double[] { 1, 1 };
			if(cPriceScale != null) {
				dScales = cPriceScale.GetPriceScale(price);
			}
			
			string sScale = dScales[0].ToString();
			int iIndex = sScale.IndexOf('.');
			if (iIndex > -1) {
				__iDecimals = sScale.Length - iIndex - 1;
			}
		}

		internal void SetPrice(double value) {
			__dPrice = value;
		}

		internal void SetTime(TimeSpan value) {
			__cTime = value;
		}

		internal void SetVolume(double value) {
			__dVolume = value;
		}
	}
}