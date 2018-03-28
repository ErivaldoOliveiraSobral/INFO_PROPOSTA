using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;


namespace PortalDeFluxos.Core.BLL.Dados
{
    public class DadosPainel : BaseDB
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
            Boolean isAdmin)
        {

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@filtro",
                        Value = filtro
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@filtro2",
                        Value = filtro2
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
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@statusFluxo",
                        Value = statusFluxo == StatusFluxo.Default ? null: (Int32?) statusFluxo
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
                        DbType = System.Data.DbType.Boolean,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@isAdmin",
                        Value = isAdmin
                    }
                };
            #endregion

            //Lista de e-mails de tarefas escalonadas para envio
            return ExecutarProcedure<Solicitacao>(
                PortalWeb.ContextoWebAtual
                , "spConsultarInstanciaFluxo", dr =>
                                                {
                                                    return new Solicitacao()
                                                    {
                                                        CodigoLista = dr["CodigoLista"] != DBNull.Value ? (Guid?)dr["CodigoLista"] : null,
                                                        CodigoItem = dr["CodigoItem"] != DBNull.Value ? (Int32?)dr["CodigoItem"] : null,
                                                        NomeSolicitacao = dr["NomeSolicitacao"] != DBNull.Value ? (String)dr["NomeSolicitacao"] : null,
                                                        NomeFluxo = dr["NomeFluxo"] != DBNull.Value ? (String)dr["NomeFluxo"] : null,
                                                        NomeSolicitante = dr["NomeSolicitante"] != DBNull.Value ? (String)dr["NomeSolicitante"] : null,
                                                        NomeEtapa = dr["NomeEtapa"] != DBNull.Value ? (String)dr["NomeEtapa"] : null,
                                                        StatusFluxo = dr["StatusFluxo"] != DBNull.Value ? (String)dr["StatusFluxo"] : null,
                                                        TotalRecordCount = dr["TotalRecordCount"] != DBNull.Value ? (Int32?)dr["TotalRecordCount"] : null,
                                                        DescricaoUrlItem = dr["DescricaoUrlItem"] != DBNull.Value ? (String)dr["DescricaoUrlItem"] : null,
                                                        NomeGerenteRegiao = dr["NomeGerenteRegiao"] != DBNull.Value ? (String)dr["NomeGerenteRegiao"] : null,
                                                        NomeDiretorVendas = dr["NomeDiretorVendas"] != DBNull.Value ? (String)dr["NomeDiretorVendas"] : null,
                                                    };
                                                }
                , parametros);
        }


        public static List<MinhasTarefasPendente> ConsultarMinhasTarefasPendente(
            String filtro,
            Int32 indicePagina,
            Int32 registrosPorPagina,
            String ordernarPor,
            String ordernarDirecao,
            String login)
        {

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@filtro",
                        Value = filtro
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
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@login",
                        Value = String.IsNullOrEmpty(login) ? null : login
                    }
                };
            #endregion

            //Lista de e-mails de tarefas escalonadas para envio
            return ExecutarProcedure<MinhasTarefasPendente>(
                PortalWeb.ContextoWebAtual
                , "spConsultarMinhasTarefas", dr =>
                {
                    return new MinhasTarefasPendente()
                    {
                        IdTarefa = dr["IdTarefa"] != DBNull.Value ? (Int32?)dr["IdTarefa"] : null,
                        CodigoLista = dr["CodigoLista"] != DBNull.Value ? (Guid?)dr["CodigoLista"] : null,
                        CodigoItem = dr["CodigoItem"] != DBNull.Value ? (Int32?)dr["CodigoItem"] : null,
                        NomeSolicitacao = dr["NomeSolicitacao"] != DBNull.Value ? (String)dr["NomeSolicitacao"] : null,
                        NomeFluxo = dr["NomeFluxo"] != DBNull.Value ? (String)dr["NomeFluxo"] : null,
                        NomeSolicitante = dr["NomeSolicitante"] != DBNull.Value ? (String)dr["NomeSolicitante"] : null,
                        NomeEtapa = dr["NomeEtapa"] != DBNull.Value ? (String)dr["NomeEtapa"] : null,
                        NomeTarefa = dr["NomeTarefa"] != DBNull.Value ? (String)dr["NomeTarefa"] : null,
                        TotalRecordCount = dr["TotalRecordCount"] != DBNull.Value ? (Int32?)dr["TotalRecordCount"] : null,
                        DescricaoUrlItem = dr["DescricaoUrlItem"] != DBNull.Value ? (String)dr["DescricaoUrlItem"] : null,
                        TempoDecorrido = dr["TempoDecorrido"] != DBNull.Value ? (String)dr["TempoDecorrido"] : null,
                        DescricaoUrlTarefa = dr["DescricaoUrlTarefa"] != DBNull.Value ? (String)dr["DescricaoUrlTarefa"] : null,
                        Ambiente2007 = dr["Ambiente2007"] != DBNull.Value ? (Boolean)dr["Ambiente2007"] : false,
                        NomeGerenteRegiao = dr["NomeGerenteRegiao"] != DBNull.Value ? (String)dr["NomeGerenteRegiao"] : null,
                        NomeDiretorVendas = dr["NomeDiretorVendas"] != DBNull.Value ? (String)dr["NomeDiretorVendas"] : null,
                    };
                }
                , parametros);
        }

        public static List<TarefasPendentes> ConsultarTarefasPendentes(
            String filtro,
            Guid codigoLista,
            Int32 codigoItem,
            Int32 indicePagina,
            Int32 registrosPorPagina,
            String ordernarPor,
            String ordernarDirecao
            )
        {

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@filtro",
                        Value = filtro
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Guid,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@CodigoLista",
                        Value = codigoLista
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@CodigoItem",
                        Value = codigoItem
                    }
                    ,
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
                    }
                };
            #endregion

            //Lista de e-mails de tarefas escalonadas para envio
            return ExecutarProcedure<TarefasPendentes>(
                PortalWeb.ContextoWebAtual
                , "spConsultarTarefasPendentes", dr =>
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
                        TotalRecordCount = dr["TotalRecordCount"] != DBNull.Value ? (Int32?)dr["TotalRecordCount"] : null,
                        DataInclusao = dr["DataInclusao"] != DBNull.Value ? (String)dr["DataInclusao"] : null,
                        DataSLA = dr["DataSLA"] != DBNull.Value ? (String)dr["DataSLA"] : null,
                        NomeLista = dr["NomeLista"] != DBNull.Value ? (String)dr["NomeLista"] : null,
                        DescricaoUrlTarefa = dr["DescricaoUrlTarefa"] != DBNull.Value ? (String)dr["DescricaoUrlTarefa"] : null,
                        Ambiente2007 = dr["Ambiente2007"] != DBNull.Value ? (Boolean)dr["Ambiente2007"] : false
                    };
                }
                , parametros);
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

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@filtro",
                        Value = filtro
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Guid,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@CodigoLista",
                        Value = codigoLista
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@CodigoItem",
                        Value = codigoItem
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
                    }
                };
            #endregion

            //Lista de e-mails de tarefas escalonadas para envio
            return ExecutarProcedure<TarefasRealizadas>(
                PortalWeb.ContextoWebAtual
                , "spConsultarTarefasRealizadas", dr =>
                {
                    return new TarefasRealizadas()
                    {
                        IdTarefa = dr["IdTarefa"] != DBNull.Value ? (Int32?)dr["IdTarefa"] : null,
                        CodigoLista = dr["CodigoLista"] != DBNull.Value ? (Guid?)dr["CodigoLista"] : null,
                        CodigoItem = dr["CodigoItem"] != DBNull.Value ? (Int32?)dr["CodigoItem"] : null,
                        NomeSolicitacao = dr["NomeSolicitacao"] != DBNull.Value ? (String)dr["NomeSolicitacao"] : null,
                        NomeFluxo = dr["NomeFluxo"] != DBNull.Value ? (String)dr["NomeFluxo"] : null,
                        NomeCompletadoPor = dr["NomeCompletadoPor"] != DBNull.Value ? (String)dr["NomeCompletadoPor"] : null,
                        NomeEtapa = dr["NomeEtapa"] != DBNull.Value ? (String)dr["NomeEtapa"] : null,
                        NomeTarefa = dr["NomeTarefa"] != DBNull.Value ? (String)dr["NomeTarefa"] : null,
                        TotalRecordCount = dr["TotalRecordCount"] != DBNull.Value ? (Int32?)dr["TotalRecordCount"] : null,
                        DescricaoAcaoEfetuada = dr["DescricaoAcaoEfetuada"] != DBNull.Value ? (String)dr["DescricaoAcaoEfetuada"] : null,
                        ComentarioAprovacao = dr["ComentarioAprovacao"] != DBNull.Value ? (String)dr["ComentarioAprovacao"] : null,
                        DataFinalizado = dr["DataFinalizado"] != DBNull.Value ? (String)dr["DataFinalizado"] : null,
                    };
                }
                , parametros);
        }

        public static String ConsultarTituloDetalheSolicitacao(
            Guid codigoLista,
            Int32 codigoItem)
        {

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Guid,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@listId",
                        Value = codigoLista
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@itemId",
                        Value = codigoItem
                    }
                };
            #endregion

            List<Object> titulos = ExecutarProcedure<object>(
                PortalWeb.ContextoWebAtual
                , "spObterTituloSolicitacao", dr =>
                {
                    return dr["TituloSolicitacao"].ToString();
                }
                , parametros);

            return titulos.Count > 0 ? titulos[0].ToString() : String.Empty;
        }
    }
}
