using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Products;
using Zeghs.Services;
using Zeghs.Managers;
using Zeghs.Settings;

namespace Zeghs.Forms {
	internal partial class frmCreateScriptSetting : Form {
		private static Dictionary<string, string> __cAddSymbolIds = new Dictionary<string, string>(64);

		private bool __bEdit = true;
		private bool __bNewSetting = false;  //是否為新建立的設定
		private ChartSetting __cChartSetting = null;
		private RequestSetting __cRequestSetting = null;

		/// <summary>
		///   [取得] 圖表設定值
		/// </summary>
		internal ChartSetting ChartSetting {
			get {
				return __cChartSetting;
			}
		}

		/// <summary>
		///   [取得/設定] 最大 Layer 圖層個數(根據已經新增的圖表資訊內的 LayerIndex 判斷目前使用到的最大圖層個數)
		/// </summary>
		internal int MaxLayerCount {
			get;
			set;
		}

		/// <summary>
		///   [取得] 請求設定值
		/// </summary>
		internal RequestSetting RequestSetting {
			get {
				return __cRequestSetting;
			}
		}

		internal frmCreateScriptSetting()
			: this(new ChartSetting(), new RequestSetting(), true) {
				__bEdit = false;
		}

		internal frmCreateScriptSetting(ChartSetting chartSetting, RequestSetting requestSetting, bool isNewSetting) {
			__cChartSetting = chartSetting;
			__cRequestSetting = requestSetting;
			__bNewSetting = isNewSetting;

			InitializeComponent();
			InitializeSourceGrid();
		}

		private void GetChartSetting() {
			PenStyle[] cPenStyles = new PenStyle[2];
			int iChartIndex = listChartType.SelectedIndex;
			switch (iChartIndex) {
				case 0:  //美國線
					__cChartSetting.ChartType = EChartType.OHLC;
					break;
				case 1:  //HLC線
					__cChartSetting.ChartType = EChartType.HLC;
					break;
				case 2:  //蠟燭線
					__cChartSetting.ChartType = EChartType.Candlestick;
					cPenStyles = new PenStyle[3];
					break;
				case 3:  //收盤線
					__cChartSetting.ChartType = EChartType.ClosingLine;
					break;
			}

			cPenStyles[0] = new PenStyle(colorUp.SelectedColor, comboUpWidth.SelectedIndex + 1);
			cPenStyles[1] = new PenStyle(colorDown.SelectedColor, comboDownWidth.SelectedIndex + 1);
			if (iChartIndex == 2) {
				cPenStyles[2] = new PenStyle(colorLine.SelectedColor, comboLineWidth.SelectedIndex + 1);
			}
			__cChartSetting.PenStyles = cPenStyles;
			__cChartSetting.LegendColor = colorLegend.SelectedColor;

			bool bSubChart = listSubChart.SelectedIndex == 0;
			__cChartSetting.IsSubChart = bSubChart;
			__cChartSetting.LayerIndex = comboLayer.SelectedIndex;
			__cChartSetting.IsShowNewPrice = checkShowNewPrice.Checked;

			//座標資訊設定
			AxisSetting cAxisSetting = new AxisSetting();
			int iAxisIndex = listAxisRange.SelectedIndex;
			switch (iAxisIndex) {
				case 0:  //目前區間
					cAxisSetting.AxisScope = EAxisScope.CurrentScope;
					break;
				case 1:  //全部數列
					cAxisSetting.AxisScope = EAxisScope.AllScope;
					break;
				case 2:  //變動大小
					cAxisSetting.AxisScope = EAxisScope.ChangeScope;
					break;
				case 3:  //價格區間
					cAxisSetting.AxisScope = EAxisScope.PriceScaleScope;
					break;
			}

			if (checkMargin.Checked) {
				double dValue = 0;
				if (double.TryParse(txtTopMargin.Text, out dValue)) {
					cAxisSetting.MarginTop = dValue;
				}

				if (double.TryParse(txtBottomMargin.Text, out dValue)) {
					cAxisSetting.MarginBottom = dValue;
				}
			}

			if (checkManualAxis.Checked) {
				double dValue = 0;
				if (radioScaleGap.Checked) {
					if (double.TryParse(txtScaleGap.Text, out dValue)) {
						cAxisSetting.ScaleMode = EAxisScaleMode.ScaleGap;
					}
				} else if (radioScaleCount.Checked) {
					if (double.TryParse(txtScaleCount.Text, out dValue)) {
						cAxisSetting.ScaleMode = EAxisScaleMode.ScaleCount;
					}
				} else {
					cAxisSetting.ScaleMode = EAxisScaleMode.None;
				}
				cAxisSetting.ScaleValue = dValue;
			} else {
				cAxisSetting.ScaleMode = EAxisScaleMode.None;
			}
			__cChartSetting.Axis = cAxisSetting;
		}

