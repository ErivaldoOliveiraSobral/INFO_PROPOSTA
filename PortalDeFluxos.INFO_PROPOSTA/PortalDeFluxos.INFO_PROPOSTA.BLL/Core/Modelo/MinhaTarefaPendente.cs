using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class MinhasTarefasPendente : BasePainel
    {
        public Int32? IdTarefa { get; set; }
        public Guid? CodigoLista { get; set; }
        public Int32? CodigoItem { get; set; }
        public String NomeSolicitacao { get; set; }
        public String NomeFluxo { get; set; }
        public String NomeSolicitante { get; set; }
        public String NomeEtapa { get; set; }
        public String NomeTarefa { get; set; }
        public String DescricaoUrlItem { get; set; }
        public String TempoDecorrido { get; set; }
        public String DescricaoUrlTarefa { get; set; }
        public String DescricaoUrlDetalhes { get; set; }
        public Boolean Ambiente2007 { get; set; }
        public String NomeGerenteRegiao { get; set; }
        public String NomeDiretorVendas { get; set; }
    }
}
