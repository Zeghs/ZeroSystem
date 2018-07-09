using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using SourceGrid;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Products;
using Zeghs.Services;
using Zeghs.Managers;

namespace Zeghs.Forms {
	internal partial class frmProductManager : Form {
		private static _ProductInfo CreateProductInfo(string exchangeName, string dataSource, Product product, AbstractProductProperty property) {
			_ProductInfo cProductInfo = new _ProductInfo() {
				CommodityId = (property == null) ? string.Empty : property.CommodityId,
				Description = (property == null) ? string.Empty : property.Description,
				DataSource = (dataSource == null) ? string.Empty : dataSource,
				ExchangeName = exchangeName,
				ProductId = product.SymbolId,
				ProductName = product.SymbolName
			};
			return cProductInfo;
		}

		private TreeNode __cSelectedNode = null;
		private List<string> __cDataSources = null;
		private List<_ProductInfo> __cAllData = null;
		private List<_ProductInfo> __cBasicData = null;
		private List<_ProductInfo> __cCustomData = null;
		private ToolStripButton __cPrevFilterButton = null;
		private SortRangeRowsEventArgs __cSortRangeRowEvent = null;

		internal frmProductManager() {
			__cDataSources = new List<string>(64);
			__cAllData = new List<_ProductInfo>(1024);
			__cBasicData = new List<_ProductInfo>(1024);
			__cCustomData = new List<_ProductInfo>(128);

			//讀取所有可用的資料來源
			List<AbstractQuoteService> cServices = QuoteManager.Manager.QuoteServices;
			int iCount = cServices.Count;
			for (int i = 0; i < iCount; i++) {
				AbstractQuoteService cService = cServices[i];
				__cDataSources.Add(cService.DataSource);
			}

			InitializeComponent();
			InitializeSourceGrid();

			__cPrevFilterButton = toolItem_All;
		}

		private void RefreshCategoryNode(string exchangeName, ESymbolCategory category) {
			string sCategory = Enum.GetName(typeof(ESymbolCategory), category);

			TreeNode cRootNode = treeExchanges.Nodes[0];
			TreeNode cExchangeNode = cRootNode.Nodes[exchangeName];
			TreeNode cCategoryNode = cExchangeNode.Nodes[sCategory];

			AbstractExchange cExchange = ProductManager.Manager.GetExchange(exchangeName);
			List<string> cSymbols = cExchange.GetProductClassify(category);
			if (cSymbols != null) {
				cCategoryNode.Text = string.Format("{0}({1})", sCategory, cSymbols.Count);
			}
		}

		private void RefreshSymbolCategorys(string exchangeName, ESymbolCategory category) {
			AbstractExchange cExchange = ProductManager.Manager.GetExchange(exchangeName);
			List<string> cSymbols = cExchange.GetProductClassify(category);

			__cAllData.Clear();
			__cBasicData.Clear();
			__cCustomData.Clear();

			int iCount = cSymbols.Count;
			int iSourceCount = __cDataSources.Count;
			for (int i = 0; i < iCount; i++) {
				string sSymbol = cSymbols[i];
				Product cProduct = cExchange.GetProduct(sSymbol);
				AbstractProductProperty cBaseProperty = cExchange.GetProperty(sSymbol);

				_ProductInfo cBasicInfo = CreateProductInfo(exchangeName, null, cProduct, cBaseProperty);
				__cAllData.Add(cBasicInfo);

				if (cBaseProperty != null) {
					__cBasicData.Add(cBasicInfo);

					for (int j = 0; j < iSourceCount; j++) {
						string sDataSource = __cDataSources[j];
						AbstractProductProperty cProperty = cExchange.GetProperty(sSymbol, sDataSource);
						if (cProperty != cBaseProperty) {
							_ProductInfo cProductInfo = CreateProductInfo(exchangeName, sDataSource, cProduct, cProperty);
							__cAllData.Add(cProductInfo);
							__cCustomData.Add(cProductInfo);
						}
					}
				}
			}

			source.Refresh();
			if (__cSortRangeRowEvent != null) {
				this.dataGrid.SortRangeRows(__cSortRangeRowEvent.Range, __cSortRangeRowEvent.KeyColumn, __cSortRangeRowEvent.Ascending, __cSortRangeRowEvent.CellComparer);
			}
		}

