using System.Drawing;
using System.Windows.Forms;

namespace Zeghs.Chart {
	/// <summary>
	///   輸入設備狀態類別(輸入設備包含滑鼠, 鍵盤...)
	/// </summary>
	public sealed class InputDeviceStatus {
		private struct _InputStatus {
			public EInputDeviceEvent _event;
			public KeyEventArgs keyboard;
			public MouseEventArgs mouse;
		}

		private bool __bDrag = false;
		private bool __bClick = false;
		private _InputStatus __cCurrent;
		private _InputStatus __cPrevious;

		/// <summary>
		///   [取得] 是否點擊滑鼠
		/// </summary>
		public bool Click {
			get {
				return __bClick;
			}
		}

		/// <summary>
		///   [取得] 目前觸發的事件列舉
		/// </summary>
		public EInputDeviceEvent Event {
			get {
				return __cCurrent._event;
			}
		}

		/// <summary>
		///   [取得] 是否滑鼠再拖曳狀態
		/// </summary>
		public bool IsDrag {
			get {
				return __bDrag;
			}
		}

		internal InputDeviceStatus() {
			__cCurrent._event = EInputDeviceEvent.None;
		}

		public KeyEventArgs GetCurrentKeyboardArgs() {
			return __cCurrent.keyboard;
		}

		public KeyEventArgs GetPreviousKeyboardArgs() {
			return __cPrevious.keyboard;
		}

		public MouseEventArgs GetCurrentMouseArgs() {
			return __cCurrent.mouse;
		}

		public MouseEventArgs GetPreviousMouseArgs() {
			return __cPrevious.mouse;
		}

		internal void SetKeyEventArgs(EInputDeviceEvent evt, KeyEventArgs args) {
			__cPrevious._event = __cCurrent._event;
			__cPrevious.keyboard = __cCurrent.keyboard;
			__cCurrent._event = evt;
			__cCurrent.keyboard = args;
		}

		internal void SetMouseEventArgs(EInputDeviceEvent evt, MouseEventArgs args) {
			__cPrevious._event = __cCurrent._event;
			__cPrevious.mouse = __cCurrent.mouse;
			__cCurrent._event = evt;
			__cCurrent.mouse = args;

			if (__bDrag) {
				if (__cCurrent._event == EInputDeviceEvent.MouseUp) {
					__bDrag = false;
					__bClick = true;
				}
			} else {
				if (__cCurrent._event == EInputDeviceEvent.MouseMove && __cPrevious._event == EInputDeviceEvent.MouseDown) {
					__bDrag = true;
					__bClick = false;
				}
			}
		}
	}
}