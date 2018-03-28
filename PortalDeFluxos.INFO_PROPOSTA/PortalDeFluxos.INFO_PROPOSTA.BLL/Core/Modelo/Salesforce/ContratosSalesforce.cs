using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.Salesforce
{
    [Serializable]
    public class ContratosSalesforce
    {
        public int NumeroPropostaSalesForce { get; set; }

        public Nullable<System.DateTime> CONT_VencimentoContratoPrazo { get; set; }

        public Nullable<System.DateTime> CONT_VencimentoContratoVolume { get; set; }

        public string CONT_PossuiCasoBase { get; set; }

        public Nullable<System.Decimal> CONT_ConcessaoFPAnterior { get; set; }

        public string CONT_TipoContrato { get; set; }

        public string CONT_TemDenuncia { get; set; }

        public Nullable<System.Decimal> CONT_VolumeContratadoGasolina { get; set; }

        public Nullable<System.Decimal> CONT_VolumeRealGasolina { get; set; }

        public Nullable<System.Decimal> CONT_VolumePendenteGasolina { get; set; }

        public Nullable<System.Decimal> CONT_VolumeTotalNovoContratoGasolina { get; set; }

        public Nullable<System.Decimal> CONT_VolumeContratadoEtanol { get; set; }

        public Nullable<System.Decimal> CONT_VolumeRealEtanol { get; set; }

        public Nullable<System.Decimal> CONT_VolumePendenteEtanol { get; set; }

        public Nullable<System.Decimal> CONT_VolumeTotalNovoContratoEtanol { get; set; }

        public Nullable<System.Decimal> CONT_VolumeContratadoDiesel { get; set; }

        public Nullable<System.Decimal> CONT_VolumeRealDiesel { get; set; }

        public Nullable<System.Decimal> CONT_VolumePendenteDiesel { get; set; }

        public Nullable<System.Decimal> CONT_VolumeTotalNovoContratoDiesel { get; set; }

        public Nullable<System.Decimal> CONT_VolumeContratadoTotal { get; set; }

        public Nullable<System.Decimal> CONT_VolumeRealTotal { get; set; }

        public Nullable<System.Decimal> CONT_VolumePendenteTotal { get; set; }

        public Nullable<System.Decimal> CONT_VolumeTotalNovoContratoTotal { get; set; }
    }
}
