using System;
using Zeghs.Utils;

namespace Zeghs.Data {
	public sealed class ForeignInvestmentData {
		public static ForeignInvestmentData Create(ZBuffer buffer) {
			ForeignInvestmentData cData = new ForeignInvestmentData();
			cData.自營商多方交易口數 = buffer.GetInt32();
			cData.自營商空方交易口數 = buffer.GetInt32();
			cData.自營商多方未平倉口數 = buffer.GetInt32();
			cData.自營商空方未平倉口數 = buffer.GetInt32();
			cData.投信多方交易口數 = buffer.GetInt32();
			cData.投信空方交易口數 = buffer.GetInt32();
			cData.投信多方未平倉口數 = buffer.GetInt32();
			cData.投信空方未平倉口數 = buffer.GetInt32();
			cData.外資多方交易口數 = buffer.GetInt32();
			cData.外資空方交易口數 = buffer.GetInt32();
			cData.外資多方未平倉口數 = buffer.GetInt32();
			cData.外資空方未平倉口數 = buffer.GetInt32();
			return cData;
		}

		public int 自營商多方交易口數 = 0;
		public int 自營商空方交易口數 = 0;
		public int 自營商多方未平倉口數 = 0;
		public int 自營商空方未平倉口數 = 0;
		public int 投信多方交易口數 = 0;
		public int 投信空方交易口數 = 0;
		public int 投信多方未平倉口數 = 0;
		public int 投信空方未平倉口數 = 0;
		public int 外資多方交易口數 = 0;
		public int 外資空方交易口數 = 0;
		public int 外資多方未平倉口數 = 0;
		public int 外資空方未平倉口數 = 0;
	}
}