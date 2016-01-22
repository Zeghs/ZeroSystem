using System;
using PowerLanguage;
using Zeghs.Events;

namespace Zeghs.Data {
	internal sealed class Series<T> : SeriesAbstract<T> {
		/// <summary>
		///   當需要請求資訊時所觸發的事件
		/// </summary>
		internal event EventHandler<SeriesRequestEvent> onRequest = null;

		private bool __bClone = false;

		/// <summary>
		///   [取得/設定] 是否為絕對索引值模式(預設為相對索引值模式, 需要以 Current 屬性來做偏移)
		/// </summary>
		internal bool IsAbsolute {
			get;
			set;
		}

		/// <summary>
		///   [取得] 目前或是之前的資料
		/// </summary>
		/// <param name="barsAgo">目前或是之前的索引(0=目前的 Bar)</param>
		/// <returns>返回值:從索引值獲得的所需資料</returns>
		public override T this[int barsAgo] {
			get {
				int iIndex = (this.IsAbsolute) ? barsAgo : this.Current - barsAgo - 1;
				if (iIndex < 0) {
					if (onRequest != null) {
						onRequest(this, new SeriesRequestEvent(iIndex));
					}
				}
				return this.GetValue(iIndex);
			}
		}

		internal Series(int size) {
			this.IsAbsolute = true;
			this.Initialize(size);
		}

		internal Series<T> Clone() {
			Series<T> cClone = this.MemberwiseClone() as Series<T>;
			cClone.__bClone = true;
			cClone.IsAbsolute = false;
			
			return cClone;
		}

		internal void SetData(int index, T value) {
			this.SetValue(index, value);
		}

		protected override void Dispose(bool disposing) {
			this.RemoveRequest();

			if (!__bClone) {
				base.Dispose(disposing);
			}
		}

		internal void RemoveRequest() {
			if (onRequest != null) {
				onRequest = null;
			}
		}
	}
}