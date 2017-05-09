using System;
using System.Windows.Forms;
using System.Collections.Generic;
using WeifenLuo.WinFormsUI.Docking;
using Zeghs.Scripts;
using Zeghs.Managers;
using Zeghs.Informations;

namespace Zeghs.Forms {
	internal partial class frmScriptViewer : DockContent {
		internal frmScriptViewer() {
			InitializeComponent();
		}

		private void frmScriptViewer_Load(object sender, EventArgs e) {
			List<ScriptInformation> cScripts = ScriptManager.Manager.Scripts;

			TreeNode cModuleItem = treeView.Nodes["treeItem_Module"];
			TreeNode cSignalItem = cModuleItem.Nodes["treeItem_Signal"];
			TreeNode cScriptItem = cModuleItem.Nodes["treeItem_Script"];
			
			int iCount = cScripts.Count;
			for (int i = 0; i < iCount; i++) {
				ScriptInformation cScriptInfo = cScripts[i];
				TreeNode cNode = new TreeNode();
				cNode.ImageIndex = 1;
				cNode.SelectedImageIndex = 1;
				cNode.Tag = cScriptInfo;
				cNode.Name = cScriptInfo.FullName;
				cNode.Text = cScriptInfo.Property.Comment;

				ScriptType cType = cScriptInfo.Property.ScriptType;
				switch(cType) {
					case ScriptType.Signal:
						cSignalItem.Nodes.Add(cNode);
						break;
					case ScriptType.Script:
						cScriptItem.Nodes.Add(cNode);
						break;
				}
			}

			cModuleItem.Expand();
		}

		private void frmScriptViewer_Resize(object sender, EventArgs e) {
			this.treeView.Top = 21;
			this.treeView.Height = this.ClientSize.Height - treeView.Top - 1;
		}

		private void treeView_DoubleClick(object sender, EventArgs e) {
			TreeNode cSelectedNode = treeView.SelectedNode;
			if (cSelectedNode != null) {
				ScriptInformation cInfo = cSelectedNode.Tag as ScriptInformation;
				if (cInfo != null) {
					frmScriptProperty frmScriptProperty  = new Forms.frmScriptProperty(cInfo);
					frmScriptProperty.ShowDialog();
				}
			}
		}

		private void treeView_MouseMove(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				TreeNode cNode = treeView.SelectedNode;
				object cScriptData = cNode.Tag;
				if (cScriptData != null) {
					DataObject cData = new DataObject("__script", cScriptData);
					treeView.DoDragDrop(cData, DragDropEffects.Move);
				}
			}
		}
	}
}