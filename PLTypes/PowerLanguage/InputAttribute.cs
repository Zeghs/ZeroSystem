using System;
using System.Reflection;
using System.Collections.Generic;

namespace PowerLanguage {
	/// <summary>
	///   腳本參數類別
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public sealed class InputAttribute : Attribute {
		/// <summary>
		///   取得腳本物件內的所有使用者自訂的必要參數
		/// </summary>
		/// <param name="target">腳本物件類別</param>
		/// <returns>返回值: 必要參數列表</returns>
		public static List<InputAttribute> GetParameters(object target) {
			List<InputAttribute> cInputs = new List<InputAttribute>();

			PropertyInfo[] cPropertys = target.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo cProperty in cPropertys) {
				bool bDefined = Attribute.IsDefined(cProperty, typeof(InputAttribute));
				if (bDefined) {
					InputAttribute cInput = Attribute.GetCustomAttribute(cProperty, typeof(InputAttribute)) as InputAttribute;

					cInput.SetData(target, cProperty);
					cInputs.Add(cInput);
				}
			}
			return cInputs;
		}

		private object __cInstance = null;
		private PropertyInfo __cPropertyInfo = null;

		/// <summary>
		///   [取得/設定] 參數說明
		/// </summary>
		public string Comment {
			get;
			set;
		}

		/// <summary>
		///   [取得] 是否為列舉型別
		/// </summary>
		public bool IsEnum {
			get {
				return __cPropertyInfo.PropertyType.IsEnum;
			}
		}

		/// <summary>
		///   [取得] 參數名稱
		/// </summary>
		public string Name {
			get {
				return __cPropertyInfo.Name;
			}
		}

		/// <summary>
		///   [取得] 參數值
		/// </summary>
		public object Value {
			get {
				return __cPropertyInfo.GetValue(__cInstance, null);
			}
		}

		/// <summary>
		///   建構子
		/// </summary>
		public InputAttribute() {
		}

		/// <summary>
		///   建構子
		/// </summary>
		/// <param name="comment">參數說明</param>
		public InputAttribute(string comment) {
			this.Comment = comment;
		}

		/// <summary>
		///   設定資料
		/// </summary>
		/// <param name="value">字串值</param>
		public void SetValue(string value) {
			object oValue = null;

			Type cType = __cPropertyInfo.PropertyType;
			if (cType == typeof(int)) {
				int iValue = 0;
				int.TryParse(value, out iValue);
				oValue = iValue;
			} else if (cType == typeof(double)) {
				double dValue = 0;
				double.TryParse(value, out dValue);
				oValue = dValue;
			} else if (cType == typeof(string)) {
				oValue = value;
			} else if (cType == typeof(float)) {
				float fValue = 0;
				float.TryParse(value, out fValue);
				oValue = fValue;
			} else if (cType == typeof(long)) {
				long lValue = 0;
				long.TryParse(value, out lValue);
				oValue = lValue;
			} else if (cType == typeof(uint)) {
				uint uValue = 0;
				uint.TryParse(value, out uValue);
				oValue = uValue;
			} else if (cType == typeof(ulong)) {
				ulong ulValue = 0;
				ulong.TryParse(value, out ulValue);
				oValue = ulValue;
			} else if (cType == typeof(byte)) {
				byte bValue = 0;
				byte.TryParse(value, out bValue);
				oValue = bValue;
			} else if (cType == typeof(char)) {
				char chValue = (char) 0;
				char.TryParse(value, out chValue);
				oValue = chValue;
			} else if (cType == typeof(bool)) {
				oValue = bool.Parse(value);
			} else if (cType == typeof(short)) {
				short shValue = 0;
				short.TryParse(value, out shValue);
				oValue = shValue;
			} else if (cType == typeof(ushort)) {
				ushort usValue = 0;
				ushort.TryParse(value, out usValue);
				oValue = usValue;
			} else if (cType.IsEnum) {
				oValue = Enum.Parse(cType, value);
			}
			this.SetValue(oValue);
		}

		/// <summary>
		///   設定資料
		/// </summary>
		/// <param name="value">字串值</param>
		public void SetValue(object value) {
			__cPropertyInfo.SetValue(__cInstance, value, null);
		}

		/// <summary>
		///   設定必要資料
		/// </summary>
		/// <param name="instance">object類別</param>
		/// <param name="property">PropertyInfo類別</param>
		internal void SetData(object instance, PropertyInfo property) {
			__cInstance = instance;
			__cPropertyInfo = property;
		}
	}
}