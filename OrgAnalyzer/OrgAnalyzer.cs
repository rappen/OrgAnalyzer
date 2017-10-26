using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using Microsoft.Xrm.Sdk.Query;

namespace Rappen.XTB.OrgAnalyzer
{
    public partial class OrgAnalyzer : PluginControlBase, IGitHubPlugin
    {
        private Settings mySettings;

        public string RepositoryName => "OrgAnalyzer";

        public string UserName => "Rappen";

        public OrgAnalyzer()
        {
            InitializeComponent();
        }

        private void OrgAnalyzer_Load(object sender, EventArgs e)
        {
            //ShowInfoNotification("This is a notification that can lead to XrmToolBox repository", new Uri("http://github.com/MscrmTools/XrmToolBox"));

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();
                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrgAnalyzer_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrgAnalyzer_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            mySettings.LastUsedOrganizationWebappUrl = e.ConnectionDetail.WebApplicationUrl;
            LogInfo("Connection has changed to: {0}", e.ConnectionDetail.WebApplicationUrl);
        }

        internal void UpdateUI(Action action)
        {
            MethodInvoker mi = delegate
            {
                action();
            };
            if (InvokeRequired)
            {
                Invoke(mi);
            }
            else
            {
                mi();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ExecuteMethod(Analyze);
        }

        private void Analyze()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Analyzing...",
                Work = handleWork,
                ProgressChanged = handleProgress,
                PostWorkCallBack = handlePostWork
            });
        }

        private void handleWork(BackgroundWorker worker, DoWorkEventArgs args)
        {
            worker.ReportProgress(30, "Organization");
            var org = Service.RetrieveMultiple(
                new QueryExpression("organization")
                {
                    ColumnSet = new ColumnSet(true)
                })
                .Entities
                .FirstOrDefault();
            worker.ReportProgress(60, "Solutions");
            var sols = Service.RetrieveMultiple(
                new QueryExpression("solution")
                {
                    ColumnSet = new ColumnSet(true)
                })
                .Entities
                .Select(s => new Solution(s))
                .OrderBy(s => s.UniqueName).ToList();
            worker.ReportProgress(80, "Finalizing");
            args.Result = new OrgMetrics(org, sols);
        }

        private void handleProgress(ProgressChangedEventArgs args)
        {
            SetWorkingMessage(args.UserState?.ToString());
        }

        private void handlePostWork(RunWorkerCompletedEventArgs args)
        {
            if (args.Error != null)
            {
                MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var result = args.Result as OrgMetrics;
            if (result != null)
            {
                propertyGrid1.SelectedObject = result;
            }
        }
    }
}