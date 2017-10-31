using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.ComponentModel;

namespace Rappen.XTB.OrgAnalyzer
{
    public class User : EntityProxy
    {
        private string dummy;
        private List<Role> roles;

        public User(Entity user, List<Role> roles) : base(user)
        {
            SetRoles(roles);
        }

        public void SetRoles(List<Role> roles)
        {
            this.roles = roles;
            setBrowsableProperty("Roles", this.roles != null);
            setBrowsableProperty("RolesTemp", this.roles == null);
        }

        [Category("Identification")]
        public string FullName { get => getEntityValueStr("fullname"); set => dummy = value; }

        [Category("Identification")]
        public string Email { get => getEntityValueStr("internalemailaddress"); }

        [Category("Identification")]
        public string Domainname { get => getEntityValueStr("domainname"); }

        [Category("Properties")]
        [DisplayName("Is Active")]
        public bool IsActive { get => GetAttributeValue<OptionSetValue>("statecode")?.Value == 0; }

        [Category("Roles")]
        [Browsable(false)]
        [Editor(typeof(CustomCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<Role> Roles { get => roles; }

        [Category("Roles")]
        [Browsable(true)]
        [DisplayName("Roles")]
        public string RolesTemp { get => "Click to load"; }

        public override string ToString()
        {
            return FullName;
        }
    }
}
