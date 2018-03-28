using Microsoft.SharePoint;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace PortalDeFluxos.Core.Wrapper
{
    public class ControleCustomGrid : ControleBase
    {
        public ControleCustomGrid(Control control)
        {
            _controle = control;
        }

        public GridConfigurationCore GetConfiguracaoAtual()
        {
            object value = _controle.GetType().GetProperty("GetConfiguracaoAtual").GetValue(_controle, null);
            if (value is String)
                return Serializacao.DeserializeFromJson<GridConfigurationCore>(value.ToString());
            else
                return null;
        }

        public void LoadControlValues(GridConfigurationCore configuracao)
        {
            _controle.GetType().GetMethod("LoadControlValues").Invoke(_controle, new object[] { Serializacao.SerializeToJson(configuracao) });
        }

        public String GetControlId()
        {
            object value = _controle.GetType().GetProperty("GetControlId").GetValue(_controle, null);
            if (value is String)
                return (String)value;
            else
                return String.Empty;
        }
    }
}
