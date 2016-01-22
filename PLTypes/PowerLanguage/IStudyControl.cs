using System;

namespace PowerLanguage {
	/// <summary>
	///   腳本控制介面
	/// </summary>
	public interface IStudyControl {
		/// <summary>
		///   加入變數類別方法
		/// </summary>
		/// <param name="var">變數擴充介面</param>
		void AddVariable(IVariables var);
		
		/// <summary>
		///   取得其他的 Bars 資訊
		/// </summary>
		/// <param name="data_stream">資料串流編號(0 為起始編號)</param>
		/// <returns>返回值: IInstrument 介面</returns>
		IInstrument BarsOfData(int data_stream);
	}
}