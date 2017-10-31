using Microsoft.Xrm.Sdk;
using System.ComponentModel;

namespace Rappen.XTB.OrgAnalyzer
{
    public class Role : EntityProxy
    {
        private string dummy;

        public Role(Entity entity) : base(entity) { }

        [Category("Identity")]
        public string Name { get => getEntityValueStr("name"); set => dummy = value; }

        [Category("Properties")]
        public bool Customizable { get => entity.GetAttributeValue<BooleanManagedProperty>("iscustomizable").Value; }

        public override string ToString()
        {
            return Name;
        }
    }
}