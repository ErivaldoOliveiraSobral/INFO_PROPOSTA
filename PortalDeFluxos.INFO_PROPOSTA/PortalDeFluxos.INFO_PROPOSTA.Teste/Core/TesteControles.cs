using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortalDeFluxos.Core.BLL.Negocio;
using PortalDeFluxos.Core.BLL.Modelo;
using System.Collections.Generic;
using PortalDeFluxos.Core.BLL;
using System.Diagnostics;
using Microsoft.SharePoint.Client.WorkflowServices;
using PortalDeFluxos.Core.BLL.Utilitario;
using Iteris;
using Microsoft.SharePoint.Client.Utilities;
using Microsoft.SharePoint.Client;
using Iteris.SharePoint;
using System.Linq;
using System.Globalization;
using Microsoft.SharePoint.Utilities;
using System.Reflection;
using System.Data.Entity.Core.Metadata.Edm;

namespace PortalDeFluxos.Core.Test
{
    [TestClass]
    public class TesteControles
    {
        [TestMethod]
        public void testScale()
        {
            AnoContratual teste = new AnoContratual();
            PropertyInfo p = teste.GetType().GetProperties().First(_ => _.Name == "Bonificacao");
            if (p == null)
                return;
            object[] titulo = p.GetCustomAttributes(typeof(ScaleAttribute), true);
            if (titulo == null || titulo.Length == 0)
                return;
            Int32 scale = ((ScaleAttribute)titulo.GetValue(0)).Scale;
        }

        [TestMethod]
        public void TestTemplate()
        {
            String resultado = NegocioGeradorTT.ControleAspxBd<AnoContratual>();
            AnoContratual entidadeBd = new AnoContratual();
            String teste = entidadeBd.ToString();
        }
    }
}
