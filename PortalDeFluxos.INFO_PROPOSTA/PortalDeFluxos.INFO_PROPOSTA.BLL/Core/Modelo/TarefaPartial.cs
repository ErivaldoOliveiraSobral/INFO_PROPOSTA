using PortalDeFluxos.Core.BLL.Negocio;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public partial class Tarefa 
    {
		private bool? _aprovacaoEmail;

		private InstanciaFluxo _instanciaFluxo = null;

		private ListaSP_RaizenConfiguracoesDeFluxo _configuracao = null;
        /// <summary>Carrega a configuração da tarefa</summary>
        public ListaSP_RaizenConfiguracoesDeFluxo Configuracao
        { 
            get
            {
                if (_configuracao == null && this.CodigoConfiguracao > 0 && this.CodigoConfiguracao != null)
                    _configuracao = new ListaSP_RaizenConfiguracoesDeFluxo().Obter((int)this.CodigoConfiguracao);
                return _configuracao;
            }
        }

		public InstanciaFluxo InstanciaAtual
		{
			get
			{
				if (_instanciaFluxo == null && this.IdInstanciaFluxo > 0)
					_instanciaFluxo = new InstanciaFluxo().Obter(this.IdInstanciaFluxo);
				return _instanciaFluxo;
			}
		}


		public bool AprovacaoEmail
		{
			get
			{
				if (_aprovacaoEmail == null)
					_aprovacaoEmail = this.Configuracao.AprovacaoEmail != null && (bool)this.Configuracao.AprovacaoEmail ? 
						true : NegocioComum.ExibirAnexoFacil(this.InstanciaAtual.CodigoItem, this.InstanciaAtual.CodigoLista);

				return (bool)_aprovacaoEmail;
			}
		}
    }
}
