using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

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
            CustomCollectionEditor.MySelectedGridItemChanged += CustomCollectionEditor_MySelectedGridItemChanged;
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
            propertyGrid1.SelectedObject = null;
            propertyGrid1.Refresh();
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
            ExecuteMethod(LoadOrganization);
        }

        private void LoadOrganization()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Work = loadOrganization,
                ProgressChanged = handleProgress,
                PostWorkCallBack = handlePostWork
            });
        }

        private void LoadSolutions()
        {
            if (propertyGrid1.SelectedObject is OrgMetrics metrics && metrics.Solutions == null)
            {
                WorkAsync(new WorkAsyncInfo
                {
                    Work = loadSolutions,
                    AsyncArgument = propertyGrid1.SelectedObject,
                    ProgressChanged = handleProgress,
                    PostWorkCallBack = handlePostWork
                });
            }
        }

        private void LoadUsers()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Work = loadUsers,
                AsyncArgument = propertyGrid1.SelectedObject,
                ProgressChanged = handleProgress,
                PostWorkCallBack = handlePostWork
            });
        }

        private void LoadRoles(User user)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Work = loadRoles,
                AsyncArgument = user ?? propertyGrid1.SelectedObject,
                ProgressChanged = handleProgress,
                PostWorkCallBack = handlePostWork
            });
        }

        private void loadOrganization(BackgroundWorker worker, DoWorkEventArgs args)
        {
            worker.ReportProgress(10, "Loading Organization...");
            var org = Service.RetrieveMultiple(
                new QueryExpression("organization")
                {
                    ColumnSet = new ColumnSet(true)
                })
                .Entities
                .FirstOrDefault();
            worker.ReportProgress(80, "Finalizing");
            args.Result = new OrgMetrics(org, LoadRoles);
        }

        private void loadSolutions(BackgroundWorker worker, DoWorkEventArgs args)
        {
            var metrics = args.Argument as OrgMetrics;
            worker.ReportProgress(20, "Loading Solutions...");
            metrics.SetSolutions(Service.RetrieveMultiple(
                new QueryExpression("solution")
                {
                    ColumnSet = new ColumnSet(true)
                })
                .Entities
                .Select(s => new Solution(s))
                .OrderBy(s => s.UniqueName).ToList());
        }

        private void loadUsers(BackgroundWorker worker, DoWorkEventArgs args)
        {
            var metrics = args.Argument as OrgMetrics;
            worker.ReportProgress(30, "Loading Users...");
            metrics.SetUsers(Service.RetrieveMultiple(
                new QueryExpression("systemuser")
                {
                    ColumnSet = new ColumnSet(true)
                })
                .Entities
                .Select(s => new User(s, LoadRoles))
                .OrderBy(s => s.FullName).ToList());
        }

        private void loadRoles(BackgroundWorker worker, DoWorkEventArgs args)
        {
            var entity = args.Argument as EntityWithRoles;
            worker.ReportProgress(40, "Loading Roles...");
            var roles = new List<Role>(Service.RetrieveMultiple(entity.GetRolesQuery())
                .Entities
                .Select(r => new Role(r))
                .OrderBy(r => r.Name));
            entity.SetRoles(roles);
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
            else
            {
                propertyGrid1.Refresh();
            }
        }

        internal void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.NewSelection.GridItemType == GridItemType.Category && e.NewSelection.Label == "Solutions" && e.NewSelection.Expanded)
            {
                LoadSolutions();
            }
            else if (e.NewSelection?.PropertyDescriptor?.Name == "SolutionsTemp")
            {
                LoadSolutions();
            }
            else if (e.NewSelection?.PropertyDescriptor?.Name == "UsersTemp")
            {
                LoadUsers();
            }
            else if (e.NewSelection?.PropertyDescriptor?.Name == "RolesTemp")
            {
                LoadRoles(null);
            }
        }

        private void CustomCollectionEditor_MySelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.NewSelection?.PropertyDescriptor?.Name == "RolesTemp")
            {
                var user = (sender as PropertyGrid)?.SelectedObject as User;
                LoadRoles(user);
            }
        }

    }
}