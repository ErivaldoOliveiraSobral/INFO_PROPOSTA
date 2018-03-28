using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class TarefasRealizadas : BasePainel
    {
        public Int32? IdTarefa { get; set; }
        public Guid? CodigoLista { get; set; }
        public Int32? CodigoItem { get; set; }
        public String NomeSolicitacao { get; set; }
        public String NomeFluxo { get; set; }
        public String NomeCompletadoPor { get; set; }
        public String NomeEtapa { get; set; }
        public String NomeTarefa { get; set; }
        public String DescricaoAcaoEfetuada { get; set; }
        public String ComentarioAprovacao { get; set; }
        public String DataFinalizado { get; set; }
    }
}
