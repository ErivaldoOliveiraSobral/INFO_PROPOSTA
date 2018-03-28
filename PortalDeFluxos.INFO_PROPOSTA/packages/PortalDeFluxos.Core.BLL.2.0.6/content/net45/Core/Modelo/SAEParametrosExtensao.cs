using Microsoft.SharePoint.Client;
using System;
using PortalDeFluxos.Core.BLL.Utilitario;
using Microsoft.SharePoint.Client.Utilities;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public partial class SAEParametros
    {
		/// <summary>
		/// Index da linha
		/// Informação obtida da propriedade celula
		/// </summary>
        public Int32 Row { get; set; }
		/// <summary>
		/// Nome da Coluna EX: A, AA e etc
		/// Informação obtida da propriedade celula
		/// </summary>
		public String Column { get; set; }
		/// <summary>
		/// Ordem da coluna - Respeita ordenação excel
		/// Informação obtida da propriedade celula
		/// </summary>
		public Int32 IndexColumn { get; set; }
		/// <summary>
		/// Se o input precisa ser processado
		/// </summary>
		public Boolean ProcessInput { get; set; }
		/// <summary>
		/// Valor formatado - que será incluido no excel
		/// </summary>
		public String InputValue { get; set; }
    }
}
