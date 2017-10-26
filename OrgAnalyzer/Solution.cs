using Microsoft.Xrm.Sdk;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Windows.Forms;

namespace Rappen.XTB.OrgAnalyzer
{
    public class Solution : EntityProxy
    {
        #region Public Constructors

        public Solution(Entity solution)
        {
            this.entity = solution;
        }

        #endregion Public Constructors

        #region Public Properties

        [Category("Identity")]
        [DisplayName("Unique Name")]
        public string UniqueName { get => getEntityValueStr("uniquename"); set => entity["uniquename"] = value; }

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

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return UniqueName;
        }

        #endregion Public Methods
    }

    public class SolutionCollectionEditor : CollectionEditor
    {
        #region Public Constructors

        public SolutionCollectionEditor(Type type) : base(type)
        {
        }

        #endregion Public Constructors

        #region Protected Methods

        // Override this method in order to access the containing user controls
        // from the default Collection Editor form or to add new ones...
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm collectionForm = base.CreateCollectionForm();
            Form frmCollectionEditorForm = collectionForm as Form;
            RemoveControl(frmCollectionEditorForm, "addButton");
            RemoveControl(frmCollectionEditorForm, "removeButton");
            RemoveControl(frmCollectionEditorForm, "okButton");
            RemoveControl(frmCollectionEditorForm, "upButton");
            RemoveControl(frmCollectionEditorForm, "downButton");
            var cancel = GetControl(frmCollectionEditorForm, "cancelButton");
            if (cancel != null)
            {
                cancel.Text = "Close";
            }
            var prop = GetControl(frmCollectionEditorForm, "propertyBrowser") as PropertyGrid;
            if (prop != null)
            {
                prop.ToolbarVisible = false;
                prop.PropertySort = PropertySort.Categorized;
            }
            collectionForm.Width = 800;
            collectionForm.Height = 600;
            var list = GetControl(frmCollectionEditorForm, "listbox");
            if (list != null)
            {
                list.Width = 300;
            }
            return collectionForm;
        }

        #endregion Protected Methods

        #region Private Methods

        private static Control GetControl(Form form, string control)
        {
            var cntr = form.Controls.Find(control, true);
            if (cntr.Length > 0)
            {
                return cntr[0];
            }
            return null;
        }

        private static void RemoveControl(Form form, string control)
        {
            var cntr = GetControl(form, control);
            if (cntr != null && cntr.Parent != null)
            {
                cntr.Parent.Controls.Remove(cntr);
            }
        }

        #endregion Private Methods
    }

    public class SolutionConverter : ExpandableObjectConverter
    {
        #region Public Methods

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Solution))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is Solution)
            {
                var sp = (Solution)value;
                return $"{sp.FriendlyName} {sp.Version}";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Public Methods
    }
}