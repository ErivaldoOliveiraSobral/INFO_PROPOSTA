using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Negocio;
using PortalDeFluxos.Core.BLL.Utilitario;
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
    public class Serializacao
    {
        #region [ JSON Serialization Methods ]
        public static TObject DeserializeFromJson<TObject>(String json)
        {
            JavaScriptSerializer serializer = null;

            if (String.IsNullOrEmpty(json))
                return default(TObject);

            serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            return serializer.Deserialize<TObject>(json);
        }

        public static String SerializeToJson(Object value)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            return serializer.Serialize(value);
        }
        #endregion

        #region [ XML Serialization Methods ]
        public static TObject DeserializeFromXml<TObject>(String xmlText)
        {
            XmlSerializer serializer = null;
            XmlTextReader xmlReader = null;
            Object item = null;

            if (String.IsNullOrEmpty(xmlText))
                return default(TObject);

            using (StringReader stringReader = new StringReader(xmlText))
            {
                xmlReader = new XmlTextReader(stringReader);
                serializer = new XmlSerializer(typeof(TObject));
                item = serializer.Deserialize(xmlReader);

                if (xmlReader != null)
                    xmlReader.Close();
            }
            return (TObject)item;
        }

        public static String SerializeToXml<TObject>(TObject @object)
        {
            XmlSerializer serializer = null;
            StringBuilder output = null;
            String key = typeof(TObject).FullName;

            if (@object == null)
                return null;

            output = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(output, new XmlWriterSettings() { OmitXmlDeclaration = true }))
            {
                serializer = new XmlSerializer(typeof(TObject));
                serializer.Serialize(writer, @object);
                return output.ToString();
            }
        }

        public static TObject DeserializeFromXmlDataContract<TObject>(String xmlText)
        {
            DataContractSerializer serializer = null;
            XmlTextReader xmlReader = null;

            if (String.IsNullOrEmpty(xmlText))
                return default(TObject);

            using (StringReader stringReader = new StringReader(xmlText))
            {
                xmlReader = new XmlTextReader(stringReader);
                serializer = new DataContractSerializer(typeof(TObject));
                return (TObject)serializer.ReadObject(xmlReader);
            }
        }

        public static String SerializeToXmlDataContract<TObject>(TObject @object)
        {
            DataContractSerializer serializer = null;
            StringBuilder output = null;

            if (@object == null)
                return null;

            output = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(output))
            {
                serializer = new DataContractSerializer(@object.GetType());
                serializer.WriteObject(writer, @object);
            }
            return output.ToString();
        }
        #endregion

        /// <summary>Busca a parametrização no banco, desserializa o objeto e retorna</summary>
        /// <param name="url">Url de contexto</param>
        /// <returns>Retorna a configuração do banco</returns>
        public static List<ConfiguracaoEmailAttachment> CarregarConfiguracoes()
        {
            try
            {
                Parametro p = new Parametro().Obter((int)ParametroEnum.EmailAttachment);
                if (p == null || String.IsNullOrWhiteSpace(p.Valor))
                    return new List<ConfiguracaoEmailAttachment>();

                return Serializacao.DeserializeFromJson<List<ConfiguracaoEmailAttachment>>(p.Valor);
            }
            catch (Exception ex)
            {
                new Log().Inserir("CarregarConfiguracoes", "CarregarConfiguracoes - ConfiguracaoEmailAttachment", ex);
            }

            //Se der erro, retorna um objeto vazio para não tentar novamente
            return new List<ConfiguracaoEmailAttachment>();
        }

        /// <summary>Busca a parametrização no banco, desserializa o objeto e retorna</summary>
        /// <param name="url">Url de contexto</param>
        /// <returns>Retorna a configuração do banco</returns>
        public static List<ConfiguracaoEmailMensagem> CarregarMensagens(PortalWeb pWeb)
        {
            try
            {
                Parametro p = new Parametro().Obter((int)ParametroEnum.EmailMensagem);
                if (p == null || String.IsNullOrWhiteSpace(p.Valor))
                    return new List<ConfiguracaoEmailMensagem>();

                return Serializacao.DeserializeFromJson<List<ConfiguracaoEmailMensagem>>(p.Valor);
            }
            catch (Exception ex)
            {
                new Log().Inserir("CarregarMensagens", "CarregarMensagens - ConfiguracaoEmailMensagem", ex);
            }

            //Se der erro, retorna um objeto vazio para não tentar novamente
            return new List<ConfiguracaoEmailMensagem>();
        }

        /// <summary>Busca a parametrização no banco, desserializa o objeto e retorna</summary>
        /// <param name="url">Url de contexto</param>
        /// <returns>Retorna a configuração do banco - Estrutura comercial de cada lista</returns>
        [Obsolete("Método utilizado apenas para Aditivos Gerais (migração). Utilizar")]
        public static List<DadosComercial> CarregarEstruturaComercial()
        {
            try
            {
                Parametro p = new Parametro().Obter((int)ParametroEnum.EstruturaComercial);
                if (p == null || String.IsNullOrWhiteSpace(p.Valor))
                    return new List<DadosComercial>();

                return Serializacao.DeserializeFromJson<List<DadosComercial>>(p.Valor);
            }
            catch (Exception ex)
            {
                new Log().Inserir("CarregarEstruturaComercial", "CarregarEstruturaComercial - DadosComercial", ex);
            }

            //Se der erro, retorna um objeto vazio para não tentar novamente
            return new List<DadosComercial>();
        }
    }
}
