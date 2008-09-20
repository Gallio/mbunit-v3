using System.ComponentModel;
using System.Windows.Forms;

namespace Gallio.Icarus.Controls
{
    class ToolStripMenuItem : System.Windows.Forms.ToolStripMenuItem, IBindableComponent
    {
        private BindingContext bindingContext;
        private ControlBindingsCollection dataBindings;

        [Browsable(false)]
        public BindingContext BindingContext
        {
            get
            {
                if (bindingContext == null)
                    bindingContext = new BindingContext();
                return bindingContext;
            }
            set { bindingContext = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ControlBindingsCollection DataBindings
        {
            get
            {
                if (dataBindings == null)
                    dataBindings = new ControlBindingsCollection(this);
                return dataBindings;
            }
        }
    }
}
