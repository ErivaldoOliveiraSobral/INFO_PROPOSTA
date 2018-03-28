using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.AnexoFacilAPI
{
	public class PropostaApiModelCollection
	{
		public List<PropostaApiModel> PropostaApi { get; set; }
	}

	public class PropostaApiModel
	{
		public int Id { get; set; }
		public int CodigoItem { get; set; }
		public Guid CodigoLista { get; set; }
		public string Titulo { get; set; }
		public string IBM { get; set; }
		public string RazaoSocial { get; set; }
		public string CNPJ { get; set; }
		public string Email { get; set; }
		public string Link { get; set; }
		public string LoginUsuarioPerfil { get; set; }
		public int CodigoIntegracao { get; set; }
		public bool PropostaCombo { get; set; }
		public EstruturaComercialApiModel EstruturaComercial { get; set; }
	}

	public class EstruturaComercialApiModel
	{
		public string GT { get; set; }
		public string GR { get; set; }
		public string Diretor { get; set; }
		public string GDR { get; set; }
		public string CDR { get; set; }
	}
}
