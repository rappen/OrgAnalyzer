using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ComponentModel;

namespace Rappen.XTB.OrgAnalyzer
{
    public class User : EntityWithRoles
    {
        private string dummy;

        public User(Entity entity, Action<User> loadRoles) : base(entity, loadRoles) { }

        [Category("Identity")]
        public string FullName { get => getEntityValueStr("fullname"); set => dummy = value; }

        [Category("Identity")]
        public string Email { get => getEntityValueStr("internalemailaddress"); }

        [Category("Identity")]
        public string Domainname { get => getEntityValueStr("domainname"); }

        [Category("Properties")]
        [DisplayName("Is Active")]
        public bool IsActive { get => GetAttributeValue<OptionSetValue>("statecode")?.Value == 0; }

        public override string ToString()
        {
            return FullName;
        }

        public override QueryExpression GetRolesQuery()
        {
            var qx = new QueryExpression("role")
            {
                ColumnSet = new ColumnSet(true)
            };
            var mm = qx.AddLink("systemuserroles", "roleid", "roleid");
            mm.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, Id);
            return qx;
        }
    }
}
