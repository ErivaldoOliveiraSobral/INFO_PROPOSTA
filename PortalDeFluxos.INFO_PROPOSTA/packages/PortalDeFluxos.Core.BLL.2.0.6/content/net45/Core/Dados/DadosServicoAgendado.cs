using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace PortalDeFluxos.Core.BLL.Dados
{
    public class DadosServicoAgendado : BaseDB
    {
        public static List<ServicoAgendado> ConsultarServicoAgendado(
            Int32 indicePagina,
            Int32 registrosPorPagina,
            String ordernarPor,
            String ordernarDirecao,
            Boolean? ativo = null,
            String nomeServico = "")
        {

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                    
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Boolean,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@ativo",
                        Value = ativo
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@nomeServico",
                        Value =  String.IsNullOrEmpty(nomeServico) ? null : nomeServico 
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@indicePagina",
                        Value = indicePagina
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@registrosPorPagina",
                        Value = registrosPorPagina
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@ordernarPor",
                        Value = String.IsNullOrEmpty(ordernarPor) ? null : ordernarPor
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@ordernarDirecao",
                        Value = ordernarDirecao
                    },
                };
            #endregion

            //Lista de e-mails de tarefas escalonadas para envio
            return ExecutarProcedure<ServicoAgendado>(
                PortalWeb.ContextoWebAtual
                , "spConsultarServicoAgendado", dr =>
                {
                    return new ServicoAgendado()
                    {
                        IdServicoAgendado = (Int32)dr["IdServicoAgendado"],
                        NomeAssemblyType = dr["NomeAssemblyType"] != DBNull.Value ? (String)dr["NomeAssemblyType"] : null,
                        DataUltimaExecucao = dr["DataUltimaExecucao"] != DBNull.Value ? (DateTime)dr["DataUltimaExecucao"] : (DateTime?)null,
                        DataProximaExecucao = dr["DataProximaExecucao"] != DBNull.Value ? (DateTime)dr["DataProximaExecucao"] : (DateTime?)null,
                        DescricaoAgenda = dr["DescricaoAgenda"] != DBNull.Value ? (String)dr["DescricaoAgenda"] : null,
                        LoginAlteracao = dr["LoginAlteracao"] != DBNull.Value ? (String)dr["LoginAlteracao"] : null,
                        Ativo = (bool)dr["Ativo"],
                        Logar = dr["Logar"] != DBNull.Value ? (bool)dr["Logar"] : (bool?)null,
                        TotalRecordCount = dr["TotalRecordCount"] != DBNull.Value ? (Int32?)dr["TotalRecordCount"] : null
                    };
                }
                , parametros);
        }
    }
}
