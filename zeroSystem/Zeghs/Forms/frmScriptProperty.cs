using System;
using System.Windows.Forms;
using Zeghs.Scripts;
using Zeghs.Informations;

namespace Zeghs.Forms {
	internal partial class frmScriptProperty : Form {
		internal frmScriptProperty(ScriptInformation scriptInformation) {
			InitializeComponent();

			ScriptPropertyAttribute cProperty = scriptInformation.Property;
			labVersion.Text = cProperty.Version;
			labComment.Text = cProperty.Comment;
			labCopyright.Text = cProperty.Copyright;
			labName.Text = scriptInformation.Name;
			labFullName.Text = scriptInformation.FullName;
			labScriptType.Text = cProperty.ScriptType.ToString();
		}

		private void btnOK_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.OK;
		}
	}
}