using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Defs;
using Luban.Server.Common;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string InputDataDir
        {
            get => Properties.Settings.Default.dataDir;
            set
            {
                Properties.Settings.Default.dataDir = value;
                Properties.Settings.Default.Save();
            }
        }

        private void AssistantTab_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private bool HasSetRootDefineFile()
        {
            return !string.IsNullOrWhiteSpace(RootDefineFile) && File.Exists(RootDefineFile);
        }

        private bool TryChooseRootDefineFile(out string rootDefineFile)
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = "xml";
            dialog.Filter = "root file (*.xml)|*.xml";
            dialog.Title = "Select Root Xml File";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                rootDefineFile = dialog.FileName;
                return true;
            }
            rootDefineFile = null;
            return false;
        }

        private bool TryChooseInputDataDir(out string inputDataDir)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "Select Data Dir";
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (dialog.ShowDialog() == DialogResult.OK && Directory.Exists(dialog.SelectedPath))
            {
                inputDataDir = dialog.SelectedPath;
                return true;
            }
            inputDataDir = null;
            //var dialog = new OpenFileDialog();
            //dialog.Title = "Select Data Dir";
            //dialog.CheckFileExists = false;
            //dialog.CheckPathExists = true;
            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    inputDataDir = dialog.FileName;
            //    return true;
            //}
            //inputDataDir = null;
            return false;
        }

        private void BtnChooseRootFileClick(object sender, RibbonControlEventArgs e)
        {
            if (TryChooseRootDefineFile(out var rootDefineFile))
            {
                RootDefineFile = rootDefineFile;
            }
        }

        private void BtnChooseDataDirClick(object sender, RibbonControlEventArgs e)
        {
            if (TryChooseInputDataDir(out var dataDir))
            {
                InputDataDir = dataDir;
            }
        }

        private bool TryGetTableName(out string tableName)
        {
            tableName = "test.TbExcelFromJson";
            return true;
        }

        private void LoadDataToCurrentDoc()
        {
            if (!TryGetTableName(out var tableName))
            {
                MessageBox.Show($"meta行未指定table名");
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    await LoadUtil.LoadTableDataToCurrentWorkSheetAsync(RootDefineFile, InputDataDir, tableName);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, e.Message);
                }
            });

        }

        private bool PromptIgnoreNotSaveData()
        {
            if (HasNotsaveDataInCurrentWorksapce())
            {
                if (MessageBox.Show("有未保存的数据，确定要覆盖吗？", "警告", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        private bool HasNotsaveDataInCurrentWorksapce()
        {
            return false;
        }

        private void BtnLoadDataClick(object sender, RibbonControlEventArgs e)
        {
            if (!HasSetRootDefineFile())
            {
                MessageBox.Show("未设置root定义文件");
                return;
            }
            if (!Directory.Exists(InputDataDir))
            {
                MessageBox.Show("未设置加载目录");
                return;
            }
            if (PromptIgnoreNotSaveData())
            {
                LoadDataToCurrentDoc();
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
