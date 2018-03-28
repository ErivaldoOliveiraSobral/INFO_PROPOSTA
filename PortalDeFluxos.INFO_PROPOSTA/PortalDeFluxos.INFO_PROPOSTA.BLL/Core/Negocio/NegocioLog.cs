using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using Iteris;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioLog
    {
        public static void Inserir(this Log item)
        {
            //Efetua a inclusão do log no banco
            Inserir(item, null);
        }

        public static void Inserir(this Log item, Exception ex)
        {
            Inserir(item, item.DescricaoOrigem ?? "Não definido", item.NomeProcesso ?? "PortalDeFluxos.Core", ex);
        }

        public static void Inserir(this Log item, Origem origem, String processo,String mensagemExtra, Exception ex)
        {
            item.DescricaoOrigem = origem.GetTitle();
            item.NomeProcesso = processo.Length > 200 ? processo.Substring(0, 200) : processo;

            //Se o erro estiver como False, preenche se possuir exception
            if (!item.Erro)
                item.Erro = ex != null;  /* Define se é erro */

            if (ex != null)
            {
                item.DescricaoMensagem = BuscarMensagemErro(ex);
                item.DescricaoDetalhe = item.Contexto != null ?
                                         String.Concat("URL: ", item.Contexto.Url, Environment.NewLine) +
                                          BuscarPilhaErro(ex) + mensagemExtra :
                                         String.Empty +
                                          BuscarPilhaErro(ex) + mensagemExtra;
            }

            //A descrição não pode ser nula
            if (String.IsNullOrWhiteSpace(item.DescricaoMensagem))
                item.DescricaoMensagem = "Descrição não informada";

            //Efetua a inclusão do log no banco
            NegocioComum.Inserir(item);

        }

        public static void Inserir(this Log item, Origem origem, String processo, Exception ex)
        {
            item.DescricaoOrigem = origem.GetTitle();
            item.NomeProcesso = processo.Length > 200 ? processo.Substring(0, 200) : processo;

            //Se o erro estiver como False, preenche se possuir exception
            if (!item.Erro)
                item.Erro = ex != null;  /* Define se é erro */

            if (ex != null)
            {
                item.DescricaoMensagem = BuscarMensagemErro(ex);
                item.DescricaoDetalhe = item.Contexto != null ?
                                         String.Concat("URL: ", item.Contexto.Url, Environment.NewLine) +
                                          BuscarPilhaErro(ex) :
                                         String.Empty +
                                          BuscarPilhaErro(ex);
            }

            //A descrição não pode ser nula
            if (String.IsNullOrWhiteSpace(item.DescricaoMensagem))
                item.DescricaoMensagem = "Descrição não informada";

            //Efetua a inclusão do log no banco
            NegocioComum.Inserir(item);
        }

        public static void Inserir(this Log item, String origem, String processo, Exception ex)
        {
            item.DescricaoOrigem = origem;
            item.NomeProcesso = processo.Length > 200 ? processo.Substring(0, 200) : processo;

            //Se o erro estiver como False, preenche se possuir exception
            if (!item.Erro)
                item.Erro = ex != null;  /* Define se é erro */

            if (ex != null)
            {
                item.DescricaoMensagem = BuscarMensagemErro(ex);
                item.DescricaoDetalhe  = item.Contexto != null ?
                                         String.Concat("URL: ", item.Contexto.Url, Environment.NewLine) +
                                          BuscarPilhaErro(ex) :
                                         String.Empty +
                                          BuscarPilhaErro(ex);
            }
            
            //A descrição não pode ser nula
            if (String.IsNullOrWhiteSpace(item.DescricaoMensagem))
                item.DescricaoMensagem = "Descrição não informada";

            //Efetua a inclusão do log no banco
            NegocioComum.Inserir(item);
        }

        public static void InserirMensagem(this Log item, String origem, String processo, String mensagem, String detalhe = "")
        {
            //Insere o item no log
            item.DescricaoOrigem = origem;
            item.NomeProcesso = processo.Length > 200 ? processo.Substring(0, 200) : processo;
            item.DescricaoMensagem = mensagem;
            item.DescricaoDetalhe = detalhe != String.Empty ? detalhe : item.DescricaoDetalhe;
            item.Erro = false;

            //Efetua a inclusão do log no banco
            NegocioComum.Inserir(item);
        }

        /// <summary>Exclui físicamente o log da aplicação após x dias</summary>
        /// <param name="item"></param>
        /// <param name="intervaloDias">Intervalo de dias</param>
        public static void Limpar(this Log item)
        {
            //Efetua a limpeza do log
            new Dados.DadosLog().LimparLog(item);
        }

        /// <summary>
        /// Consultar log utilizado pela web part
        /// </summary>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordernarPor"></param>
        /// <param name="ordernarDirecao"></param>
        /// <returns></returns>
        public static List<Log> ConsultarLog(
            Int32 indicePagina,
            Int32 registrosPorPagina,
            String ordernarPor,
            String ordernarDirecao,
            Int64? id = null,
            DateTime? de = null,
            DateTime? ate = null,
            Boolean? erro = null,
            String origem = "",
            String processo = "",
            String mensagem = "",
            String login = "")
        {
            return DadosLog.ConsultarLog
                    (
                    indicePagina,
                    registrosPorPagina,
                    ordernarPor,
                    ordernarDirecao,
                    id,
                    de,
                    ate,
                    erro,
                    origem,
                    processo,
                    mensagem,
                    login);
        }

        #region [ Métodos Auxiliares ]
        /// <summary>Função recursiva para buscar a mensagem de erro</summary>
        /// <param name="ex">Exception</param>
        /// <returns></returns>
        private static String BuscarMensagemErro(Exception ex)
        {
            if (ex == null)
                return String.Empty;

            return ex.InnerException == null ? ex.Message : String.Concat(ex.Message, Environment.NewLine, BuscarMensagemErro(ex.InnerException));
        }

        /// <summary>Função recursiva para buscar a pilha de erro</summary>
        /// <param name="ex">Exception</param>
        /// <returns></returns>
        private static String BuscarPilhaErro(Exception ex)
        {
            if (ex == null)
                return String.Empty;

            return ex.InnerException == null ? ex.StackTrace : String.Concat(ex.StackTrace, Environment.NewLine, BuscarMensagemErro(ex.InnerException));
        }
        #endregion
    }
}
