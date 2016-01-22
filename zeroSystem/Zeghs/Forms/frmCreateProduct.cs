using System;
using System.Windows.Forms;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Products;
using Zeghs.Services;
using Zeghs.Managers;

namespace Zeghs.Forms {
	internal partial class frmCreateProduct : Form {
		private string __sDataSource = null;
		private string __sExchangeName = null;
		private Product __cProduct = null;
		private AbstractProductProperty __cProperty = null;

		internal string DataSource {
			get {
				return __sDataSource;
			}
		}

		internal string ExchangeName {
			get {
				return __sExchangeName;
			}
		}

		internal Product Product {
			get {
				return __cProduct;
			}
		}

		internal AbstractProductProperty Property {
			get {
				return __cProperty;
			}
		}

		internal frmCreateProduct() {
			InitializeComponent();
		}

		private void frmCreateProduct_Load(object sender, EventArgs e) {
			//讀取所有交易所資訊
			List<AbstractExchange> cExchanges = ProductManager.Manager.Exchanges;
			int iCount = cExchanges.Count;
			for (int i = 0; i < iCount; i++) {
				comboExchange.Items.Add(cExchanges[i].ShortName);
			}
			comboExchange.SelectedIndex = 0;

			//讀取所有可用的資料來源
			List<AbstractQuoteService> cServices = QuoteManager.Manager.QuoteServices;
			iCount = cServices.Count;
			for (int i = 0; i < iCount; i++) {
				comboDataSource.Items.Add(cServices[i].DataSource);
			}
			comboDataSource.SelectedIndex = 0;

			//讀取所有商品分類
			string[] sCategorys = Enum.GetNames(typeof(ESymbolCategory));
			foreach (string sCategory in sCategorys) {
				comboCategory.Items.Add(sCategory);
			}
			comboCategory.SelectedIndex = 0;
		}

		private void btnOk_Click(object sender, EventArgs e) {
			string sSymbolId = txtProductId.Text;
			if (sSymbolId.Length == 0) {  //如果沒有輸入商品代號就離開
				return;
			}

			__sExchangeName = comboExchange.Text;
			string sCommodityId = txtCommodity.Text;

			AbstractExchange cExchange = ProductManager.Manager.GetExchange(__sExchangeName);
			__cProduct = cExchange.GetProduct(sSymbolId);
			if (__cProduct == null) {
				string sProductName = txtProductName.Text;
				ESymbolCategory cCategory = (ESymbolCategory) Enum.Parse(typeof(ESymbolCategory), comboCategory.Text);

				__cProduct = new Product();
				__cProduct.SymbolId = sSymbolId;
				__cProduct.SymbolName = sProductName;
				__cProduct.Category = cCategory;
				if(sCommodityId.Length > 0) {
					__cProduct.CommodityId = sCommodityId;
				}

				cExchange.AddProduct(__cProduct);
			}

			sSymbolId = __cProduct.SymbolId;
			__sDataSource = comboDataSource.Text;
			__sDataSource = (__sDataSource.Length == 0) ? null : __sDataSource;
	
			string sCommodity = (sCommodityId.Length == 0) ? sSymbolId : sCommodityId;
			cExchange.AddProperty(sCommodity, __sDataSource);
			__cProperty = cExchange.GetProperty(sSymbolId, __sDataSource);

			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}
	}
}