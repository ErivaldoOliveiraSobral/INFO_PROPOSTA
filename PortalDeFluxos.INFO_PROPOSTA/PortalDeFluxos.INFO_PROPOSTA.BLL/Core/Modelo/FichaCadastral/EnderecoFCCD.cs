using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.FichaCadastral
{
    [Serializable]
    public class EnderecoFCCD
    {
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> Id { get; set; }

        public string Cep { get; set; }

        public string Logradouro { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> CaixaPostal { get; set; }

        public string CepPostal { get; set; }

        public string FkCidade { get; set; }

        public string FkRegiao { get; set; }

        public string FkPais { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<byte> FkTipoEndereco { get; set; }
    }
}
