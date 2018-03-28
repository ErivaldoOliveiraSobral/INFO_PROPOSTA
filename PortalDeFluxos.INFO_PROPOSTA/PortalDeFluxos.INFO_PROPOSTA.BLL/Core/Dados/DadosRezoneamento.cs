using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;


namespace PortalDeFluxos.Core.BLL.Dados
{
    public class DadosRezoneamento : BaseDB
    {
        public static void ObterEstruturasComerciaisModificadas()
        {
            ExecutarProcedure(
                PortalWeb.ContextoWebAtual
                , "spObterEstruturasComerciaisModificadas"
                , null
                , null);
        }

        /// <summary>
        /// Atualiza a tabela de estrutura comercial local com os dados do Saleforce
        /// </summary>
        public static void AtualizarEstruturaComercial()
        {
            ExecutarProcedure(
                PortalWeb.ContextoWebAtual
                , "spAtualizarEstruturaComercial"
                , null
                , null);
        }

        public static void LimparEstruturaComercialSalesForce()
        {
            ExecutarProcedure(
                PortalWeb.ContextoWebAtual
                , "spLimparEstruturaComercialSalesForce"
                , null
                , null);
        }

        public static void LimparEstruturaComercialModificada()
        {
            ExecutarProcedure(
                PortalWeb.ContextoWebAtual
                , "spLimparEstruturaComercialModificada"
                , null
                , null);
        }
    }
}
