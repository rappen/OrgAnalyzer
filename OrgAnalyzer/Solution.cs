using Microsoft.Xrm.Sdk;
using System;
using System.ComponentModel;

namespace Rappen.XTB.OrgAnalyzer
{
    public class Solution : EntityProxy
    {
        private string dummy;

        public Solution(Entity solution) : base(solution) { }

        [Category("Identity")]
        [DisplayName("Unique Name")]
        public string UniqueName { get => getEntityValueStr("uniquename"); set => dummy = value; }

        [Category("Identity")]
        [DisplayName("Friendly Name")]
        public string FriendlyName { get => getEntityValueStr("friendlyname"); }

        [Category("Identity")]
        public string Version { get => getEntityValueStr("version"); }

        [Category("Identity")]
        [DisplayName("Solution Package Version")]
        public string PackageVersion { get => getEntityValueStr("solutionpackageversion"); }

        [Category("Properties")]
        [DisplayName("Is Managed")]
        public bool IsManaged { get => bool.Parse(getEntityValueStr("ismanaged", "false")); }

        [DisplayName("Installed")]
        public override DateTime Created => base.Created;

        public override string ToString()
        {
            return UniqueName;
        }
    }
}