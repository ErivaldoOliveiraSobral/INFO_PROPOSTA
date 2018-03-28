using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace PortalDeFluxos.Core.BLL.Dados
{
    public class DadosLog : BaseDB
    {
        /// <summary>Efetua a limpeza do log após x dias</summary>
        /// <param name="intervaloDias">Intervalo de dias</param>
        /// <param name="item"></param>
        public void LimparLog(Log item)
        {
            using (ContextoBanco db = RetornarContextoDB(item.Contexto))
            {
                //Executa a procedure spLimparLog
                db.Database.ExecuteSqlCommand("spLimparLog");
            }
        }

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

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int64,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@id",
                        Value = id
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.DateTime,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@de",
                        Value = de
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.DateTime,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@ate",
                        Value = ate
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Boolean,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@erro",
                        Value = erro
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@origem",
                        Value =  String.IsNullOrEmpty(origem) ? null : origem 
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@processo",
                        Value = String.IsNullOrEmpty(processo) ? null : processo
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@mensagem",
                        Value = String.IsNullOrEmpty(mensagem) ? null : mensagem
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@login",
                        Value = String.IsNullOrEmpty(login) ? null : login 
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
            return ExecutarProcedure<Log>(
                PortalWeb.ContextoWebAtual
                , "spConsultarLog", dr =>
                {
                    return new Log()
                    {
                        IdLog = (Int64)dr["IdLog"],
                        NomeProcesso = dr["NomeProcesso"] != DBNull.Value ? (String)dr["NomeProcesso"] : null,
                        DescricaoOrigem = dr["DescricaoOrigem"] != DBNull.Value ? (String)dr["DescricaoOrigem"] : null,
                        DescricaoMensagem = dr["DescricaoMensagem"] != DBNull.Value ? (String)dr["DescricaoMensagem"] : null,
                        DescricaoDetalhe = dr["DescricaoDetalhe"] != DBNull.Value ? (String)dr["DescricaoDetalhe"] : null,
                        Erro = (bool)dr["Erro"],
                        LoginInclusao = dr["LoginInclusao"] != DBNull.Value ? (String)dr["LoginInclusao"] : null,
                        DataInclusao = (DateTime)dr["DataInclusao"],
                        TotalRecordCount = dr["TotalRecordCount"] != DBNull.Value ? (Int32?)dr["TotalRecordCount"] : null
                    };
                }
                , parametros);
        }
    }
}
