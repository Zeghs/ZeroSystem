using System.Collections.Generic;
using PowerLanguage;

namespace Zeghs.Products {
	internal sealed class ProductClassify {
		private HashSet<string> __cKeys = null;
		private Dictionary<ESymbolCategory, List<string>> __cClassify = null;

		internal int ClassifyCount {
			get {
				return __cClassify.Count;
			}
		}

		internal ProductClassify() {
			__cKeys = new HashSet<string>();
			__cClassify = new Dictionary<ESymbolCategory, List<string>>(16);
		}

		internal void Add(Product product) {
			string sSymbolId = product.SymbolId;
			lock (__cKeys) {
				if (!__cKeys.Contains(sSymbolId)) {
					ESymbolCategory cType = product.Category;

					List<string> cList = null;
					if (!__cClassify.TryGetValue(cType, out cList)) {
						cList = new List<string>();
						__cClassify.Add(cType, cList);
					}

					cList.Add(sSymbolId);
					__cKeys.Add(sSymbolId);  //將分類過的股票加入至 HashSet 避免加入到重複的股票
				}
			}
		}

		internal void Clear() {
			lock (__cKeys) {
				foreach (List<string> cList in __cClassify.Values) {
					cList.Clear();
				}
				__cClassify.Clear();
				__cKeys.Clear();
			}
		}

		internal List<string> GetClassify(ESymbolCategory productType) {
			List<string> cSymbolIds = null;
			
			lock (__cKeys) {
				__cClassify.TryGetValue(productType, out cSymbolIds);
			}
			return cSymbolIds;
		}

		internal void Remove(Product product) {
			string sSymbolId = product.SymbolId;
			lock (__cKeys) {
				if (__cKeys.Contains(sSymbolId)) {
					ESymbolCategory cType = product.Category;

					List<string> cList = null;
					if (__cClassify.TryGetValue(cType, out cList)) {
						cList.Remove(sSymbolId);  //效率可能會比較慢(考量到股票支數可能不多, 所以還可以接受這樣的效率)
					}
					__cKeys.Remove(sSymbolId);
				}
			}
		}
	}
}