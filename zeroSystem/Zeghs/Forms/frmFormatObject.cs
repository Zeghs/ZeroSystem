using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Chart;
using Zeghs.Scripts;
using Zeghs.Settings;
using Zeghs.Informations;

namespace Zeghs.Forms {
	internal partial class frmFormatObject : Form {
		internal static void Create(WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel, ScriptInformation script) {
			frmFormatObject frmFormatObject = new frmFormatObject();
			frmFormatObject.SetScriptInformation(script);

			DialogResult cResult = frmFormatObject.ShowDialog();
			frmFormatObject.Dispose();

			if (cResult == DialogResult.OK) {
				ProfileSetting cProfile = frmFormatObject.Profile;
				switch (cProfile.ScriptType) {
					case ScriptType.Script:
						break;
					case ScriptType.Signal:
						frmSignalViewer.Create(dockPanel, cProfile);
						break;
				}
			}
		}

		private static _DataStreamInfo CreateDataStreamInfo(int dataStream, ChartSetting chart, RequestSetting request, bool isNewSetting) {
			string[] sRanges = GetRequestRanges(request.Range);

			_DataStreamInfo cInfo = new _DataStreamInfo();
			cInfo.DataStream = dataStream;
			cInfo.SymbolId = request.SymbolId;
			cInfo.Range = string.Format("{0} {1}", sRanges[1], (sRanges[0][0] == 'f') ? string.Empty : sRanges[0]);
			cInfo.LastDate = sRanges[2];
			cInfo.Period = request.DataPeriod;
			cInfo.SubChart = (chart.IsSubChart) ? string.Format("SubChart #{0}", chart.LayerIndex + 1) : "Hide";
			cInfo.IsNewSetting = isNewSetting;
			return cInfo;
		}
		
		private static string[] GetRequestRanges(string data) {
			string[] sData = data.Split(',');
			string[] sRanges = sData[1].Split(';');

			DateTime cToday = DateTime.Today;
			DateTime cDate = DateTime.Parse(sRanges[0]);
			if (cDate > cToday) {
				cDate = cToday;
			}
			return new string[] { sData[0], sRanges[1], cDate.ToString("yyyy/MM/dd") };
		}

		private int __iMaxLayerIndex = -1;
		private int __iPrevDataRequestCount = 0;
		private bool __bModifyDataRequest = false;
		private bool __bPropertyCompleted = false;
		private string __sScriptName = null;
		private ProfileSetting __cProfile = null;
		private List<ChartSetting> __cCharts = null;
		private List<int> __cModifyRemoveItems = null;
		private ScriptSetting __cScriptSetting = null;
		private ChartProperty __cChartProperty = null;
		private Action __cBeforeCompletedCallback = null;
		private ScriptType __cScriptType = ScriptType.Signal;

		internal ProfileSetting Profile {
			get {
				return __cProfile;
			}
		}

		internal bool ModifyDataRequest {
			get {
				return __bModifyDataRequest;
			}
		}

		internal frmFormatObject(ProfileSetting profile = null, Action beforeCompletedCallback = null) {
			__cModifyRemoveItems = new List<int>(16);
			__cBeforeCompletedCallback = beforeCompletedCallback;

			if (profile == null) {
				__cCharts = new List<ChartSetting>(4);

				ChartProperty DefaultProperty = GlobalSettings.ChartProfile.DefaultProperty;
				__cChartProperty = new ChartProperty();
				__cChartProperty.AxisFont = DefaultProperty.AxisFont;
				__cChartProperty.LegendFont = DefaultProperty.LegendFont;
				__cChartProperty.TextFont = DefaultProperty.TextFont;
				__cChartProperty.TitleFont = DefaultProperty.TitleFont;
				__cChartProperty.TitleFont = DefaultProperty.TitleFont;
				__cChartProperty.AxisColor = DefaultProperty.AxisColor;
				__cChartProperty.BackgroundColor = DefaultProperty.BackgroundColor;
				__cChartProperty.DrawAideLineColor = DefaultProperty.DrawAideLineColor;
				__cChartProperty.ForeColor = DefaultProperty.ForeColor;
				__cChartProperty.GridColor = DefaultProperty.GridColor;
				__cChartProperty.TradeLineColor = DefaultProperty.TradeLineColor;
				__cChartProperty.TradeSymbolColor = DefaultProperty.TradeSymbolColor;
				__cChartProperty.IsShowGrid = true;
				__cChartProperty.ChartSettings = __cCharts;
			} else {
				__cProfile = profile;
				__cChartProperty = profile.ChartProperty;
				__cCharts = __cChartProperty.ChartSettings;
				__cScriptSetting = profile.Script;

				__iPrevDataRequestCount = __cScriptSetting.DataRequests.Count;
				__bPropertyCompleted = true;
			}

			InitializeComponent();
			InitializeSourceGrid();
		}

