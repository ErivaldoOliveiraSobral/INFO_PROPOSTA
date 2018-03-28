using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using Iteris;
using System.Reflection;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioServicoAgendado
    {

        #region [ Properties ]
        public const string ServicoRezoneamento = "PortalDeFluxos.Core.Servicos.Rezoneamento";
        public const string ServicoSincronizarSP2007 = "Raizen.PortalDeFluxos.Servicos.SincronizarSP2007";
        #endregion

        #region [ucServicoAgendado]
        public static void AtivarServicoAgendado(Int32 idServicoAgendado)
        {
            ServicoAgendado servico = new ServicoAgendado().Obter(idServicoAgendado);
            if (servico != null)
            {
                servico.Ativo = !servico.Ativo;
                servico.Atualizar();
            }
        }

        public static void AtivarLogServicoAgendado(Int32 idServicoAgendado)
        {
            ServicoAgendado servico = new ServicoAgendado().Obter(idServicoAgendado);
            if (servico != null)
            {
                servico.Logar = servico.Logar == null ? true : !(bool)servico.Logar;
                servico.Atualizar();
            }
        }

        public static void ExecutarServicoAgendado(Int32 idServicoAgendado)
        {
            ServicoAgendado servico = new ServicoAgendado().Obter(idServicoAgendado);
            if (servico != null)
                ExecutarServicoAgendado(servico, execucaoManual: true);
        }

        public static void ExecutarServicoAgendado(ServicoAgendado servico, String NomeJob = "",
            DateTime? dataProximaExecucao = null, Boolean execucaoManual = false)
        {
            try
            {
                ServicoAgendado servicoClone = (ServicoAgendado)servico.Clone();

                //Cria a instância da classe
                Assembly assembly = Assembly.Load(servico.NomeAssemblyFullName);
                Type type = assembly.GetType(servico.NomeAssemblyType);
                var service = Activator.CreateInstance(type);

                if (service == null)
                    throw new InvalidOperationException(String.Format("Serviço não encontrado {0} - {1}", servico.NomeAssemblyType, servico.NomeAssemblyFullName));
                if (service.GetType().GetMethod("Executar") == null)
                    throw new InvalidOperationException(String.Format("O serviço {0} - {1} - precisa implementar a interface IPortalFluxoServico - Método Executar não encontrado", servico.NomeAssemblyType, servico.NomeAssemblyFullName));

                if (servico.Logar != null && (Boolean)servico.Logar)
                    new Log() { NomeProcesso = "TimerJob", DescricaoOrigem = NomeJob, DescricaoMensagem = String.Format("Iniciado serviço {0}", servico.NomeAssemblyType) }.Inserir();


                // Executa o serviço customizado que está agendado
                if (!servico.NomeAssemblyType.Contains(ServicoSincronizarSP2007)) //Este serviço ainda não mudamos as referencias
                {

                    if (!execucaoManual && !String.IsNullOrWhiteSpace(servico.DescricaoAgenda))
                        servico.DataProximaExecucao = dataProximaExecucao;
                    servico.DataAlteracao = DateTime.Now;
                    servico.DataUltimaExecucao = DateTime.Now;
                    servico.AtualizarPropEspecifico(servicoClone);//Atualiza info do serviço antes de iniciar a execução para que o mesmo não seja executado várias vezes.
                    service.GetType()
                        .GetMethod("Executar")
                        .Invoke(service, new object[] { PortalWeb.ContextoWebAtual.Url });
                }

                if (servico.Logar != null && (Boolean)servico.Logar)
                    new Log() { NomeProcesso = "TimerJob", DescricaoOrigem = NomeJob, DescricaoMensagem = String.Format("Serviço executado com sucesso {0}", servico.NomeAssemblyType) }.Inserir();
            }
            catch (Exception ex)
            {
                new Log().Inserir(NomeJob, String.Format("Error in service {0} - Type: {1} - Assembly: {2}", servico.IdServicoAgendado, servico.NomeAssemblyType, servico.NomeAssemblyFullName), ex);
            }
        }

        /// <summary>
        /// Consultar Serviços agendados utilizado pela web part
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

        public static List<ServicoAgendado> BuscarServicosAgendado(Boolean rezoneamento = false)
        {
            //Busca os serviços que devem ser executados
            if (rezoneamento)
                return new ServicoAgendado().Consultar(i =>
                                                (!i.DataProximaExecucao.HasValue || i.DataProximaExecucao < DateTime.Now)
                                                && i.Ativo
                                                && i.NomeAssemblyType.Equals(ServicoRezoneamento));

            else
                return new ServicoAgendado().Consultar(i =>
                                                (!i.DataProximaExecucao.HasValue || i.DataProximaExecucao < DateTime.Now)
                                                && i.Ativo
                                                && !i.NomeAssemblyType.Equals(ServicoRezoneamento));

        }
        #endregion

    }
}