		private bool GetRequestSetting() {
			string sDataSources = comboDataSources.Text;
			if (sDataSources.Length == 0) {
				MessageBox.Show(__sMessageContent_001, __sMessageHeader_001, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			string sSymbolId = comboProduct.Text;
			if (sSymbolId.Length == 0) {
				MessageBox.Show(__sMessageContent_002, __sMessageHeader_001, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			__cRequestSetting.DataFeed = sDataSources;
			__cRequestSetting.SymbolId = sSymbolId;
			__cRequestSetting.Exchange = __cAddSymbolIds[sSymbolId];
			__cRequestSetting.DataPeriod = string.Format("{0},{1}", txtPeriod.Text, comboResolution.Text);

			DateTime cEndDate = DateTime.Now.Date;
			if (radioRange2.Checked) {
				if (cEndDate == dtEndDate.Value.Date) {
					cEndDate = RequestSetting.MAX_REQUEST_LASTDATE;
				}
				__cRequestSetting.Range = string.Format("fromTo,{0};{1}", cEndDate.ToString("yyyy/MM/dd"), dtStartDate.Value.ToString("yyyy/MM/dd"));
			} else {
				string sCount = txtCount.Text;
				if (sCount.Length == 0) {
					MessageBox.Show(__sMessageContent_003, __sMessageHeader_001, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}

				int iCount = 0;
				if (!int.TryParse(sCount, out iCount)) {
					MessageBox.Show(__sMessageContent_004, __sMessageHeader_001, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}

				if (cEndDate == dtTODate.Value.Date) {
					cEndDate = RequestSetting.MAX_REQUEST_LASTDATE;
				}
				__cRequestSetting.Range = string.Format("{0},{1};{2}", (comboRequestMode.SelectedIndex == 0) ? "barsBack" : "daysBack", cEndDate.ToString("yyyy/MM/dd"), iCount);
			}
			return true;
		}

		private void LoadProducts(string dataSource) {
			List<AbstractExchange> cExchanges = ProductManager.Manager.Exchanges;

			int iCount = cExchanges.Count;
			if (iCount > 0) {
				for (int i = 0; i < iCount; i++) {
					AbstractExchange cExchange = cExchanges[i];

					int iIndex = 0;
					string sExchangeName = cExchange.ShortName;
					ESymbolCategory[] cCategorys = Enum.GetValues(typeof(ESymbolCategory)) as ESymbolCategory[];
					foreach (ESymbolCategory cCategory in cCategorys) {
						List<string> cSymbols = cExchange.GetProductClassify(cCategory);
						if (cSymbols != null && cSymbols.Count > 0) {
							++iIndex;
							tabControl_Products.TabPages.Add(cCategory.ToString());
							if (iIndex == __cSources.Count) {
								__cSources.Add(new SimpleBoundList<_ProductInfo>(cSymbols.Count));
							}

							foreach (string sSymbolId in cSymbols) {
								AbstractProductProperty cProperty = cExchange.GetProperty(sSymbolId, dataSource);
								if (cProperty != null) {
									_ProductInfo cProductInfo = new _ProductInfo() {
										ProductId = sSymbolId,
										Description = cProperty.Description,
										ExchangeName = sExchangeName
									};

									__cSources[0].Add(cProductInfo);
									__cSources[iIndex].Add(cProductInfo);
								}
							}
						}
					}
				}
			}
		}

		private void RefreshGrid() {
			int iIndex = tabControl_Products.SelectedIndex;

			__cCurrentPages.Controls.RemoveAt(0);

			this.dataGrid.DataSource = __cSources[iIndex];
			TabPage cPage = tabControl_Products.TabPages[iIndex];
			cPage.Controls.Add(this.dataGrid);

			__cCurrentPages = cPage;
		}

		private void SetChartSetting() {
			bool bCandlestick = false;
			switch (__cChartSetting.ChartType) {
				case EChartType.OHLC:  //美國線
					listChartType.SelectedIndex = 0;
					break;
				case EChartType.HLC:  //HLC線
					listChartType.SelectedIndex = 1;
					break;
				case EChartType.Candlestick:  //蠟燭線
					listChartType.SelectedIndex = 2;
					bCandlestick = true;
					break;
				case EChartType.ClosingLine:  //收盤線
					listChartType.SelectedIndex = 3;
					break;
			}

			PenStyle[] cPenStyles = __cChartSetting.PenStyles;
			colorUp.SelectedColor = cPenStyles[0].Color;
			comboUpWidth.SelectedIndex = cPenStyles[0].Width - 1;
			colorDown.SelectedColor = cPenStyles[1].Color;
			comboDownWidth.SelectedIndex = cPenStyles[1].Width - 1;
			if (bCandlestick) {
				colorLine.SelectedColor = cPenStyles[2].Color;
				comboLineWidth.SelectedIndex = cPenStyles[2].Width - 1;
			}
			colorLegend.SelectedColor = __cChartSetting.LegendColor;

			comboLayer.SelectedIndex = (__cChartSetting.IsSubChart) ? __cChartSetting.LayerIndex : 0;
			listSubChart.SelectedIndex = (__cChartSetting.IsSubChart) ? 0 : 1;
			checkShowNewPrice.Checked = __cChartSetting.IsShowNewPrice;

			//座標資訊設定
			AxisSetting cAxisSetting = __cChartSetting.Axis;
			switch (cAxisSetting.AxisScope) {
				case EAxisScope.CurrentScope:  //目前區間
					listAxisRange.SelectedIndex = 0;
					break;
				case EAxisScope.AllScope:  //全部數列
					listAxisRange.SelectedIndex = 1;
					break;
				case EAxisScope.ChangeScope:  //變動大小
					listAxisRange.SelectedIndex = 2;
					break;
				case EAxisScope.PriceScaleScope:  //價格區間
					listAxisRange.SelectedIndex = 3;
					break;
			}

			if (cAxisSetting.MarginTop == 0 && cAxisSetting.MarginBottom == 0) {
				checkMargin.Checked = false;
				txtTopMargin.Text = txtBottomMargin.Text = "0";
			} else {
				checkMargin.Checked = true;
				txtTopMargin.Text = cAxisSetting.MarginTop.ToString();
				txtBottomMargin.Text = cAxisSetting.MarginBottom.ToString();
			}

			if (cAxisSetting.ScaleMode != EAxisScaleMode.None) {
				checkManualAxis.Checked = true;

				switch (cAxisSetting.ScaleMode) {
					case EAxisScaleMode.ScaleGap:
						radioScaleGap.Checked = true;
						txtScaleGap.Text = cAxisSetting.ScaleValue.ToString();
						break;
					case EAxisScaleMode.ScaleCount:
						radioScaleCount.Checked = true;
						txtScaleCount.Text = cAxisSetting.ScaleValue.ToString();
						break;
				}
			}
		}

		private void SetRequestSetting() {
			comboDataSources.Items.Add(__cRequestSetting.DataFeed);
			comboDataSources.SelectedIndex = 0;
			comboProduct.Items.Add(__cRequestSetting.SymbolId);
			comboProduct.SelectedIndex = 0;

			string[] sPeriods = __cRequestSetting.DataPeriod.Split(',');
			txtPeriod.Text = sPeriods[0];
			comboResolution.Text = sPeriods[1];

			string[] sRangeModes = __cRequestSetting.Range.Split(',');
			string[] sRanges = sRangeModes[1].Split(';');

			DateTime cEndDate = DateTime.Parse(sRanges[0]);
			dtEndDate.Value = (cEndDate == RequestSetting.MAX_REQUEST_LASTDATE) ? DateTime.Today : cEndDate;
			if (sRangeModes[0].Equals("fromTo")) {
				radioRange2.Checked = true;
				dtStartDate.Value = DateTime.Parse(sRanges[1]);
			} else {
				radioRange1.Checked = true;
				comboRequestMode.SelectedIndex = (sRangeModes[0].Equals("barsBack")) ? 0 : 1;
				txtCount.Text = sRanges[1];
			}
		}

		private void frmCreateScriptSetting_Load(object sender, EventArgs e) {
			//列舉所有圖表周期
			EResolution[] cResolutions = Enum.GetValues(typeof(EResolution)) as EResolution[];
			foreach (EResolution cResolution in cResolutions) {
				comboResolution.Items.Add(cResolution.ToString());
			}

			//加入圖層索引資料
			for (int i = 1; i <= this.MaxLayerCount; i++) {
				comboLayer.Items.Add(i);
			}

			//加入線寬資料資料
			for (int i = 0; i < 16; i++) {
				comboDownWidth.Items.Add(i);
				comboLineWidth.Items.Add(i);
				comboUpWidth.Items.Add(i);
			}

			comboUpWidth.SelectedIndex = 0;
			comboDownWidth.SelectedIndex = 0;
			comboLineWidth.SelectedIndex = 0;

			//設定顏色值
			colorLegend.SelectedColor = Color.Yellow;
			colorUp.SelectedColor = Color.Red;
			colorDown.SelectedColor = Color.Lime;
			colorLine.SelectedColor = Color.Gray;

			comboTimeZone.SelectedIndex = 0;

			if (__bEdit) {
				SetChartSetting();
				SetRequestSetting();

				if (!__bNewSetting) {
					foreach (Control cControl in pageItem_Product.Controls) {
						cControl.Enabled = false;
					}

					foreach (Control cControl in pageItem_Settings.Controls) {
						cControl.Enabled = false;
					}
					listSubChart.Enabled = true;
					listChartType.Enabled = false;
				}
			} else {
				//加入所有已經啟動的報價資料來源名稱
				List<AbstractQuoteService> cServices = QuoteManager.Manager.QuoteServices;
				int iCount = cServices.Count;
				for (int i = 0; i < iCount; i++) {
					AbstractQuoteService cService = cServices[i];
					comboDataSources.Items.Add(cService.DataSource);
				}

				//加入使用者之前所選過的商品名稱
				if (__cAddSymbolIds.Count > 0) {
					foreach (string sSymbolId in __cAddSymbolIds.Keys) {
						comboProduct.Items.Insert(0, sSymbolId);
					}
					comboProduct.SelectedIndex = 0;
				}

				//設定 ComboBox & listBox 索引值
				comboLayer.Items.Add("New");
				comboLayer.SelectedIndex = 0;
				comboDataSources.SelectedIndex = 0;
				comboResolution.SelectedIndex = 0;
				comboRequestMode.SelectedIndex = 0;
				listSubChart.SelectedIndex = 0;
				listChartType.SelectedIndex = 2;
				listAxisRange.SelectedIndex = 3;

				//預設選取第一種區間模式
				radioRange1.Checked = true;
			}
		}

		private void comboDataSources_SelectedIndexChanged(object sender, EventArgs e) {
			__cSources[0].Clear();  //清除所有商品資訊(0=所有商品資訊)

			int iCount = __cSources.Count;
			for (int i = iCount - 1; i > 0; i--) {
				SimpleBoundList<_ProductInfo> cProducts = __cSources[i];
				cProducts.Clear();  //清除分類商品資訊

				__cSources.RemoveAt(i);  //移除分類商品
				tabControl_Products.TabPages.RemoveAt(i);  //移除分類商品頁籤
			}

			LoadProducts(comboDataSources.Text);
			RefreshGrid();
		}

		private void listChartType_SelectedIndexChanged(object sender, EventArgs e) {
			bool bVisible = listChartType.SelectedIndex == 2;
			labLine.Visible = bVisible;
			colorLine.Visible = bVisible;
			comboLineWidth.Visible = bVisible;
		}

		private void listSubChart_SelectedIndexChanged(object sender, EventArgs e) {
			if (__bNewSetting) {
				comboLayer.Enabled = listSubChart.SelectedIndex == 0;
			}
		}

		private void radioRange1_CheckedChanged(object sender, EventArgs e) {
			label5.Enabled = false;
			label6.Enabled = false;
			dtStartDate.Enabled = false;
			dtEndDate.Enabled = false;

			label4.Enabled = true;
			txtCount.Enabled = true;
			comboRequestMode.Enabled = true;
			dtTODate.Enabled = true;
		}

		private void radioRange2_CheckedChanged(object sender, EventArgs e) {
			label4.Enabled = false;
			txtCount.Enabled = false;
			comboRequestMode.Enabled = false;
			dtTODate.Enabled = false;

			label5.Enabled = true;
			label6.Enabled = true;
			dtStartDate.Enabled = true;
			dtEndDate.Enabled = true;
		}


		private void checkMargin_CheckedChanged(object sender, EventArgs e) {
			bool bEnabled = checkMargin.Checked;
			labTopMargin.Enabled = bEnabled;
			txtTopMargin.Enabled = bEnabled;
			labTopMarginP.Enabled = bEnabled;
			labBottomMargin.Enabled = bEnabled;
			txtBottomMargin.Enabled = bEnabled;
			labBottomMarginP.Enabled = bEnabled;
		}

		private void checkManualAxis_CheckedChanged(object sender, EventArgs e) {
			bool bEnabled = checkManualAxis.Checked;
			radioScaleGap.Enabled = bEnabled;
			txtScaleGap.Enabled = bEnabled;
			radioScaleCount.Enabled = bEnabled;
			txtScaleCount.Enabled = bEnabled;
		}

		private void tabControl_Products_SelectedIndexChanged(object sender, EventArgs e) {
			RefreshGrid();
		}

		private void btnOK_Click(object sender, EventArgs e) {
			if (__bNewSetting) {
				bool bRet = GetRequestSetting();  //設定請求資訊
				if (!bRet) {
					return;
				}
			}

			GetChartSetting();  //設定圖表資訊

			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void dataGrid_DoubleClick(object sender, EventArgs e) {
			if (this.dataGrid.SelectedDataRows != null) {
				_ProductInfo cProductInfo = this.dataGrid.SelectedDataRows[0] as _ProductInfo;
				
				string sSymbolId = cProductInfo.ProductId;
				if (!__cAddSymbolIds.ContainsKey(sSymbolId)) {
					comboProduct.Items.Insert(0, sSymbolId);
					comboProduct.SelectedIndex = 0;

					__cAddSymbolIds.Add(cProductInfo.ProductId, cProductInfo.ExchangeName);
				}
			}
		}
	}
}