		private void SaveSettings() {
			DialogResult cResult = MessageBox.Show(__sMessageContent_003, __sMessageHeader_003, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (cResult == DialogResult.Yes) {
				List<AbstractExchange> cExchanges = ProductManager.Manager.Exchanges;

				int iCount = cExchanges.Count;
				for (int i = 0; i < iCount; i++) {
					AbstractExchange cExchange = cExchanges[i];
					cExchange.Save();  //交易所資訊儲存
					cExchange.Load();  //重新讀取交易所設定值
				}

				MessageBox.Show(__sMessageContent_004, __sMessageHeader_004, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void frmProductManager_Load(object sender, EventArgs e) {
			List<AbstractExchange> cExchanges = ProductManager.Manager.Exchanges;

			int iCount = cExchanges.Count;
			if (iCount > 0) {
				TreeNode cRootNode = treeExchanges.Nodes[0];

				for (int i = 0; i < iCount; i++) {
					AbstractExchange cExchange = cExchanges[i];
					TreeNode cTreeExchange = new TreeNode(cExchange.ShortName, 1, 1);
					cTreeExchange.Name = cExchange.ShortName;

					ESymbolCategory[] cCategorys = Enum.GetValues(typeof(ESymbolCategory)) as ESymbolCategory[];
					foreach(ESymbolCategory cCategory in cCategorys) {
						List<string> cSymbols = cExchange.GetProductClassify(cCategory);
						if (cSymbols != null && cSymbols.Count > 0) {
							string sCategory = Enum.GetName(typeof(ESymbolCategory), cCategory);
							TreeNode cTreeCategory = new TreeNode(string.Format("{0}({1})", sCategory, cSymbols.Count), 2, 2);
							cTreeCategory.Name = sCategory;
							cTreeCategory.Tag = cCategory;
							cTreeExchange.Nodes.Add(cTreeCategory);
						}
					}
					cRootNode.Nodes.Add(cTreeExchange);
				}
			}
		}

		private void btnOk_Click(object sender, EventArgs e) {
			SaveSettings();
			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void treeExchanges_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
			TreeNode cNode = e.Node;
			if (cNode.Tag == null) {
				return;
			}

			string sExchangeName = cNode.Parent.Text;
			ESymbolCategory cCategory = (ESymbolCategory) cNode.Tag;
			__cSelectedNode = cNode;

			RefreshSymbolCategorys(sExchangeName, cCategory);
		}

		private void toolItem_Filter_Click(object sender, EventArgs e) {
			ToolStripButton cButton = sender as ToolStripButton;
			int iIndex = int.Parse(cButton.Tag as string);
			int iPrevIndex = int.Parse(__cPrevFilterButton.Tag as string);
			if (iIndex != iPrevIndex) {
				switch (iIndex) {
					case 0:  //顯示全部
						source.SetDataSource(__cAllData);
						break;
					case 1:  //顯示交易所基礎設定
						source.SetDataSource(__cBasicData);
						break;
					case 2:  //顯示使用者定義
						source.SetDataSource(__cCustomData);
						break;
				}

				__cPrevFilterButton.Checked = false;
				cButton.Checked = true;
				__cPrevFilterButton = cButton;

				source.Refresh();
				if (__cSortRangeRowEvent != null) {
					this.dataGrid.SortRangeRows(__cSortRangeRowEvent.Range, __cSortRangeRowEvent.KeyColumn, __cSortRangeRowEvent.Ascending, __cSortRangeRowEvent.CellComparer);
				}
			}
		}

		private void toolItem_Modify_Click(object sender, EventArgs e) {
			ToolStripButton cButton = sender as ToolStripButton;
			int iIndex = int.Parse(cButton.Tag as string);

			string sExchange = null;
			string sDataSource = null;
			Product cProduct = null;
			AbstractProductProperty cProperty = null;
			switch (iIndex) {
				case 1:  //新增
					frmCreateProduct frmCreateProduct = new frmCreateProduct();
					DialogResult cResult = frmCreateProduct.ShowDialog();
					if (cResult == DialogResult.OK) {
						sDataSource = frmCreateProduct.DataSource;
						sExchange = frmCreateProduct.ExchangeName;
						cProduct = frmCreateProduct.Product;
						cProperty = frmCreateProduct.Property;

						ESymbolCategory cCategory = cProduct.Category;
						RefreshCategoryNode(sExchange, cCategory);     //更新樹狀結構
						RefreshSymbolCategorys(sExchange, cCategory);  //更新表格
					}
					break;
				case 2:  //修改
				case 3:  //刪除
					if (dataGrid.SelectedDataRows.Length > 0) {
						object oData = dataGrid.SelectedDataRows[0];
						if (oData != null) {
							_ProductInfo cProductInfo = oData as _ProductInfo;

							string sSymbolId = cProductInfo.ProductId;
							sExchange = cProductInfo.ExchangeName;
							sDataSource = cProductInfo.DataSource;
							sDataSource = (sDataSource.Length == 0) ? null : sDataSource;
							AbstractExchange cExchange = ProductManager.Manager.GetExchange(sExchange);
							cProduct = cExchange.GetProduct(sSymbolId);

							if (iIndex == 2) {  //修改動作
								cProperty = cExchange.GetProperty(sSymbolId, sDataSource);
							} else {  //刪除動作
								bool bDeleteProperty = sDataSource != null;
								if (!bDeleteProperty) {
									DialogResult cDeleteResult = MessageBox.Show(__sMessageContent_001, __sMessageHeader_001, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
									bDeleteProperty |= cDeleteResult == DialogResult.Yes;
								}

								if (bDeleteProperty) {
									DialogResult cDeleteResult = MessageBox.Show(__sMessageContent_002, __sMessageHeader_002, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
									if (cDeleteResult == DialogResult.Yes) {
										cExchange.RemoveProperty(sSymbolId, sDataSource);  //要先移除屬性設定(因為屬性會從 Product 結構取得商品資訊, 所以要先移除屬性設定)
									}

									if (sDataSource == null) {  //如果資料報價來源是 null 才可以移除商品(如果為 null 會詢問使用者是否要刪除)
										cExchange.RemoveProduct(sSymbolId);
									}

									ESymbolCategory cCategory = cProduct.Category;
									RefreshCategoryNode(sExchange, cCategory);     //更新樹狀結構
									RefreshSymbolCategorys(sExchange, cCategory);  //更新表格
								}
								return;
							}
						}
					}
					break;
			}

			if (cProduct != null && cProperty != null) {
				frmProductPropertySettings frmProductPropertySettings = new frmProductPropertySettings();
				frmProductPropertySettings.SetParameters(sExchange, sDataSource, cProduct, cProperty.Clone());
				DialogResult cResult = frmProductPropertySettings.ShowDialog();
				if (cResult == DialogResult.OK) {
					string sExchangeName = __cSelectedNode.Parent.Text;
					ESymbolCategory cCategory = (ESymbolCategory) __cSelectedNode.Tag;

					RefreshSymbolCategorys(sExchangeName, cCategory);  //更新表格
				}
			}
		}

		private void toolItem_Save_Click(object sender, EventArgs e) {
			SaveSettings();
		}

		private void dataGrid_SortedRangeRows(object sender, SortRangeRowsEventArgs e) {
			__cSortRangeRowEvent = e;
		}
	}
}