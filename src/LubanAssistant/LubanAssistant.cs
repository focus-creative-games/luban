using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Tools.Ribbon;
using Luban.Common.Utils;

namespace LubanAssistant
{
    public partial class LubanAssistant
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            LogUtil.InitSimpleNLogConfigure(NLog.LogLevel.Info);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        //protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        //{
        //    return new ToolTab();
        //}


        protected override IRibbonExtension[] CreateRibbonObjects()
        {
            return new IRibbonExtension[] { new AssistantTab() };
        }

        #region VSTO 生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
