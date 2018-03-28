using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.Salesforce
{
    [Serializable]
    public class InformacoesSalesforce
    {
        public List<Campo> Oportunidades { get; set; }
        public List<Campo> MediaCombustiveis { get; set; }
        public string RazaoSocial { get; set; }
        public string IBM { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Gt { get; set; }
        public string Gr { get; set; }
        public string FaixaVolume { get; set; }
        public string FaixaMargem { get; set; }
        public string DiretorVendas { get; set; }
        public string PerfilPosto { get; set; }
        public string TipoDaRegiao { get; set; }
        public string MunicipioFoco { get; set; }
    }
}
