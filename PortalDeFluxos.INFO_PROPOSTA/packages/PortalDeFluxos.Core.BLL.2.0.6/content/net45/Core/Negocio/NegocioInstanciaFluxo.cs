using Iteris;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace PortalDeFluxos.Core.BLL.Negocio
{
	public static class NegocioInstanciaFluxo
	{
		#region [Sincronismo]

		/// <summary>Sincroniza o status do fluxo do Workflow Manager com o banco de dados e caso necessário, reinicia o fluxo</summary>
		/// <param name="fluxo">Fluxo</param>
		public static void SincronizarStatusFluxo(this InstanciaFluxo fluxo)
		{
			//Busca a quantidade de vezes que o fluxo deve ser iniciado (quando haver erros)
			Parametro p = new Parametro().Obter((int)ParametroEnum.RestartFluxos);
			ConfiguracaoRestartFluxos config = p == null ? new ConfiguracaoRestartFluxos() : Serializacao.DeserializeFromJson<ConfiguracaoRestartFluxos>(p.Valor);
			int qtdTentativasParaIniciar = config.NumeroTentativasInicio;
			int tempoMinimoSucesso = config.TempoMinutosSucesso;

			DateTime dataMinima = DateTime.Now.AddMinutes(-tempoMinimoSucesso);//somente fluxos parados por um periodo mínimo são processados.
			//Busca todos os fluxos ativos e com status em andamento do ambiente 2013 ou fluxos com erro que devem ser reiniciados.
			List<InstanciaFluxo> instancias = new InstanciaFluxo().Consultar(i => !i.Lista.Ambiente2007 &&
																				  i.StatusFluxo.HasValue &&
																				  (i.StatusFluxo.Value == (int)StatusFluxo.EmAndamento
																					|| (i.StatusFluxo.Value == (int)StatusFluxo.Erro
																							&& (i.NumeroTentativaInicio == null
																								|| i.NumeroTentativaInicio.Value <= qtdTentativasParaIniciar)
																						)) &&
																				  i.DataAlteracao < dataMinima &&
																				  i.CodigoInstanciaFluxo.HasValue && //Precisa ter código de instância de fluxo	
																				  i.Ativo);
			//Verifica se possui instâncias a serem verificadas
			if (instancias.Count == 0)
				return;

			//Itens a serem atualizados no banco (Para operação em lote)
			List<InstanciaFluxo> listaAtualizar = new List<InstanciaFluxo>();
			//Varre as instâncias retornadas verificando o status dos fluxos
			foreach (var item in instancias)
			{
				try
				{
					if (ProcessarStatusInstanciaFluxo(item, qtdTentativasParaIniciar, tempoMinimoSucesso))//Sincroniza no Repositório (erros/Cancelados)
						listaAtualizar.Add(item);
				}
				catch (Exception ex)
				{
					new Log().Inserir("Portal de Fluxos - TimerJob - SincronizarStatusFluxo",
										String.Format("Erro ao tentar tratar o fluxo:{0} - Id lista:{1} - Id item:{2}",
														item.CodigoInstanciaFluxo.Value,
														item.CodigoLista,
														item.CodigoItem
													 ), ex);
				}
			}


			//Efetua o log dos itens processados
			if (listaAtualizar.Count > 0)
				AdicionarLog(listaAtualizar, instancias.Count);
		}

		/// <summary>Sincroniza a instância do fluxo no banco</summary>
		/// <param name="instancia">Instância</param>
		/// <param name="qtdTentativasParaIniciar">Quantidade de tentativas para iniciar o fluxo</param>
		/// <returns>Se deve atualizar no banco</returns>
		private static bool ProcessarStatusInstanciaFluxo(InstanciaFluxo instancia, Int32 qtdTentativasParaIniciar, Int32 tempoConsideradoSucesso)
		{

			InstanciaFluxo instanciaClone = (InstanciaFluxo)instancia.Clone();
			Boolean retorno = false;
			//Variáveis para controle de reinicio de fluxo
			bool reiniciarFluxo = qtdTentativasParaIniciar >        //Verifica se pode reiniciar o fluxo, pela quantidade de tentativas parametrizadas
								   (instancia.NumeroTentativaInicio.HasValue ? instancia.NumeroTentativaInicio.Value : 0);


			#region [Obter Info do fluxo]
			WorkflowServicesManager wfServiceManager = new WorkflowServicesManager(PortalWeb.ContextoWebAtual.SPClient, PortalWeb.ContextoWebAtual.SPWeb);
			WorkflowInstanceService wfInstanceService = wfServiceManager.GetWorkflowInstanceService();
			WorkflowInstance wfInstance = ObterInstanciaAtual(wfInstanceService, instancia.CodigoInstanciaFluxo.Value); 
			#endregion

			if (wfInstance != null && !CancelarInstanciasFluxoAntigas(instancia, instanciaClone, wfInstanceService, wfInstance))
				return false;//não precisa ser sincronizado - item deletado ou não conseguiu cancelar todos os fluxos.

			if (wfInstance != null && InstanciaEmAndamento(instancia, instanciaClone, wfInstance, tempoConsideradoSucesso))
				return false;//não precisou ser sincronizado

			#region [Se estiver em algum dos status - o status da instancia irá mudar.]
			if (wfInstance == null ||
					wfInstance.ServerObjectIsNull.Value ||
					wfInstance.Status == WorkflowStatus.Canceled ||
					wfInstance.Status == WorkflowStatus.Canceling ||
					wfInstance.Status == WorkflowStatus.Invalid ||
					wfInstance.Status == WorkflowStatus.NotSpecified ||
					wfInstance.Status == WorkflowStatus.NotStarted ||
					wfInstance.Status == WorkflowStatus.Completed ||
					wfInstance.Status == WorkflowStatus.Terminated)
			{
				String mensagemFluxo = ObterMensagemErro(wfInstance);
				//Ajusta o status e adiciona na lista de itens a serem atualizados
				instancia.StatusFluxo = wfInstance != null && !wfInstance.ServerObjectIsNull.Value && wfInstance.Status == WorkflowStatus.Completed ?
										(int)StatusFluxo.Finalizado :
										wfInstance != null && !wfInstance.ServerObjectIsNull.Value && !String.IsNullOrWhiteSpace(mensagemFluxo) && wfInstance.Status == WorkflowStatus.Terminated ?
										(int)StatusFluxo.Erro :
										(int)StatusFluxo.Cancelado;

				Boolean erroTratado = wfInstance == null
								|| mensagemFluxo.IndexOf("System.Activities.Statements.WorkflowTerminatedException: An activity referenced by the associated workflow has been deprecated.") > -1
								|| mensagemFluxo.IndexOf("The instance has been terminated because the workflow associated with this instance is no longer available") > -1
								|| mensagemFluxo.IndexOf("The remote server returned an error: (503) Server Unavailable") > -1;

				instancia.StatusFluxo = erroTratado ? (int)StatusFluxo.Erro : instancia.StatusFluxo;
			}
			else
				instancia.StatusFluxo = (int)StatusFluxo.EmAndamento;
			
			#endregion

			#region [Caso o statusFluxo mudou e InstanciaFluxo sofreu atualização no periodo, não processa]

			if (instancia.StatusFluxo != instanciaClone.StatusFluxo)
			{
				InstanciaFluxo instanciaAtual = new InstanciaFluxo().Obter(instancia.IdInstanciaFluxo);
				if (instanciaAtual.DataAlteracao != instancia.DataAlteracao)
					reiniciarFluxo = false;				
			}

			#endregion

			//Se o fluxo estiver com erro e a quantidade de tentativas não estourou, tenta reiniciar novamente
			if (instancia.StatusFluxo == (int)StatusFluxo.Erro
				&& reiniciarFluxo)
				retorno =  ReiniciarFluxoErro(instancia, instanciaClone, wfInstance, wfInstanceService);

			return retorno;
		}

		private static bool CancelarInstanciasFluxoAntigas(InstanciaFluxo instancia
			, InstanciaFluxo instanciaClone
			, WorkflowInstanceService wfInstanceService
			, WorkflowInstance wfInstanceAtual)
		{
			Boolean retorno = true;
			if (wfInstanceAtual == null)
				return false;
			try
			{
				WorkflowInstanceCollection wfInstances =
					wfInstanceService.EnumerateInstancesForListItem(instancia.CodigoLista, instancia.CodigoItem);
				PortalWeb.ContextoWebAtual.SPClient.Load(wfInstances);
				PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
				foreach (WorkflowInstance workflow in wfInstances)
					if (wfInstanceAtual.Id != workflow.Id)
						wfInstanceService.TerminateWorkflow(workflow);
			}
			catch (Exception ex)
			{
				string msgException = ex.InnerException == null
					? ex.Message
					: String.Concat(ex.Message, Environment.NewLine, BuscarMensagemErro(ex.InnerException));

				if (!string.IsNullOrEmpty(msgException) && msgException.Trim().Equals("O item não existe. Ele pode ter sido excluído por outro usuário."))
				{
					instancia.Ativo = false;//Item foi exluído, desativa a instancia fluxo.
					if (instancia.DataAlteracao == instancia.DataAlteracao)
						instancia.AtualizarPropEspecifico(instanciaClone);
					retorno = false;
				}
			}

			return retorno;
		}

		private static bool InstanciaEmAndamento(InstanciaFluxo instancia
			 , InstanciaFluxo instanciaClone
			 , WorkflowInstance wfInstanceAtual
			 , Int32 tempoConsideradoSucesso)
		{
			Boolean retorno = false;
			if (wfInstanceAtual == null)
				return false;
			TimeSpan tempoUltimoRestart = instancia.DataRestartWorkflow != null ? (TimeSpan)(DateTime.Now - instancia.DataRestartWorkflow) : new TimeSpan(0, tempoConsideradoSucesso, 0);
			//Se estiver ok, vai para o próximo item
			if (wfInstanceAtual != null && !wfInstanceAtual.ServerObjectIsNull.Value && wfInstanceAtual.Status == WorkflowStatus.Started)
			{
				if (tempoUltimoRestart.TotalMinutes >= tempoConsideradoSucesso)//está ativo à tempo suficiente para zerarmos o contador de tentativas
				{
					instancia.NumeroTentativaInicio = 0;
					if (instancia.DataAlteracao == instancia.DataAlteracao)
						instancia.AtualizarPropEspecifico(instanciaClone);
				}
				retorno = true;
			}

			return retorno;
		}

		private static bool ReiniciarFluxoErro(InstanciaFluxo instancia
			, InstanciaFluxo instanciaClone
			, WorkflowInstance wfInstance
			, WorkflowInstanceService wfInstanceService)
		{
			Boolean retorno = false;
			try
			{
				if (wfInstance != null && wfInstance.Status != WorkflowStatus.Terminated)//se necessário, finaliza o fluxo
				{
					wfInstanceService.TerminateWorkflow(wfInstance);
					PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

					while (wfInstance.Status != WorkflowStatus.Terminated)//Garante que o fluxo foi cancelado.
					{
						Thread.Sleep(500);
						wfInstance = ObterInstanciaAtual(wfInstanceService, instancia.CodigoInstanciaFluxo.Value);
					}
				}
				retorno = IniciarFluxo(instancia, instanciaClone: instanciaClone);
			}
			catch (Exception ex)
			{
				AdicionarLog(instancia, null, ex);
			}

			return retorno;
		}

		#endregion

		#region [Fluxo]

		public static void AndarFluxo(this InstanciaFluxo fluxo, Tarefa tarefaAtual, Boolean privilegioElevado = false)
		{

			if (tarefaAtual.TipoTarefa == (byte)TipoTarefa.Primeiro)
			{
				bool tarefasPendentes = false;
				if (!fluxo.CodigoInstanciaFluxo.HasValue)
					throw new NullReferenceException("Não foi possível encontrar o código de instância do fluxo.");

				AtualizarTarefasRestantes(fluxo, tarefaAtual);

				if (tarefaAtual.AprovacaoPor == (byte)TipoAprovacaoPor.Alcada)
				{
					//Verificar se existem mais aprovadores na tarefa Headline 
					tarefasPendentes = NegocioTarefaCustom.ConsultarTarefasHeadlinePendentes(tarefaAtual, fluxo);
				}
				if (!tarefasPendentes)
				{
					if (privilegioElevado)
						PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
						{
							AndarFluxo(fluxo.CodigoInstanciaFluxo.Value, tarefaAtual);
						});
					else
						AndarFluxo(fluxo.CodigoInstanciaFluxo.Value, tarefaAtual);
				}

				fluxo.ErroCancelado = false;//Mudar somente no andar do fluxo
				fluxo.Atualizar();
			}
			else
			{
				throw new NotImplementedException("Tipo de tarefa não implementada");
			}
		}

		private static void AndarFluxo(Guid instanciaFluxo, Tarefa tarefaAtual)
		{
			//Obtendo as configuracoes do fluxo
			WorkflowServicesManager workflowServicesManager = new WorkflowServicesManager(PortalWeb.ContextoWebAtual.SPClient, PortalWeb.ContextoWebAtual.SPWeb);
			WorkflowInstanceService workflowInstanceService = workflowServicesManager.GetWorkflowInstanceService();
			WorkflowInstance workflowInstance = workflowInstanceService.GetInstance(instanciaFluxo);

			PortalWeb.ContextoWebAtual.SPClient.Load(workflowInstance);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			if (workflowInstance == null)
				throw new Exception(String.Format(MensagemPortal.InstanciaFluxoNaoEncontrada.GetTitle(), instanciaFluxo));

			var acao = new
			{
				DescricaoAcaoEfetuada = tarefaAtual != null ? tarefaAtual.DescricaoAcaoEfetuada : String.Empty,
				ComentarioAprovacao = tarefaAtual != null ? tarefaAtual.ComentarioAprovacao : String.Empty
			};

			workflowInstanceService.PublishCustomEvent(workflowInstance, tarefaAtual.CodigoTarefa.ToString(), new JavaScriptSerializer().Serialize(acao).ToString());
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
		}

		public static void AtualizarTarefasRestantes(InstanciaFluxo fluxo, Tarefa tarefaAtual)
		{
			List<Tarefa> tarefas = new Tarefa().Consultar(t => t.CodigoTarefa == tarefaAtual.CodigoTarefa && t.IdTarefa != tarefaAtual.IdTarefa && t.Ativo == true);
			if (tarefas != null && tarefas.Count > 0)
			{
				tarefas.ForEach(item =>
				{
					item.TarefaCompleta = true;
				});
				tarefas.Atualizar();
			}

			if (!String.IsNullOrEmpty(tarefaAtual.DescricaoAcaoFinalizarEtapa))
			{
				List<String> acoesParaFinalizarEtapa = tarefaAtual.DescricaoAcaoFinalizarEtapa.Split(';').ToList();

				if (acoesParaFinalizarEtapa.Contains(tarefaAtual.DescricaoAcaoEfetuada))//A etapa foi reprovada (todas as tarefas devem ser finalizadas - e o fluxo marcado como reprovado - momentaneamente)
				{
					List<Tarefa> tarefasCancelar = new Tarefa().Consultar(t => t.IdInstanciaFluxo == tarefaAtual.IdInstanciaFluxo &&
																				t.CodigoTarefa != tarefaAtual.CodigoTarefa &&
																				t.NomeEtapa == tarefaAtual.NomeEtapa &&
																				t.Ativo == true &&
																				t.TarefaCompleta == false
																		   );

					if (fluxo.EtapaParalela != null && (bool)fluxo.EtapaParalela)
					{
						fluxo.FluxoReprovado = true;
						fluxo.Atualizar();
					}

					if (tarefasCancelar != null && tarefasCancelar.Count > 0)
					{
						tarefasCancelar.ForEach(tc =>
						{
							tc.TarefaCompleta = true;
							tc.DataFinalizado = DateTime.Now;
						});
						tarefasCancelar.Atualizar();



						List<Guid?> codigoTarefaDistintos = tarefasCancelar.Select(t => t.CodigoTarefa).Distinct().ToList();

						foreach (Guid? codigoTarefa in codigoTarefaDistintos)
						{
							if (codigoTarefa.HasValue)
							{
								AndarFluxo(fluxo.CodigoInstanciaFluxo.Value, new Tarefa() { CodigoTarefa = codigoTarefa });
							}
						}
					}
				}
			}
		}

		public static bool IniciarFluxo(this EntidadePropostaSP item, String nomeWorkflow = "", Guid idLista = new Guid())
		{
			List lista = null;
			if (idLista == Guid.Empty)
				lista = ComumSP.ObterLista(item);
			else
				lista = ComumSP.ObterLista(idLista);

			if (lista == null)
				throw new Exception("Lista não foi encontrada.");

			InstanciaFluxo instancia = new InstanciaFluxo()
											.Consultar(inst =>
																 inst.Ativo
															  && inst.CodigoLista == lista.Id
															  && inst.CodigoItem == item.ID
													   )
											 .OrderByDescending(o => o.IdInstanciaFluxo)
											.FirstOrDefault();

			return IniciarFluxo(instancia, lista, item, nomeWorkflow);
		}

		private static bool IniciarFluxo(InstanciaFluxo instancia, List lista = null, EntidadePropostaSP item = null, String nomeWorkflow = "", InstanciaFluxo instanciaClone = null)
		{
			Boolean retorno = true;

			if (lista == null)
				lista = ComumSP.ObterLista(instancia.CodigoLista);
			if (item == null)
				item = NegocioComum.ObterProposta(instancia.CodigoLista, instancia.CodigoItem);
			if (lista == null)
				throw new Exception("Lista não foi encontrada.");

			if ((instancia != null && !instancia.DataFinalizado.HasValue) || (instancia != null && instancia.StatusFluxo == (Int32)StatusFluxo.Erro))
			{
				if (instanciaClone == null)
					instanciaClone = (InstanciaFluxo)instancia.Clone();
				instancia.ErroCancelado = true;//Flag de controle - Não se trata de um inicio de fluxo erro
				instancia.NumeroTentativaInicio = instancia.NumeroTentativaInicio == null ? 1 : (instancia.NumeroTentativaInicio + 1);
				instancia.DataRestartWorkflow = DateTime.Now;
				InstanciaFluxo instanciaAtual = new InstanciaFluxo().Obter(instancia.IdInstanciaFluxo);
				if (instanciaAtual.DataAlteracao == instancia.DataAlteracao)
					instancia.AtualizarPropEspecifico(instanciaClone);
				else
					return false;
			}
			else if (instancia != null && instancia.DataFinalizado.HasValue)//Fluxo deve começar do inicio
			{
				#region [Marca tarefas - como antigas]

				#region [Limpa o estado Atual do Fluxo - ao reiniciar volta para primeira etapa]
				item.EstadoAtualFluxo = String.Empty;
				item.Atualizar(lista.Id);
				#endregion

				#region [Marca tarefas - como antigas]

				List<Tarefa> tarefasAntigas = new Tarefa().Consultar(t => t.IdInstanciaFluxo == instancia.IdInstanciaFluxo);
				if (tarefasAntigas != null && tarefasAntigas.Count > 0)
				{
					tarefasAntigas.ForEach(t =>
					{
						t.AprovacoesAtuais = false;
					});
					tarefasAntigas.Atualizar();
				}

				#endregion

				#region [Tarefas ativas incompletas - desativa]

				List<Tarefa> tarefasAtivas = new Tarefa().Consultar(t => t.IdInstanciaFluxo == instancia.IdInstanciaFluxo && t.TarefaCompleta == false);
				if (tarefasAtivas != null && tarefasAtivas.Count > 0)
				{
					tarefasAtivas.ForEach(t =>
					{
						t.Ativo = false;
					});
					tarefasAtivas.Atualizar();
				}

				#endregion

				#region [Adicionar um registro na tabela de tarefas - para sinalizar por quem o fluxo foi iniciado]

				Tarefa tarefaFluxoReiniciado = new Tarefa();
				tarefaFluxoReiniciado.TarefaCompleta = true;
				tarefaFluxoReiniciado.EmailEnviado = true;
				tarefaFluxoReiniciado.CopiarSuperior = false;
				tarefaFluxoReiniciado.TarefaEscalonada = false;
				tarefaFluxoReiniciado.DescricaoAcaoEfetuada = MensagemPortal.FluxoReiniciado.GetTitle();
				tarefaFluxoReiniciado.DataFinalizado = DateTime.Now;
				tarefaFluxoReiniciado.IdInstanciaFluxo = instancia.IdInstanciaFluxo;
				tarefaFluxoReiniciado.NomeEtapa = MensagemPortal.FluxoReiniciado.GetTitle();
				tarefaFluxoReiniciado.NomeTarefa = MensagemPortal.FluxoReiniciado.GetTitle();
				tarefaFluxoReiniciado.ComentarioAprovacao = MensagemPortal.FluxoReiniciado.GetTitle();
				tarefaFluxoReiniciado.EmailResponsavel = PortalWeb.ContextoWebAtual.UsuarioAtual.Email;
				tarefaFluxoReiniciado.NomeResponsavel = PortalWeb.ContextoWebAtual.UsuarioAtual.Nome;
				tarefaFluxoReiniciado.LoginResponsavel = PortalWeb.ContextoWebAtual.UsuarioAtual.Login;
				tarefaFluxoReiniciado.NomeCompletadoPor = PortalWeb.ContextoWebAtual.UsuarioAtual.Nome;
				tarefaFluxoReiniciado.LoginCompletadoPor = PortalWeb.ContextoWebAtual.UsuarioAtual.Login;
				tarefaFluxoReiniciado.DataAtribuido = DateTime.Now;
				tarefaFluxoReiniciado.Inserir();

				#endregion

				#endregion
			}

			#region [Busca o fluxo]

			nomeWorkflow = nomeWorkflow == String.Empty ? lista.Title : nomeWorkflow; //Por default o nome do workflow é igual ao nome da lista
			WorkflowServicesManager wfServiceManager = new WorkflowServicesManager(PortalWeb.ContextoWebAtual.SPClient, PortalWeb.ContextoWebAtual.SPWeb);
			WorkflowSubscriptionService workflowSubscriptionService = wfServiceManager.GetWorkflowSubscriptionService();

			// Obtem workflow associations
			var workflowAssociations = workflowSubscriptionService.EnumerateSubscriptionsByList(lista.Id);
			PortalWeb.ContextoWebAtual.SPClient.Load(workflowAssociations);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			var def = from defs in workflowAssociations
					  where defs.Name == nomeWorkflow
					  select defs;
			WorkflowSubscription wfSubscription = def.FirstOrDefault();

			if (wfSubscription == null)
				throw new Exception("Workflow não foi encontrado.");

			WorkflowInstanceService wfInstanceService = wfServiceManager.GetWorkflowInstanceService();
			if (item.FluxoAtivo(lista, wfInstanceService, wfSubscription))//Verifica se exite fluxo ativo
				return false;
			#endregion

			#region [Inicia o workflow]

			var startParameters = new Dictionary<string, object>();
			var guidFluxo = wfInstanceService.StartWorkflowOnListItem(wfSubscription, item.ID, startParameters);
			if (guidFluxo != null)
			{
				retorno = true;
			}

			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			#endregion

			return retorno;
		}

		public static void ReiniciarFluxo(this EntidadePropostaSP item, String nomeWorkflow = "", Guid idLista = new Guid())
		{
			List lista = null;
			if (idLista == Guid.Empty)
				lista = ComumSP.ObterLista(item);
			else
				lista = ComumSP.ObterLista(idLista);

			if (lista == null)
				throw new Exception("lista não encontrada");

			#region [Limpa o estado Atual do Fluxo - ao reiniciar volta para primeira etapa]
			item.EstadoAtualFluxo = String.Empty;
			item.Atualizar();
			#endregion

			#region [Completa tarefas pendentes tarefas pendentes]

			List<InstanciaFluxo> instanciasFluxo = new InstanciaFluxo().Consultar(i => i.CodigoItem == item.ID && i.CodigoLista == lista.Id && i.Ativo == true);
			if (instanciasFluxo != null && instanciasFluxo.Count > 0)
			{
				foreach (InstanciaFluxo instanciaFluxo in instanciasFluxo)
				{
					List<Tarefa> tarefas = new Tarefa().Consultar(t => t.IdInstanciaFluxo == instanciaFluxo.IdInstanciaFluxo
						&& t.Ativo == true && t.TarefaCompleta != true);
					if (tarefas != null && tarefas.Count > 0)
					{
						tarefas.ForEach(t =>
						{
							t.TarefaCompleta = true;
						});
						tarefas.Atualizar();
					}
				}

				#region [Adicionar um registro na tabela de tarefas - para sinalizar por quem o fluxo foi reiniciado]

				NegocioTarefaCustom.AdicionarItemTarefaFluxoReiniciado(MensagemPortal.FluxoReiniciado.GetTitle()
							  , MensagemPortal.FluxoReiniciado.GetTitle()
							  , MensagemPortal.FluxoReiniciadoMensagem.GetTitle()
							  , instanciasFluxo.OrderBy(i => i.IdInstanciaFluxo).LastOrDefault().IdInstanciaFluxo);

				#endregion
			}
			#endregion

			#region [Marca tarefas - como antigas]

			Int32 idInstanciaFluxo = instanciasFluxo.FirstOrDefault().IdInstanciaFluxo;
			List<Tarefa> tarefasAntigas = new Tarefa().Consultar(t => t.IdInstanciaFluxo == idInstanciaFluxo);
			if (tarefasAntigas != null && tarefasAntigas.Count > 0)
			{
				tarefasAntigas.ForEach(t =>
				{
					t.AprovacoesAtuais = false;
				});
				tarefasAntigas.Atualizar();
			}

			#endregion

			if (FluxoAtivo(item, nomeWorkflow))
				item.CancelarFluxo(lista);

			while (FluxoAtivo(item, nomeWorkflow))//Garante que o fluxo foi cancelado.
			{ Thread.Sleep(500); }

			item.IniciarFluxo();

		}

		public static bool PropostaEmAprovacao(Guid codigoLista, Int32 codigoItem)
		{
			InstanciaFluxo instancia = new InstanciaFluxo()
											.Consultar(inst =>
																 inst.Ativo
															  && inst.CodigoLista == codigoLista
															  && inst.CodigoItem == codigoItem
															  && (inst.StatusFluxo == (int)StatusFluxo.EmAndamento || (int)inst.StatusFluxo == (int)StatusFluxo.Erro)
													   )
											 .OrderByDescending(o => o.IdInstanciaFluxo)
											.FirstOrDefault();
			if (instancia != null)
				return true;
			else
				return false;
		}

		public static WorkflowInstance ObterInstanciaAtual(WorkflowInstanceService wfInstanceService, Guid instanceId)
		{
			WorkflowInstance wfInstance = null;
			try
			{
				wfInstance = wfInstanceService.GetInstance(instanceId);
				PortalWeb.ContextoWebAtual.SPClient.Load(wfInstance);
				PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
			}
			catch
			{
				wfInstance = null;

			}
			return wfInstance;
		}
			

		#region [Fluxo Ativo]

		/// <summary>
		/// Verifica se o fluxo esta ativo
		/// </summary>
		/// <param name="item"></param>
		/// <param name="lista"></param>
		/// <param name="wfInstanceService"></param>
		/// <param name="wfSubscription"></param>
		/// <returns></returns>
		public static Boolean FluxoAtivo(this EntidadePropostaSP item, List lista, WorkflowInstanceService wfInstanceService, WorkflowSubscription wfSubscription)
		{
			WorkflowInstanceCollection wfInstances = wfInstanceService.EnumerateInstancesForListItem(lista.Id, item.ID);
			PortalWeb.ContextoWebAtual.SPClient.Load(wfInstances);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			var def = from defs in wfInstances
					  where defs.WorkflowSubscriptionId == wfSubscription.Id
							&& defs.Status == WorkflowStatus.Started
					  select defs;

			WorkflowInstance instance = def.FirstOrDefault();

			return instance != null;
		}

		/// <summary>
		/// Verifica se o fluxo está ativo
		/// </summary>
		/// <param name="lista"></param>
		/// <param name="codigoItem"></param>
		/// <param name="nomeWorkflow">Se não for informado, procura fluxo com o mesmo nome da lista</param>
		/// <returns></returns>
		public static Boolean FluxoAtivo(List lista, Int32 codigoItem, String nomeWorkflow = "")
		{
			WorkflowServicesManager wfServiceManager = new WorkflowServicesManager(PortalWeb.ContextoWebAtual.SPClient, PortalWeb.ContextoWebAtual.SPWeb);
			WorkflowSubscriptionService workflowSubscriptionService = wfServiceManager.GetWorkflowSubscriptionService();
			WorkflowInstanceService wfInstanceService = wfServiceManager.GetWorkflowInstanceService();

			nomeWorkflow = nomeWorkflow == String.Empty ? lista.Title : nomeWorkflow; //Por default o nome do workflow é igual ao nome da lista
			// Obtem workflow associations
			var workflowAssociations = workflowSubscriptionService.EnumerateSubscriptionsByList(lista.Id);
			PortalWeb.ContextoWebAtual.SPClient.Load(workflowAssociations);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			var def = from defs in workflowAssociations
					  where defs.Name == nomeWorkflow
					  select defs;
			WorkflowSubscription wfSubscription = def.FirstOrDefault();

			WorkflowInstanceCollection wfInstances = wfInstanceService.EnumerateInstancesForListItem(lista.Id, codigoItem);
			PortalWeb.ContextoWebAtual.SPClient.Load(wfInstances);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			var wfInstance = from defs in wfInstances
							 where defs.WorkflowSubscriptionId == wfSubscription.Id
								   && defs.Status == WorkflowStatus.Started
							 select defs;

			WorkflowInstance instance = wfInstance.FirstOrDefault();

			return instance != null;
		}

		/// <summary>
		/// Verifica se o fluxo está ativo
		/// </summary>
		/// <param name="item"></param>
		/// <param name="nomeWorkflow">Se não for informado, procura fluxo com o mesmo nome da lista</param>
		/// <returns></returns>
		public static Boolean FluxoAtivo(this EntidadePropostaSP item, String nomeWorkflow = "")
		{
			List lista = ComumSP.ObterLista(item);
			if (lista == null)
				throw new Exception("Lista não foi encontrada.");

			return FluxoAtivo(lista, item.ID, nomeWorkflow);
		}

		/// <summary>
		/// Verifica se o fluxo está ativo
		/// </summary>
		/// <param name="codigoLista"></param>
		/// <param name="codigoItem"></param>
		/// <param name="nomeWorkflow">Se não for informado, procura fluxo com o mesmo nome da lista</param>
		/// <returns></returns>
		public static Boolean FluxoAtivo(Guid codigoLista, Int32 codigoItem, String nomeWorkflow = "")
		{
			List lista = ComumSP.ObterLista(codigoLista);
			if (lista == null)
				throw new Exception("Lista não foi encontrada.");

			return FluxoAtivo(lista, codigoItem, nomeWorkflow);
		}

		#endregion

		#region [Cancelar Fluxo]

		/// <summary>
		/// Cancela os fluxos associados ao item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="nomeWorkflow">Se não for informado, cancela todos os fluxos</param>
		public static void CancelarFluxo(this EntidadePropostaSP item, String nomeWorkflow = "")
		{
			List lista = ComumSP.ObterLista(item);
			if (lista == null)
				throw new Exception("Lista não foi encontrada.");

			CancelarFluxo(lista.Id, item.ID, nomeWorkflow);
		}

		/// <summary>
		/// Cancela os fluxos associados ao item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="lista"></param>
		/// <param name="nomeWorkflow"> Se não for informado, cancela todos os fluxos.</param>
		public static void CancelarFluxo(this EntidadePropostaSP item, List lista, String nomeWorkflow = "")
		{
			CancelarFluxo(lista.Id, item.ID, nomeWorkflow);
		}

		/// <summary>
		/// Cancela os fluxos associados ao item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="lista"></param>
		/// <param name="nomeWorkflow"> Se não for informado, cancela todos os fluxos.</param>
		public static void CancelarFluxo(Guid codigoLista, Int32 codigoItem, String nomeWorkflow = "")
		{
			WorkflowServicesManager wfServiceManager = new WorkflowServicesManager(PortalWeb.ContextoWebAtual.SPClient, PortalWeb.ContextoWebAtual.SPWeb);
			WorkflowSubscriptionService workflowSubscriptionService = wfServiceManager.GetWorkflowSubscriptionService();

			// Obtem workflow associations
			WorkflowSubscriptionCollection workflowAssociations = workflowSubscriptionService.EnumerateSubscriptionsByList(codigoLista);
			PortalWeb.ContextoWebAtual.SPClient.Load(workflowAssociations);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			if (nomeWorkflow != String.Empty)
			{
				var def = from defs in workflowAssociations
						  where defs.Name == nomeWorkflow
						  select defs;
				WorkflowSubscription wfSubscription = def.FirstOrDefault();

				CancelarFluxo(codigoLista, codigoItem, wfServiceManager, wfSubscription);
			}
			else
			{
				foreach (WorkflowSubscription wfSubscription in workflowAssociations)
				{
					CancelarFluxo(codigoLista, codigoItem, wfServiceManager, wfSubscription);
				}
			}
		}

		public static void CancelarFluxo(WorkflowInstanceService workflowInstanceService, WorkflowInstance workflowInstance, InstanciaFluxo instancia, byte statusFluxo = (byte)StatusFluxo.Cancelado)
		{
			InstanciaFluxo instanciaOriginal = (InstanciaFluxo)instancia.Clone();
			//Cancelar WF
			workflowInstanceService.CancelWorkflow(workflowInstance);
			if (instancia == null)
				return;
			instancia.DataFinalizado = DateTime.Now;
			instancia.StatusFluxo = statusFluxo;
			instancia.AtualizarPropEspecifico(instanciaOriginal);
		}

		/// <summary>
		/// Cancela os fluxos associados ao item.
		/// </summary>
		/// <param name="codigoLista"></param>
		/// <param name="codigoItem"></param>
		/// <param name="wfServiceManager"></param>
		/// <param name="wfSubscription"></param>
		public static WorkflowInstanceCollection CancelarFluxo(Guid codigoLista, Int32 codigoItem, WorkflowServicesManager wfServiceManager, WorkflowSubscription wfSubscription = null, byte statusFluxo = (byte)StatusFluxo.Cancelado)
		{
			WorkflowInstanceService wfInstanceService = wfServiceManager.GetWorkflowInstanceService();
			WorkflowInstanceCollection wfInstances = wfInstanceService.EnumerateInstancesForListItem(codigoLista, codigoItem);
			PortalWeb.ContextoWebAtual.SPClient.Load(wfInstances);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			foreach (WorkflowInstance workflow in wfInstances)
			{
				if (workflow.Status == WorkflowStatus.Started &&
					(wfSubscription == null || workflow.WorkflowSubscriptionId == wfSubscription.Id))
				{
					wfInstanceService.CancelWorkflow(workflow);
				}

				List<InstanciaFluxo> instancia = new InstanciaFluxo().Consultar(i => i.CodigoInstanciaFluxo == workflow.Id && i.Ativo == true);

				if (instancia != null && instancia.Count > 0)
				{
					instancia.ForEach(i =>
					{
						i.DataFinalizado = DateTime.Now;
						i.StatusFluxo = statusFluxo;
					});
					instancia.Atualizar();
				}
			}

			return wfInstances;
		}


		#endregion

		#endregion

		#region [Instancia Fluxo]

		public static InstanciaFluxo ValidarInstanciaFluxo(
			this InstanciaFluxo instanciaAtualDoFluxo,
			DateTime dataCriacao,
			ListItem item,
			ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa = null,
			String nomeEtapa = "",
			String estadoAtual = "")
		{
			return instanciaAtualDoFluxo.AtualizarInstanciaFluxo(dataCriacao, item, null, configuracaoTarefa, nomeEtapa, estadoAtual);
		}

		public static InstanciaFluxo AtualizarInstanciaFluxo(this InstanciaFluxo instanciaAtualDoFluxo
			, DateTime dataCriacao
			, ListItem item
			, DateTime? dataFinalizacao
			, ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa = null
			, String nomeEtapa = ""
			, String estadoAtual = "")
		{
			InstanciaFluxo instancia = new InstanciaFluxo()
										.Consultar(inst =>
															 inst.Ativo
														  && inst.CodigoLista == instanciaAtualDoFluxo.CodigoLista
														  && inst.CodigoItem == instanciaAtualDoFluxo.CodigoItem
												   )
										 .OrderByDescending(o => o.IdInstanciaFluxo)
										.FirstOrDefault();
			if (instancia == null)
			{
				if (item == null)
					throw new Exception("AtualizarInstanciaFluxo o Listitem está nulo.");

				FieldLookupValue autor = item[PropriedadesItem.Criador.GetTitle()] as FieldLookupValue;
				if (autor == null)
					throw new Exception("Falha ao obter autor do item.");

				//Busca o usuário criador
				Usuario usuarioCriador = PortalWeb.ContextoWebAtual.BuscarUsuario(autor.LookupId);

				//Valida o usuário criador
				if (usuarioCriador == null)
					throw new NullReferenceException(String.Format("Usuário autor não encontrado: {0}",
																	autor != null && !String.IsNullOrWhiteSpace(autor.LookupValue) ?
																	autor.LookupValue :
																	"(valor nulo)"));
				if (configuracaoTarefa != null)
					instanciaAtualDoFluxo.NomeEtapa = String.Format(StatusProposta.Andamento.GetTitle(), configuracaoTarefa.NomeEtapa);

				instanciaAtualDoFluxo.DataInicio = dataCriacao;
				instanciaAtualDoFluxo.LoginSolicitante = usuarioCriador.Login;
				instanciaAtualDoFluxo.NomeSolicitante = usuarioCriador.Nome;
				instanciaAtualDoFluxo.NomeSolicitacao = item[PropriedadesItem.Title.GetTitle()].ToString();
				instanciaAtualDoFluxo.Inserir();

				NegocioSincronizacao.SincronizarInstanciaFluxo(instanciaAtualDoFluxo.CodigoLista, instanciaAtualDoFluxo.CodigoItem);
			}
			else
			{
				InstanciaFluxo instanciaClone = (InstanciaFluxo)instancia.Clone();

				// Atualizar o status atual da etapa
				if (dataFinalizacao != null)
					instancia.NomeEtapa = nomeEtapa != "" ? nomeEtapa : StatusProposta.Finalizada.GetTitle();
				else if (configuracaoTarefa != null)
					instancia.NomeEtapa = nomeEtapa != "" ? nomeEtapa : String.Format(StatusProposta.Andamento.GetTitle(), configuracaoTarefa.NomeEtapa);

				instancia.DataFinalizado = dataFinalizacao.HasValue ? dataFinalizacao : null;
				instancia.StatusFluxo = dataFinalizacao.HasValue ? (byte)StatusFluxo.Finalizado : (byte)StatusFluxo.EmAndamento;
				instancia.CodigoInstanciaFluxo = instanciaAtualDoFluxo.CodigoInstanciaFluxo;
				instancia.CodigoFluxo = instanciaAtualDoFluxo.CodigoFluxo;

				//instancia.ErroCancelado = false; //Mudar somente no andar do fluxo
				instancia.AtualizarPropEspecifico(instanciaClone);

				instanciaAtualDoFluxo = instancia;

				if (dataFinalizacao.HasValue || instancia.DataFinalizado.HasValue)//Atualiza flags de controle das aprovacoes atuais
				{
					List<Tarefa> tarefasAtuais = new Tarefa().Consultar(t => t.IdInstanciaFluxo == instancia.IdInstanciaFluxo);
					tarefasAtuais.ForEach(tarefa =>
					{
						tarefa.AprovacoesAtuais = false;
					});
					tarefasAtuais.Atualizar();
				}
			}

			#region [Atualiza informações da etapa e do Estado no item]

			AtualizarItemStatusCancelado(instanciaAtualDoFluxo.CodigoLista, instanciaAtualDoFluxo.CodigoItem, estadoAtual, instanciaAtualDoFluxo.NomeEtapa, instancia);

			#endregion

			return instanciaAtualDoFluxo;
		}

		public static Microsoft.SharePoint.Client.List AtualizarItemStatusCancelado(Guid codigoLista, Int32 codigoItem, String estadoAtual, String Etapa, InstanciaFluxo instancia, Boolean enviarEmailCancelamento = true)
		{
			Microsoft.SharePoint.Client.List lista = PortalWeb.ContextoWebAtual.ObterLista(codigoLista);
			PortalWeb.ContextoWebAtual.SPClient.Load(lista);
			PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

			if (lista.Title != "Aditivos Gerais")
			{
				EntidadePropostaSP itemSP = NegocioComum.ObterProposta(codigoLista, codigoItem);
				if (itemSP != null)
				{
					itemSP.EstadoAtualFluxo = String.IsNullOrEmpty(estadoAtual) ? itemSP.EstadoAtualFluxo : estadoAtual;
					itemSP.Etapa = String.IsNullOrEmpty(Etapa) ? string.Empty : Etapa;
					itemSP.Atualizar(codigoLista);
				}

				//Envia e-mail
				if (estadoAtual == StatusProposta.Reprovada.GetTitle())
					NegocioEmail.EnviarEmailCancelamento(instancia, lista, TipoTemplate.Reprovar); //Envia e-mail de reprovação
				else if (estadoAtual == StatusProposta.Cancelada.GetTitle())
					NegocioEmail.EnviarEmailCancelamento(instancia, lista, TipoTemplate.Cancelar); //Envia e-mail de cancelamento
			}

			return lista;
		}

		public static string PopularEtapaFluxo(Guid codigoLista, int codigoItem, String etapa)
		{
			EntidadePropostaSP proposta = NegocioComum.ObterProposta(codigoLista, codigoItem);
			string vEtapa = string.Empty;
			if (proposta != null)
			{
				vEtapa = String.IsNullOrEmpty(proposta.Etapa)
					? String.Empty
					: proposta.Etapa.Replace(StatusProposta.Andamento.ToString(), etapa)
									.Replace(StatusProposta.Reprovada.ToString(), etapa);
			}
			return vEtapa;
		}
		
		public static InstanciaFluxo FinalizarInstanciaFluxo(this InstanciaFluxo instanciaAtualDoFluxo, DateTime dataFinalizacao, String nomeEtapa = "", String estadoAtual = "")
		{
			return instanciaAtualDoFluxo.AtualizarInstanciaFluxo(DateTime.Now, null, dataFinalizacao, null, nomeEtapa, estadoAtual);
		}

		#endregion

		#region [Histórico Workflow]

		/// <summary>
		/// Retorna um html com o histórico das tarefas
		/// </summary>
		/// <param name="historicoTarefa"></param>
		/// <returns></returns>
		private static String CarregarHistoricoWorkflow(List<Tarefa> historicoTarefa)
		{
			StringBuilder content = new StringBuilder();
			StringWriter writer = new StringWriter(content);
			HtmlTextWriter textWriter = new HtmlTextWriter(writer);

			HtmlTable table = new HtmlTable();
			HtmlTableRow tableRow = null;
			HtmlTableCell tableCell = null;
			//Timeline timelineControl = new Timeline();
			String[] headers = new String[] {
                           "Ação",
                           "Executor",
                           "Tipo",//Outcome
                           "Atribuido em",
                           "Completado em",
                           "Comentários"
                    };

			if (historicoTarefa == null)
				historicoTarefa = new List<Tarefa>();

			tableRow = new HtmlTableRow();

			foreach (String header in headers)
			{
				tableCell = new HtmlTableCell("TH");
				tableCell.Attributes["style"] = "font-family: Arial; font-weight:normal; border:1px #C0C0C0 solid; border-right:0px; padding:5px; background:#DFDFDF; color:#808080;";
				tableCell.InnerText = Microsoft.SharePoint.Client.Utilities.HttpUtility.HtmlEncode(header);

				tableRow.Cells.Add(tableCell);
			}

			table.Rows.Add(tableRow);

			foreach (Tarefa historico in historicoTarefa)
			{
				tableRow = new HtmlTableRow();

				tableCell = new HtmlTableCell();
				tableCell.Attributes["style"] = "font-family: Arial; border:1px #C0C0C0 solid; border-right:0px; border-top:0px; padding:2px 5px; background:#F4F4F4; color:#666666;";
				tableCell.InnerText = Microsoft.SharePoint.Client.Utilities.HttpUtility.HtmlEncode(historico.NomeTarefa);
				tableRow.Cells.Add(tableCell);

				tableCell = new HtmlTableCell();
				tableCell.Attributes["style"] = "font-family: Arial; border:1px #C0C0C0 solid; border-right:0px; border-top:0px; padding:2px 5px; background:#F4F4F4; color:#666666;";
				tableCell.InnerHtml = Microsoft.SharePoint.Client.Utilities.HttpUtility.HtmlEncode(historico.NomeCompletadoPor);
				tableRow.Cells.Add(tableCell);

				tableCell = new HtmlTableCell();
				tableCell.Attributes["style"] = "font-family: Arial; border:1px #C0C0C0 solid; border-right:0px; border-top:0px; padding:2px 5px; background:#F4F4F4; color:#666666;";
				tableCell.InnerText = Microsoft.SharePoint.Client.Utilities.HttpUtility.HtmlEncode(historico.DescricaoAcaoEfetuada);
				tableRow.Cells.Add(tableCell);

				tableCell = new HtmlTableCell();
				tableCell.Attributes["style"] = "font-family: Arial; border:1px #C0C0C0 solid; border-right:0px; border-top:0px; padding:2px 5px; background:#F4F4F4; color:#666666;";
				tableCell.InnerText = Microsoft.SharePoint.Client.Utilities.HttpUtility.HtmlEncode(historico.DataAtribuido.ToString());
				tableRow.Cells.Add(tableCell);

				tableCell = new HtmlTableCell();
				tableCell.Attributes["style"] = "font-family: Arial; border:1px #C0C0C0 solid; border-right:0px; border-top:0px; padding:2px 5px; background:#F4F4F4; color:#666666;";
				tableCell.InnerText = Microsoft.SharePoint.Client.Utilities.HttpUtility.HtmlEncode(historico.DataFinalizado.HasValue ? historico.DataFinalizado.Value.ToString() : String.Empty);
				tableRow.Cells.Add(tableCell);

				tableCell = new HtmlTableCell();
				tableCell.Attributes["style"] = "font-family: Arial; border:1px #C0C0C0 solid; border-right:0px; border-top:0px; padding:2px 5px; background:#F4F4F4; color:#666666;text-align: left;";
				tableCell.InnerHtml = historico.ComentarioAprovacao != null ? historico.ComentarioAprovacao.Replace("\n", "<br/>") : "";
				tableRow.Cells.Add(tableCell);

				table.Rows.Add(tableRow);
			}

			table.Attributes["class"] = "msn-status-tabela";
			table.Attributes["style"] = "font-family: Arial; border-right:1px #C0C0C0 solid; text-align:center;font-size:11px;";
			table.CellPadding = 0;
			table.CellSpacing = 0;

			table.RenderControl(textWriter);

			return content.ToString();
		}

		/// <summary>
		/// Carrega a propriedade HistoricoFluxo com um html contendo o histórico das tarefas aprovadas
		/// </summary>
		/// <param name="instanciaFluxo"></param>
		public static void CarregarHistoricoWorkflow(this InstanciaFluxo instanciaFluxo)
		{
			List<Tarefa> historicoTarefa = new Tarefa().Consultar(t =>
																   t.Ativo
																&& t.IdInstanciaFluxo == instanciaFluxo.IdInstanciaFluxo
																&& t.TarefaCompleta
																&& !String.IsNullOrEmpty(t.LoginCompletadoPor)
																)
												  .OrderByDescending(to => to.DataFinalizado).ToList();

			instanciaFluxo.HistoricoFluxo = CarregarHistoricoWorkflow(historicoTarefa);
		}

		#endregion

		#region [UcFluxosRaizen]

		/// <summary>
		/// Consultar Instancias de fluxos utilizado pela web part
		/// </summary>
		/// <param name="indicePagina"></param>
		/// <param name="registrosPorPagina"></param>
		/// <param name="ordernarPor"></param>
		/// <param name="ordernarDirecao"></param>
		/// <returns></returns>
		public static List<ServicoAgendado> ConsultarServicoAgendado(
			Int32 indicePagina,
			Int32 registrosPorPagina,
			String ordernarPor,
			String ordernarDirecao,
			Boolean? ativo = null,
			String nomeServico = "")
		{
			return DadosServicoAgendado.ConsultarServicoAgendado
					(
					indicePagina,
					registrosPorPagina,
					ordernarPor,
					ordernarDirecao,
					ativo,
					nomeServico);
		}

		#endregion

		#region [ucFluxosRaizen]

		/// <summary>
		/// Consultar Serviços agendados utilizado pela web part
		/// </summary>
		/// <param name="indicePagina"></param>
		/// <param name="registrosPorPagina"></param>
		/// <param name="ordernarPor"></param>
		/// <param name="ordernarDirecao"></param>
		/// <returns></returns>
		public static List<InstanciaFluxo> ConsultarInstanciaFluxo(
			Int32 indicePagina,
			Int32 registrosPorPagina,
			String ordernarPor,
			String ordernarDirecao,
			Boolean? ativo = null,
			Int32? status = null,
			String nomeLista = null,
			Int32? codigoItem = null)
		{
			return DadosInstanciaFluxo.ConsultarInstanciaFluxo
					(
					indicePagina,
					registrosPorPagina,
					ordernarPor,
					ordernarDirecao,
					ativo,
					status,
					nomeLista,
					codigoItem);
		}

		#endregion

		#region [Métodos Auxiliares]

		public static String ObterMensagemErro(WorkflowInstance wfInstance)
		{
			String mensagem = "";
			try
			{
				mensagem = wfInstance.FaultInfo;
			}
			catch
			{
				mensagem = "";
			}
			return mensagem;
		}

		private static String BuscarMensagemErro(Exception ex)
		{
			if (ex == null)
				return String.Empty;

			return ex.InnerException == null ? ex.Message : String.Concat(ex.Message, Environment.NewLine, BuscarMensagemErro(ex.InnerException));
		}

		private static void AdicionarLog(InstanciaFluxo instancia, WorkflowInstance wfInstance = null, Exception ex = null)
		{

			if (ex == null && wfInstance != null)
				ex = new Exception(String.Format("Instância do Fluxo: {0} - Id lista:{1} - Id item:{2} - Erro: {3}",
													  instancia.CodigoInstanciaFluxo.Value,
													  instancia.CodigoLista,
													  instancia.CodigoItem,
													  wfInstance.FaultInfo));
			else
				ex = new Exception(String.Format("Instância do Fluxo: {0} - Id lista:{1} - Id item:{2}",
													  instancia.CodigoInstanciaFluxo.Value,
													  instancia.CodigoLista,
													  instancia.CodigoItem));

			new Log().Inserir("Portal de Fluxos - TimerJob - SincronizarStatusFluxo", "TimerJob - Log de Erro no fluxo", ex);
		}

		private static void AdicionarLog(List<InstanciaFluxo> listaAtualizar, Int32 quantidadeInstancias, Exception ex = null)
		{

			StringBuilder detalheMensagem = new StringBuilder();
			String formatacao = "Id Lista:{0} - Id Itens:{1}";
			foreach (var lista in listaAtualizar.GroupBy(_ => _.CodigoLista).Select(_ => _.First()).ToList())
			{
				String itens = String.Join(",", listaAtualizar.Where(_ => _.CodigoLista == lista.CodigoLista).Select(_ => _.CodigoItem).ToArray());
				detalheMensagem.Append(String.Format(formatacao, lista.CodigoLista, itens));
				detalheMensagem.Append(Environment.NewLine);
			}

			new Log().InserirMensagem("Portal de Fluxos - TimerJob - SincronizarStatusFluxo"
							, "TimerJob"
							, String.Format("Serviço SincronizarStatusFluxo - Ajustado o status de {0} fluxo(s) de {1} instância(s) ativa(s)."
								, listaAtualizar.Count, quantidadeInstancias)
							, detalheMensagem.ToString());
		}

		#endregion
	}
}
