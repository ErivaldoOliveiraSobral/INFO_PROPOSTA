using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Negocio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace PortalDeFluxos.Core.BLL.Utilitario
{
    /// <summary>
    /// Classe utilizada para controlar rollback Sharepoint (apenas update de item)
    /// </summary>
    public class TransacaoUpdateSP
    {
        private List<EntidadeSP> itens;
        public TransacaoUpdateSP()
        {
            itens = new List<EntidadeSP>();
        }

        public void Adicionar(EntidadeSP item)
        {
            itens.Add((EntidadeSP)item.Clone());
        }

        public void Cancelar()
        {
            foreach (var item in itens)
                item.Atualizar();
        }
    }
}
