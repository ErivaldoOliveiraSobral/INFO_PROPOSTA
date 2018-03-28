using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioDocumento
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTipoProposta"></param>
        /// <param name="agrupador"></param>
        /// <returns></returns>
        public static List<Documento> ObterDocumentos(int idTipoProposta, String agrupador)
        {
            return DadosDocumento.ObterDocumentos(idTipoProposta, agrupador);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTipoProposta"></param>
        /// <param name="agrupador"></param>
        /// <returns></returns>
        public static List<TipoPropostaDocumento> ObterDocumentosProposta(int idItem, int idTipoProposta, String agrupador)
        {
            return DadosDocumento.ObterDocumentosProposta(idItem, idTipoProposta, agrupador);
        }
    }
}
