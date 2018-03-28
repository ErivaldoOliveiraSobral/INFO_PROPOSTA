using Iteris.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortalDeFluxos.Core.BLL;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Negocio;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using Iteris;
using System.Xml;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using PortalDeFluxos.Core.BLL.Dados;
using Microsoft.SharePoint.Client;
using Iteris.SharePoint.Design;

namespace PortalDeFluxos.Core.Test
{

    [TestClass]
    public class TesteEntidadeSP
    {
        [TestMethod]
        public void Teste1()
        {
            using (PortalWeb pweb = new PortalWeb("http://pi"))
            {
                //ListaSP_RaizenConfiguracoesDeFluxo teste1 = new ListaSP_RaizenConfiguracoesDeFluxo().Obter(19);

                //ListaSP_RaizenConfiguracoesDeFluxo configuracaoTarefa = new ListaSP_RaizenConfiguracoesDeFluxo().Obter(47);
                //configuracaoTarefa.Lembretes.CarregarDados();
                //configuracaoTarefa.TemplateDeEmail.CarregarDados();
                //Boolean? enviarPDF = configuracaoTarefa.TemplateDeEmail.Enviarpdf;
            }
        }
    }
}
