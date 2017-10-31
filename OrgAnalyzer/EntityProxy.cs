using Microsoft.Xrm.Sdk;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Rappen.XTB.OrgAnalyzer
{
    public class EntityProxy
    {
        protected Entity entity;

        public EntityProxy(Entity entity)
        {
            this.entity = entity;
        }

        protected void setBrowsableProperty(string strPropertyName, bool bIsBrowsable)
        {
            // Get the Descriptor's Properties
            PropertyDescriptor theDescriptor = TypeDescriptor.GetProperties(this.GetType())[strPropertyName];

            // Get the Descriptor's "Browsable" Attribute
            BrowsableAttribute theDescriptorBrowsableAttribute = (BrowsableAttribute)theDescriptor.Attributes[typeof(BrowsableAttribute)];
            FieldInfo isBrowsable = theDescriptorBrowsableAttribute.GetType().GetField("Browsable", BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Instance);

            // Set the Descriptor's "Browsable" Attribute
            isBrowsable.SetValue(theDescriptorBrowsableAttribute, bIsBrowsable);
        }

        protected string getEntityValueStr(string attribute, string defaultvalue = null)
        {
            if (entity != null && entity.Contains(attribute))
            {
                return entity[attribute].ToString();
            }
            return defaultvalue;
        }

        [Category("Identity")]
        public virtual Guid Id { get => entity.Id; }

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