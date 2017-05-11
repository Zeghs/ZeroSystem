using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Zeghs.Chart;

namespace Zeghs.Actions {
	/// <summary>
	///   使用者操作/動作行為類別
	/// </summary>
	public sealed class Behavior {
		private IAction __cCurrent = null;
		private List<IAction> __cActions = null;
		private ChartParameter __cParameter = null;
		private DrawContainer __cDrawContainer = null;
		private Dictionary<string, IAction> __cCustoms = null;

		/// <summary>
		///   [取得] 使用者繪製物件保存容器
		/// </summary>
		public DrawContainer DrawContainer {
			get {
				return __cDrawContainer;
			}
		}

		internal Dictionary<string, IAction> CustomActions {
			get {
				return __cCustoms;
			}
		}

		internal bool Enabled {
			get;
			set;
		}

		internal Behavior(ZChart chart, Control context, InputDeviceStatus status) {
			__cDrawContainer = new DrawContainer();
			__cParameter = new ChartParameter(chart, context, this, status);

			__cActions = new List<IAction>(8);
			__cActions.Add(new Resize());
			__cActions.Add(new Move());
			__cActions.Add(new Zoom());
		}

		/// <summary>
		///   取得使用者自訂動作介面
		/// </summary>
		/// <param name="name">動作介面名稱</param>
		/// <returns>返回值: IAction 介面</returns>
		public IAction GetCustomAction(string name) {
			IAction cAction = null;
			
			if (__cCustoms != null) {
				__cCustoms.TryGetValue(name, out cAction);
			}
			return cAction;
		}

		/// <summary>
		///   執行使用者動作介面
		/// </summary>
		/// <param name="updated">是否 chart 之前曾刷新</param>
		internal void Action(bool updated) {
			if (this.Enabled) {
				__cParameter.Updated = updated;

				if (__cCurrent == null) {
					int iCount = __cActions.Count;
					for (int i = 0; i < iCount; i++) {
						IAction cAction = __cActions[i];
						cAction.Action(__cParameter);

						if (__cParameter.IsAction) {
							__cCurrent = cAction;
							break;
						}
					}
				} else {
					__cCurrent.Action(__cParameter);
					if (!__cParameter.IsAction) {
						__cCurrent = null;
					}
				}
			}
		}

		internal void DrawObjects() {
			ZChart cChart = __cParameter.Chart;
			AxisX cAxisX = cChart.AxisX;
			int iEndNumber = cAxisX.BarNumber + cAxisX.BarCount - 1;
			
			HashSet<DrawObject> cObjects = __cDrawContainer.GetDrawObjects(cAxisX.BarNumber, iEndNumber);
			foreach (DrawObject cObject in cObjects) {
				if (cObject.Exist) {
					IDrawable cDrawable = GetCustomAction(cObject.Name) as IDrawable;
					if (cDrawable != null) {
						if (cObject.LayerIndex < cChart.Layers.Count) {
							cDrawable.DrawObject(cChart.Layers[cObject.LayerIndex], cObject);
						}
					}
				}
			}
		}

		internal bool SetCustomAction(string name, PowerLanguage.PenStyle pen) {
			bool bRet = true;
			if (__cCurrent == null) {
				IAction cAction = null;
				if (__cCustoms != null && __cCustoms.TryGetValue(name, out cAction)) {
					if (cAction is IDrawable) {
						bRet = false;
					} else {
						__cCurrent = cAction;
						__cParameter.IsAction = true;
					}
				} else {
					bRet = false;
				}
			} else {
				if (__cCustoms != null && name != null && __cCustoms.ContainsKey(name)) {
					if (!name.Equals("Cross")) {
						__cParameter.CustomPainter = name;
						__cParameter.CustomPen = pen;
					}
				} else {
					__cCurrent = null;
					__cParameter.IsAction = false;
					__cParameter.CustomPainter = null;
					
					bRet = false;
				}
			}
			return bRet;
		}

		internal void SetCustomActions(List<IAction> actions) {
			if (__cCustoms == null) {
				__cCustoms = new Dictionary<string, IAction>(16);

				foreach (IAction cAction in actions) {
					__cCustoms.Add(cAction.Name, cAction);
				}
			}
		}

		internal void SetCustomPenStyle(PowerLanguage.PenStyle pen) {
			__cParameter.CustomPen = pen;
		}
	}
}