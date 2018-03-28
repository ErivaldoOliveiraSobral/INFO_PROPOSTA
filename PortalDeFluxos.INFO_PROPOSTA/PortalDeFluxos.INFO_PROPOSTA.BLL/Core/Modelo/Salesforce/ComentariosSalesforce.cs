using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.Salesforce
{
    [Serializable]
    public class ComentariosSalesforce
    {
        public int NumeroPropostaSalesForce { get; set; }

        public string COME_DescricaoCasoBase { get; set; }

        public string COME_DescricaoCasoInvestimento { get; set; }

        public string COME_DescricaoSensibilidade1 { get; set; }

        public string COME_DescricaoSensibilidade2 { get; set; }

        public string COME_DescricaoCAPEX { get; set; }

        public string COME_InformacaoCompeticao { get; set; }

        public string COME_InformacaoRedeShell { get; set; }

        public string COME_DescricaoContratos { get; set; }

        public string COME_DescricaoLojas { get; set; }

        public string COME_DescricaoAtivos { get; set; }

        public string COME_DescricaoMeioAmbiente { get; set; }

        public string COME_DescricaoCredito { get; set; }

        public string COME_DescricaoRelacionamento { get; set; }

        public string COME_LicencaOperacao { get; set; }

        public string COME_Permits { get; set; }

        public Nullable<System.Int32> COME_NumeroTanques { get; set; }

        public Nullable<System.Int32> COME_CapacidadeTanques { get; set; }

        public Nullable<System.Int32> COME_NumeroBombas { get; set; }

        public Nullable<System.Int32> COME_NumeroBicos { get; set; }

        public string COME_Layout { get; set; }

        public string COME_ClassificacaoCidade { get; set; }
    }
}
