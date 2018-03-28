using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace PortalDeFluxos.Core.BLL.Dados
{
    public class DadosTarefa : BaseDB
    {

		/// <summary>
		/// Retorna as tarefas que precisam ser aprovadas automaticamente.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static List<Tarefa> ConsultarTarefasAprovacaoAutomatica()
		{
			using (ContextoBanco db = RetornarContextoDB(PortalWeb.ContextoWebAtual))
			{
				List<Tarefa> tarefas = (from t0 in db.Tarefa
							   join t1 in db.InstanciaFluxo on t0.IdInstanciaFluxo equals t1.IdInstanciaFluxo
							   where
									t0.Ativo == true 
								&&	t1.Ativo == true
								&&	t1.StatusFluxo == 1
								&&  t0.TarefaCompleta == false
								&&	t0.AprovacaoAutomatica == true
								&&	t0.DataSLA < DateTime.Now
							   select t0).ToList();

				return tarefas;
			}
		}

        /// <summary>Busca os dados da lista pela tarefa informada</summary>
        /// <param name="item">Item</param>
        /// <returns>Objeto com as informações da lista</returns>
        public static Lista ObterLista(Tarefa item)
        {
            using (ContextoBanco db = RetornarContextoDB(item.Contexto))
            {
                Lista lista = (from t0 in db.Tarefa
                               join t1 in db.InstanciaFluxo on t0.IdInstanciaFluxo equals t1.IdInstanciaFluxo
                               join t2 in db.Lista on t1.CodigoLista equals t2.CodigoLista
                               where
                                  t0.IdTarefa == item.IdTarefa
                               select t2).FirstOrDefault();

                return lista;
            }
        }

        /// <summary>Busca os e-mails pendentes para envio</summary>
        /// <param name="item">Item</param>
        /// <returns>Lista de e-mails para envio</returns>
        public static List<MensagemEmail> ConsultarEmailsPendentes(Tarefa item)
        {
            return ExecutarProcedure<MensagemEmail>(item.Contexto, "spConsultarEmailsPendentes", dr =>
                {
                    return new MensagemEmail()
                    {
                        IdTarefa = dr["IdTarefa"] != DBNull.Value ? (int?)dr["IdTarefa"] : null,
                        Para = dr["EmailResponsavel"] != DBNull.Value ? (string)dr["EmailResponsavel"] : null,
                        Copia = dr["EmailSuperior"] != DBNull.Value ? (string)dr["EmailSuperior"] : null,
                        Assunto = dr["DescricaoAssuntoEmail"] != DBNull.Value ? (string)dr["DescricaoAssuntoEmail"] : null,
                        Corpo = dr["DescricaoMensagemEmail"] != DBNull.Value ? (string)dr["DescricaoMensagemEmail"] : null,
                        EnviarPdf = dr["EnviarPdf"] != DBNull.Value ? (Boolean)dr["EnviarPdf"] : false
                    };
                });
        }

        /// <summary>Busca os e-mails de lembrete pendentes para envio</summary>
        /// <param name="item">Item</param>
        /// <returns>Lista de e-mails para envio</returns>
        public static List<MensagemEmail> ConsultarLembretesPendentes(Tarefa item)
        {
            return ExecutarProcedure<MensagemEmail>(item.Contexto, "spConsultarLembretesPendentes", dr =>
                {
                    return new MensagemEmail()
                        {
                            IdTarefa = dr["IdTarefa"] != DBNull.Value ? (int?)dr["IdTarefa"] : null,
                            IdLembrete = dr["IdLembrete"] != DBNull.Value ? (int?)dr["IdLembrete"] : null,
                            Para = dr["EmailPara"] != DBNull.Value ? (string)dr["EmailPara"] : null,
                            Copia = dr["EmailSuperior"] != DBNull.Value ? (string)dr["EmailSuperior"] : null,
                            Assunto = dr["DescricaoAssunto"] != DBNull.Value ? (string)dr["DescricaoAssunto"] : null,
                            Corpo = dr["DescricaoMensagem"] != DBNull.Value ? (string)dr["DescricaoMensagem"] : null,
                            EnviarPdf = dr["EnviarPdf"] != DBNull.Value ? (Boolean)dr["EnviarPdf"] : false
                        };
                });
        }

        /// <summary>Efetua o escalonamento das tarefas de fluxo</summary>
        /// <param name="item">Item</param>
        /// <returns>Lista de e-mails para envio</returns>
        public static List<MensagemEmail> EscalonarTarefas(Tarefa item)
        {
            //Lista de e-mails de tarefas escalonadas para envio
            return ExecutarProcedure<MensagemEmail>(item.Contexto, "spEscalonarTarefas", dr =>
            {
                return new MensagemEmail()
                    {
                        IdTarefa = dr["IdTarefa"] != DBNull.Value ? (int?)dr["IdTarefa"] : null,
                        Para = dr["EmailResponsavel"] != DBNull.Value ? (string)dr["EmailResponsavel"] : null,
                        Assunto = dr["DescricaoAssuntoEmailEscalonamento"] != DBNull.Value ? (string)dr["DescricaoAssuntoEmailEscalonamento"] : null,
                        Corpo = dr["DescricaoMensagemEmailEscalonamento"] != DBNull.Value ? (string)dr["DescricaoMensagemEmailEscalonamento"] : null
                    };
            });
        }

        /// <summary>Efetua a execução da procedure de sincronização do ambiente 2007 com o ambiente 2013</summary>
        /// <param name="item">Item</param>
        public static void SincronizarDados2007(Tarefa item)
        {
            //Lista de e-mails de tarefas escalonadas para envio
            ExecutarProcedure(item.Contexto, "spSincronizarDados2007", 120, null);
        }

        /// <summary>Define como enviado os e-mails das tarefas informadas</summary>
        /// <param name="item">Item</param>
        /// <param name="chaves">Código das tarefas que o e-mail foi enviado</param>
        public static void AtualizarEmailEnviado(Tarefa item, List<int> chaves)
        {
            using (ContextoBanco db = RetornarContextoDB(item.Contexto))
            {
                //Usuário que está efetuando a operação
                string usuario = Thread.CurrentPrincipal.Identity != null && Thread.CurrentPrincipal.Identity.Name != null ?
                                 Thread.CurrentPrincipal.Identity.Name : "Sistema";

                //Atualiza os itens no banco
                foreach (var idTarefa in chaves)
                    db.Database.ExecuteSqlCommand("UPDATE Tarefa SET EmailEnviado = 1, DataAlteracao = getdate(), LoginAlteracao = @p0 WHERE IdTarefa = @p1",
                                                    usuario,
                                                    idTarefa);
            }
        }

        /// <summary>Define como enviado os e-mails do lembrete de tarefa</summary>
        /// <param name="item">Item</param>
        /// <param name="chaves">Código das tarefas que o e-mail foi enviado</param>
        public static void AtualizarLembreteEnviado(Tarefa item, List<int> lembretes)
        {
            using (ContextoBanco db = RetornarContextoDB(item.Contexto))
            {
                //Usuário que está efetuando a operação
                string usuario = Thread.CurrentPrincipal.Identity != null && Thread.CurrentPrincipal.Identity.Name != null ?
                                 Thread.CurrentPrincipal.Identity.Name : "Sistema";

                //Atualiza os itens no banco
                foreach (var idLembrete in lembretes)
                    db.Database.ExecuteSqlCommand("UPDATE Lembrete SET EmailEnviado = 1, DataAlteracao = getdate(), LoginAlteracao = @p0 WHERE IdLembrete = @p1",
                                                    usuario,
                                                    idLembrete);
            }
        }
    }
}
