using Microsoft.Xrm.Sdk;
using System;
using System.ComponentModel;

namespace Rappen.XTB.OrgAnalyzer
{
    public class EntityProxy
    {
        protected Entity entity;

        protected string getEntityValueStr(string attribute, string defaultvalue = null)
        {
            if (entity != null && entity.Contains(attribute))
            {
                return entity[attribute].ToString();
            }
            return defaultvalue;
        }

        [Category("Timestamps")]
        public virtual DateTime Created { get => entity.GetAttributeValue<DateTime>("createdon"); }

        [Category("Timestamps")]
        public virtual DateTime Updated { get => entity.GetAttributeValue<DateTime>("updatedon"); }

        public T GetAttributeValue<T>(string attributeLogicalName)
        {
            return entity.GetAttributeValue<T>(attributeLogicalName);
        }
    }
}