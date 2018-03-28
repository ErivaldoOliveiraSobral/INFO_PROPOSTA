using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo.FichaCadastral
{
    [Serializable]
    public class FichaCadastralFCCD
    {
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> Id { get; set; }

        public string Ibm { get; set; }

        public string Cnpj { get; set; }

        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        public string InscricaoEstadual { get; set; }

        public string FkStatusficha { get; set; }

        public string Statusficha { get; set; }

        public string Telefone1 { get; set; }

        public string Telefone2 { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> FkRepresentante { get; set; }

        public string FkEmpresa { get; set; }

        public string FkOrganizacaovenda { get; set; }

        public string FkBase { get; set; }

        public string FkCanal { get; set; }

        public string Canal { get; set; }

        public string Incoterms { get; set; }

        public string Distancia { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> InicioVendas { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> DataRegularizacaoAnp { get; set; }

        public string FkSegmentomercado { get; set; }

        public string FkCondicaopagamento { get; set; }

        public string FkFaixarecebimento { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<byte> FkTiponegocio { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> FkPlataforma { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<byte> FkPerfil { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> FkCiclosfaturamento { get; set; }

        public string TipoCliente { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<byte> FkTipocliente { get; set; }

        public string PricingZone { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> GapVPower { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> Prazo { get; set; }

        public string ObservacaoDescarga { get; set; }

        public string ObservacaoGeral { get; set; }

        public string CodConceitoPesquisa { get; set; }

        public string NroTelex { get; set; }

        public string FkGrupoempresa { get; set; }

        public string IndOrdemCompraNecessaria { get; set; }

        public string FkGrupocontabil { get; set; }

        public string IndPrincipalClienteContaNacional { get; set; }

        public string FkReferenciageografica { get; set; }

        public string NumCartaoAtlas { get; set; }

        public string FkTipopessoa { get; set; }

        public string CPF { get; set; }

        public string ZonaDistribuicao { get; set; }

        public string FkCtgcambio { get; set; }

        public string FkGrupocliente { get; set; }

        public string FkMoeda { get; set; }

        public string FkGrupopreco { get; set; }

        public string FkClassificacaoCliente { get; set; }

        public string FkTipodomicilio { get; set; }

        public string FkAplicacaoprimaria { get; set; }

        public string FkTipoidentidade { get; set; }

        public string FkSegmentacaobbb { get; set; }

        public string FkPropriedadeCanal { get; set; }

        public string FkGrupocanal { get; set; }

        public string FkTipoServico { get; set; }

        public string FkCompeticaofornecimento { get; set; }

        public string FkDestino { get; set; }

        public string FkRelacionamentoshell { get; set; }

        public string FkContaglobal { get; set; }

        public string FkSetorindustrial { get; set; }

        public string FkClassificacaonace1 { get; set; }

        public string FkClassificacaonace2 { get; set; }

        public string FkTipoloja { get; set; }

        public string FkTipooperacao { get; set; }

        public string FkPropriedade { get; set; }

        public string FkTipolocalizacao { get; set; }

        public string FkTipocontrato { get; set; }

        public string PropostaItens { get; set; }

        public string FkCondicaoexpedicao { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> FkPrioridaderemessa { get; set; }

        public string FkGrupoConta { get; set; }

        public string FkTipoSociedade { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<bool> EnviarAreaFiscal { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<bool> isInclusao { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<bool> isDuplicado { get; set; }

        public string UsuarioInclusao { get; set; }

        public string UsuarioAlteracao { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> DataInclusao { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> DataAlteracao { get; set; }

        public string Generic { get; set; }

        public string PagadorRecebedor { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<bool> IsPossuiParceiro { get; set; }

        public string IbmParceiro { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<bool> IsNaoContribuinte { get; set; }
    }
}