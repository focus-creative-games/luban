using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LubanAssistant
{
    public partial class AssistantTab
    {
        private string RootDefineFile
        {
            get => Properties.Settings.Default.rootDefineFile;
            set
            {
                Properties.Settings.Default.rootDefineFile = value;
                Properties.Settings.Default.Save();
            }
        }

        private void AssistantTab_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private bool CheckChooseRootDefineFile()
        {
            if (string.IsNullOrWhiteSpace(RootDefineFile) || !File.Exists(RootDefineFile))
            {
                if (TryChooseRootDefineFile(out var rootDefineFile))
                {
                    RootDefineFile = rootDefineFile;
                    return true;
                }
            }
            return false;
        }

        private bool TryChooseRootDefineFile(out string rootDefineFile)
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = "xml";
            dialog.Filter = "root file (*.xml)|*.xml";
            dialog.Title = "Choose Root Xml File";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                rootDefineFile = dialog.FileName;
                return true;
            }
            rootDefineFile = null;
            return false;
        }

        private void BtnChooseRootFileClick(object sender, RibbonControlEventArgs e)
        {
            if (TryChooseRootDefineFile(out var rootDefineFile))
            {
                RootDefineFile = rootDefineFile;
            }
        }

        private void BtnLoadClick(object sender, RibbonControlEventArgs e)
        {
            if (CheckChooseRootDefineFile())
            {
                MessageBox.Show("load");
            }
        }

        private void BtnSaveAllClick(object sender, RibbonControlEventArgs e)
        {
            MessageBox.Show("点击save");
        }

        private void BtnSaveSelectedClick(object sender, RibbonControlEventArgs e)
        {
            MessageBox.Show("点击save");
        }
    }
}
