using System;
using System.Windows.Forms;
using System.Collections.Generic;
using PowerLanguage;
using Zeghs.Data;
using Zeghs.Scripts;
using Zeghs.Settings;
using Zeghs.Informations;

namespace Zeghs.Forms {
	internal partial class frmFormatObject : Form {
		private string __sScriptName = null;
		private ProfileSetting __cProfile = null;
		private List<ChartSetting> __cCharts = null;
		private ScriptSetting __cScriptSetting = null;
		private ScriptType __cScriptType = ScriptType.Signal;

		internal ProfileSetting Profile {
			get {
				return __cProfile;
			}
		}

		internal frmFormatObject() {
			__cCharts = new List<ChartSetting>(32);

			InitializeComponent();
			InitializeSourceGrid();
		}

		internal void SetScriptInformation(ScriptInformation scriptInformation) {
			__sScriptName = scriptInformation.FullName;
			__cScriptType = scriptInformation.Property.ScriptType;

			switch (__cScriptType) {
				case ScriptType.Script:
					break;
				case ScriptType.Signal:
					__cScriptSetting = new SignalSetting();
					break;
			}
		}

		private string[] GetRanges(string data) {
			string[] sData = data.Split(',');
			string[] sRanges = sData[1].Split(';');
			
			DateTime cToday = DateTime.Today;
			DateTime cDate = DateTime.Parse(sRanges[0]);
			if (cDate > cToday) {
				cDate = cToday;
			}
			return new string[] { sRanges[1], cDate.ToString("yyyy/MM/dd") };
		}

		private void btnDataStreamAdd_Click(object sender, EventArgs e) {
			int iDataStream = source.Count + 1;
			frmCreateScriptSetting frmCreateScriptSetting = new frmCreateScriptSetting();
			frmCreateScriptSetting.DataStream = iDataStream;
			
			DialogResult cResult = frmCreateScriptSetting.ShowDialog();
			frmCreateScriptSetting.Dispose();

			if (cResult == DialogResult.OK) {
				ChartSetting cChartSetting = frmCreateScriptSetting.ChartSetting;
				RequestSetting cRequestSetting = frmCreateScriptSetting.RequestSetting;
				__cScriptSetting.DataRequests.Add(cRequestSetting);  //加入至腳本設定值內

				string[] sRanges = GetRanges(cRequestSetting.Range);

				_DataStreamInfo cInfo = new _DataStreamInfo();
				cInfo.DataStream = iDataStream;
				cInfo.SymbolId = cRequestSetting.SymbolId;
				cInfo.Range = sRanges[0];
				cInfo.LastDate = sRanges[1];
				cInfo.Period = cRequestSetting.DataPeriod;
				cInfo.SubChart = (cChartSetting.IsSubChart) ? "SubChart" : "Hide";

				__cCharts.Add(cChartSetting);

				source.Add(cInfo);
				source.Refresh();
			}

			bool bEnabled = source.Count > 0;
			btnOK.Enabled = bEnabled;
			btnDataStreamRemove.Enabled = bEnabled;
		}

		private void btnDataStreamRemove_Click(object sender, EventArgs e) {
			if (dataGrid.SelectedDataRows.Length > 0) {
				_DataStreamInfo cInfo = dataGrid.SelectedDataRows[0] as _DataStreamInfo;
				if (cInfo != null) {
					int iIndex = cInfo.DataStream - 1;
					__cScriptSetting.DataRequests.RemoveAt(iIndex);
					__cCharts.RemoveAt(iIndex);
					source.RemoveAt(iIndex);
					
					int iCount = source.Count;
					for (int i = iIndex; i < iCount; i++) {
						_DataStreamInfo cData = source[i] as _DataStreamInfo;
						cData.DataStream = i + 1;
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
					break;
			}
		}

		private void btnOK_Click(object sender, EventArgs e) {
			__cProfile = new ProfileSetting();
			__cProfile.Charts = __cCharts;
			__cProfile.ProfileId = Guid.NewGuid().ToString();
			__cProfile.Script = __cScriptSetting;
			__cProfile.ScriptName = __sScriptName;
			__cProfile.ScriptType = __cScriptType;
			__cProfile.Window = new WindowStatus();

			ProfileManager.Manager.AddProfile(__cProfile);
			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}
	}
}