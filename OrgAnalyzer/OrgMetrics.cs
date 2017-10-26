using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Rappen.XTB.OrgAnalyzer
{
    public class OrgMetrics : EntityProxy
    {
        public OrgMetrics(Entity org, List<Solution> sols)
        {
            this.entity = org;
            this.Solutions = sols;
        }

        [Category("Solutions")]
        [Editor(typeof(SolutionCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<Solution> Solutions { get; }

        [Category("Organization")]
        public string Name { get => getEntityValueStr("name"); }

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
        [DisplayName("Solution Count (total)")]
        public int SolutionCount { get => Solutions.Count; }

        [Category("Solutions")]
        [DisplayName("Solution Count (unmanaged)")]
        [Description("Number of solutions with flag IsManaged = false.")]
        public int SolutionCountUnmanaged { get => Solutions.Where(s => !s.IsManaged).Count(); }

        [Category("Solutions")]
        [DisplayName("Default Solution")]
        [TypeConverter(typeof(SolutionConverter))]
        public Solution DefaultSolution { get => Solutions.Where(s => s.UniqueName == "Default").FirstOrDefault(); }

        [Browsable(false)]
        public override DateTime Updated => base.Updated;
    }
}