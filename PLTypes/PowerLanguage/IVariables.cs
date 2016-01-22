using System;

namespace PowerLanguage {
	/// <summary>
	///   變數擴充類別
	/// </summary>
	public interface IVariables {
		/// <summary>
		///   [取得] 資料串流編號
		/// </summary>
		int DataStream {
			get;
		}

		/// <summary>
		///  移動 Current 索引
		/// </summary>
		/// <param name="index">索引值</param>
		void Move(int index);
	}
}