using Microsoft.SharePoint.Utilities;
using PortalDeFluxos.Core.BLL.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ImapX;
using PortalDeFluxos.Core.BLL.Utilitario;
using PortalDeFluxos.Core.BLL.Dados;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using Iteris;
using System.Reflection;
using System.Globalization;
using System.Linq.Expressions;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioComumSAE
    {
		#region [PreencherDicionarioSAE]
		public static void PreencherDicionarioSAE<TModel>(this Dictionary<string, string> dicionarioSAE
			, TModel objeto
			, string prefixo = ""
			, Boolean concatenarPrefixo = false
			, String propriedadeNome = ""
			, Boolean concatenarPropriedadeNome = false)
			where TModel : Entidade
		{
			if (objeto == null)
				return;
			String formatoChave = "{0}:{1}";
			String prefixoPropriedade = String.IsNullOrEmpty(prefixo) ?
				((System.Reflection.MemberInfo)(objeto.GetType().BaseType)).Name :
				concatenarPrefixo ? String.Concat(((System.Reflection.MemberInfo)(objeto.GetType().BaseType)).Name, prefixo) :
				prefixo;
			PropertyInfo[] propriedades = objeto.GetType().GetProperties();
			for (int i = 0; i < propriedades.Count(); i++)
			{
				if (Reflexao.IsVirtualProperty(objeto.GetType(), propriedades[i].Name))
					continue;

				string nomeProp = String.IsNullOrEmpty(propriedadeNome) ? propriedades[i].Name :
					concatenarPropriedadeNome ? String.Concat(propriedades[i].Name, propriedadeNome) : propriedadeNome;
				string valor = propriedades[i].GetValue(objeto) != null ?
							propriedades[i].GetValue(objeto).GetType() == typeof(decimal) || propriedades[i].GetValue(objeto).GetType() == typeof(decimal?) ?
							((decimal)propriedades[i].GetValue(objeto)).ToStringInvariantCulture() :
							propriedades[i].GetValue(objeto).GetType() == typeof(int) || propriedades[i].GetValue(objeto).GetType() == typeof(int?) ?
							((int)propriedades[i].GetValue(objeto)).ToStringInvariantCulture()
						: propriedades[i].GetValue(objeto).ToString()
						: String.Empty;

				String chave = String.Format(formatoChave, prefixoPropriedade, nomeProp);
				if (dicionarioSAE.ContainsKey(chave))
					dicionarioSAE[chave] = valor;
				else
					dicionarioSAE.Add(chave, valor);
			}
		}

		public static void PreencherDicionarioSAE<TModel>(this Dictionary<string, string> dicionarioSAE
			, TModel objeto
			, String chaveEspecifica
			, String valor)
			where TModel : Entidade
		{
			if (!String.IsNullOrEmpty(chaveEspecifica))
			{
				if (dicionarioSAE.ContainsKey(chaveEspecifica))
					dicionarioSAE[chaveEspecifica] = valor;
				else
					dicionarioSAE.Add(chaveEspecifica, valor);
				return;
			}
		}

		public static void PreencherDicionarioSAE<TModel>(this Dictionary<string, string> dicionarioSAE
			, TModel objeto
			, Expression<Func<TModel, object>> propriedade
			, String valor
			, string prefixo = ""
			, Boolean concatenarPrefixo = false
			, String propriedadeNome = ""
			, Boolean concatenarPropriedadeNome = false
			, String chaveEspecifica = "")
			where TModel : Entidade
		{
			if (!String.IsNullOrEmpty(chaveEspecifica))
			{
				dicionarioSAE.PreencherDicionarioSAE(objeto, chaveEspecifica, valor);
				return;
			}

			if (objeto == null)
				return;

			String formatoChave = "{0}:{1}";
			String entidadeName = ((System.Reflection.MemberInfo)(objeto.GetType().BaseType)).Name == "EntidadeDB" ? objeto.GetType().Name : ((System.Reflection.MemberInfo)(objeto.GetType().BaseType)).Name;
			String prefixoPropriedade = String.IsNullOrEmpty(prefixo) ?
				entidadeName :
				concatenarPrefixo ? String.Concat(entidadeName, prefixo) :
				prefixo;
			String nomeProp = String.IsNullOrEmpty(propriedadeNome) ? Reflexao.GetPropertyName((LambdaExpression)propriedade) :
				concatenarPropriedadeNome ? String.Concat(Reflexao.GetPropertyName((LambdaExpression)propriedade), propriedadeNome) : propriedadeNome;
			String chave = String.Format(formatoChave, prefixoPropriedade, nomeProp);
			if (dicionarioSAE.ContainsKey(chave))
				dicionarioSAE[chave] = valor;
			else
				dicionarioSAE.Add(chave, valor);
		}

		public static void PreencherDicionarioSAE<TModel>(this Dictionary<string, string> dicionarioSAE
			, List<TModel> listItens
			, Expression<Func<TModel, object>> propriedade
			, Int32 numeroAnosProjeto = -1
			, Decimal? valorDefault = null
			, String prefixo = ""
			, Boolean concatenarPrefixo = false
			, String propriedadeNome = ""
			, Boolean concatenarPropriedadeNome = false
			, Dictionary<String, String> datasourceOrigem = null)
			where TModel : EntidadeDB
		{
			if (listItens == null || listItens.Count == 0)
				return;

			String propertyName = Reflexao.GetPropertyName((LambdaExpression)propriedade);
			String formatoChave = "{0}:{1}";
			String valorAnoChave = String.Concat(PrefixoSAE.ValorAno.GetTitle(), "{0}");
			String entidadeName = ((System.Reflection.MemberInfo)(listItens.FirstOrDefault().GetType().BaseType)).Name == "EntidadeDB" ? listItens.FirstOrDefault().GetType().Name : ((System.Reflection.MemberInfo)(listItens.FirstOrDefault().GetType().BaseType)).Name;
			String prefixoPropriedade = String.IsNullOrEmpty(prefixo) ?
				entidadeName :
				concatenarPrefixo ? String.Concat(entidadeName, prefixo) :
				prefixo;

			if (String.IsNullOrEmpty(propertyName) || listItens == null)
				return;

			Int32 contador = 1;
			foreach (TModel item in listItens)
			{
				if ((item.GetType().GetProperty("Ano") != null
					&& (Int32)item.GetType().GetProperty("Ano").GetValue(item) > numeroAnosProjeto
					&& numeroAnosProjeto > 0)//Neste caso retorna todos os anos
					|| (item.GetType().GetProperty("Ano") == null
						&& contador > numeroAnosProjeto
						&& numeroAnosProjeto > 0))//Neste caso retorna todos os anos
					continue;

				Int32 identificador = item.GetType().GetProperty("Ano") != null ?
					(Int32)item.GetType().GetProperty("Ano").GetValue(item) : contador;

				String prefixoChave = item.GetType().GetProperty("CasoBase") == null || item.GetType().GetProperty("CasoBase").GetValue(item) == null ?
						propertyName :
						(Boolean)item.GetType().GetProperty("CasoBase").GetValue(item) ?
					String.Concat(propertyName, PrefixoSAE.CasoBase.GetTitle()) :
					String.Concat(propertyName, PrefixoSAE.CasoInv.GetTitle());

				String chavePropriedade = String.IsNullOrEmpty(propriedadeNome) ? propertyName : concatenarPropriedadeNome ?
					String.Concat(String.Concat(propertyName, propriedadeNome), identificador) : String.Concat(propriedadeNome, identificador);

				object valorObjeto = item.GetType().GetProperty(propertyName).GetValue(item);
				String value = valorObjeto is Decimal ? ((Decimal?)valorObjeto).ToStringInvariantCulture(valorDefault) : valorObjeto == null ? "" : valorObjeto.ToString();
				String chave = String.Format(formatoChave, item is AnoContratual ? prefixoChave : prefixoPropriedade
					, item is AnoContratual ? String.Format(valorAnoChave, identificador.ToString()) : chavePropriedade);

				if (datasourceOrigem != null && datasourceOrigem.ContainsKey(value))
					value = datasourceOrigem[value];

				if (dicionarioSAE.ContainsKey(chave))
					dicionarioSAE[chave] = value;
				else
					dicionarioSAE.Add(chave, value);

				contador++;
			}
		}
		#endregion

		#region [CarregarEntidade]

		public static void CarregarEntitade<TModel>(this Dictionary<string, string> dicionarioSAE
			, TModel objeto
			, string prefixo = ""
			, Boolean concatenarPrefixo = false
			, String propriedadeNome = ""
			, Boolean concatenarPropriedadeNome = false)
			where TModel : Entidade
		{
			if (objeto == null)
				return;
			String formatoChave = "{0}:{1}";
			String entidadeName = ((System.Reflection.MemberInfo)(objeto.GetType().BaseType)).Name == "EntidadeDB" ? objeto.GetType().Name : ((System.Reflection.MemberInfo)(objeto.GetType().BaseType)).Name;
			String prefixoPropriedade = String.IsNullOrEmpty(prefixo) ?
				entidadeName :
				concatenarPrefixo ? String.Concat(entidadeName, prefixo) :
				prefixo;
			PropertyInfo[] propriedades = objeto.GetType().GetProperties();
			for (int i = 0; i < propriedades.Count(); i++)
			{
				if (Reflexao.IsVirtualProperty(objeto.GetType(), propriedades[i].Name))
					continue;

				string nomeProp = String.IsNullOrEmpty(propriedadeNome) ? propriedades[i].Name :
					concatenarPropriedadeNome ? String.Concat(propriedades[i].Name, propriedadeNome) : propriedadeNome;

				String chavePropriedade = String.Format(formatoChave, prefixoPropriedade, nomeProp);
				if (dicionarioSAE.ContainsKey(chavePropriedade))
					Reflexao.DefinePropriedade(objeto, propriedades[i].Name, dicionarioSAE[chavePropriedade], CultureInfo.InvariantCulture);
			}
		}

		public static void CarregarEntitade<TModel>(this Dictionary<string, string> dicionarioSAE
			, TModel objeto
			, Expression<Func<TModel, object>> propriedade
			, string prefixo = ""
			, Boolean concatenarPrefixo = false
			, String propriedadeNome = ""
			, Boolean concatenarPropriedadeNome = false
			, String chaveEspecifica = "")
			where TModel : Entidade
		{
			if (!String.IsNullOrEmpty(chaveEspecifica))
			{
				if (dicionarioSAE.ContainsKey(chaveEspecifica))
					Reflexao.DefinePropriedade(objeto, Reflexao.GetPropertyName((LambdaExpression)propriedade), dicionarioSAE[chaveEspecifica], CultureInfo.InvariantCulture);
				return;
			}

			if (objeto == null)
				return;

			String formatoChave = "{0}:{1}";
			String entidadeName = ((System.Reflection.MemberInfo)(objeto.GetType().BaseType)).Name == "EntidadeDB" ? objeto.GetType().Name : ((System.Reflection.MemberInfo)(objeto.GetType().BaseType)).Name;
			String prefixoPropriedade = String.IsNullOrEmpty(prefixo) ?
				entidadeName :
				concatenarPrefixo ? String.Concat(entidadeName, prefixo) :
				prefixo;
			String nomeProp = String.IsNullOrEmpty(propriedadeNome) ? Reflexao.GetPropertyName((LambdaExpression)propriedade) :
				concatenarPropriedadeNome ? String.Concat(Reflexao.GetPropertyName((LambdaExpression)propriedade), propriedadeNome) : propriedadeNome;
			String chave = String.Format(formatoChave, prefixoPropriedade, nomeProp);
			if (dicionarioSAE.ContainsKey(chave))
				Reflexao.DefinePropriedade(objeto, Reflexao.GetPropertyName((LambdaExpression)propriedade), dicionarioSAE[chave], CultureInfo.InvariantCulture);
		}

		public static void CarregarEntitade<TModel>(this Dictionary<string, string> dicionarioSAE
			, List<TModel> listItens
			, Expression<Func<TModel, object>> propriedade
			, Int32 numeroAnosProjeto = -1
			, Decimal? valorDefault = null
			, String prefixo = ""
			, Boolean concatenarPrefixo = false
			, String propriedadeNome = ""
			, Boolean concatenarPropriedadeNome = false)
			where TModel : EntidadeDB
		{
			if (listItens == null || listItens.Count == 0)
				return;

			String propertyName = Reflexao.GetPropertyName((LambdaExpression)propriedade);
			String formatoChave = "{0}:{1}";
			String valorAnoChave = String.Concat(PrefixoSAE.ValorAno.GetTitle(), "{0}");
			String entidadeName = ((System.Reflection.MemberInfo)(listItens.FirstOrDefault().GetType().BaseType)).Name == "EntidadeDB" ? listItens.FirstOrDefault().GetType().Name : ((System.Reflection.MemberInfo)(listItens.FirstOrDefault().GetType().BaseType)).Name;
			String prefixoPropriedade = String.IsNullOrEmpty(prefixo) ?
				entidadeName :
				concatenarPrefixo ? String.Concat(entidadeName, prefixo) :
				prefixo;

			if (String.IsNullOrEmpty(propertyName) || listItens == null)
				return;

			Int32 contador = 1;
			foreach (TModel item in listItens)
			{
				if ((item.GetType().GetProperty("Ano") != null
					&& (Int32)item.GetType().GetProperty("Ano").GetValue(item) > numeroAnosProjeto
					&& numeroAnosProjeto > 0)//Neste caso retorna todos os anos
					|| (item.GetType().GetProperty("Ano") == null
						&& contador > numeroAnosProjeto
						&& numeroAnosProjeto > 0))//Neste caso retorna todos os anos
					continue;

				Int32 identificador = item.GetType().GetProperty("Ano") != null ?
					(Int32)item.GetType().GetProperty("Ano").GetValue(item) : contador;

				String prefixoChave = item.GetType().GetProperty("CasoBase") == null || item.GetType().GetProperty("CasoBase").GetValue(item) == null ?
						propertyName :
						(Boolean)item.GetType().GetProperty("CasoBase").GetValue(item) ?
					String.Concat(propertyName, PrefixoSAE.CasoBase.GetTitle()) :
					String.Concat(propertyName, PrefixoSAE.CasoInv.GetTitle());

				String chavePropriedade = String.IsNullOrEmpty(propriedadeNome) ? propertyName : concatenarPropriedadeNome ?
					String.Concat(String.Concat(propertyName, propriedadeNome), identificador) : String.Concat(propriedadeNome, identificador);

				String value = ((Decimal?)item.GetType().GetProperty(propertyName).GetValue(item)).ToStringInvariantCulture(valorDefault);
				String chave = String.Format(formatoChave, item is AnoContratual ? prefixoChave : prefixoPropriedade
					, item is AnoContratual ? String.Format(valorAnoChave, identificador.ToString()) : chavePropriedade);

				if (dicionarioSAE.ContainsKey(chave))
					Reflexao.DefinePropriedade(item, Reflexao.GetPropertyName((LambdaExpression)propriedade), dicionarioSAE[chave], CultureInfo.InvariantCulture);

				contador++;
			}
		}

		#endregion

		#region [Helpers]
		public static void LimparSincronizarDicionario(this Dictionary<string, string> dicionarioSAE
			, String sistema
			, SentidoSAE sentido)
		{
			List<SAEParametros> saeParametros = new SAEParametros().Consultar(p => p.Direcao == (Int32)sentido && p.Sistema == sistema);
			for (int i = dicionarioSAE.Count - 1; i >= 0; i--)
			{
				if (!saeParametros.Any(_ => _.CampoSistema == dicionarioSAE.ElementAt(i).Key))
					dicionarioSAE.Remove(dicionarioSAE.ElementAt(i).Key);
			}

			foreach (SAEParametros item in saeParametros)
			{
				if (!dicionarioSAE.ContainsKey(item.CampoSistema))
					dicionarioSAE.Add(item.CampoSistema, String.Empty);
			}
		}

		public static String ObterValorEnumerador(Dictionary<String, String> enumeradores_, Int32? index_)
		{
			if (index_ == null)
				return String.Empty;

			string valor = String.Empty;

			if (enumeradores_.ContainsKey(index_.ToString()))
				valor = enumeradores_[index_.ToString()];

			return valor;
		}
		#endregion

		#region [Arquivo SAE]

		/// <summary>
		/// Retorna o ultimo SAE gerado do item e o nome do arquivo
		/// </summary>
		/// <param name="codigoLista"></param>
		/// <param name="codigoItem"></param>
		/// <returns></returns>
		public static KeyValuePair<String, FileInformation> DownloadSAE(Guid codigoLista, Int32 codigoItem)
		{
			KeyValuePair<String, FileInformation> pdfInformation = new KeyValuePair<string, FileInformation>();

			if (PortalWeb.ContextoWebAtual == null)
				throw new ArgumentNullException("PortalWeb.ContextoWebAtual", "ContextoWebAtual não pode ser nulo");
			try
			{
				List listaOrigem = ComumSP.ObterLista(codigoLista);
				String nomeLista = listaOrigem.Title;
				pdfInformation = DownloadSAE(nomeLista, codigoItem);
			}
			catch (Exception ex)
			{
				new Log().Inserir("NegocioComumSAE", "DownloadSAE", ex);
			}

			return pdfInformation;
		}

		// <summary>
		/// Retorna o ultimo pdf gerado do item e o nome do arquivo
		/// </summary>
		/// <param name="nomeLista"></param>
		/// <param name="codigoItem"></param>
		/// <returns></returns>
		public static KeyValuePair<String, FileInformation> DownloadSAE(String nomeLista, Int32 codigoItem)
		{
			KeyValuePair<String, FileInformation> saeInformation = new KeyValuePair<string, FileInformation>();

			FileInformation fileInformation = null;
			Anexo ultimoSAE = null;
			if (PortalWeb.ContextoWebAtual == null)
				throw new ArgumentNullException("PortalWeb.ContextoWebAtual", "ContextoWebAtual não pode ser nulo");
			try
			{

				using (PortalWeb pWebDocumento = new PortalWeb(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
				{
					PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
					{
						List<Anexo> anexos = NegocioComum.ObterAnexos(nomeLista, String.Format("{0}/{1}", codigoItem, "SAE"));

						ultimoSAE = anexos.OrderByDescending(_ => _.DataUpload).FirstOrDefault();
						if (ultimoSAE != null)
							fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(PortalWeb.ContextoWebAtual.SPClient, ultimoSAE.RelativeUrl);
					});
				}

			}
			catch (Exception ex)
			{
				new Log().Inserir("NegocioComumSAE", "DownloadSAE", ex);
			}

			if (ultimoSAE != null)
				saeInformation = new KeyValuePair<string, FileInformation>(ultimoSAE.NomeArquivo, fileInformation);

			return saeInformation;
		}

		/// <summary>
		/// Envia um e-mail com a planilha SAE salva
		/// </summary>		
		/// <param name="codigoLista"></param>
		/// <param name="codigoItem"></param>
		/// <param name="emailTo">Destinatário</param>
		/// <param name="assuntoEmail">Assunto do e-mail</para
		/// <returns>Se foi enviado ou não</returns>
		public static Boolean EnviarArquivoSAE(
			  Guid codigoLista
			, Int32 codigoItem
			, string emailTo
			, string assuntoEmail)
		{
			Boolean retorno = true;
			try
			{
				byte[] saeContent;
				KeyValuePair<String, FileInformation> arquivoSae = DownloadSAE(codigoLista, codigoItem);
				if (!String.IsNullOrEmpty(arquivoSae.Key))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						arquivoSae.Value.Stream.CopyTo(ms);
						saeContent = ms.ToArray();
					}
					EnviarArquivoSAE(saeContent, arquivoSae.Key, emailTo, assuntoEmail);
				}
				else
				{
					new Log().Inserir("NegocioComumSAE", "EnviarArquivoSAE", new Exception(String.Format("{0} - Lista:{1} - Item:{2}","Arquivo SAE não encontrado",codigoLista.ToString(),codigoItem.ToString())));
					return false;
				}
									
			}
			catch (Exception ex)
			{
				new Log().Inserir("NegocioComumSAE", "EnviarArquivoSAE", ex);
				retorno = false;
			}

			return retorno;
		}

		/// <summary>
		/// Envia um e-mail com a planilha SAE
		/// </summary>
		/// <param name="saeContent"> Conteúdo da Planilha SAE</param>
		/// <param name="fileName">Nome da planilha</param>
		/// <param name="emailTo">Destinatário</param>
		/// <param name="assuntoEmail">Assunto do e-mail</param>
		public static void EnviarArquivoSAE(byte[] saeContent
			, String fileName
			, string emailTo
			, string assuntoEmail)
		{
			MensagemEmail mensagem = new MensagemEmail();
			mensagem.Assunto = assuntoEmail;
			mensagem.Corpo = "Segue planilha SAE solicitada.";
			mensagem.De = "noreplay@raizen.com";
			mensagem.Para = emailTo;

			MensagemEmailAnexo anexo = new MensagemEmailAnexo();
			anexo.FileData = saeContent.ToArray();
			anexo.FileName = fileName;
			anexo.FileSize = saeContent.ToArray().Length;
			anexo.MediaType = "application/ms-excel";

			mensagem.Anexos = new List<MensagemEmailAnexo>();
			mensagem.Anexos.Add(anexo);

			NegocioEmail.Enviar(mensagem);

		} 
		/// <summary>
		/// Salva a planilha SAE na pasta de arquivos da proposta
		/// </summary>
		/// <param name="saeContent">Conteúdo da Planilha SAE</param>
		/// <param name="itemId">Id da proposta</param>
		/// <param name="sistema">Tipo de proposta (nome da lista)</param>
		/// <param name="fileName">Nome da planilha SAE</param>
		public static void SalvarArquivoSAE(byte[] saeContent
			, Int32 itemId
			, String sistema
			, String fileName)
		{
			FileCreationInformation saeFile = new FileCreationInformation();
			saeFile.Content = saeContent;
			saeFile.Overwrite = true;
			saeFile.Url = fileName;
			NegocioComum.UploadAnexo(sistema, String.Format("{0}/{1}", itemId.ToString(), "SAE"), saeFile,removerArquivos:true);
		}

		#endregion
	}
}