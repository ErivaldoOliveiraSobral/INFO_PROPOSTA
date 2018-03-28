using Microsoft.SharePoint.WebControls;
using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using PortalDeFluxos.Core.BLL.Negocio;
using System.Web;
using Iteris;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Linq.Expressions;
using System.Data;
using PortalDeFluxos.Core.BLL.Core.Modelo;

namespace PortalDeFluxos.Core.BLL.Utilitario
{
	public static class FormHelper
	{
		#region [Load]

		/// <summary>
		/// Utilizado para atrelar o DataSource com controles do tipo  DropDownList/CheckBoxList/RadioButtonList
		/// </summary>
		/// <param name="control"></param>
		/// <param name="dataSource"></param>
		public static void LoadDataSource(object control, Dictionary<String, String> dataSource, Boolean ordenarValue = true, String selectedValue = null)
		{
			if (dataSource == null)
				return;

			if (control is DropDownList)
			{
				((DropDownList)control).DataValueField = "Key";
				((DropDownList)control).DataTextField = "Value";
				((DropDownList)control).DataSource = ordenarValue ? dataSource.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value) : dataSource;
				((DropDownList)control).DataBind();
				if (selectedValue != null)
					((DropDownList)control).SelectedValue = selectedValue;
			}
			else if (control is CheckBoxList)
			{
				((CheckBoxList)control).DataValueField = "Key";
				((CheckBoxList)control).DataTextField = "Value";
				((CheckBoxList)control).DataSource = ordenarValue ? dataSource.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value) : dataSource;
				((CheckBoxList)control).DataBind();
			}
			else if (control is RadioButtonList)
			{
				((RadioButtonList)control).DataValueField = "Key";
				((RadioButtonList)control).DataTextField = "Value";
				((RadioButtonList)control).DataSource = ordenarValue ? dataSource.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value) : dataSource;
				((RadioButtonList)control).DataBind();
				if (selectedValue != null)
					((RadioButtonList)control).SelectedValue = selectedValue;
			}
			else if (control is ListBox)
			{
				((ListBox)control).DataValueField = "Key";
				((ListBox)control).DataTextField = "Value";
				((ListBox)control).DataSource = ordenarValue ? dataSource.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value) : dataSource;
				((ListBox)control).DataBind();
			}
		}

		#endregion

		#region [Set]

		/// <summary>
		/// Seleciona valores nos controles do tipo DropDownList/CheckBoxList/RadioButtonList
		/// </summary>
		/// <param name="control"></param>
		/// <param name="selectedValues"></param>
		public static void SetSelectedValues(object control, String[] selectedValues)
		{
			if (selectedValues == null || selectedValues.Length == 0)
				return;

			if (control is CheckBoxList)
			{
				foreach (ListItem item in ((CheckBoxList)control).Items)
					item.Selected = selectedValues.Contains(item.Value);
			}
			else if (control is ListControl)
				((ListControl)control).SelectedValue = selectedValues[0];

		}

		/// <summary>
		/// Seleciona valores nos controles do tipo DropDownList/CheckBoxList/RadioButtonList - Limpa o datasource e seleciona apenas o valor específico
		/// </summary>
		/// <param name="control"></param>
		/// <param name="selectedValues"></param>
		public static void SetSelectedValues(object control, Usuario usuario)
		{
			Dictionary<String, String> values = new Dictionary<String, String>();

			if (usuario == null)
			{
				LoadDataSource(control, values);
				return;
			}

			values.Add(usuario.Id.ToString(), usuario.Nome);
			LoadDataSource(control, values);

			if (control is CheckBoxList)
			{
				foreach (ListItem item in ((CheckBoxList)control).Items)
					item.Selected = item.Value == usuario.Id.ToString();
			}
			else if (control is ListControl)
				((ListControl)control).SelectedValue = usuario.Id.ToString();

		}

		/// <summary>
		/// Obtém valores nos controles do tipo Checkbox e retorna um Boolean
		/// </summary>
		/// <param name="control"></param>
		/// <param name="selectedValues"></param>
		public static bool GetCheckboxValue(object control)
		{
			bool value = false;
			if (control is CheckBox)
			{
				value = ((CheckBox)control).Checked;
			}
			return value;
		}

		/// <summary>
		/// Seleciona valores nos controles do tipo Checkbox
		/// </summary>
		/// <param name="control"></param>
		/// <param name="selectedValues"></param>
		public static void SetCheckboxValue(object control, Boolean? value)
		{

			if (control is CheckBox)
			{
				((CheckBox)control).Checked = value != null ? (Boolean)value : false;
			}
			else
				((CheckBox)control).Checked = false;
		}

		/// <summary>
		/// Popula controles do tipo número;
		/// </summary>
		/// <param name="control"></param>
		/// <param name="valor"></param>
		public static void SetNumberValues(TextBox control, object valor, Int32 casasDecimais)
		{
			((TextBox)control).Text = valor.ToNumberMask(casasDecimais);
		}

		/// <summary>
		/// Popula controles do tipo número;
		/// </summary>
		/// <param name="control"></param>
		/// <param name="valor"></param>
		public static void SetNumberValues(Label control, object valor, Int32 casasDecimais)
		{
			((Label)control).Text = valor.ToNumberMask(casasDecimais);
		}

		/// <summary>
		/// Popula controles do tipo texto;
		/// </summary>
		/// <param name="control"></param>
		/// <param name="valor"></param>
		public static void SetTextValues(TextBox control, object valor)
		{
			((TextBox)control).Text = valor != null ? valor.ToString() : String.Empty;
		}

		/// <summary>
		/// Popula controles do tipo texto;
		/// </summary>
		/// <param name="control"></param>
		/// <param name="valor"></param>
		public static void SetTextValues(Label control, object valor)
		{
			((Label)control).Text = valor != null ? valor.ToString() : String.Empty;
		}

		/// <summary>
		/// Popula controles do tipo data;
		/// </summary>
		/// <param name="control"></param>
		/// <param name="valor"></param>
		public static void SetDateValues(TextBox control, object valor)
		{
			if (valor is DateTime)
				((TextBox)control).Text = valor != null ? valor.ToString() : String.Empty;
		}

		/// <summary>
		/// Popula controles do tipo peoplePicker;
		/// </summary>
		/// <param name="control"></param>
		/// <param name="valor"></param>
		public static void SetPeoplePickerValues(ClientPeoplePicker control, List<Usuario> usuarios)
		{
			control.AllEntities.Clear();
			control.Validate();
			foreach (Usuario usuario in usuarios)
				SetPeoplePickerValues(control, usuario);
		}

		/// <summary>
		/// Popula controles do tipo peoplePicker;
		/// </summary>
		/// <param name="control"></param>
		/// <param name="valor"></param>
		public static void SetPeoplePickerValues(ClientPeoplePicker control, Usuario usuario)
		{
			if (usuario == null)
			{
				if (control.AllowMultipleEntities == false)
					control.AllEntities.Clear();
				control.Validate();
				return;
			}

			PickerEntity entity = new PickerEntity();

			entity.Key = usuario.Login;
			entity = new PeopleEditor().ValidateEntity(entity);
			entity.IsResolved = true;

			control.AddEntities(new List<PickerEntity> { entity });
		}

		/// <summary>
		/// Popula controles do tipo peoplePicker;
		/// </summary>
		/// <param name="control"></param>
		/// <param name="valor"></param>
		public static void SetPeoplePickerValues(ClientPeoplePicker control, String loginUsuario, String url = "")
		{
			if (control.AllowMultipleEntities == false)
				control.AllEntities.Clear();
			if (String.IsNullOrEmpty(loginUsuario))
				return;

			control.Validate();
			if (loginUsuario.IsEmail() && !String.IsNullOrEmpty(url))
			{
				using (PortalWeb pweb = new PortalWeb(url))
				{
					try
					{
						var usuario = PortalWeb.ContextoWebAtual.BuscarUsuarioPorEmail(loginUsuario);
						loginUsuario = usuario.Login.RemoverClaims();
					}
					catch (Exception ex)
					{
						loginUsuario = String.Empty;
						new Log().Inserir(ex);
					}
				}
			}

			if (String.IsNullOrEmpty(loginUsuario))
			{
				if (control.AllowMultipleEntities == false)
					control.AllEntities.Clear();
				control.Validate();

				return;
			}

			PickerEntity entity = new PickerEntity();
			entity.Key = loginUsuario;
			entity = new PeopleEditor().ValidateEntity(entity);
			entity.IsResolved = true;

			control.AddEntities(new List<PickerEntity> { entity });
		}

		#endregion

		#region [Get]

		public static UsuarioGrupoBase GetPeoplePickerValue(PickerEntity entity)
		{
			UsuarioGrupoBase usuarioGrupo = new UsuarioGrupoBase();
			Int32 id = -1;
			if (entity.EntityData["SPGroupID"] != null && Int32.TryParse(entity.EntityData["SPGroupID"].ToString(), out id))
				usuarioGrupo = PortalWeb.ContextoWebAtual.BuscarGrupo(id);
			else
				usuarioGrupo = PortalWeb.ContextoWebAtual.BuscarUsuarioPorNomeLogin(entity.Key.RemoverClaims());

			return usuarioGrupo;

		}
		public static UsuarioGrupoBase GetPeoplePickerValue(ClientPeoplePicker control)
		{
			UsuarioGrupoBase usuarioGrupo = new UsuarioGrupoBase();
			foreach (PickerEntity entity in control.ResolvedEntities)
			{
				usuarioGrupo = GetPeoplePickerValue(entity);
				break;
			}

			return control.ResolvedEntities != null && control.ResolvedEntities.Count > 0 ? usuarioGrupo : null;
		}

		public static List<UsuarioGrupoBase> GetPeoplePickerValues(ClientPeoplePicker control)
		{
			List<UsuarioGrupoBase> usuariosGrupos = new List<UsuarioGrupoBase>();
			foreach (PickerEntity entity in control.ResolvedEntities)
				usuariosGrupos.Add(GetPeoplePickerValue(entity));
			return usuariosGrupos.Count > 0 ? usuariosGrupos : null;
		}

		public static DateTime? GetDateValue(TextBox control)
		{
			DateTime value = DateTime.MinValue;
			return DateTime.TryParse(control.Text, out value) ? value : (DateTime?)null;
		}

		/// <summary>
		/// Retorna valor sem os caracteres -./
		/// </summary>
		/// <param name="control"></param>
		/// <param name="valor"></param>
		public static String GetTextValuesClean(TextBox control)
		{
			return GetTextValuesClean(control.Text);
		}

		/// <summary>
		/// Retorna valor sem os caracteres -./
		/// </summary>
		/// <param name="control"></param>
		/// <param name="valor"></param>
		public static String GetTextValuesClean(String valor)
		{
			return String.IsNullOrEmpty(valor) ? null : valor.Replace(".", "").Replace("-", "").Replace("/", "").Trim();
		}

		public static Decimal? GetDecimalValue(TextBox control)
		{
			return control.Text.ToDecimal();
		}

		public static Int32? GetIntValue(TextBox control)
		{
			Int32? value = control.Text.ToInt32();
			return value != null ? Convert.ToInt32(value) : (Int32?)null;
		}

		public static Int32? GetIntValue(String textValue)
		{
			Int32? value = textValue.ToInt32();
			return value != null ? Convert.ToInt32(value) : (Int32?)null;
		}

		public static Int64? GetBigIntValue(TextBox control)
		{
			Int64? value = control.Text.ToInt();
			return value != null ? value : (Int64?)null;
		}

		public static Int64? GetBigIntValue(String textValue)
		{
			Int64? value = textValue.ToInt();
			return value != null ? value : (Int64?)null;
		}

		public static String GetSelectedValue(object control)
		{
			String value = String.Empty;

			if (control is ListControl)
				value = ((ListControl)control).SelectedValue;

			return value;
		}

		public static String[] GetSelectedValues(object control)
		{
			String[] values = new String[1];

			if (control is CheckBoxList)
			{
				List<String> lValues = new List<String>();
				foreach (ListItem item in ((CheckBoxList)control).Items)
					if (item.Selected)
						lValues.Add(item.Value);
				values = lValues.ToArray();
			}
			else if (control is ListControl)
				values[0] = ((ListControl)control).SelectedValue;

			return values;
		}

		public static String GetSelectedText(object control)
		{
			String value = String.Empty;

			if (control is ListControl)
				value = ((ListControl)control).SelectedItem != null ? ((ListControl)control).SelectedItem.Text : String.Empty;

			return value;
		}

		public static Boolean GetBooleanValue(DropDownList control)
		{
			Boolean value = false;
			Boolean.TryParse(control.SelectedValue, out value);
			return value;
		}

		public static String GetLogin(Usuario control)
		{
			return control != null ? control.Login : String.Empty;
		}

		public static T GetEntidade<T>(object control)
			where T : EntidadeSP, new()
		{
			String value = GetSelectedValue(control);
			Int32 id = Int32.MinValue;

			if (!Int32.TryParse(value, out id) || id < 0)
				return null;

			T entidade = new T();
			entidade.ID = id;
			return entidade;
		}

		public static List<T> GetEntidades<T>(object control)
			where T : EntidadeSP, new()
		{
			List<T> entidades = new List<T>();

			String[] values = GetSelectedValues(control);

			foreach (String value in values)
			{
				Int32 id = Int32.MinValue;
				if (!Int32.TryParse(value, out id))
					break;
				T entidade = new T();
				entidade.ID = id;
				entidades.Add(entidade);
			}

			return entidades.Count > 0 ? entidades : null;
		}

		public static Dictionary<String, String> GetBooleanDictionary(Boolean emptyValue = false)
		{
			Dictionary<String, String> datasource = new Dictionary<String, String>();

			if (emptyValue)
				datasource.Add("-1", " - Selecione -");

			datasource.Add(Boolean.TrueString, "Sim");
			datasource.Add(Boolean.FalseString, "Não");

			return datasource;
		}

		public static String GetDictionaryValue(Dictionary<String, String> dataSource, String key)
		{
			return key != null && dataSource.ContainsKey(key) ? dataSource[key] : "";
		}

		#endregion

		#region [Métodos auxiliares]

		public static String ObterMascaraDecimal(Int32 casasDecimais = 2)
		{
			String numeroCasas = "0";
			numeroCasas = numeroCasas.PadRight(casasDecimais, '0');
			String formatacao = "{0:0." + numeroCasas + "}";
			return formatacao;
		}

		/// <summary>
		/// Devolve o decimal no formato ex: 123.123,22
		/// Devolve o int no formato ex: 12344
		/// </summary>
		/// <param name="valor"></param>
		/// <param name="casasDecimais"> Numero de casas decimais</param>
		/// <returns></returns>
		public static String GetNumeroFormatado(object valor, Int32 casasDecimais = 2)
		{
			String retorno = "";

			if (valor is Decimal || valor is float || valor is double)
				retorno = valor != null ? String.Format(ObterMascaraDecimal(casasDecimais), valor).Replace(",", ".") : String.Empty;
			else if (valor is Int32 || valor is Int64 || valor is int || valor is Int16)
				retorno = valor != null ? valor.ToString() : String.Empty;

			if (!String.IsNullOrEmpty(retorno))
			{
				String vInt = retorno.Split('.')[0];
				string vNeg = vInt.IndexOf('-') >= 0 ? "-" : "";
				vInt = vInt.Replace("-", "");
				while (vInt.Split('.')[0].Length > 3)
				{
					vInt = Regex.Replace(vInt, @"(\d*)(\d{3})", m => String.Format("{0}{1}{2}"
						, m.Groups[1].Value, m.Groups[1].Value != "" ? "." : "", m.Groups[2].Value));
				}

				if (casasDecimais > 0 && retorno.Split('.').Length > 1)
				{
					String vDec = retorno.Split('.')[1];
					retorno = vNeg + vInt + "," + vDec;
				}
				else
					retorno = vNeg + vInt;
			}

			return retorno;
		}

		public static String GetDataFormatada(object valor, Boolean somenteData = true)
		{
			String retorno = "";

			if (valor is DateTime)
				retorno = valor != null ?
						somenteData ? ((DateTime)valor).ToString("dd/MM/yyyy") : ((DateTime)valor).ToString("dd/MM/yyyy HH:mm")
					: String.Empty;

			return retorno;
		}

		#endregion

		#region [Download]
		public static void Download(String nomeArquivo, DataTable dataTable, Boolean endProcess = true)
		{
			String conteudo = NegocioRelatorio.ObterHtmlTextWriter(dataTable);
			String style = @"<style> td { mso-number-format:'\@' } </style> ";
			Download(nomeArquivo, string.Format("{0} {1}", style, conteudo), endProcess, "application/vnd.ms-excel");
		}

		public static void Download(String nomeArquivo, List<ExcelSheet> tables, Boolean endProcess = true)
		{
			String conteudo = NegocioRelatorio.ObterExcel(tables);
			Download(nomeArquivo, conteudo, endProcess);
		}

		public static void Download(String nomeArquivo, String arquivo, Boolean endProcess = true, String contentType = null)
		{
			HttpContext.Current.Response.Expires = 0;//specify the duration of time before a page cached on a browser expires
			HttpContext.Current.Response.Buffer = true;//specify the property to buffer the output page
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.ClearContent();//erase any buffered HTML output

			if (String.IsNullOrEmpty(contentType))
				HttpContext.Current.Response.ContentType = GetContentType(nomeArquivo);
			else
				HttpContext.Current.Response.ContentType = contentType;

			HttpContext.Current.Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", nomeArquivo));//add a new HTML header and value to the Response sent to the client
			HttpContext.Current.Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");

			HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Unicode;
			HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
			HttpContext.Current.Response.Output.Write(arquivo);
			HttpContext.Current.Response.Flush();
			HttpContext.Current.Response.Close();

			if (endProcess)
				HttpContext.Current.Response.End();//end the processing of the current page to ensure that no other HTML content is sent
		}
		public static void Download(String nomeArquivo, Microsoft.SharePoint.Client.FileInformation fileInformation, Int32 codigoItem = 0)
		{
			byte[] content;
			using (var ms = new MemoryStream())
			{
				var buf = new byte[1024 * 16];
				int byteSize;
				while ((byteSize = fileInformation.Stream.Read(buf, 0, buf.Length)) > 0)
				{
					ms.Write(buf, 0, byteSize);
				}
				content = ms.ToArray();
			}


			HttpContext.Current.Response.Expires = 0;//specify the duration of time before a page cached on a browser expires
			HttpContext.Current.Response.Buffer = true;//specify the property to buffer the output page
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.ClearContent();//erase any buffered HTML output

			HttpContext.Current.Response.ContentType = GetContentType(nomeArquivo);
			HttpContext.Current.Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", nomeArquivo));//add a new HTML header and value to the Response sent to the client
			HttpContext.Current.Response.AddHeader("Content-Length", content.Length.ToString());
			HttpContext.Current.Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
			if (codigoItem > 0)
				HttpContext.Current.Response.AddHeader("Set-Cookie", String.Format("codigoItem={0}; path=/", codigoItem.ToString()));
			HttpContext.Current.Response.BinaryWrite(content);//write specified information of current HTTP output to Byte array
			HttpContext.Current.Response.Flush();
			HttpContext.Current.Response.Close();
			HttpContext.Current.Response.End();//end the processing of the current page to ensure that no other HTML content is sent
		}

		public static string GetContentType(String nomeArquivo)
		{
			String retorno = "application/force-download";
			String[] tipo = nomeArquivo.Split('.');

			if (tipo.Length > 0)
			{
				switch (tipo[1])
				{
					case "xls":
						retorno = "application/vnd.ms-excel";
						break;
					case "xlsx":
						retorno = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
						break;
					case "pdf":
						retorno = "application/pdf";
						break;
					case "doc":
						retorno = "application/msword";
						break;
					case "docx":
						retorno = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
						break;
					case "ppt":
						retorno = "application/vnd.ms-powerpoint";
						break;
					case "pptx":
						retorno = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
						break;
					default:
						retorno = "application/octet-stream";
						break;
				}
			}
			return retorno;
		}

		#endregion

		#region [Helper - Extension]

		public static String ToStringInvariantCulture(this double value)
		{
			return ((Decimal)value).ToString(CultureInfo.InvariantCulture);
		}

		public static String ToStringInvariantCulture(this double? value, Decimal? defaultValue = null)
		{
			String retorno = "";
			if (value != null)
				retorno = ((decimal)(double)value).ToString(CultureInfo.InvariantCulture);
			else if (defaultValue != null)
				retorno = ((decimal)defaultValue).ToString(CultureInfo.InvariantCulture);
			return retorno;
		}

		public static String ToStringInvariantCulture(this Decimal value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		public static String ToStringInvariantCulture(this Decimal? value, Decimal? defaultValue = null)
		{
			String retorno = "";
			if (value != null)
				retorno = ((Decimal)value).ToString(CultureInfo.InvariantCulture);
			else if (defaultValue != null)
				retorno = ((Decimal)defaultValue).ToString(CultureInfo.InvariantCulture);
			return retorno;
		}

		public static String ToStringInvariantCulture(this Int32 value)
		{
			String retorno = "";
			retorno = value.ToString(CultureInfo.InvariantCulture);
			return retorno;
		}

		public static String ToStringInvariantCulture(this Int32? value)
		{
			String retorno = "";
			if (value != null)
				retorno = ((Int32)value).ToString(CultureInfo.InvariantCulture);
			return retorno;
		}

		public static Int32 ToNotNullable(this Int32? value, Int32 defaultValue)
		{
			return value != null ? (Int32)value : defaultValue;
		}

		public static Decimal ToNotNullable(this Decimal? value, Decimal defaultValue)
		{
			return value != null ? (Decimal)value : defaultValue;
		}

		public static Boolean ToNotNullable(this Boolean? value, Boolean defaultValue)
		{
			return value != null ? (Boolean)value : defaultValue;
		}

		#endregion

		#region [Properties/CSS]

		public static PropertyInfo GetProperty<TModel, TValue>(this TModel item, Expression<Func<TModel, TValue>> selector)
			where TModel : Entidade, new()
		{
			Expression body = selector;
			if (body is LambdaExpression)
			{
				body = ((LambdaExpression)body).Body;
			}
			switch (body.NodeType)
			{
				case ExpressionType.MemberAccess:
					return (PropertyInfo)((MemberExpression)body).Member;
				default:
					throw new InvalidOperationException();
			}
		}

		public static String GetPropertyName<TModel, TValue>(this TModel item, Expression<Func<TModel, TValue>> selector)
			where TModel : Entidade, new()
		{
			return item.GetProperty(selector).Name;
		}


		public static String GetMaskCss<TModel, TValue>(this TModel item, Expression<Func<TModel, TValue>> selector)
			where TModel : Entidade, new()
		{
			PropertyInfo p = item.GetProperty(selector);

			String mascara = "form-control";
			if ((p.PropertyType == typeof(Nullable<decimal>) || p.PropertyType == typeof(decimal)))
			{
				object[] precisionAttribute = p.GetCustomAttributes(typeof(ScaleAttribute), true);
				if (precisionAttribute == null || precisionAttribute.Length == 0)
					mascara += " numero intMask";
				else
				{
					Int32 scale = ((ScaleAttribute)precisionAttribute.GetValue(0)).Scale;
					mascara += String.Format(" numero decimal{0}", scale > 0 ? scale.ToString() : "");
				}
			}
			else if ((p.PropertyType == typeof(Nullable<int>) || p.PropertyType == typeof(int)))
				mascara += " numero intMask";

			return mascara;
		}

		#endregion

		#region [Set Control/Load Property]

		#region [Set Control]
		public static void SetControl<T, TProperty>(this T entidade
			, WebControl control
			, Expression<Func<T, TProperty>> propriedade
			, object defaultValue = null
			, Int32 scale = 0)
			where T : Entidade, new()
		{
			if (control is ClientPeoplePicker)
				throw new Exception("Utilizar sobregarga específica para Usuário.");

			PropertyInfo propertyInfo = entidade.GetProperty(propriedade);
			if (propertyInfo == null) return;
			object valorPropriedade = propertyInfo.GetValue(entidade, null);

			if (control is Label)
				((Label)control).Text = valorPropriedade != null ?
					propertyInfo.IsNumber() ? valorPropriedade.ToNumberMask(scale == 0 ? propertyInfo.GetScale() : scale) : valorPropriedade.ToString()
					: defaultValue != null ? defaultValue.ToString() : String.Empty;
			else if (control is TextBox)
				((TextBox)control).Text = valorPropriedade != null ?
					propertyInfo.IsNumber() ? valorPropriedade.ToNumberMask(scale == 0 ? propertyInfo.GetScale() : scale) : valorPropriedade.ToString()
					: defaultValue != null ? defaultValue.ToString() : String.Empty;
			else if (control is CheckBox)
				((CheckBox)control).Checked = valorPropriedade != null ?
					propertyInfo.IsBoolean() ? (Boolean)valorPropriedade : false
					: defaultValue != null && defaultValue is Boolean ? (Boolean)defaultValue : false;
			else if (valorPropriedade is String[])
			{
				String[] arrayString = (String[])valorPropriedade;
				if (control is CheckBoxList)
					foreach (ListItem item in ((CheckBoxList)control).Items)
						item.Selected = arrayString == null || arrayString.Length == 0 ? false : arrayString.Contains(item.Value);
				else if (control is ListControl && arrayString != null && arrayString.Length > 0)
					((ListControl)control).SelectedValue = arrayString[0];
			}
			else if (control is DropDownList)
				((DropDownList)control).SelectedValue = valorPropriedade == null ? "-1" : propertyInfo.PropertyType.BaseType == typeof(EntidadeSP) ?
					((EntidadeSP)valorPropriedade).ID.ToString() : valorPropriedade.ToString();
			else if (control is RadioButtonList)
				((RadioButtonList)control).SelectedValue = valorPropriedade == null ? "-1" : propertyInfo.PropertyType.BaseType == typeof(EntidadeSP) ?
					((EntidadeSP)valorPropriedade).ID.ToString() : valorPropriedade.ToString();
		}

		public static void SetControl(this Usuario usuario
			, ClientPeoplePicker control)
		{
			if (usuario == null)
			{
				if (control.AllowMultipleEntities == false)
					control.AllEntities.Clear();
				control.Validate();
				return;
			}

			PickerEntity entity = new PickerEntity();

			entity.Key = usuario.Login;
			entity = new PeopleEditor().ValidateEntity(entity);
			entity.IsResolved = true;

			control.AddEntities(new List<PickerEntity> { entity });
		}

		public static void SetControl<T, TProperty>(this List<Usuario> usuarios
			, ClientPeoplePicker control)
		{
			control.AllEntities.Clear();
			control.Validate();
			foreach (Usuario usuario in usuarios)
				usuario.SetControl(control);
		}
		#endregion

		#region [Load Control]
		public static void LoadProperty<T, TProperty>(this T entidade
			, WebControl control
			, Expression<Func<T, TProperty>> propriedade)
			where T : Entidade, new()
		{
			if (control is ClientPeoplePicker)
				throw new Exception("Utilizar sobregarga específica para Usuário.");

			PropertyInfo propertyInfo = entidade.GetProperty(propriedade);
			if (propertyInfo == null) return;
			Reflexao.DefinePropriedade(entidade, propertyInfo, control.GetControlValue(propertyInfo));
		}

		public static void LoadProperty(this Usuario usuario
			, ClientPeoplePicker control)
		{
			//TODO:
		}

		public static void LoadProperty<T, TProperty>(this List<Usuario> usuarios
			, ClientPeoplePicker control)
		{
			//TODO:
		}
		#endregion

		#region [Helper]

		public static Boolean IsNumber(this PropertyInfo propertyInfo)
		{
			Boolean isNumber = false;

			if (propertyInfo.PropertyType == typeof(Decimal)
				|| propertyInfo.PropertyType == typeof(Decimal?)
				|| propertyInfo.PropertyType == typeof(Int32)
				|| propertyInfo.PropertyType == typeof(Int32?)
				|| propertyInfo.PropertyType == typeof(Int64)
				|| propertyInfo.PropertyType == typeof(Int64?)
				|| propertyInfo.PropertyType == typeof(Int16)
				|| propertyInfo.PropertyType == typeof(Int16?)
				|| propertyInfo.PropertyType == typeof(Single)
				|| propertyInfo.PropertyType == typeof(Single?)
				|| propertyInfo.PropertyType == typeof(Double)
				|| propertyInfo.PropertyType == typeof(Double?))
				isNumber = true;

			return isNumber;
		}

		public static Boolean IsBoolean(this PropertyInfo propertyInfo)
		{
			Boolean isBoolean = false;

			if (propertyInfo.PropertyType == typeof(Boolean)
				|| propertyInfo.PropertyType == typeof(Boolean?))
				isBoolean = true;

			return isBoolean;
		}

		public static object GetControlValue(this WebControl control, PropertyInfo propertyInfo)
		{
			object defaultValue = propertyInfo.GetType().IsValueType ? Activator.CreateInstance(propertyInfo.GetType()) : null;
			String controlValue = String.Empty;

			if (control is Label)
				controlValue = ((Label)control).Text;
			else if (control is TextBox)
				controlValue = ((TextBox)control).Text;
			else if (control is CheckBox)
				controlValue = ((CheckBox)control).Checked.ToString();
			else if (control is DropDownList)
				controlValue = ((DropDownList)control).SelectedValue != "-1" ? ((DropDownList)control).SelectedValue : String.Empty;
			else if (control is RadioButtonList)
				controlValue = ((RadioButtonList)control).SelectedValue != "-1" ? ((RadioButtonList)control).SelectedValue : String.Empty;

			if (propertyInfo.IsNumber())//Mask Number
				controlValue = controlValue.ToDecimal().ToString();

			return String.IsNullOrEmpty(controlValue) ? defaultValue : controlValue;

		}

		#endregion

		#endregion
	}
}
