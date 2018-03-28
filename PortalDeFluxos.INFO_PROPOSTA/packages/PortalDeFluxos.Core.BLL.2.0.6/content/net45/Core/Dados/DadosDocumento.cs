using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace PortalDeFluxos.Core.BLL.Dados
{
    public class DadosDocumento : BaseDB
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTipoProposta"></param>
        /// <param name="agrupador"></param>
        /// <returns></returns>
        public static List<Documento> ObterDocumentos(int idTipoProposta, string agrupador)
        {

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@idTipoProposta",
                        Value = idTipoProposta
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@agrupador",
                        Value = agrupador
                    }                  
            };
            #endregion

            return ExecutarProcedure<Documento>(
                PortalWeb.ContextoWebAtual
                , "spObterDocumentos", dr =>
                {
                    return new Documento()
                    {
                        IdDocumento = (Int32)dr["IdDocumento"],
                        Nome = dr["Nome"] != DBNull.Value ? (String)dr["Nome"] : null,
                        Agrupador = dr["Agrupador"] != DBNull.Value ? (String)dr["Agrupador"] : null
                    };
                }
                , parametros);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idItem"></param>
        /// <param name="idTipoProposta"></param>
        /// <param name="agrupador"></param>
        /// <returns></returns>
        public static List<TipoPropostaDocumento> ObterDocumentosProposta(int idItem, int idTipoProposta, string agrupador)
        {

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                 new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@idItem",
                        Value = idItem
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@idTipoProposta",
                        Value = idTipoProposta
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@agrupador",
                        Value = agrupador
                    }                  
            };
            #endregion

            return ExecutarProcedure<TipoPropostaDocumento>(
                PortalWeb.ContextoWebAtual
                , "spObterDocumentosProposta", dr =>
                {
                    return new TipoPropostaDocumento()
                    {
                        IdItem = (Int32)dr["IdItem"],
                        IdTipoProposta = (Int32)dr["IdTipoProposta"],
                        IdTipoPropostaDocumento = (Int32)dr["IdTipoPropostaDocumento"],
                        IdDocumento = (Int32)dr["IdDocumento"],
                        Tem = dr["Tem"] != DBNull.Value ? (Boolean)dr["Tem"] : false,
                        Atende = dr["Atende"] != DBNull.Value ? (Boolean)dr["Atende"] : false,
                        Dispensado = dr["Dispensado"] != DBNull.Value ? (Boolean)dr["Dispensado"] : false,
                        Excecao = dr["Excecao"] != DBNull.Value ? (Boolean)dr["Excecao"] : false,
                        Documento = new Documento() { 
                            Nome = dr["Nome"] != DBNull.Value ? (String)dr["Nome"] : null,
                            Agrupador = dr["Agrupador"] != DBNull.Value ? (String)dr["Agrupador"] : null
                        }
                    };
                }
                , parametros);
        }
    }
}
