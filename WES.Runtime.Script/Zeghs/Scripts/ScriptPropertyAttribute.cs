using System;
using Zeghs.Scripts;

namespace Zeghs.Scripts {
	/// <summary>
	///   腳本屬性類別
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class ScriptPropertyAttribute : Attribute {
		/// <summary>
		///   [取得/設定] 公司名稱
		/// </summary>
		public string Company {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 腳本說明
		/// </summary>
		public string Comment {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 商標名稱
		/// </summary>
		public string Copyright {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 腳本類型
		/// </summary>
		public ScriptType ScriptType {
			get;
			set;
		}

		/// <summary>
		///   [取得/設定] 版本編號
		/// </summary>
		public string Version {
			get;
			set;
		}
	}
}