using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace PortalDeFluxos.Core.BLL.Dados
{
    public class DadosRelatorio : BaseDB
    {
        public static DataSet ObterRelatorioTarefas(String periodoInicio, String periodoFim, Boolean consultaPorTarefa, string[] dsHeader)
        {

            #region [Parâmetros]
            List<DbParameter> parametros = new List<DbParameter>()
            {
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@PeriodoInicio",
                        Value = periodoInicio
                    },
                    new SqlParameter()
                    {
                        DbType = System.Data.DbType.String,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@PeriodoFim",
                        Value = periodoFim
                    },    
                     new SqlParameter()
                    {
                        DbType = System.Data.DbType.Boolean,
                        Direction = System.Data.ParameterDirection.Input,
                        ParameterName = "@ConsultaPorTarefa",
                        Value = consultaPorTarefa
                    }    
            };
            #endregion

            return ExecutarProcedureDataSet(PortalWeb.ContextoWebAtual, "spConsultarRelatorioOperacionalTarefa", parametros, dsHeader);

        }

    }
}
