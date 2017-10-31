using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Rappen.XTB.OrgAnalyzer
{
    public abstract class EntityWithRoles : EntityProxy
    {
        private List<Role> roles;
        private Action<User> loadRoles;

        public EntityWithRoles(Entity entity, Action<User> loadRoles) : base(entity)
        {
            this.loadRoles = loadRoles;
        }

        public abstract QueryExpression GetRolesQuery();

        protected virtual void SetPropertyVisibility()
        {
            setBrowsableProperty("Roles", roles != null);
            setBrowsableProperty("RolesTemp", roles == null);
        }

        private List<Role> getRoles()
        {
            if (roles == null)
            {
                loadRoles(this as User);
            }
            return roles;
        }

        public void SetRoles(List<Role> roles)
        {
            this.roles = roles;
            SetPropertyVisibility();
        }

        [Category("Roles")]
        [Browsable(false)]
        [Editor(typeof(CustomCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<Role> Roles { get => getRoles(); }

        [Category("Roles")]
        [Browsable(true)]
        [DisplayName("Roles")]
        public string RolesTemp { get => "Click to load"; }

        [Category("Roles")]
        [DisplayName("Role Count (total)")]
        public int? RoleCount { get => roles?.Count; }

        [Category("Roles")]
        [DisplayName("Role Count (customizable)")]
        [Description("Number of cusomizable roles.")]
        public int? RoleCountCustomizable { get => roles?.Where(r => !r.Customizable)?.Count(); }
    }
}
