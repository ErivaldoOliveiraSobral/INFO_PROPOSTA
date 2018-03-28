using Iteris;
using Iteris.SharePoint;
using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Atributos;
using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Modelo.T4;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioGeradorTT
    {
        public static string GenerateTableName(string value)
        {
            return "Lista" + value.ToObjectName();
        }

        public static string GenerateClassName(string value)
        {
            return "ListaSP_" + value.ToObjectName();
        }

        public static string GenerateControlId(string value)
        {
            return "sp_" + value.ToObjectName();
        }

        public static string GenerateBdControlId(string value)
        {
            return "bd_" + value.ToObjectName();
        }

        public static string GenerateControlDivDataId(string value)
        {
            return "dtp_" + value.ToObjectName();
        }

        #region [ConfiguraoModeloListaTT]

        public static List<ConfiguraoModeloLista> ObterListasProcessar(String configuracaoPath)
        {
            String json = System.IO.File.ReadAllText(configuracaoPath);
            return Serializacao.DeserializeFromJson<List<ConfiguraoModeloLista>>(json);
        }

        public static String ObterCamposModelo(String urlSite, ModeloListaSP configuracaoTT)
        {
            StringBuilder camposModelo = new StringBuilder();
            using (PortalWeb pWeb = new PortalWeb(urlSite))
            {
                List<Field> fieldModelo = ObterCamposLista(urlSite, configuracaoTT.NomeLista, configuracaoTT.ListaProposta);
                foreach (Field field in fieldModelo)
                {
                    String campo = ObterConfiguracaoCampo(field, configuracaoTT.ListaProposta, configuracaoTT.NaoReiniciar);
                    if (campo != String.Empty)
                    {
                        camposModelo.Append(campo);
                        camposModelo.Append(Environment.NewLine);
                    }
                }

            }
            return camposModelo.ToString();
        }

        private static List<Field> ObterCamposLista(String urlSite, String nomeLista, Boolean listaProposta)
        {
            List<Field> fieldsProcessar = new List<Field>();

            List listEntidade = BaseSP.ObterLista(nomeLista);
            PortalWeb.ContextoWebAtual.SPClient.Load(listEntidade.Fields);
            PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

            List<String> camposIgnorar = new List<String>();

            foreach (PropertyInfo p in new EntidadeSP().GetType().GetProperties())
            {
                object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                if (titulo == null || titulo.Length == 0)
                    continue;
                camposIgnorar.Add(((InternalNameAttribute)titulo.GetValue(0)).Name);
            }
            if (listaProposta)
                foreach (PropertyInfo p in new EntidadePropostaSP().GetType().GetProperties())
                {
                    object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                    if (titulo == null || titulo.Length == 0)
                        continue;
                    camposIgnorar.Add(((InternalNameAttribute)titulo.GetValue(0)).Name);
                }
            fieldsProcessar = listEntidade.Fields.ToList().Where(i => !camposIgnorar.Contains(i.InternalName)
                && !i.Hidden
                && i.CanBeDeleted).ToList<Field>();

            return fieldsProcessar;
        }

        private static String ObterConfiguracaoCampo(Field field, Boolean listaProposta, List<String> excecaoReinicioAutomatico)
        {
            StringBuilder attributo = new StringBuilder();
            String atributoFormat = "[InternalName(\"{0}\"{1})]";
            String construtorFormat = "public {0} {1} {{ get; set; }}";

            attributo.Append("\t\t");
            attributo.Append(String.Format(atributoFormat, field.InternalName
                , !listaProposta ?
                    "" : excecaoReinicioAutomatico.Contains(field.InternalName) ?
                            "" : ",FieldType.ReiniciarWF"));
            attributo.Append(Environment.NewLine);
            attributo.Append("\t\t");
            attributo.Append(String.Format(construtorFormat, ObterTipo(field), field.InternalName.ToObjectName().Trim()));
            attributo.Append(Environment.NewLine);
            return ObterTipo(field) != String.Empty ? attributo.ToString() : String.Empty;
        }

        private static String ObterTipo(Field field)
        {
            String typeName = String.Empty;

            if (field is FieldText || field is FieldMultiLineText || field is FieldChoice)
                typeName = "String";
            else if (field is FieldMultiChoice)
                typeName = "String[]";
            else if (field is FieldUser)
            {
                if (((FieldUser)field).SelectionMode == FieldUserSelectionMode.PeopleAndGroups)
                {
                    if (((FieldUser)field).AllowMultipleValues)
                        typeName = "List<Grupo>";
                    else
                        typeName = "Grupo";
                }
                else
                {
                    if (((FieldUser)field).AllowMultipleValues)
                        typeName = "List<Usuario>";
                    else
                        typeName = "Usuario";
                }
            }
            else if (field is FieldLookup)
            {
                String sGuidLista = field.SchemaXml.Substring(field.SchemaXml.IndexOf("\"{") + 2, field.SchemaXml.IndexOf("}\"") - field.SchemaXml.IndexOf("\"{") - 2);
                Guid guidLista;
                if (Guid.TryParse(sGuidLista, out guidLista))
                {
                    List listaLookup = BaseSP.ObterLista(guidLista);
                    if (listaLookup != null)
                    {
                        if (((FieldLookup)field).AllowMultipleValues)
                            typeName = "List<" + GenerateClassName(listaLookup.Title) + ">";
                        else
                            typeName = GenerateClassName(listaLookup.Title);
                    }
                    else
                        typeName = guidLista.ToString();
                }
            }
            else
            {
                if (field is FieldNumber)
                {
                    if (!IsDecimal(field))
                        typeName = "Int32";
                    else
                        typeName = "Decimal";
                }
                else if (field is FieldDateTime)
                    typeName = "DateTime";
                else if (field.SchemaXml.IndexOf("Type=\"Boolean\"") > 0)
                    typeName = "Boolean";

                typeName = typeName != String.Empty ? typeName + (field.Required ? "" : "?") : String.Empty;
            }
            return typeName;
        }

        #endregion

        #region [ConfiguraoTemplateAspx]

        public static ConfiguraoTT ObterConfiguracaoTT(String configuracaoPath)
        {
            String json = System.IO.File.ReadAllText(configuracaoPath);
            return Serializacao.DeserializeFromJson<ConfiguraoTT>(json);
        }

        #endregion

        #region [ConfiguraoListaCsAspx]

        public static ConfiguraoTT ObterConfiguraoListaCsAspx(String configuracaoPath)
        {
            String json = System.IO.File.ReadAllText(configuracaoPath);
            return Serializacao.DeserializeFromJson<ConfiguraoTT>(json);
        }

        #region [Lista]

        public static String ObterCamposASPX<T>(String urlSite, List<String> naoObrigatorios)
            where T : EntidadeSP, new()
        {
            StringBuilder camposASPX = new StringBuilder();
            using (PortalWeb pWeb = new PortalWeb(urlSite))
            {
                T entidadeSP = new T();

                List listEntidade = BaseSP.ObterLista(entidadeSP);
                PortalWeb.ContextoWebAtual.SPClient.Load(listEntidade.Fields);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();
                List<String> ucCampos = new List<String>() { "IBM", "SiteCode"
                    , "UtilizaZoneamentoPadrao", "GerenteTerritorio", "GerenteRegiao", "DiretorVendas", "CDR", "GDR" };
                Boolean possuiEstruturaComercial = false;

                foreach (PropertyInfo p in entidadeSP.GetType().GetProperties())
                {
                    object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                    if (titulo == null || titulo.Length == 0)
                        continue;
                    if (ucCampos.Contains(((InternalNameAttribute)titulo.GetValue(0)).Name))
                    {
                        possuiEstruturaComercial = true;
                        break;
                    }
                }

                if (possuiEstruturaComercial)
                {
                    camposASPX.Append("\t\t\t<div class=\"row\">");
                    camposASPX.Append("\t\t\t\t<div runat=\"server\" ID=\"divBuscarEstruturaComercial_IBM\" style=\"width: 100%\"></div>");
                    camposASPX.Append("\t\t\t</div>");
                    camposASPX.Append(Environment.NewLine);
                    camposASPX.Append("\t\t\t<div class=\"row\">");
                    camposASPX.Append("\t\t\t\t<asp:PlaceHolder runat=\"server\" ID=\"plhEstruturaGt\"></asp:PlaceHolder>");
                    camposASPX.Append("\t\t\t</div>");
                    camposASPX.Append(Environment.NewLine);
                    camposASPX.Append("\t\t\t<div class=\"row\">");
                    camposASPX.Append("\t\t\t\t<asp:PlaceHolder runat=\"server\" ID=\"plhEstruturaGr\"></asp:PlaceHolder>");
                    camposASPX.Append("\t\t\t</div>");
                    camposASPX.Append(Environment.NewLine);
                    camposASPX.Append("\t\t\t<div class=\"row\">");
                    camposASPX.Append("\t\t\t\t<asp:PlaceHolder runat=\"server\" ID=\"plhEstruturaDiretor\"></asp:PlaceHolder>");
                    camposASPX.Append("\t\t\t</div>");
                    camposASPX.Append(Environment.NewLine);
                    camposASPX.Append("\t\t\t<div class=\"row\">");
                    camposASPX.Append("\t\t\t\t<asp:PlaceHolder runat=\"server\" ID=\"plhEstruturaCdr\"></asp:PlaceHolder>");
                    camposASPX.Append("\t\t\t</div>");
                    camposASPX.Append(Environment.NewLine);
                    camposASPX.Append("\t\t\t<div class=\"row\">");
                    camposASPX.Append("\t\t\t\t<asp:PlaceHolder runat=\"server\" ID=\"plhEstruturaGdr\"></asp:PlaceHolder>");
                    camposASPX.Append("\t\t\t</div>");
                    camposASPX.Append(Environment.NewLine);
                }

                foreach (PropertyInfo p in entidadeSP.GetType().GetProperties())
                {
                    object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                    if (titulo == null || titulo.Length == 0)
                        continue;
                    if (((InternalNameAttribute)titulo.GetValue(0)).ReadOnly)
                        continue;
                    if (ucCampos.Contains(((InternalNameAttribute)titulo.GetValue(0)).Name))
                        continue;
                    camposASPX.Append(ObterConfiguracaoCampoAspx(titulo, naoObrigatorios, listEntidade));
                    camposASPX.Append(Environment.NewLine);
                }
            }
            return camposASPX.ToString();
        }

        private static String ObterConfiguracaoCampoAspx(object[] titulo, List<String> naoObrigatorios, List listEntidade)
        {
            StringBuilder attributo = new StringBuilder();

            String divrow = "\t\t<div class=\"row\">";
            String div = "\t\t<div class=\"col-xs-12 col-sm-6 col-md-6\">";
            String divClose = "\t\t</div>";
            String displayFormat = "\t\t\t<span>{0}</span>";
            String reiniciarWfClass = ((InternalNameAttribute)titulo.GetValue(0)).Type == Atributos.FieldType.ReiniciarWF ?
                "reiniciarWF" : "";

            String internalName = ((InternalNameAttribute)titulo.GetValue(0)).Name;
            Field fieldProcessar = listEntidade.Fields.ToList().Where(i => i.InternalName == internalName).FirstOrDefault<Field>();

            attributo.Append(divrow);
            attributo.Append(Environment.NewLine);
            attributo.Append(div);
            attributo.Append(Environment.NewLine);
            attributo.Append(String.Format(displayFormat, fieldProcessar.Title));
            attributo.Append(Environment.NewLine);
            attributo.Append(divClose);
            attributo.Append(Environment.NewLine);
            attributo.Append(div);
            attributo.Append(Environment.NewLine);
            attributo.Append(String.Format("\t\t\t{0}", ObterControleAspx(fieldProcessar, reiniciarWfClass, naoObrigatorios)));
            attributo.Append(Environment.NewLine);
            attributo.Append(divClose);
            attributo.Append(Environment.NewLine);
            attributo.Append(divClose);
            attributo.Append(Environment.NewLine);

            return attributo.ToString();
        }

        private static String ObterControleAspx(Field field, String reiniciarWfClass, List<String> naoObrigatorios)
        {
            #region [Formatação]

            String controleFormatPadrao = "<asp:{0} runat=\"server\" ID=\"{1}\" CssClass=\"{2}\" {3} {4}></asp:{0}>";
			String controleFormatTextAtributos = String.Format("MaxLength=\"{0}\"", ObterMaxLength(field));
            String controleFormatMultiTextAtributos = "TextMode=\"MultiLine\" Rows=\"6\"";
            String controleFormatPeoplePicker = "<SharePoint:ClientPeoplePicker runat=\"server\" ID=\"{0}\" ValidationEnabled=\"true\" VisibleSuggestions=\"3\" Rows=\"1\" AutoFillEnabled=\"True\" Required=\"{1}\"  AllowMultipleEntities=\"{2}\" />";
            String requerido = naoObrigatorios.Contains(field.InternalName) ? "" : "required=\"\"";
            String controleAspx = String.Empty;

            #region [Datepicker]
            String divDataFormat = "<div id=\"{0}\" class=\"input-group date time\">";
            String divDataFechar = "</div>";
            String controleFormatData = "\t<asp:TextBox runat=\"server\" ID=\"{0}\" CssClass=\"form-control datepicker\"></asp:TextBox>";
            StringBuilder glyphiconData = new StringBuilder();
            glyphiconData.Append("\t<span class=\"input-group-addon\">");
            glyphiconData.Append(Environment.NewLine);
            glyphiconData.Append("\t\t<i class=\"glyphicon glyphicon-calendar\"></i>");
            glyphiconData.Append(Environment.NewLine);
            glyphiconData.Append("\t</span>");
            #endregion
            #endregion

            #region [Controles]
            if (field is FieldText)
                controleAspx = String.Format(controleFormatPadrao, "TextBox"
                    , GenerateControlId(field.InternalName), reiniciarWfClass + " form-control", controleFormatTextAtributos, requerido);
            else if (field is FieldMultiLineText)
                controleAspx = String.Format(controleFormatPadrao, "TextBox"
                    , GenerateControlId(field.InternalName), reiniciarWfClass + " form-control", controleFormatMultiTextAtributos, requerido);
            else if (field is FieldChoice)
                controleAspx = String.Format(controleFormatPadrao, "DropDownList"
                    , GenerateControlId(field.InternalName), reiniciarWfClass + " form-control", "type=\"DropDownList\"", requerido);
            else if (field is FieldMultiChoice)
                controleAspx = String.Format(controleFormatPadrao, "CheckBoxList"
                    , GenerateControlId(field.InternalName), reiniciarWfClass + " form-control checkboxlist", "RepeatDirection=\"Vertical\" RepeatLayout=\"Flow\"", requerido);
            else if (field is FieldUser)
            {
                controleAspx = String.Format(controleFormatPeoplePicker, GenerateControlId(field.InternalName)
                           , requerido != "" ? "true" : "false", ((FieldUser)field).AllowMultipleValues ? "true" : "false");
            }
            else if (field is FieldLookup)
            {
                #region [FieldLookup]
                if (((FieldLookup)field).AllowMultipleValues)
                    controleAspx = String.Format(controleFormatPadrao, "CheckBoxList"
                    , GenerateControlId(field.InternalName), reiniciarWfClass + " form-control checkboxlist", "RepeatDirection=\"Vertical\" RepeatLayout=\"Flow\"", requerido);
                else
                    controleAspx = String.Format(controleFormatPadrao, "DropDownList"
                    , GenerateControlId(field.InternalName), reiniciarWfClass + " form-control", "type=\"DropDownList\"", requerido);
                #endregion
            }
            else
            {
                if (field is FieldNumber)
                {
                    #region [Number]
                    if (IsDecimal(field))
                        controleAspx = String.Format(controleFormatPadrao, "TextBox"
                            , GenerateControlId(field.InternalName), reiniciarWfClass + " form-control numero decimal" + ObterScaleSP(field), controleFormatTextAtributos, requerido);
                    else
                        controleAspx = String.Format(controleFormatPadrao, "TextBox"
                            , GenerateControlId(field.InternalName), reiniciarWfClass + " form-control numero int", controleFormatTextAtributos, requerido);
                    #endregion
                }
                else if (field is FieldDateTime)
                {
                    #region [DatePicker]
                    StringBuilder controleData = new StringBuilder();
                    controleData.Append(String.Format(divDataFormat, GenerateControlDivDataId(field.InternalName)));
                    controleData.Append(Environment.NewLine);
                    controleData.Append(String.Format(controleFormatData, GenerateControlId(field.InternalName)));
                    controleData.Append(Environment.NewLine);
                    controleData.Append(glyphiconData.ToString());
                    controleData.Append(Environment.NewLine);
                    controleData.Append(divDataFechar);
                    #endregion
                    controleAspx = controleData.ToString();
                }
                else if (field.SchemaXml.IndexOf("Type=\"Boolean\"") > 0)
                    controleAspx = String.Format(controleFormatPadrao, "CheckBox"
                            , GenerateControlId(field.InternalName), reiniciarWfClass + " checkbox checkboxlist spanOverflow"
                            , String.Format("Text=\"{0}\"", field.Title), "");
            }
            #endregion

            return controleAspx;
        }

		public static string ObterScaleSP(Field field)
		{
			String scale = "0";

			if (field is FieldNumber)
			{
				Dictionary<String, String> dic = ((XObject)XDocument.Parse(field.SchemaXml).Elements().FirstOrDefault()).Document.Root.Attributes().ToDictionary(r => r.Name.LocalName, r => r.Value);

				if (dic.ContainsKey("Decimals"))
					scale = dic["Decimals"];
				else
					scale = "5";
			}

			return scale;
		}

		public static string ObterMaxLength(Field field)
		{
			String maxLength = "0";
			Dictionary<String, String> dic = ((XObject)XDocument.Parse(field.SchemaXml).Elements().FirstOrDefault()).Document.Root.Attributes().ToDictionary(r => r.Name.LocalName, r => r.Value);

			if (dic.ContainsKey("MaxLength"))
				maxLength = dic["MaxLength"];
			else
				maxLength = "255";

			return maxLength;

		}
        #endregion

        #region [BD]

        public static String ControleAspxBd<T>()
             where T : EntidadeDB, new()
        {
            T entidadeBd = new T();
            StringBuilder camposASPX = new StringBuilder();

            foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            {
                camposASPX.Append(ObterConfiguracaoCampoAspx(p));
                camposASPX.Append(Environment.NewLine);
            }
            return camposASPX.ToString();
        }

        public static String ObterConfiguracaoCampoAspx(PropertyInfo p)
        {
            StringBuilder attributo = new StringBuilder();

            String divrow = "\t\t<div class=\"row\">";
            String div = "\t\t<div class=\"col-xs-12 col-sm-6 col-md-6\">";
            String divClose = "\t\t</div>";
            String displayFormat = "\t\t\t<span>{0}</span>";
            String reiniciarWfClass = "reiniciarWF";
            String controle = ObterControleAspx(p, reiniciarWfClass);

            attributo.Append(divrow);
            attributo.Append(Environment.NewLine);
            attributo.Append(div);
            attributo.Append(Environment.NewLine);
            attributo.Append(String.Format(displayFormat, p.Name));
            attributo.Append(Environment.NewLine);
            attributo.Append(divClose);
            attributo.Append(Environment.NewLine);
            attributo.Append(div);
            attributo.Append(Environment.NewLine);
            attributo.Append(String.Format("\t\t\t{0}", controle));
            attributo.Append(Environment.NewLine);
            attributo.Append(divClose);
            attributo.Append(Environment.NewLine);
            attributo.Append(divClose);
            attributo.Append(Environment.NewLine);

            return controle != String.Empty ? attributo.ToString() : String.Empty;

        }

        public static String ObterControleAspx(PropertyInfo p, String reiniciarWfClass)
        {
            #region [Formatação]

            String controleFormatPadrao = "<asp:{0} runat=\"server\" ID=\"{1}\" CssClass=\"{2}\" {3} {4}></asp:{0}>";
            String controleFormatTextAtributos = String.Format("MaxLength=\"{0}\"",ObterMaxLength(p));
            String requerido = Nullable.GetUnderlyingType(p.PropertyType) != null ? "" : "required=\"\"";
            String controleAspx = String.Empty;

            #region [Datepicker]
            String divDataFormat = "<div id=\"{0}\" class=\"input-group date time\">";
            String divDataFechar = "</div>";
            String controleFormatData = "\t<asp:TextBox runat=\"server\" ID=\"{0}\" CssClass=\"form-control datepicker\"></asp:TextBox>";
            StringBuilder glyphiconData = new StringBuilder();
            glyphiconData.Append("\t<span class=\"input-group-addon\">");
            glyphiconData.Append(Environment.NewLine);
            glyphiconData.Append("\t\t<i class=\"glyphicon glyphicon-calendar\"></i>");
            glyphiconData.Append(Environment.NewLine);
            glyphiconData.Append("\t</span>");
            #endregion

            #endregion

            #region [Controles]

            if (p.PropertyType == typeof(Nullable<Guid>) || p.PropertyType == typeof(Guid) || p.PropertyType == typeof(string))
                controleAspx = String.Format(controleFormatPadrao, "TextBox"
                   , GenerateBdControlId(p.Name), reiniciarWfClass + " form-control", controleFormatTextAtributos, requerido);
            else if (p.PropertyType == typeof(Nullable<Int32>) || p.PropertyType == typeof(Int32)
                || p.PropertyType == typeof(Nullable<byte>) || p.PropertyType == typeof(byte))
                controleAspx = String.Format(controleFormatPadrao, "TextBox"
                    , GenerateBdControlId(p.Name), reiniciarWfClass + " form-control numero int", controleFormatTextAtributos, requerido);
            else if (p.PropertyType == typeof(Nullable<decimal>) || p.PropertyType == typeof(decimal))
                controleAspx = String.Format(controleFormatPadrao, "TextBox"
                    , GenerateBdControlId(p.Name), String.Format(reiniciarWfClass + " form-control numero {0}", ObterMascaraDecimal(p))
                    , controleFormatTextAtributos, requerido);
            else if (p.PropertyType == typeof(Nullable<System.DateTime>) || p.PropertyType == typeof(System.DateTime))
            {
                #region [DatePicker]
                StringBuilder controleData = new StringBuilder();
                controleData.Append(String.Format(divDataFormat, GenerateControlDivDataId(p.Name)));
                controleData.Append(Environment.NewLine);
                controleData.Append(String.Format(controleFormatData, GenerateBdControlId(p.Name)));
                controleData.Append(Environment.NewLine);
                controleData.Append(glyphiconData.ToString());
                controleData.Append(Environment.NewLine);
                controleData.Append(divDataFechar);
                controleAspx = controleData.ToString();
                #endregion
            }
            else if (p.PropertyType == typeof(Nullable<System.Boolean>) || p.PropertyType == typeof(Boolean))
                controleAspx = String.Format(controleFormatPadrao, "CheckBox"
                    , GenerateBdControlId(p.Name), reiniciarWfClass + " checkbox checkboxlist spanOverflow"
                    , String.Format("Text=\"{0}\"", p.Name), "");
            else if (typeof(IEnumerable).IsAssignableFrom(p.PropertyType) || (p.PropertyType.BaseType).BaseType == typeof(Entidade))
                controleAspx = String.Format(controleFormatPadrao, "DropDownList"
                , GenerateBdControlId(p.Name), reiniciarWfClass + " form-control", "type=\"DropDownList\"", requerido);

            #endregion

            return controleAspx;
        }

        public static String ObterMascaraDecimal(PropertyInfo p)
        {
            String mascara = "";
            if ((p.PropertyType == typeof(Nullable<decimal>) || p.PropertyType == typeof(decimal)))
            {
                object[] scaleAttribute = p.GetCustomAttributes(typeof(ScaleAttribute), true);
                if (scaleAttribute == null || scaleAttribute.Length == 0)
                    mascara = "decimal";
                else
                {
                    Int32 scale = ((ScaleAttribute)scaleAttribute.GetValue(0)).Scale;
                    mascara = String.Format("decimal{0}", scale > 0 ? scale.ToString() : "");
                }
            }
            return mascara;
        }

		public static String ObterMaxLength(PropertyInfo p)
		{
			String maxLength = "";
			if (p.PropertyType == typeof(string))
			{
				object[] maxLengthAttribute = p.GetCustomAttributes(typeof(MaxLengthAttribute), true);
				if (maxLengthAttribute == null)
					maxLength = "255";
				else
					maxLength = ((MaxLengthAttribute)maxLengthAttribute.GetValue(0)).MaxLength.ToString();
			}else if ((p.PropertyType == typeof(Nullable<decimal>) || p.PropertyType == typeof(decimal)))
			{
				Int32 length = 0;

				object[] scaleAttribute = p.GetCustomAttributes(typeof(ScaleAttribute), true);
				object[] precisionAttribute = p.GetCustomAttributes(typeof(PrecisionAttribute), true);

				if (precisionAttribute == null)
					length = 18;
				else
				{
					Int32 precision = ((PrecisionAttribute)precisionAttribute.GetValue(0)).Precision;
					length = precision;
				}

				if (scaleAttribute != null)
				{
					Int32 scale = ((ScaleAttribute)scaleAttribute.GetValue(0)).Scale;
					length += scale < 0 ? Math.Abs(scale) : 0;
				}
				Int32 mascaraElementos = length % 3 > 0 ? length / 3 - 1 : length / 3;
				length += mascaraElementos > 0 ? mascaraElementos : 0;
				maxLength = length.ToString();
			}
			else if ((p.PropertyType == typeof(Nullable<int>) || p.PropertyType == typeof(int)))
				maxLength = "13";//com a máscacara 
			else
				maxLength = "255";
			return maxLength;
		}
        #endregion


        #region [ CS SP]

        public static String PopularCarregarControlesSP(String urlSite, String nomeLista)
        {
            String conteudoMetodo = String.Empty;
            List<String> camposIgnorar = new List<String>();
            List<Field> fieldsProcessar = new List<Field>();

            using (PortalWeb pWeb = new PortalWeb(urlSite))
            {
                List listEntidade = BaseSP.ObterLista(nomeLista);
                PortalWeb.ContextoWebAtual.SPClient.Load(listEntidade.Fields);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

                foreach (PropertyInfo p in new EntidadeSP().GetType().GetProperties())
                {
                    object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                    if (titulo == null || titulo.Length == 0)
                        continue;
                    camposIgnorar.Add(((InternalNameAttribute)titulo.GetValue(0)).Name);
                }

                fieldsProcessar = listEntidade.Fields.ToList().Where(i => !camposIgnorar.Contains(i.InternalName)
                    && !i.Hidden
                    && i.CanBeDeleted).ToList<Field>();
                conteudoMetodo = ObterConteudoCarregarControlesSP(nomeLista, fieldsProcessar);
            }
            return conteudoMetodo;
        }

        private static String ObterConteudoCarregarControlesSP(String nomeLista, List<Field> fieldsProcessar)
        {
            StringBuilder metodo = new StringBuilder();
            String chamadaChoiceSP = "FormHelper.LoadDataSource({0}, new {1}.ObterDataSourceChoiceSP(i => i.{2}));";
            String chamadaLookupSP = "FormHelper.LoadDataSource({0}, new {1}().Consultar().ObterDataSourceLookup(true));";

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;
                if (field is FieldChoice || field is FieldMultiChoice)
                    metodoAtual = String.Format(chamadaChoiceSP, GenerateControlId(field.InternalName), GenerateClassName(nomeLista), field.InternalName.ToObjectName());
                else if (field is FieldLookup && !(field is FieldUser))
                {
                    String sGuidLista = field.SchemaXml.Substring(field.SchemaXml.IndexOf("\"{") + 2, field.SchemaXml.IndexOf("}\"") - field.SchemaXml.IndexOf("\"{") - 2);
                    Guid guidLista;
                    if (Guid.TryParse(sGuidLista, out guidLista))
                    {
                        List listaLookup = BaseSP.ObterLista(guidLista);
                        if (listaLookup != null)
                            metodoAtual = String.Format(chamadaLookupSP, GenerateControlId(field.InternalName), GenerateClassName(listaLookup.Title));
                    }
                }

                if (metodoAtual != String.Empty)
                {
                    metodo.Append(String.Format("\t{0}", metodoAtual));
                    metodo.Append(Environment.NewLine);
                }
            }

            return metodo.ToString();
        }

        public static String PopularCarregarDadosSP(String urlSite, String nomeLista)
        {
            String conteudoMetodo = String.Empty;
            List<String> camposIgnorar = new List<String>();
            List<Field> fieldsProcessar = new List<Field>();

            using (PortalWeb pWeb = new PortalWeb(urlSite))
            {
                List listEntidade = BaseSP.ObterLista(nomeLista);
                PortalWeb.ContextoWebAtual.SPClient.Load(listEntidade.Fields);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

                foreach (PropertyInfo p in new EntidadeSP().GetType().GetProperties())
                {
                    object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                    if (titulo == null || titulo.Length == 0)
                        continue;
                    camposIgnorar.Add(((InternalNameAttribute)titulo.GetValue(0)).Name);
                }
                List<String> ucCampos = new List<String>() { "IBM", "SiteCode"
                    , "UtilizaZoneamentoPadrao", "GerenteTerritorio", "GerenteRegiao", "DiretorVendas", "CDR", "GDR" };

                fieldsProcessar = listEntidade.Fields.ToList().Where(i => !camposIgnorar.Contains(i.InternalName)
                    && !i.Hidden
                    && i.CanBeDeleted
                    && !ucCampos.Contains(i.InternalName)).ToList<Field>();
                conteudoMetodo = ObterConteudoCarregarDadosSP(nomeLista, fieldsProcessar);
            }
            return conteudoMetodo;
        }

        private static String ObterConteudoCarregarDadosSP(String nomeLista, List<Field> fieldsProcessar)
        {
            StringBuilder metodos = new StringBuilder();
            String objectName = "_proposta" + GenerateClassName(nomeLista);
            #region [Formatação]

            //implamentação antiga
            //String chamadaDropDownList = "FormHelper.SetSelectedValues({0}, new String[] {{ " + objectName + ".{1} != null ? " + objectName + ".{1}.ID.ToString() : String.Empty }});";
            //String chamadaCheckBox = "FormHelper.SetCheckboxValue({0}, " + objectName + ".{1});";
            //String chamadaCheckBoxList = "FormHelper.SetSelectedValues({0}, " + objectName + ".{1});";
            //String chamadaTextBox = "FormHelper.SetTextValues({0}, " + objectName + ".{1});";
            //String chamadaNumber = "FormHelper.SetNumberValues({0}, " + objectName + ".{1}, {2});";
            //String chamadaData = "FormHelper.SetDateValues({0}, " + objectName + ".{1});";
            //String chamadaPeoplePicker = "FormHelper.SetPeoplePickerValues({0}, " + objectName + ".{1});";

            String chamadaSetControl = objectName + ".SetControl({0},_ => _.{1});";//nova implementação
            String chamadaPeoplePicker = objectName + ".{0}.SetControl({1});";
            #endregion

            #region [DropDownList & RadioButtonList]

            metodos.Append("\t#region [DropDownList & RadioButtonList & CheckBoxList]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                metodoAtual = String.Format(chamadaSetControl, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                if ((field is FieldChoice || field is FieldLookup || field is FieldMultiChoice) && !(field is FieldUser))
                    metodoAtual = String.Format(chamadaSetControl, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                else
                    continue;
                //nova implementação
                //if ((field is FieldChoice || field is FieldLookup || field is FieldMultiChoice) && ((FieldLookup)field).AllowMultipleValues && !(field is FieldUser))
                //    metodoAtual = String.Format(chamadaCheckBoxList, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                //else if ((field is FieldChoice || field is FieldLookup || field is FieldMultiChoice) && !(field is FieldUser))
                //    metodoAtual = String.Format(chamadaDropDownList, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                //else
                //    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t\t\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [CheckBox]

            metodos.Append("\t\t#region [CheckBox]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field.SchemaXml.IndexOf("Type=\"Boolean\"") > 0)
                    metodoAtual = String.Format(chamadaSetControl, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                //metodoAtual = String.Format(chamadaCheckBox, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t\t\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [TexBox]

            metodos.Append("\t\t#region [TexBox]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field is FieldText || field is FieldMultiLineText)
                    metodoAtual = String.Format(chamadaSetControl, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                //metodoAtual = String.Format(chamadaTextBox, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t\t\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [Number]

            metodos.Append("\t\t#region [Number]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field is FieldNumber)
                {
                    metodoAtual = String.Format(chamadaSetControl, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                    //if (field.SchemaXml.IndexOf("Decimals=\"2\"") > 0)
                    //    metodoAtual = String.Format(chamadaNumber, GenerateControlId(field.InternalName), field.InternalName.ToObjectName(), "2");
                    //else
                    //    metodoAtual = String.Format(chamadaNumber, GenerateControlId(field.InternalName), field.InternalName.ToObjectName(), "0");
                }
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t\t\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [Data]

            metodos.Append("\t\t#region [Data]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field is FieldDateTime)
                    metodoAtual = String.Format(chamadaSetControl, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                //metodoAtual = String.Format(chamadaData, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t\t\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [PeoplePicker]

            metodos.Append("\t\t#region [PeoplePicker]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field is FieldUser)
                    metodoAtual = String.Format(chamadaPeoplePicker, field.InternalName.ToObjectName(), GenerateControlId(field.InternalName));
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t\t\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t\t#endregion");
            metodos.Append(Environment.NewLine);
            #endregion

            return metodos.ToString();
        }

        public static String PopularSalvarSP(String urlSite, String nomeLista)
        {
            String conteudoMetodo = String.Empty;
            List<String> camposIgnorar = new List<String>();
            List<Field> fieldsProcessar = new List<Field>();
            List<String> ucCampos = new List<String>() { "IBM", "SiteCode"
                    , "UtilizaZoneamentoPadrao", "GerenteTerritorio", "GerenteRegiao", "DiretorVendas", "CDR", "GDR" };

            using (PortalWeb pWeb = new PortalWeb(urlSite))
            {
                List listEntidade = BaseSP.ObterLista(nomeLista);
                PortalWeb.ContextoWebAtual.SPClient.Load(listEntidade.Fields);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

                foreach (PropertyInfo p in new EntidadeSP().GetType().GetProperties())
                {
                    object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                    if (titulo == null || titulo.Length == 0)
                        continue;
                    camposIgnorar.Add(((InternalNameAttribute)titulo.GetValue(0)).Name);
                }

                fieldsProcessar = listEntidade.Fields.ToList().Where(i => !camposIgnorar.Contains(i.InternalName)
                    && !i.Hidden
                    && i.CanBeDeleted
                    && !ucCampos.Contains(i.InternalName)).ToList<Field>();
                conteudoMetodo = ObterConteudoSalvarSP(nomeLista, fieldsProcessar);
            }
            return conteudoMetodo;
        }

        private static String ObterConteudoSalvarSP(String nomeLista, List<Field> fieldsProcessar)
        {
            StringBuilder metodos = new StringBuilder();
            String objectName = "_proposta" + GenerateClassName(nomeLista);
            #region [Formatação]

            //String chamadaChoiceSP = objectName + ".{0} = FormHelper.GetSelectedValue({1});";
            //String chamadaCheckBox = objectName + ".{0} = {1}.Checked;";
            //String chamadaCheckBoxListSP = objectName + ".{0} = FormHelper.GetSelectedValues({1});";
            //String chamadaTextBox = objectName + ".{0} = {1}.Text;";
            //String chamadaDecimal = objectName + ".{0} = FormHelper.GetDecimalValue({1});";
            //String chamadaInt = objectName + ".{0} = FormHelper.GetIntValue({1});";
            //String chamadaData = objectName + ".{0} = FormHelper.GetDateValue({1});";
            //String chamadaPeoplePicker = objectName + ".{0} = FormHelper.GetPeoplePickerValue({1});";
            //String chamadaPeoplePickers = objectName + ".{0} = FormHelper.GetPeoplePickerValues({1});";

            String chamadaCheckBoxListLookup = objectName + ".{0} = FormHelper.GetEntidades<{1}>({2});";
            String chamadaDropDownListLookup = objectName + ".{0} = FormHelper.GetEntidade<{1}>({2});";
            String chamadaLoadProperty = objectName + ".LoadProperty({0},_ => _.{1});";//nova implementação
            String chamadaLoadPropertyUsuario = objectName + ".{0}.LoadProperty({1});";
            #endregion

            #region [DropDownList & RadioButtonList]

            metodos.Append("#region [DropDownList & RadioButtonList]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field is FieldChoice && !(field is FieldMultiChoice))
                    metodoAtual = String.Format(chamadaLoadProperty, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                else if (field is FieldLookup && !((FieldLookup)field).AllowMultipleValues && !(field is FieldUser))
                {
                    String sGuidLista = field.SchemaXml.Substring(field.SchemaXml.IndexOf("\"{") + 2, field.SchemaXml.IndexOf("}\"") - field.SchemaXml.IndexOf("\"{") - 2);
                    Guid guidLista;
                    if (Guid.TryParse(sGuidLista, out guidLista))
                    {
                        List listaLookup = BaseSP.ObterLista(guidLista);
                        if (listaLookup != null)
                            metodoAtual = String.Format(chamadaDropDownListLookup, field.InternalName.ToObjectName(), GenerateClassName(listaLookup.Title), GenerateControlId(field.InternalName));
                    }
                }
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [CheckBoxList - Lookup]

            metodos.Append("\t#region [CheckBoxList]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field is FieldMultiChoice)
                    metodoAtual = String.Format(chamadaLoadProperty, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                else if (field is FieldLookup && ((FieldLookup)field).AllowMultipleValues && !(field is FieldUser))
                {
                    String sGuidLista = field.SchemaXml.Substring(field.SchemaXml.IndexOf("\"{") + 2, field.SchemaXml.IndexOf("}\"") - field.SchemaXml.IndexOf("\"{") - 2);
                    Guid guidLista;
                    if (Guid.TryParse(sGuidLista, out guidLista))
                    {
                        List listaLookup = BaseSP.ObterLista(guidLista);
                        if (listaLookup != null)
                            metodoAtual = String.Format(chamadaCheckBoxListLookup, field.InternalName.ToObjectName(), GenerateClassName(listaLookup.Title), GenerateControlId(field.InternalName));
                    }
                }
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [CheckBox]

            metodos.Append("\t#region [CheckBox]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field.SchemaXml.IndexOf("Type=\"Boolean\"") > 0)
                    metodoAtual = String.Format(chamadaLoadProperty, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [TexBox]

            metodos.Append("\t#region [TexBox]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field is FieldText || field is FieldMultiLineText)
                    metodoAtual = String.Format(chamadaLoadProperty, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [Number]

            metodos.Append("\t#region [Number]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field is FieldNumber)
                {
                    metodoAtual = String.Format(chamadaLoadProperty, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                    //if (field.SchemaXml.IndexOf("Decimals=\"2\"") > 0)
                    //    metodoAtual = String.Format(chamadaDecimal, field.InternalName.ToObjectName(), GenerateControlId(field.InternalName));
                    //else
                    //    metodoAtual = String.Format(chamadaInt, field.InternalName.ToObjectName(), GenerateControlId(field.InternalName));
                }
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [Data]

            metodos.Append("\t#region [Data]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field is FieldDateTime)
                    metodoAtual = String.Format(chamadaLoadProperty, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                //metodoAtual = String.Format(chamadaData, field.InternalName.ToObjectName(), GenerateControlId(field.InternalName));
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [PeoplePicker]

            metodos.Append("\t#region [PeoplePicker]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (Field field in fieldsProcessar)
            {
                String metodoAtual = String.Empty;

                if (field is FieldUser)
                    metodoAtual = String.Format(chamadaLoadPropertyUsuario, GenerateControlId(field.InternalName), field.InternalName.ToObjectName());
                else
                    continue;

                //if (field is FieldUser && !((FieldUser)field).AllowMultipleValues)
                //    metodoAtual = String.Format(chamadaPeoplePicker, field.InternalName.ToObjectName(), GenerateControlId(field.InternalName));
                //else if (field is FieldUser && ((FieldUser)field).AllowMultipleValues)
                //    metodoAtual = String.Format(chamadaPeoplePickers, field.InternalName.ToObjectName(), GenerateControlId(field.InternalName));
                //else
                //    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t#endregion");
            metodos.Append(Environment.NewLine);
            #endregion

            return metodos.ToString();
        }

        #endregion

        #region [ CS BD]

        public static String PopularCarregarControlesBD<T>()
             where T : EntidadeDB, new()
        {
            T entidadeBd = new T();

            StringBuilder metodo = new StringBuilder();
            String chamadaLookup = "//FormHelper.LoadDataSource({0}, new {1}().Consultar().ObterDataSource(c => c.Id, c => c.Descricao, true));";

            foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            {
                String metodoAtual = String.Empty;
                if (p.PropertyType != typeof(string) && (typeof(IEnumerable).IsAssignableFrom(p.PropertyType)
                    || (p.PropertyType.BaseType).BaseType == typeof(Entidade)))
                    metodoAtual = String.Format(chamadaLookup, GenerateBdControlId(p.Name), p.Name.ToObjectName());

                if (metodoAtual != String.Empty)
                {
                    metodo.Append(String.Format("\t{0}", metodoAtual));
                    metodo.Append(Environment.NewLine);
                }
            }

            return metodo.ToString();
        }

        public static String PopularCarregarDadosBD<T>()
             where T : EntidadeDB, new()
        {
            T entidadeBd = new T();
            StringBuilder metodos = new StringBuilder();
            String objectName = "_proposta" + entidadeBd.GetType().Name;

            #region [Formatação]

            //String chamadaDropDownList = "FormHelper.SetSelectedValues({0}, new String[] {{ " + objectName + ".{1} != null ? " + objectName + ".{1}.ID.ToString() : String.Empty }});";
            //String chamadaCheckBox = "FormHelper.SetCheckboxValue({0}, " + objectName + ".{1});";
            //String chamadaTextBox = "FormHelper.SetTextValues({0}, " + objectName + ".{1});";
            //String chamadaNumber = "FormHelper.SetNumberValues({0}, " + objectName + ".{1});";
            //String chamadaData = "FormHelper.SetDateValues({0}, " + objectName + ".{1});";

            String chamadaSetControl = objectName + ".SetControl({0},_ => _.{1});";//nova implementação

            #endregion

            #region [DropDownList]

            metodos.Append("\t#region [DropDownList]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            {
                String metodoAtual = String.Empty;

                if (p.PropertyType != typeof(string) && (typeof(IEnumerable).IsAssignableFrom(p.PropertyType)
                    || (p.PropertyType.BaseType).BaseType == typeof(Entidade)))
                    metodoAtual = String.Format(chamadaSetControl, GenerateBdControlId(p.Name), p.Name.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t\t\t{0}", "//apontar para o id"));
                    metodos.Append(Environment.NewLine);
                    metodos.Append(String.Format("\t\t\t//{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [CheckBox]

            metodos.Append("\t\t#region [CheckBox]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            {
                String metodoAtual = String.Empty;

                if (p.PropertyType == typeof(Nullable<System.Boolean>) || p.PropertyType == typeof(Boolean))
                    metodoAtual = String.Format(chamadaSetControl, GenerateBdControlId(p.Name), p.Name.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t\t\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [TexBox]

            metodos.Append("\t\t#region [TexBox]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            {
                String metodoAtual = String.Empty;

                if (p.PropertyType == typeof(Nullable<Guid>) || p.PropertyType == typeof(Guid) || p.PropertyType == typeof(string))
                    metodoAtual = String.Format(chamadaSetControl, GenerateBdControlId(p.Name), p.Name.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t\t\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [Number]

            metodos.Append("\t\t#region [Number]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            {
                String metodoAtual = String.Empty;

                if (p.PropertyType == typeof(Nullable<decimal>) || p.PropertyType == typeof(decimal)
                    || p.PropertyType == typeof(Nullable<Int32>) || p.PropertyType == typeof(Int32)
                    || p.PropertyType == typeof(Nullable<byte>) || p.PropertyType == typeof(byte))
                    metodoAtual = String.Format(chamadaSetControl, GenerateBdControlId(p.Name), p.Name.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t\t\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [Data]

            metodos.Append("\t\t#region [Data]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            {
                String metodoAtual = String.Empty;

                if (p.PropertyType == typeof(Nullable<System.DateTime>) || p.PropertyType == typeof(System.DateTime))
                    metodoAtual = String.Format(chamadaSetControl, GenerateBdControlId(p.Name), p.Name.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t\t\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            return metodos.ToString();
        }

        public static String PopularSalvarBD<T>()
             where T : EntidadeDB, new()
        {
            T entidadeBd = new T();
            StringBuilder metodos = new StringBuilder();
            String objectName = "_proposta" + entidadeBd.GetType().Name;

            #region [Formatação]

            //String chamadaDropDownListLookup = objectName + ".{0} = FormHelper.GetEntidade<{1}>({2});";
            //String chamadaCheckBox = objectName + "{0} = {1}.Checked;";
            //String chamadaTextBox = objectName + ".{0} = {1}.Text;";
            //String chamadaDecimal = objectName + ".{0} = FormHelper.GetDecimalValue({1});";
            //String chamadaInt = objectName + ".{0} = FormHelper.GetIntValue({1});";
            //String chamadaData = objectName + ".{0} = FormHelper.GetDateValue({1});";

            //String chamadaDropDownListLookup = "//"+objectName + ".{0} = FormHelper.GetEntidade<{1}>({2});";
            String chamadaLoadProperty = objectName + ".LoadProperty({0},_ => _.{1});";//nova implementação

            #endregion

            //#region [DropDownList]

            //metodos.Append("#region [DropDownList]");
            //metodos.Append(Environment.NewLine);
            //metodos.Append(Environment.NewLine);

            //foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            //{
            //    String metodoAtual = String.Empty;

            //    if (p.PropertyType != typeof(string) && (p.PropertyType.BaseType).BaseType == typeof(Entidade))
            //        metodoAtual = String.Format(chamadaDropDownListLookup, p.Name.ToObjectName(), p.PropertyType.GenericTypeArguments[0].Name, GenerateBdControlId(p.Name));
            //    else
            //        continue;

            //    if (metodoAtual != String.Empty)
            //    {
            //        metodos.Append(String.Format("\t{0}", metodoAtual));
            //        metodos.Append(Environment.NewLine);
            //    }
            //}
            //metodos.Append(Environment.NewLine);
            //metodos.Append("\t#endregion");
            //metodos.Append(Environment.NewLine);
            //metodos.Append(Environment.NewLine);

            //#endregion

            #region [CheckBox]

            metodos.Append("\t#region [CheckBox]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            {
                String metodoAtual = String.Empty;

                if (p.PropertyType == typeof(Nullable<System.Boolean>) || p.PropertyType == typeof(Boolean))
                    metodoAtual = String.Format(chamadaLoadProperty, GenerateBdControlId(p.Name), p.Name.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [TexBox]

            metodos.Append("\t#region [TexBox]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            {
                String metodoAtual = String.Empty;

                if (p.PropertyType == typeof(Nullable<Guid>) || p.PropertyType == typeof(Guid) || p.PropertyType == typeof(string))
                    metodoAtual = String.Format(chamadaLoadProperty, GenerateBdControlId(p.Name), p.Name.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [Number]

            metodos.Append("\t#region [Number]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            {
                String metodoAtual = String.Empty;

                if (p.PropertyType == typeof(Nullable<decimal>) || p.PropertyType == typeof(decimal))
                    metodoAtual = String.Format(chamadaLoadProperty, GenerateBdControlId(p.Name), p.Name.ToObjectName());
                else if (p.PropertyType == typeof(Nullable<Int32>) || p.PropertyType == typeof(Int32)
                || p.PropertyType == typeof(Nullable<byte>) || p.PropertyType == typeof(byte))
                    metodoAtual = String.Format(chamadaLoadProperty, GenerateBdControlId(p.Name), p.Name.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            #region [Data]

            metodos.Append("\t#region [Data]");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            foreach (PropertyInfo p in entidadeBd.GetType().GetProperties())
            {
                String metodoAtual = String.Empty;

                if (p.PropertyType == typeof(Nullable<System.DateTime>) || p.PropertyType == typeof(System.DateTime))
                    metodoAtual = String.Format(chamadaLoadProperty, GenerateBdControlId(p.Name), p.Name.ToObjectName());
                else
                    continue;

                if (metodoAtual != String.Empty)
                {
                    metodos.Append(String.Format("\t{0}", metodoAtual));
                    metodos.Append(Environment.NewLine);
                }
            }
            metodos.Append(Environment.NewLine);
            metodos.Append("\t#endregion");
            metodos.Append(Environment.NewLine);
            metodos.Append(Environment.NewLine);

            #endregion

            return metodos.ToString();
        }

        #endregion

        #endregion

        #region [Configuração EventReceiver]

        public static String ObterColunasBD(String urlSite, String nomeLista)
        {
            String colunas = String.Empty;
            List<String> camposIgnorar = new List<String>();
            List<Field> fieldsProcessar = new List<Field>();

            using (PortalWeb pWeb = new PortalWeb(urlSite))
            {
                List listEntidade = BaseSP.ObterLista(nomeLista);
                PortalWeb.ContextoWebAtual.SPClient.Load(listEntidade.Fields);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

                foreach (PropertyInfo p in new EntidadeSP().GetType().GetProperties())
                {
                    object[] titulo = p.GetCustomAttributes(typeof(InternalNameAttribute), true);
                    if (titulo == null || titulo.Length == 0)
                        continue;
                    camposIgnorar.Add(((InternalNameAttribute)titulo.GetValue(0)).Name);
                }

                fieldsProcessar = listEntidade.Fields.ToList().Where(i => !camposIgnorar.Contains(i.InternalName)
                    && !i.Hidden
                    && i.CanBeDeleted).ToList<Field>();
                colunas = ObterConteudoColunas(nomeLista, fieldsProcessar);
            }
            return colunas;
        }

        private static String ObterConteudoColunas(String nomeLista, List<Field> fieldsProcessar)
        {
            StringBuilder colunas = new StringBuilder();

            #region [Formatação]

            String colunaText = "[Descricao{0}] [varchar]({1}) {2}";
            String colunaBit = "[{0}] [bit] {1}";
            String colunaDateTime = "[Data{0}] [datetime] {1}";
            String colunaDecimal = "[Numero{0}] [decimal](18,{1}) {2}";
            String colunaInt = "[Numero{0}] [int] {1}";
            String colunaPeoplePicker = "[Login{0}] [varchar]({1}) {2}";

            #endregion

            #region [Controles]

            foreach (Field field in fieldsProcessar)
            {
                String colunaAtual = String.Empty;

                if (field is FieldText)
                    colunaAtual = String.Format(colunaText, field.InternalName.ToObjectName(), "255", field.Required ? "NOT NULL" : "NULL");
                else if (field is FieldMultiLineText)
                    colunaAtual = String.Format(colunaText, field.InternalName.ToObjectName(), "max", field.Required ? "NOT NULL" : "NULL");
                else if (field is FieldChoice)
                    colunaAtual = String.Format(colunaText, field.InternalName.ToObjectName(), "255", field.Required ? "NOT NULL" : "NULL");
                else if (field is FieldMultiChoice)
                    colunaAtual = String.Format(colunaText, field.InternalName.ToObjectName(), "max", field.Required ? "NOT NULL" : "NULL");
                else if (field is FieldUser)
                {
                    if (((FieldUser)field).AllowMultipleValues)
                        colunaAtual = String.Format(colunaPeoplePicker, field.InternalName.ToObjectName(), "max", field.Required ? "NOT NULL" : "NULL");
                    else
                        colunaAtual = String.Format(colunaPeoplePicker, field.InternalName.ToObjectName(), "255", field.Required ? "NOT NULL" : "NULL");
                }
                else if (field is FieldLookup)
                {
                    if (((FieldLookup)field).AllowMultipleValues)
                        colunaAtual = String.Format(colunaPeoplePicker, field.InternalName.ToObjectName(), "max", field.Required ? "NOT NULL" : "NULL");
                    else
                        colunaAtual = String.Format(colunaPeoplePicker, field.InternalName.ToObjectName(), "255", field.Required ? "NOT NULL" : "NULL");
                }
                else if (field is FieldNumber)
                {
                    if (IsDecimal(field))
                        colunaAtual = String.Format(colunaDecimal, field.InternalName.ToObjectName(), ObterScaleSP(field), field.Required ? "NOT NULL" : "NULL");
                    else
                        colunaAtual = String.Format(colunaInt, field.InternalName.ToObjectName(), field.Required ? "NOT NULL" : "NULL");
                }
                else if (field is FieldDateTime)
                    colunaAtual = String.Format(colunaDateTime, field.InternalName.ToObjectName(), field.Required ? "NOT NULL" : "NULL");
                else if (field.SchemaXml.IndexOf("Type=\"Boolean\"") > 0)
                    colunaAtual = String.Format(colunaBit, field.InternalName.ToObjectName(), field.Required ? "NOT NULL" : "NULL");

                if (colunaAtual != String.Empty)
                {
                    colunas.Append(String.Format("\t{0},", colunaAtual));
                    colunas.Append(Environment.NewLine);
                }
            }


            #endregion

            return colunas.ToString();
        }

        public static Boolean IsDecimal(Field field)
        {
            Boolean isDecimal = true;
            if (field.SchemaXml.IndexOf("Decimals=\"0\"") > 0)
                isDecimal = false;

            return isDecimal;
        }

        #endregion

    }
}