		private void SetScriptInformation(ScriptInformation scriptInformation) {
			__sScriptName = scriptInformation.FullName;
			__cScriptType = scriptInformation.Property.ScriptType;

			switch (__cScriptType) {
				case ScriptType.Script:
					break;
				case ScriptType.Signal:
					if (__cScriptSetting == null) {
						__cScriptSetting = new SignalSetting();
					}
					break;
			}
		}

		private void frmFormatObject_Load(object sender, EventArgs e) {
			if (__cProfile != null) {
				List<RequestSetting> cRequests = __cScriptSetting.DataRequests;

				int iCount = cRequests.Count;
				for (int i = 0; i < iCount; i++) {
					int iLayerIndex = __cCharts[i].LayerIndex;
					if (iLayerIndex > __iMaxLayerIndex) {
						__iMaxLayerIndex = iLayerIndex;
					}

					source.Add(CreateDataStreamInfo(i + 1, __cCharts[i], cRequests[i], false));
				}
				source.Refresh();

				btnOK.Enabled = true;
				btnProperty.Enabled = false;
				btnDataStreamRemove.Enabled = true;
			}
		}

		private void btnDataStreamAdd_Click(object sender, EventArgs e) {
			frmCreateScriptSetting frmCreateScriptSetting = new frmCreateScriptSetting();
			frmCreateScriptSetting.MaxLayerCount = __iMaxLayerIndex + 1;  //最大圖層個數=最大圖層索引 + 1
			
			DialogResult cResult = frmCreateScriptSetting.ShowDialog();
			frmCreateScriptSetting.Dispose();

			if (cResult == DialogResult.OK) {
				ChartSetting cChartSetting = frmCreateScriptSetting.ChartSetting;
				RequestSetting cRequestSetting = frmCreateScriptSetting.RequestSetting;
				__cScriptSetting.DataRequests.Add(cRequestSetting);  //加入至腳本設定值內
				__cCharts.Add(cChartSetting);

				string[] sRanges = GetRequestRanges(cRequestSetting.Range);

				int iDataStream = source.Count + 1;
				source.Add(CreateDataStreamInfo(iDataStream, cChartSetting, cRequestSetting, true));
				source.Refresh();

				if (cChartSetting.LayerIndex > __iMaxLayerIndex) {
					__iMaxLayerIndex = cChartSetting.LayerIndex;
				}
			}

			bool bEnabled = source.Count > 0;
			btnOK.Enabled = bEnabled;
			btnDataStreamRemove.Enabled = bEnabled;
		}

		private void btnDataStreamEdit_Click(object sender, EventArgs e) {
			if (dataGrid.SelectedDataRows.Length > 0) {
				_DataStreamInfo cInfo = dataGrid.SelectedDataRows[0] as _DataStreamInfo;
				if (cInfo != null) {
					int iIndex = cInfo.DataStream - 1;
					int iDataStream = source.Count;
					frmCreateScriptSetting frmCreateScriptSetting = new frmCreateScriptSetting(__cCharts[iIndex], __cScriptSetting.DataRequests[iIndex], cInfo.IsNewSetting);
					frmCreateScriptSetting.MaxLayerCount = __iMaxLayerIndex + 1;  //最大圖層個數=最大圖層索引 + 1
					
					DialogResult cResult = frmCreateScriptSetting.ShowDialog();
					frmCreateScriptSetting.Dispose();
					if (cResult == DialogResult.OK) {
						ChartSetting cChartSetting = frmCreateScriptSetting.ChartSetting;
						cInfo.SubChart = (cChartSetting.IsSubChart) ? string.Format("SubChart #{0}", cChartSetting.LayerIndex + 1) : "Hide";

						source.Refresh();
					}
				}
			}
		}

