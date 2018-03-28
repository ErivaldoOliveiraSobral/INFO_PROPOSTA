using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.FichaCadastral
{
    [Serializable]
    public class RestricaoFichaCadastralFCCD
    {
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> Id { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> FkRestricao { get; set; }

        public string Restricao { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> FkFichacadastral { get; set; }
    }
}
