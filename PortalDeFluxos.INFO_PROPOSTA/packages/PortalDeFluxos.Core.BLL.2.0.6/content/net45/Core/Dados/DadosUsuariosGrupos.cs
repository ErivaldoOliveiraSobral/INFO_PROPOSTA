using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;


namespace PortalDeFluxos.Core.BLL.Dados
{
    public class DadosUsuariosGrupos : BaseDB
    {
        public static List<UsuariosGruposSP> ConsultarGruposEUsuarios(
            Int32 indicePagina,
            Int32 registrosPorPagina,
            String ordernarPor,
            String ordernarDirecao,
            Boolean? ativo = null,
            String nome = null,
            String tipo = null,
            Boolean? possuiTarefa = null)
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
                        ParameterName = "@nome",
                        Value =  nome
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@tipo",
                        Value =  String.IsNullOrEmpty(tipo) ? null : tipo
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Boolean,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@possuiTarefa",
                        Value = possuiTarefa
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
            return ExecutarProcedure<UsuariosGruposSP>(
                PortalWeb.ContextoWebAtual
                , "spConsultarGruposEUsuarios", dr =>
                {
                    return new UsuariosGruposSP()
                    {
                        Id = (Int32)dr["Id"],
                        Nome  = dr["Nome"] != DBNull.Value ? (String)dr["Nome"] : null,
                        Tipo  = dr["Tipo"] != DBNull.Value ? (String)dr["Tipo"] : null,
                        Login = dr["Login"] != DBNull.Value ? (String)dr["Login"] : null,
                        QtdUsuarios = dr["QtdUsuarios"] != DBNull.Value ? (String)dr["QtdUsuarios"] : null,
                        QtdTarefa = dr["QtdTarefa"] != DBNull.Value ? (Int32)dr["QtdTarefa"] : 0,
                        Ativo = dr["Ativo"] != DBNull.Value ? (bool)dr["Ativo"] : false,
                        TotalRecordCount = dr["TotalRecordCount"] != DBNull.Value ? (Int32?)dr["TotalRecordCount"] : null
                    };
                }
                , parametros);
        }

        public static List<UsuarioSP> ConsultarUsuariosGrupo(Int32 idGrupo)
        {

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@idGrupo",
                        Value =  idGrupo
                    }
                };
            #endregion

            //Lista de e-mails de tarefas escalonadas para envio
            return ExecutarProcedure<UsuarioSP>(
                PortalWeb.ContextoWebAtual
                , "spConsultarUsuariosGrupo", dr =>
                {
                    return new UsuarioSP()
                    {
                        IdUsuarioSP = (Int32)dr["Id"],
                        Nome = dr["Nome"] != DBNull.Value ? (String)dr["Nome"] : null,
                        Login = dr["Login"] != DBNull.Value ? (String)dr["Login"] : null,
                        Ativo = dr["Ativo"] != DBNull.Value ? (bool)dr["Ativo"] : false
                    };
                }
                , parametros);
        }

        public static List<TarefasPendentes> ConsultarTarefasPendentesPorLoginOuGrupo(String loginUserIdGrupo)
        {

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                     new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@loginUserIdGrupo",
                        Value =  loginUserIdGrupo
                    }
                };
            #endregion

            //Lista de e-mails de tarefas escalonadas para envio
            return ExecutarProcedure<TarefasPendentes>(
                PortalWeb.ContextoWebAtual
                , "spConsultarTarefasPendentesPorLoginOuGrupo", dr =>
                {
                    return new TarefasPendentes()
                    {
                        IdTarefa = dr["IdTarefa"] != DBNull.Value ? (Int32?)dr["IdTarefa"] : null,
                        CodigoLista = dr["CodigoLista"] != DBNull.Value ? (Guid?)dr["CodigoLista"] : null,
                        CodigoItem = dr["CodigoItem"] != DBNull.Value ? (Int32?)dr["CodigoItem"] : null,
                        NomeSolicitacao = dr["NomeSolicitacao"] != DBNull.Value ? (String)dr["NomeSolicitacao"] : null,
                        NomeFluxo = dr["NomeFluxo"] != DBNull.Value ? (String)dr["NomeFluxo"] : null,
                        NomeResponsavel = dr["NomeResponsavel"] != DBNull.Value ? (String)dr["NomeResponsavel"] : null,
                        NomeEtapa = dr["NomeEtapa"] != DBNull.Value ? (String)dr["NomeEtapa"] : null,
                        NomeTarefa = dr["NomeTarefa"] != DBNull.Value ? (String)dr["NomeTarefa"] : null,
                        DataInclusao = dr["DataInclusao"] != DBNull.Value ? (String)dr["DataInclusao"] : null,
                        DataSLA = dr["DataSLA"] != DBNull.Value ? (String)dr["DataSLA"] : null,
                        NomeLista = dr["NomeLista"] != DBNull.Value ? (String)dr["NomeLista"] : null,
                        DescricaoUrlTarefa = dr["DescricaoUrlTarefa"] != DBNull.Value ? (String)dr["DescricaoUrlTarefa"] : null,
                        Ambiente2007 = dr["Ambiente2007"] != DBNull.Value ? (Boolean)dr["Ambiente2007"] : false
                    };
                }
                , parametros);
        }
    }
}
