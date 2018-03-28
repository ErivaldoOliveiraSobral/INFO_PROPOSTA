using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    [Serializable]
    public class Campo
    {
        public string Valor { get; set; }
        public string Descricao { get; set; }
        public string Categoria { get; set; }
    }
}
