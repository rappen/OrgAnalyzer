using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Rappen.XTB.OrgAnalyzer
{
    public class CustomCollectionEditor : CollectionEditor
    {
        public delegate void MySelectedGridItemChangedEventHandler(object sender,
                                       SelectedGridItemChangedEventArgs e);

        public static event MySelectedGridItemChangedEventHandler MySelectedGridItemChanged;

        #region Public Constructors

        public CustomCollectionEditor(Type type) : base(type) { }

        #endregion Public Constructors

        #region Protected Methods

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
                prop.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(collection_SelectedGridItemChanged);
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

        private void collection_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (MySelectedGridItemChanged != null)
            {
                MySelectedGridItemChanged(sender, e);
            }
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
}
