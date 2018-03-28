using Iteris;
using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint.Client.Utilities;
using Microsoft.SharePoint;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioSincronizacao
    {
        /// <summary>Operações que são sincronizadas</summary>
        public enum Operacao
        {
            Inserir,
            Atualizar,
            Excluir
        }

        #region [ Propriedades ]
        /// <summary>Url do contexto</summary>
        private static String _Url { get; set; }

        /// <summary>Bloqueador para implementação do Singleton</summary>
        private static readonly Object bloqueador = new Object();

        #endregion

        #region [ Métodos auxiliares ]
        /// <summary>Efetua a sincronização da lista</summary>
        /// <param name="properties">Propriedades</param>
        /// <param name="operacao">Operação que deve ser realizada</param>
        public static void SincronizarLista<T>(SPItemEventProperties properties, NegocioSincronizacao.Operacao operacao, Expression<Func<T, Boolean>> filtro = null)
            where T : EntidadeDB, new()
        {
            //Verifica se as propriedades estão preenchidas corretamente
            if (properties == null || properties.Web == null || String.IsNullOrWhiteSpace(properties.Web.Url))
                return;

            //Efetua a sincronização do objeto
            using (PortalWeb pWeb = new PortalWeb(properties.Web.Url))
            {
                try
                {
                    NegocioSincronizacao.SincronizarLista<T>(pWeb, properties, operacao, filtro);
                    NegocioSincronizacao.SincronizarInstanciaFluxo(properties.ListId, properties.ListItemId);
                }
                catch (Exception ex)
                {
                    new Log().Inserir("PortalDeFluxos.Core.ListEventReceiver",
                        String.Format("ListId: {0} - ItemId: {1} - Operação: {2}", properties.ListId, properties.ListItemId, ((int)operacao).ToString())
                        , ex);
                }
            }
        }

        /// <summary>Efetua a sincronizaçao da lista</summary>
        /// <param name="operacao"></param>
        private static void SincronizarLista<T>(PortalWeb pWeb, SPItemEventProperties properties, Operacao operacao, Expression<Func<T, Boolean>> filtro = null)
            where T : EntidadeDB, new()
        {
            //Busca a URL local
            if (String.IsNullOrEmpty(_Url))
                _Url = pWeb.Url;

            List<ConfiguracaoSincronizarLista> configuracao = null;
            Parametro p = new Parametro().Obter((int)ParametroEnum.SincronizarLista);
            if (p == null || String.IsNullOrWhiteSpace(p.Valor))
                configuracao = new List<ConfiguracaoSincronizarLista>();
            else
                configuracao = Serializacao.DeserializeFromJson<List<ConfiguracaoSincronizarLista>>(p.Valor);

            //Verifica se possui alguma configuração 
            if (configuracao == null || configuracao.Count == 0)
                return;

            //Busca a configuração da lista atual
            ConfiguracaoSincronizarLista config = configuracao.FirstOrDefault(i => i.NomeLista.Equals(properties.List.Title, StringComparison.InvariantCultureIgnoreCase));
            if (config == null ||
                String.IsNullOrWhiteSpace(config.NomeTabela) ||
                String.IsNullOrEmpty(config.NomeLista) ||
                config.Mapeamento.Count == 0)
                return;

            //Cria a instância do objeto
            T entidade = filtro != null ? new T().Obter(filtro)
               : new T().Obter(properties.ListItemId);  //Busca no banco mesmo para novos para casos que é uma carga de dados
            Boolean itemExisteDB = entidade != null;

            //Se for exclusão, exclui o item
            if (operacao == Operacao.Excluir)
            {
                if (entidade != null)
                    entidade.Excluir();
                return;
            }

            //Caso não seja informado os dados, não sincroniza
            if (properties.ListItem == null)
                return;

            //Caso o item seja nulo, cria a instância
            if (entidade == null)
                entidade = new T();

            //Busca as propriedades de mapeamento
            PropertyInfo[] propriedadesEntidade = entidade.GetType().GetProperties();
            List<String> propAtualizadas = null;

            //Varre todas as propriedades mapeadas, preenchendo com os valores
            foreach (SPField item in properties.ListItem.Fields)
            {
                //Verifica se a propriedade está mapeada
                if (!config.Mapeamento.ContainsKey(item.InternalName))
                    continue;

                //Busca o nome da coluna e preenche
                String nomeColuna = config.Mapeamento[item.InternalName];
                DefinirValor(pWeb, propriedadesEntidade, entidade, nomeColuna, properties.ListItem[item.Id], ref propAtualizadas, item);
            }

            //Preenche o código da lista e do item
            DefinirValor(pWeb, propriedadesEntidade, entidade, "CodigoItem", properties.ListItemId, ref propAtualizadas);
            DefinirValor(pWeb, propriedadesEntidade, entidade, "CodigoLista", properties.ListId, ref propAtualizadas);

            //Efetua a operação no banco
            if (itemExisteDB)
                entidade.AtualizarPropEspecifico(propAtualizadas);//Atualizar
            else if (operacao == Operacao.Inserir)
                entidade.Inserir();
        }

        /// <summary>Define o valor da propriedade informada no objeto</summary>
        /// <param name="pWeb">Contexto Web</param>
        /// <param name="propriedades">Lista de propriedades</param>
        /// <param name="entidade">Entidade</param>
        /// <param name="nomePropriedade">Nome da propriedade que deve ser preenchida</param>
        /// <param name="valor">Valor</param>
        private static void DefinirValor(PortalWeb pWeb, PropertyInfo[] propriedades, Entidade entidade, string nomePropriedade, object valor, ref List<String> propAtualizadas, SPField campoSP = null)
        {
            PropertyInfo p = propriedades.FirstOrDefault(i => i.Name.Equals(nomePropriedade, StringComparison.InvariantCultureIgnoreCase));
            if (p == null)
                return;

            if (propAtualizadas == null)
                propAtualizadas = new List<string>();
            propAtualizadas.Add(nomePropriedade);

            //Preenche as propriedades do item de acordo com o tipo recebido
            if (valor == null)
                p.SetValue(entidade, null);
            //ENUM
            else if (p.PropertyType.IsEnum)
                p.SetValue(entidade, Enum.Parse(p.PropertyType, (String)valor));
            //PeoplePicker
            else if (campoSP != null && campoSP.FieldValueType == typeof(SPFieldUserValue))
            {
                if (p.PropertyType == typeof(Usuario))
                    p.SetValue(entidade, pWeb.BuscarUsuario(((SPFieldUserValue)valor).LookupId));
                else //Busca somente o nome
                    p.SetValue(entidade, Convert.ChangeType(valor, p.PropertyType));
            }
            //Lookup
            else if (campoSP != null && campoSP.FieldValueType == typeof(SPFieldLookupValue) && p.PropertyType.BaseType == typeof(EntidadeSP))
            {
                SPFieldLookupValue fieldValue = new SPFieldLookupValue(valor.ToString());
                EntidadeSP lookup = (EntidadeSP)Activator.CreateInstance(p.PropertyType);
                lookup.ID = fieldValue.LookupId;
                lookup.Titulo = fieldValue.LookupValue;
                p.SetValue(entidade, lookup);
            }
            //Multi Lookup - List<EntidadeSP>
            else if (campoSP != null &&
                    campoSP.FieldValueType == typeof(SPFieldLookupValue[]) &&
                    p.PropertyType.IsGenericType &&
                    p.PropertyType.GenericTypeArguments[0].BaseType == typeof(EntidadeSP))
            {
                //Busca a coleção de itens
                SPFieldLookupValue[] lookups = valor as SPFieldLookupValue[];

                //Instância uma nova lista
                var tipoLista = typeof(List<>);
                var contrutorLista = tipoLista.MakeGenericType(p.PropertyType.GenericTypeArguments[0]);
                var listaEntidades = (IList)Activator.CreateInstance(contrutorLista);

                //Inclui os itens retornados pelo Sharepoint
                foreach (var lookup in lookups)
                {
                    EntidadeSP entidadeNova = (EntidadeSP)Activator.CreateInstance(p.PropertyType.GenericTypeArguments[0]);
                    entidadeNova.ID = lookup.LookupId;
                    entidadeNova.Titulo = lookup.LookupValue;
                    listaEntidades.Add(entidadeNova);
                }
                p.SetValue(entidade, listaEntidades);
            }
            else //Qualquer outro tipo
            {
                Type type = p.PropertyType.GetGenericArguments().FirstOrDefault();
                if (type == null)
                    type = p.PropertyType;

                p.SetValue(entidade, Convert.ChangeType(valor, type));
            }
        }

        /// <summary>Efetua a sincronizaçao da lista</summary>
        /// <param name="operacao"></param>
        public static void SincronizarInstanciaFluxo(Guid codigoLista,Int32 codigoItem)
        {
            EntidadePropostaSP proposta = NegocioComum.ObterProposta(codigoLista, codigoItem);
            if (proposta == null)
                return;

            InstanciaFluxo instancia = new InstanciaFluxo().Consultar(i => i.CodigoItem == codigoItem
                && i.CodigoLista == codigoLista && i.Ativo == true).FirstOrDefault();

            if (instancia == null)
                return;

            instancia.NomeSolicitacao = proposta.Titulo;
            instancia.LoginCdr = proposta.Cdr != null ? proposta.Cdr.Login : String.Empty;
            instancia.LoginGdr = proposta.Gdr != null ? proposta.Gdr.Login : String.Empty;
            instancia.LoginGerenteTerritorio = proposta.GerenteTerritorio != null ? proposta.GerenteTerritorio.Login : String.Empty;
            instancia.LoginGerenteRegiao = proposta.GerenteRegiao != null ? proposta.GerenteRegiao.Login : String.Empty;
            instancia.LoginDiretorVendas = proposta.DiretorVendas != null ? proposta.DiretorVendas.Login : String.Empty;

            instancia.NomeCdr = proposta.Cdr != null ? proposta.Cdr.Nome : String.Empty;
            instancia.NomeGdr = proposta.Gdr != null ? proposta.Gdr.Nome : String.Empty;
            instancia.NomeGerenteTerritorio = proposta.GerenteTerritorio != null ? proposta.GerenteTerritorio.Nome : String.Empty;
            instancia.NomeGerenteRegiao = proposta.GerenteRegiao != null ? proposta.GerenteRegiao.Nome : String.Empty;
            instancia.NomeDiretorVendas = proposta.DiretorVendas != null ? proposta.DiretorVendas.Nome : String.Empty;

            instancia.EmailCdr = proposta.Cdr != null ? proposta.Cdr.Email : String.Empty;
            instancia.EmailGdr = proposta.Gdr != null ? proposta.Gdr.Email : String.Empty;
            instancia.EmailGerenteTerritorio = proposta.GerenteTerritorio != null ? proposta.GerenteTerritorio.Email : String.Empty;
            instancia.EmailGerenteRegiao = proposta.GerenteRegiao != null ? proposta.GerenteRegiao.Email : String.Empty;
            instancia.EmailDiretorVendas = proposta.DiretorVendas != null ? proposta.DiretorVendas.Email : String.Empty;

            instancia.Atualizar();
        }

        #endregion
    }
}
