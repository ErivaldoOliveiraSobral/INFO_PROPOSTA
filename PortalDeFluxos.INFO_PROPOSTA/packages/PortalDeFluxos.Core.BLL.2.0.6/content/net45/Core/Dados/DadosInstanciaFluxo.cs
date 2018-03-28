using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace PortalDeFluxos.Core.BLL.Dados
{
    public class DadosInstanciaFluxo : BaseDB
    {
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
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@status",
                        Value =  status == null ? null : status 
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@codigoItem",
                        Value =  codigoItem == null ? null : codigoItem 
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@nomeLista",
                        Value =  nomeLista == null ? null : nomeLista 
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

            //Lista de Instância fluxos baseado nos filtros.
            return ExecutarProcedure<InstanciaFluxo>(
                PortalWeb.ContextoWebAtual
                , "spConsultarFluxosRaizen", dr =>
                {
                    return new InstanciaFluxo()
                    {
                        IdInstanciaFluxo = (Int32)dr["IdInstanciaFluxo"],
                        CodigoItem = (Int32)dr["CodigoItem"],
                        CodigoLista = (Guid)dr["CodigoLista"],
                        NomeLista = (String)dr["NomeLista"],
                        NomeSolicitacao = (String)dr["NomeSolicitacao"],
                        NomeEtapa = dr["NomeEtapa"] != DBNull.Value ? (String)dr["NomeEtapa"] : "",
                        DataAlteracao = dr["DataAlteracao"] != DBNull.Value ? (DateTime)dr["DataAlteracao"] : (DateTime?)null,
                        DataRestartWorkflow = dr["DataRestartWorkflow"] != DBNull.Value ? (DateTime)dr["DataRestartWorkflow"] : (DateTime?)null,
                        StatusFluxo = dr["StatusFluxo"] != DBNull.Value ? (Int32)dr["StatusFluxo"] : 0,
                        NumeroTentativaInicio = dr["NumeroTentativaInicio"] != DBNull.Value ? (Int32)dr["NumeroTentativaInicio"] : 0,
                        Ativo = (bool)dr["Ativo"],
                        TotalRecordCount = dr["TotalRecordCount"] != DBNull.Value ? (Int32?)dr["TotalRecordCount"] : null
                    };
                }
                , parametros);
        }
    }
}
