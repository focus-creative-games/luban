
namespace LubanAssistant
{
    partial class AssistantTab : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public AssistantTab()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group3 = this.Factory.CreateRibbonGroup();
            this.SetRootFile = this.Factory.CreateRibbonButton();
            this.load = this.Factory.CreateRibbonButton();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.reloadData = this.Factory.CreateRibbonButton();
            this.group2 = this.Factory.CreateRibbonGroup();
            this.saveAll = this.Factory.CreateRibbonButton();
            this.saveSelected = this.Factory.CreateRibbonButton();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tab1.SuspendLayout();
            this.group3.SuspendLayout();
            this.group1.SuspendLayout();
            this.group2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.group3);
            this.tab1.Groups.Add(this.group1);
            this.tab1.Groups.Add(this.group2);
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // group3
            // 
            this.group3.Items.Add(this.SetRootFile);
            this.group3.Items.Add(this.load);
            this.group3.Name = "group3";
            // 
            // SetRootFile
            // 
            this.SetRootFile.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.SetRootFile.Label = "设置Root文件";
            this.SetRootFile.Name = "SetRootFile";
            this.SetRootFile.ShowImage = true;
            this.SetRootFile.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BtnChooseRootFileClick);
            // 
            // load
            // 
            this.load.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.load.Label = "设置配置根目录";
            this.load.Name = "load";
            this.load.ScreenTip = "--input_data_dir 中指定的根目录，而不是当前配置表的input属性指定目录";
            this.load.ShowImage = true;
            this.load.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BtnChooseDataDirClick);
            // 
            // group1
            // 
            this.group1.Items.Add(this.reloadData);
            this.group1.Name = "group1";
            // 
            // reloadData
            // 
            this.reloadData.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.reloadData.Label = "加载数据";
            this.reloadData.Name = "reloadData";
            this.reloadData.ShowImage = true;
            this.reloadData.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BtnLoadDataClick);
            // 
            // group2
            // 
            this.group2.Items.Add(this.saveAll);
            this.group2.Items.Add(this.saveSelected);
            this.group2.Name = "group2";
            // 
            // saveAll
            // 
            this.saveAll.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.saveAll.Label = "保存所有";
            this.saveAll.Name = "saveAll";
            this.saveAll.ShowImage = true;
            this.saveAll.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BtnSaveAllClick);
            // 
            // saveSelected
            // 
            this.saveSelected.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.saveSelected.Label = "保存选中";
            this.saveSelected.Name = "saveSelected";
            this.saveSelected.ShowImage = true;
            this.saveSelected.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BtnSaveSelectedClick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // AssistantTab
            // 
            this.Name = "AssistantTab";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.AssistantTab_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group3.ResumeLayout(false);
            this.group3.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton load;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton saveAll;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group3;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton SetRootFile;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton saveSelected;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton reloadData;
    }

    partial class ThisRibbonCollection
    {
        internal AssistantTab AssistantTab
        {
            get { return this.GetRibbon<AssistantTab>(); }
        }
    }
}
