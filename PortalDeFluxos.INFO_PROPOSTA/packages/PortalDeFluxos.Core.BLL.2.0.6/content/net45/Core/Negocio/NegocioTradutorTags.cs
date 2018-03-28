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

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioTradutorTags
    {
        #region [Propriedades]

        #endregion [Fim - Propriedades]

        #region [Atributos]

        private const String _padrao = "{0}.*?";
        private const String _fimTag = "}";

        #endregion [Fim - Atributos]

        #region [Métodos]

        public static void PreencherObjetoTag(Dictionary<TipoTag, Object> objetos, TipoTag tipoTag, Object obj)
        {
            if (objetos.ContainsKey(tipoTag))
                objetos[tipoTag] = obj;
            else
                objetos.Add(tipoTag, obj);
        }

        public static void LimparObjetoTag(Dictionary<TipoTag, Object> objetos, TipoTag tipoTag)
        {
            if (objetos.ContainsKey(tipoTag))
                objetos.Remove(tipoTag);
        }

        internal static String TraduzirTags(Int32 idTarefa, String texto)
        {
            Dictionary<TipoTag, Object> objetos = new Dictionary<TipoTag, Object>();
            Tarefa tarefa = new Tarefa().Obter(idTarefa);
            if (tarefa == null)
                throw new Exception(String.Format(MensagemPortal.TarefaNaoEncontrada.GetTitle(), idTarefa.ToString()));

            InstanciaFluxo instancia = new InstanciaFluxo().Obter(tarefa.IdInstanciaFluxo);
            if (instancia == null)
                throw new Exception(String.Format(MensagemPortal.InstanciaFluxoNaoEncontrada.GetTitle(), idTarefa.ToString()));
            instancia.CarregarHistoricoWorkflow();

            objetos.Add(TipoTag.Fluxo, instancia);

            return TraduzirTags(objetos, texto);
        }



        /// <summary>
        /// Método para traduzir texto com as Tags pré-definidas no TipoTag, Ex. {Item:Title}
        /// </summary>
        /// <param name="texto">Texto com as tags</param>
        /// <param name="objetos">Lista de objetos para o qual iremos traduzir as tagas. Ex. ListItem, List, Tarefas, etc..</param>
        /// <returns>Texto Traduzido</returns>
        internal static String TraduzirTags(Dictionary<TipoTag, Object> objetos, String texto)
        {
            StringBuilder textoTraduzido = new StringBuilder(texto);

            // Cria um dicionário separando as tags por tipo de tag, aonda irá armazenar um dicionário de dados com uma tupla de <tag, valor>
            Dictionary<TipoTag, Dictionary<String, String>> tagsEncontradas = ObterTagsEncontradas(texto);
            tagsEncontradas = TraduzirTags(tagsEncontradas, objetos);

            foreach (TipoTag tipoTag in tagsEncontradas.Keys)
            {
                foreach (String tag in tagsEncontradas[tipoTag].Keys)
                {
                    textoTraduzido.Replace(tag, tagsEncontradas[tipoTag][tag]);
                }
            }

            return textoTraduzido.ToString();
        }

        private static Dictionary<TipoTag, Dictionary<String, String>> TraduzirTags(Dictionary<TipoTag, Dictionary<String, String>> tagsEncontradas, Dictionary<TipoTag, Object> objetos)
        {
            Dictionary<TipoTag, Dictionary<String, String>> tagsTraduzidas = new Dictionary<TipoTag, Dictionary<String, String>>();

            foreach (TipoTag tipoTag in tagsEncontradas.Keys)
            {
                foreach (String tag in tagsEncontradas[tipoTag].Keys)
                {
                    Dictionary<String, String> valoresTags = tagsTraduzidas.TryGetValue(tipoTag);
                    if (valoresTags == null)
                        valoresTags = new Dictionary<String, String>();

                    if (!valoresTags.ContainsKey(tag))
                    {
                        String valorTraduzido = TraduzirTags(tipoTag, tag, tagsEncontradas[tipoTag][tag], objetos);
                        if (!String.IsNullOrEmpty(valorTraduzido))
                        {
                            AdicionarItemDicionario(valoresTags, tag, valorTraduzido == tag ? "" : valorTraduzido);

                            AdicionarItemDicionario(tagsTraduzidas, tipoTag, valoresTags);
                        }
                        else
                        {
                            AdicionarItemDicionario(valoresTags, tag, String.Empty);

                            AdicionarItemDicionario(tagsTraduzidas, tipoTag, valoresTags);
                        }
                    }
                }
            }

            return tagsTraduzidas;
        }

        private static String TraduzirTags(TipoTag tipoTag, String tag, String propriedade, Dictionary<TipoTag, Object> objetos)
        {
            String valor = String.Empty;

            try
            {
                Object obj = ObterObjeto(tipoTag, objetos);

                switch (tipoTag)
                {
                    case TipoTag.Item:
                        valor = ObterValorSP(obj, propriedade);
                        break;

                    default:
                        valor = Reflexao.BuscarValorPropriedade(obj, propriedade).ToString();
                        if (String.IsNullOrEmpty(valor))
                            valor = tag;
                        break;
                }
            }
            catch (Microsoft.SharePoint.Client.PropertyOrFieldNotInitializedException)
            {
                valor = String.Empty;
            }
            catch (System.NullReferenceException)
            {
                valor = String.Empty;
            }

            return valor;
        }

        #endregion [Fim - Métodos]

        #region [Métodos Auxiliares]

        private static String ObterValorSP(Object obj, String propriedade)
        {
            String valor = String.Empty;

            if (((ListItem)obj)[propriedade] is FieldLookupValue)
            {
                valor = (((ListItem)obj)[propriedade] as FieldLookupValue).LookupValue;
            }
            else if (((ListItem)obj)[propriedade] is FieldLookupValue[])
            {
                var lookups = (((ListItem)obj)[propriedade] as FieldLookupValue[]);
                StringBuilder sbValor = new StringBuilder();
                if (lookups != null && lookups.Count() > 0)
                {
                    String separador = String.Empty;
                    foreach (var l in lookups)
                    {
                        sbValor.AppendFormat("{1}{0}", l.LookupValue, separador);
                        separador = ",";
                    }
                }
                valor = sbValor.ToString();
            }
            else
                valor = ((ListItem)obj)[propriedade].ToString();

            return valor;
        }

        private static Object ObterObjeto(TipoTag tipoTag, Dictionary<TipoTag, Object> objetos)
        {
            Object obj = objetos.TryGetValue(tipoTag);

            return obj;
        }

        private static Dictionary<TipoTag, Dictionary<String, String>> ObterTagsEncontradas(String texto)
        {
            Dictionary<TipoTag, Dictionary<String, String>> tagsEncontradas = new Dictionary<TipoTag, Dictionary<String, String>>();
            foreach (TipoTag tipoTag in Enum.GetValues(typeof(TipoTag)))
            {
                foreach (Match match in Regex.Matches(texto, String.Format("{0}{1}", String.Format(_padrao, tipoTag.GetTitle()), _fimTag)))
                {
                    foreach (Capture capture in match.Captures)
                    {
                        Dictionary<String, String> valoresTags = tagsEncontradas.TryGetValue(tipoTag);

                        if (valoresTags == null)
                            valoresTags = new Dictionary<String, String>();

                        // Obtem o valor do campo, Ex: {Item:Title}, retornará Title
                        String campo = ObterCampo(tipoTag, capture.Value);
                        if (String.IsNullOrEmpty(campo))
                            continue;

                        AdicionarItemDicionario(valoresTags, capture.Value, campo);

                        AdicionarItemDicionario(tagsEncontradas, tipoTag, valoresTags);
                    }
                }
            }

            return tagsEncontradas;
        }

        private static String ObterCampo(TipoTag tipoTag, String tag)
        {
            return tag.Replace(tipoTag.GetTitle(), String.Empty).Replace(_fimTag, String.Empty);
        }

        private static void AdicionarItemDicionario<TKey, T>(Dictionary<TKey, T> dictionary, TKey key, T value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        #endregion [Fim - Métodos Auxiliares]
    }
}
