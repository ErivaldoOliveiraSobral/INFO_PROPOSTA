using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class TemplateProxy : ITemplate
    {
        private Func<Control> _controlDelegate = null;

        public TemplateProxy(Func<Control> controlDelegate)
        {
            _controlDelegate = controlDelegate;
        }

        public void InstantiateIn(Control container)
        {
            Control control = null;

            if (_controlDelegate != null)
            {
                control = _controlDelegate();

                if (control != null)
                    container.Controls.Add(control);
            }
        }
    }
}
