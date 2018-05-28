using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Rappen.XTB.OrgAnalyzer
{
    public class OrgMetrics : EntityWithRoles
    {
        private List<Solution> solutions;
        private List<User> users;

        public OrgMetrics(Entity entity, Action<User> loadRoles) : base(entity, loadRoles)
        {
            SetPropertyVisibility();
        }

        protected override void SetPropertyVisibility()
        {
            base.SetPropertyVisibility();
            setBrowsableProperty("Solutions", solutions != null);
            setBrowsableProperty("SolutionsTemp", solutions == null);
            setBrowsableProperty("Users", users != null);
            setBrowsableProperty("UsersTemp", users == null);
        }

        public void SetSolutions(List<Solution> sols)
        {
            solutions = sols;
            SetPropertyVisibility();
        }

        public void SetUsers(List<User> users)
        {
            this.users = users;
            SetPropertyVisibility();
        }

        public override QueryExpression GetRolesQuery()
        {
            return new QueryExpression("role")
            {
                ColumnSet = new ColumnSet(true)
            };
        }

        [Category("Organization")]
        public string Name { get => getEntityValueStr("name"); }

        [Category("Organization")]
        public override DateTime Created => base.Created;

        [Category("Organization")]
        [DisplayName("Initial Version")]
        [Description("The version of the database when the organization was created.")]
        public Version InitialVersion { get => new Version(getEntityValueStr("initialversion")); }

        [Category("Organization")]
        [DisplayName("Reporting Group Name")]
        public string ReportingGroupName { get => getEntityValueStr("reportinggroupname"); }

        [Category("Organization")]
        [DisplayName("SQL Access Group Name")]
        public string SQLAccesGroupName { get => getEntityValueStr("sqlaccessgroupname"); }

        [Category("Solutions")]
        [Browsable(false)]
        [Editor(typeof(CustomCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<Solution> Solutions { get => solutions; }

        [Category("Solutions")]
        [Browsable(true)]
        [DisplayName("Solutions")]
        public string SolutionsTemp { get => "Click to load"; }

        [Category("Solutions")]
        [DisplayName("Solution Count (total)")]
        public int? SolutionCount { get => solutions?.Count; }

        [Category("Solutions")]
        [DisplayName("Solution Count (unmanaged)")]
        [Description("Number of solutions with flag IsManaged = false.")]
        public int? SolutionCountUnmanaged { get => solutions?.Where(s => !s.IsManaged)?.Count(); }

        [Category("Solutions")]
        [DisplayName("Default Solution")]
        public Solution DefaultSolution { get => solutions?.Where(s => s.UniqueName == "Default")?.FirstOrDefault(); }

        [Category("Users")]
        [Browsable(false)]
        [Editor(typeof(CustomCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<User> Users { get => users; }

        [Category("Users")]
        [Browsable(true)]
        [DisplayName("Users")]
        public string UsersTemp { get => "Click to load"; }

        [Category("Users")]
        [DisplayName("User Count (total)")]
        public int? UserCount { get => users?.Count; }

        [Category("Users")]
        [DisplayName("User Count (active)")]
        [Description("Number of active users.")]
        public int? UserCountActive { get => users?.Where(u => !u.IsActive)?.Count(); }

        [Browsable(false)]
        public override DateTime Updated => base.Updated;

        public SomeType TheType { get; set; }
    }

    public enum SomeType
    {
        First,
        Second,
        Third
    }
}