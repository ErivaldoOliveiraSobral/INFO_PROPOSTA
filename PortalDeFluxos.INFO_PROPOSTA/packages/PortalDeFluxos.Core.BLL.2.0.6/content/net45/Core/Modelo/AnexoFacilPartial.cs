using PortalDeFluxos.Core.BLL.Negocio;
using System.Linq;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public partial class AnexoFacil
    {
        private AnexoFacilIntegracao _anexoFacilIntegracao = null;
        /// <summary>Carrega a configuração da tarefa</summary>
        public AnexoFacilIntegracao AnexoFacilIntegracao
        {
            get
            {
                if (_anexoFacilIntegracao == null && this.CodigoIntegracao != null)
                    _anexoFacilIntegracao = new AnexoFacilIntegracao().Consultar(_ => _.CodigoIntegracao == this.CodigoIntegracao).FirstOrDefault();
                return _anexoFacilIntegracao;
            }
        }
    }
}
