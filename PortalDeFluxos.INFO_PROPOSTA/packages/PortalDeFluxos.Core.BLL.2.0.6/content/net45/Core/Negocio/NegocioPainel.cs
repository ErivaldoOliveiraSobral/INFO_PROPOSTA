using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioPainel
    {
        public static List<Solicitacao> ConsultarSolicitacao(
           String filtro,
           String filtro2,
           Int32 indicePagina,
           Int32 registrosPorPagina,
           String ordernarPor,
           String ordernarDirecao,
           StatusFluxo statusFluxo,
           String login,
           Boolean isAdmin
            )
        {
            return DadosPainel.ConsultarSolicitacao(
                filtro,
                filtro2,
                indicePagina,
                registrosPorPagina,
                ordernarPor,
                ordernarDirecao,
                statusFluxo,
                login,
                isAdmin);
        }

        public static List<MinhasTarefasPendente> ConsultarMinhasTarefasPendente(
            String filtro,
            Int32 indicePagina,
            Int32 registrosPorPagina,
            String ordernarPor,
            String ordernarDirecao,
            String login
            )
        {
            return DadosPainel.ConsultarMinhasTarefasPendente(
                filtro,
                indicePagina,
                registrosPorPagina,
                ordernarPor,
                ordernarDirecao,
                login);
        }

        public static List<TarefasRealizadas> ConsultarTarefasRealizadas(
            String filtro,
            Guid codigoLista,
            Int32 codigoItem,
            Int32 indicePagina,
            Int32 registrosPorPagina,
            String ordernarPor,
            String ordernarDirecao)
        {
            return DadosPainel.ConsultarTarefasRealizadas(
                filtro,
                codigoLista,
                codigoItem,
                indicePagina,
                registrosPorPagina,
                ordernarPor,
                ordernarDirecao
                );
        }


        public static List<TarefasPendentes> ConsultarTarefasPendentes(
                   String filtro,
                   Guid codigoLista,
                   Int32 codigoItem,
                   Int32 indicePagina,
                   Int32 registrosPorPagina,
                   String ordernarPor,
                   String ordernarDirecao)
        {
            return DadosPainel.ConsultarTarefasPendentes(
                filtro,
                codigoLista,
                codigoItem,
                indicePagina,
                registrosPorPagina,
                ordernarPor,
                ordernarDirecao
                );
        }

        public static String ConsultarTituloDetalheSolicitacao(
                   Guid codigoLista,
                   Int32 codigoItem)
        {
            return DadosPainel.ConsultarTituloDetalheSolicitacao(
                codigoLista,
                codigoItem);
        }
    }
}