		private void btnDataStreamRemove_Click(object sender, EventArgs e) {
			if (dataGrid.SelectedDataRows.Length > 0) {
				_DataStreamInfo cInfo = dataGrid.SelectedDataRows[0] as _DataStreamInfo;
				if (dataGrid.Rows.Count > 2 && cInfo != null) {
					int iIndex = cInfo.DataStream - 1;
					if (cInfo.IsNewSetting) {
						__cScriptSetting.DataRequests.RemoveAt(iIndex);
						__cCharts.RemoveAt(iIndex);
						
						source.RemoveAt(iIndex);
						
						int iCount = source.Count;
						for (int i = iIndex; i < iCount; i++) {
							_DataStreamInfo cData = source[i] as _DataStreamInfo;
							cData.DataStream = i + 1;
						}
					} else {
						__cModifyRemoveItems.Add(iIndex);
						dataGrid.Rows.HideRow(cInfo.DataStream);
					}
					source.Refresh();
				}

				bool bEnabled = source.Count > 0;
				btnOK.Enabled = bEnabled;
				btnDataStreamRemove.Enabled = bEnabled;
			}
		}

		private void btnProperty_Click(object sender, EventArgs e) {
			switch (__cScriptType) {
				case ScriptType.Script:
					break;
				case ScriptType.Signal:
					frmSignalProperty frmSignalProperty = new frmSignalProperty();
					frmSignalProperty.SetScriptSetting(__cScriptSetting);
					frmSignalProperty.ShowDialog();

					__bPropertyCompleted = frmSignalProperty.IsSetupCompleted;
					break;
			}
		}

		private void btnOK_Click(object sender, EventArgs e) {
			if (!__bPropertyCompleted) {
				MessageBox.Show(__sMessageContent_001, __sMessageHeader_001, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (__cProfile == null) {
				__cProfile = new ProfileSetting();
				__cProfile.ChartProperty = __cChartProperty;
				__cProfile.ProfileId = Guid.NewGuid().ToString();
				__cProfile.Script = __cScriptSetting;
				__cProfile.ScriptName = __sScriptName;
				__cProfile.ScriptType = __cScriptType;
				__cProfile.Window = new WindowStatus() {
					IsDock = true
				};

				ProfileManager.Manager.AddProfile(__cProfile);
			} else {
				int iCount = __cScriptSetting.DataRequests.Count;
				__bModifyDataRequest = iCount > __iPrevDataRequestCount || __cModifyRemoveItems.Count > 0;

				if (__bModifyDataRequest) {
					if (__cBeforeCompletedCallback != null) {
						__cBeforeCompletedCallback();
					}
					
					__cModifyRemoveItems.Sort();
					
					iCount = __cModifyRemoveItems.Count;
					if (iCount > 0) {
						List<RequestSetting> cDataRequests = __cScriptSetting.DataRequests;
						for (int i = iCount - 1; i >= 0; i--) {
							int iIndex = __cModifyRemoveItems[i];
							__cCharts.RemoveAt(iIndex);
							cDataRequests.RemoveAt(iIndex);
						}
					}
				}
			}
			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			if (__cProfile != null) {
				//檢查是否有新增的資料(如果有新增資料就移除)
				int iCount = source.Count;
				for (int i = 0; i < iCount; i++) {
					_DataStreamInfo cInfo = source[i] as _DataStreamInfo;
					if (cInfo.IsNewSetting) {
						__cCharts.RemoveAt(i);
						__cScriptSetting.DataRequests.RemoveAt(i);
					}
				}
			}
			this.DialogResult = DialogResult.Cancel;
		}
	}
}