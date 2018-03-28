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

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioEmail
    {
        #region | Métodos Auxiliares
        /// <summary>Ajusta o email definindo a #HashTag de assunto</summary>
        /// <param name="mensagem">Mensagem de email</param>
        public static void DefinirHashTagAssunto(List<MensagemEmail> mensagens)
        {
            mensagens.ForEach(i => DefinirHashTagAssunto(i));
        }

        /// <summary>Ajusta o email definindo a #HashTag de assunto</summary>
        /// <param name="mensagem">Mensagem de email</param>
        public static void DefinirHashTagAssunto(MensagemEmail mensagem)
        {
            //Verifica se possui o ID da Tarefa
            if (!mensagem.IdTarefa.HasValue)
                return;

            //Monta o hash do assunto do email para leitura de retorno
            mensagem.Assunto = String.Concat(mensagem.Assunto,
                                    " #",
                                    Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(mensagem.IdTarefa.Value.ToString()))),
                                    "#");
        }

        /// <summary>Ajusta o email definindo a #HashTag de assunto - email Attachment</summary>
        /// <param name="mensagem">Mensagem de email</param>
        public static void DefinirHashTagAssuntoAttachment(List<MensagemEmail> mensagens)
        {
            mensagens.ForEach(i => DefinirHashTagAssuntoAttachment(i));
        }

        /// <summary>Ajusta o email definindo a #HashTag de assunto - email Attachment</summary>
        /// <param name="mensagem">Mensagem de email</param>
        public static void DefinirHashTagAssuntoAttachment(MensagemEmail mensagem)
        {
            //Verifica se possui o ID da Tarefa
            if (!mensagem.GuidListaAttachment.HasValue && !mensagem.IdItemAttachment.HasValue)
                return;

            //Monta o hash do assunto do email para leitura de retorno
            mensagem.Assunto = String.Concat(mensagem.Assunto,
                                    " #",
                                    Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(mensagem.GuidListaAttachment.Value.ToString()))),
                                    "#",
                                    Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(mensagem.IdItemAttachment.Value.ToString()))),
                                    "#",
                                    Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(mensagem.Ambiente2013.ToString()))),
                                    "#");
        }

        /// <summary>Converte o HTML para Texto</summary>
        /// <param name="html">HTML</param>
        /// <returns>Texto</returns>
        private static string ConvertHtml2Text(string html)
        {
            //Validate parameter
            if (String.IsNullOrWhiteSpace(html))
                return String.Empty;

            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            const string lineBreak2 = @"<\/(p|P)>";//matches: </p>,</P>
            const string lineBreak3 = @"<\/(div|div)>";//matches: </div>,</div>
            const string commentBlock = @"(?s)(?<=<!--).+?(?=-->)";//Matches comment block <!-- -->

            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var lineBreakRegex2 = new Regex(lineBreak2, RegexOptions.Multiline);
            var lineBreakRegex3 = new Regex(lineBreak3, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);
            var commentBlockRegex = new Regex(commentBlock, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove Comment blocks
            text = commentBlockRegex.Replace(text, String.Empty);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Replace </p> with line breaks
            text = lineBreakRegex2.Replace(text, Environment.NewLine);
            //Replace </div> with line breaks
            text = lineBreakRegex3.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, String.Empty);

            return text;
        }

        public static String LimparCorpoEmail(String corpoEmail,String emailFrom)
        {
            String corpoEmailFormatado  = ConvertHtml2Text(corpoEmail);
            String nome = emailFrom.IsEmail() ? emailFrom.Split('@')[0] : emailFrom;

            var regexes = new List<Regex>() { 
                        new Regex("From:\\s*" + Regex.Escape(emailFrom), RegexOptions.IgnoreCase),
                        new Regex("From:\\s*" + Regex.Escape(nome), RegexOptions.IgnoreCase),
                        new Regex("De:\\s*" + Regex.Escape(emailFrom), RegexOptions.IgnoreCase),
                        new Regex("De:\\s*" + Regex.Escape(nome), RegexOptions.IgnoreCase),
                        new Regex("<" + Regex.Escape(emailFrom) + ">", RegexOptions.IgnoreCase),
                        new Regex(Regex.Escape(emailFrom) + "\\s+wrote:", RegexOptions.IgnoreCase),
                        new Regex("\\n.*On.*(\\r\\n)?wrote:\\r\\n", RegexOptions.IgnoreCase | RegexOptions.Multiline),
                        new Regex("\\n.*On.*(\\r\\n)?wrote:", RegexOptions.IgnoreCase | RegexOptions.Multiline),
                        new Regex("\\n.*Em.*(\\r\\n)?escreveu:\\r\\n", RegexOptions.IgnoreCase | RegexOptions.Multiline),
                        new Regex("\\n.*Em.*(\\r\\n)?escreveu:", RegexOptions.IgnoreCase | RegexOptions.Multiline),
                        new Regex("-+original\\s+message-+\\s*$", RegexOptions.IgnoreCase),
                        new Regex("-+mensagem\\s+original-+\\s*$", RegexOptions.IgnoreCase),
                        new Regex("-+\\smensagem\\s+original\\s-+\\s*", RegexOptions.IgnoreCase),
                        new Regex("-+\\soriginal\\s+message\\s-+\\s*", RegexOptions.IgnoreCase),
                        new Regex("from:\\s*$", RegexOptions.IgnoreCase),
                        new Regex("^>.*$", RegexOptions.IgnoreCase | RegexOptions.Multiline),
                        new Regex("\\n.*On.*<(\\r\\n)?" + Regex.Escape(emailFrom) + "(\\r\\n)?>", RegexOptions.IgnoreCase),
                        new Regex("\\n.*Em.*<(\\r\\n)?" + Regex.Escape(emailFrom) + "(\\r\\n)?>", RegexOptions.IgnoreCase),
                        new Regex("From:.*" + Regex.Escape(emailFrom), RegexOptions.IgnoreCase),
                        new Regex("From:.*" + Regex.Escape(nome), RegexOptions.IgnoreCase),
                        new Regex("De:.*" + Regex.Escape(emailFrom), RegexOptions.IgnoreCase),
                        new Regex("De:.*" + Regex.Escape(nome), RegexOptions.IgnoreCase)
                    };

            var index = corpoEmailFormatado.Length;

            foreach (var regex in regexes)
            {
                var match = regex.Match(corpoEmailFormatado);

                if (match.Success && match.Index < index)
                    index = match.Index;
            }
            return corpoEmailFormatado.Substring(0, index).Trim();
        }

        public static Boolean IsEmail(this String email)
        {
            if (String.IsNullOrEmpty(email))
                return false;
            return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        private static void CarregarPdf(MensagemEmail mensagem)
        {
            if(mensagem.EnviarPdf && mensagem.IdTarefa != null)
            {
                Tarefa tarefa = new Tarefa().Obter((Int32)mensagem.IdTarefa);
                InstanciaFluxo instancia = new InstanciaFluxo().Obter(tarefa.IdInstanciaFluxo);
                
                KeyValuePair<String,FileInformation> pdfInformation = NegocioPdf.Download(instancia.CodigoLista, instancia.CodigoItem);
                if (!String.IsNullOrEmpty(pdfInformation.Key))
                {
                    MensagemEmailAnexo anexo = new MensagemEmailAnexo();
                    byte[] pdfContent;
                    using (var ms = new MemoryStream())
                    {
                        var buf = new byte[1024 * 16];
                        int byteSize;
                        while ((byteSize = pdfInformation.Value.Stream.Read(buf, 0, buf.Length)) > 0)
                        {
                            ms.Write(buf, 0, byteSize);
                        }
                        pdfContent = ms.ToArray();
                    }
                    anexo.FileData = pdfContent;
                    anexo.FileName = pdfInformation.Key;
                    anexo.FileSize = pdfContent.Length;
                    anexo.MediaType = FormHelper.GetContentType(pdfInformation.Key);
                    mensagem.Anexos = new List<MensagemEmailAnexo>();
                    mensagem.Anexos.Add(anexo);
                }
            }
        }

        #endregion

        #region | Métodos para envio de email

        #region || Envio de email utilizando o SharePoint

        /// <summary>
        /// Efetua o envio de email (Usa o Sharepoint)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="mensagens"></param>
        public static void Enviar(String url, List<MensagemEmail> mensagens)
        {
            using (SPSite spSite = new SPSite(url))
            using (SPWeb spWeb = spSite.OpenWeb())
                mensagens.ForEach(i =>
                {
                    try
                    {
                        i.Enviado = true;
                        if (i != null)
                            Enviar(spWeb, i);
                    }
                    catch (Exception ex)
                    {
                        //Define que não foi enviado
                        i.Enviado = false;

                        //Efetua o log do erro
                        new Log().Inserir("PortalDeFluxos.Core.Servicos.Utilitario.Email",
                                                String.Format("Envio de email - Destino: {0} - Tarefa: {1}",
                                                               i.Para ?? String.Empty, i.IdTarefa.HasValue ? i.IdTarefa.Value : 0),
                                            ex);
                    }
                });
        }

        /// <summary>
        /// Efetua o envio de email (Usa o Sharepoint)
        /// </summary>
        /// <param name="web"></param>
        /// <param name="mensagem"></param>
        public static void Enviar(SPWeb web, MensagemEmail mensagem)
        {
            Usuario usuarioEmail = PortalWeb.ContextoWebAtual.BuscarUsuarioPorNomeLogin(PortalWeb.ContextoWebAtual.Configuracao.Imap.Usuario,false);

            if(usuarioEmail != null)
            {
                StringDictionary headers = new StringDictionary();

                headers.Add("to", String.Concat(mensagem.Para, ";", mensagem.Copia));
                headers.Add("from", usuarioEmail.Email);
                headers.Add("subject", mensagem.Assunto.Replace("{{", "{").Replace("}}", "}"));
                headers.Add("content-type", "text/html"); //This is the default type, so isn't neccessary.
                SPUtility.SendEmail(web, headers, mensagem.Corpo.Replace("{{", "{").Replace("}}", "}"));
            }
            else
            {
                //Define se o email foi enviado
                mensagem.Enviado = SPUtility.SendEmail(web,
                                                        false,
                                                        false,
                                                        String.Concat(mensagem.Para, ";", mensagem.Copia),
                                                        mensagem.Assunto.Replace("{{", "{").Replace("}}", "}"),//Corrigir html
                                                        mensagem.Corpo.Replace("{{", "{").Replace("}}", "}"));//Corrigir html
            }
        }

        #endregion

        #region || Envio de email utilizando outro Smtp

        public static void Enviar(MensagemEmail mensagem, Boolean async = false)
        {
            List<MensagemEmail> mensagens = new List<MensagemEmail>();
            mensagens.Add(mensagem);
            Enviar(mensagens, async);
        }

        public static void Enviar(List<MensagemEmail> mensagens, Boolean async = false)
        {
			if (!async)
				Enviar(mensagens);
			else
				EnviarAsync(mensagens);
        }

        /// <summary>
        /// Efetua o envio de email assíncrono (Não utiliza o Sharepoint)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="mensagens"></param>
        /// <param name="configuracaoImap"></param>

		public static void EnviarAsync(List<MensagemEmail> mensagens)
        {
            //Envia os emails de forma assíncrona
            Task.Run(() =>
            {
				using (SmtpClient smtpClient = ObterSmtpClient())
				{
					smtpClient.ServicePoint.MaxIdleTime = 300; 
					//Efetua o envio de todos emails
					foreach (MensagemEmail mensagem in mensagens)
					{
						try
						{
							if (mensagem != null)
								Enviar(smtpClient, PortalWeb.ContextoWebAtual.Configuracao.Imap.Usuario, mensagem);
						}
						catch (Exception ex)
						{
							//Define que não foi enviado
							mensagem.Enviado = false;
							//Efetua o log
							new Log().Inserir("PortalDeFluxos.Core.Servicos.Utilitario.Email",
													String.Format("Envio de email - Email: {0} - Tarefa: {1}",
																   mensagem.Para ?? String.Empty, mensagem.IdTarefa.HasValue ? mensagem.IdTarefa.Value : 0),
											   ex);
						}
					}
				}
            });
        }

		public static void Enviar(List<MensagemEmail> mensagens)
        {
            Usuario usuarioEmail = null;
			using (SmtpClient smtpClient = ObterSmtpClient())
			{
				smtpClient.ServicePoint.MaxIdleTime = 300; 
				//Efetua o envio de todos emails
				foreach (MensagemEmail mensagem in mensagens)
				{
					try
					{
						CarregarPdf(mensagem);
						if (usuarioEmail == null)
							usuarioEmail = PortalWeb.ContextoWebAtual.BuscarUsuarioPorNomeLogin(PortalWeb.ContextoWebAtual.Configuracao.Imap.Usuario, false);
						if (mensagem != null)
							Enviar(smtpClient, usuarioEmail != null ? usuarioEmail.Email : PortalWeb.ContextoWebAtual.Configuracao.Imap.Usuario, mensagem);
					}
					catch (Exception ex)
					{
						//Define que não foi enviado
						mensagem.Enviado = false;
						//Efetua o log
						new Log().Inserir(Origem.Servico
											, "Envio Email"
											, String.Format("Envio de email - Email: {0} - Tarefa: {1} - SMTP-From {2}",
																mensagem.Para ?? String.Empty, mensagem.IdTarefa.HasValue ? mensagem.IdTarefa.Value : 0
																, PortalWeb.ContextoWebAtual.Configuracao.Imap.Usuario)
											, ex);
					}
				}
			}
        }

        /// <summary>
        /// Efetua o envio de email (Não utiliza o Sharepoint)
        /// </summary>
        /// <param name="smtpServer"></param>
        /// <param name="smtpFrom"></param>
        /// <param name="smtpReply"></param>
        /// <param name="mensagemEmail"></param>
        /// <param name="anexos"></param>
        private static void Enviar(SmtpClient smtpClient, String smtpFrom, MensagemEmail mensagemEmail)
        {
            using(MailMessage mailMessage = new MailMessage())
            {
                mensagemEmail.Para = mensagemEmail.Para.Replace(";", ",").TrimEnd(',');
                mailMessage.From = new System.Net.Mail.MailAddress(smtpFrom);
                string[] emails = mensagemEmail.Para.Split(',');
                foreach (string address in emails)
                    if (!String.IsNullOrEmpty(address.Trim()))
                    mailMessage.To.Add(address);

                //Define o email de reply
                if (!String.IsNullOrWhiteSpace(smtpFrom))
                    mailMessage.ReplyToList.Add(smtpFrom);

                if (!String.IsNullOrWhiteSpace(mensagemEmail.Copia))
                {
                    mensagemEmail.Copia = mensagemEmail.Copia.Replace(";", ",").TrimEnd(',');
                    string[] emailsCopia = mensagemEmail.Copia.Split(',');
                    foreach (string address in emailsCopia)
                        if (!String.IsNullOrEmpty(address.Trim()))
                        mailMessage.CC.Add(address);
                }

                //Define os dados do email
                mailMessage.Subject = mensagemEmail.Assunto.Replace("}}", "}").Replace('\r', ' ').Replace('\n', ' ');
                mailMessage.Body = mensagemEmail.Corpo.Replace("}}", "}");
                mailMessage.IsBodyHtml = true;
                //new System.Net.Mail.Attachment(arquivo.OpenBinaryStream(), ObterTipoConteudoArquivo(arquivo.Name));
                //Adiciona os anexos
                if (mensagemEmail.Anexos != null && mensagemEmail.Anexos.Count > 0)
                    foreach (var anexo in mensagemEmail.Anexos)
                        mailMessage.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(anexo.FileData), ObterTipoConteudoArquivo(anexo.FileName)));

                //Remover em prd
                ServicePointManager.ServerCertificateValidationCallback =
                   delegate(object sender, X509Certificate certificate, X509Chain chain,
                       SslPolicyErrors sslPolicyErrors) { return true; };

                //Cria o SMTP e envia a mensagem
                smtpClient.Send(mailMessage);

                //Define que o email foi enviado
                mensagemEmail.Enviado = true;
            }
        }

        #endregion

        #region ||| Métodos auxiliares

        private static SmtpClient ObterSmtpClient()
        {
            return new SmtpClient()
            {
                Host = PortalWeb.ContextoWebAtual.Configuracao.Imap.Servidor,
                Port = 587,//Porta de saida
                EnableSsl = PortalWeb.ContextoWebAtual.Configuracao.Imap.SSL,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(PortalWeb.ContextoWebAtual.Configuracao.Imap.Usuario
                    , PortalWeb.ContextoWebAtual.Configuracao.Imap.Senha)
            };
        }

        private static System.Net.Mime.ContentType ObterTipoConteudoArquivo(string nomeArquivo_)
        {
            return new System.Net.Mime.ContentType()
            {
                Name = nomeArquivo_,
                MediaType = System.Web.MimeMapping.GetMimeMapping(nomeArquivo_)
            };
        }

        #endregion

        #endregion

        #region [ Métodos para busca de e-mail ]

        /// <summary>Busca as mensagens de e-mail no servidor para serem processadas</summary>
        /// <returns>Mensagens de e-mail</returns>
        public static List<MensagemEmail> BuscarEmails(PortalWeb pWeb, Boolean emailAttachment = false)
        {
            //Lista de E-mails para retorno
            List<MensagemEmail> emails = new List<MensagemEmail>();

            //Busca os dados para leitura de e-mail
            String servidor = pWeb.Configuracao.Imap.Servidor;
            Int32 porta = pWeb.Configuracao.Imap.Porta;
            Boolean SSL = pWeb.Configuracao.Imap.SSL;
            String usuario = pWeb.Configuracao.Imap.Usuario;
            String senha = pWeb.Configuracao.Imap.Senha;
            Int32 qtdLote = pWeb.Configuracao.Imap.QuantidadeLote;
            Usuario usuarioEmail = null;
            //Verifica se o servidor está preenchido
            if (String.IsNullOrWhiteSpace(servidor))
            {
                new Log().Inserir("PortalDeFluxos.Core.Servicos.Utilitario.Email", "BuscarEmail", new ArgumentNullException("Servidor não preenchido"));
                return null;
            }

            try
            {
                //Cria o IMAP.
                using (ImapX.ImapClient client = new ImapX.ImapClient())
                {
                    try
                    {
                        client.Connect(servidor, porta, SSL, false);
                        client.Behavior.MessageFetchMode = ImapX.Enums.MessageFetchMode.Full
                            | ImapX.Enums.MessageFetchMode.Basic
                            | ImapX.Enums.MessageFetchMode.GMailExtendedData;
                        if (!client.Login(usuario, senha))
                            throw new InvalidOperationException("O login ou senha para acesso ao e-mail estão invalidos");
                    }
                    catch (Exception ex)
                    {
                        new Log().Inserir("PortalDeFluxos.Core.Servicos.Utilitario.Email", "BuscarEmail", ex);
                        throw;
                    }

                    //Busca a pasta Inbox
                    ImapX.Folder folder = !emailAttachment ? client.Folders.Inbox : client.Folders["Anexo"];

                    //Pasta Arqchive
                    ImapX.Folder trash = client.Folders.Trash;

                    //Busca as mensagens não lidas (filter = NOT SEEN).
                    List<ImapX.Message> mensagens = folder.Search("NOT SEEN", ImapX.Enums.MessageFetchMode.Full, qtdLote).ToList();

                    //Carrega as mensagens para retorno
                    foreach (ImapX.Message message in mensagens)
                    {
                        MensagemEmail emailEntry = null;
                        try
                        {
                            if (usuarioEmail == null)
                                usuarioEmail = PortalWeb.ContextoWebAtual.BuscarUsuarioPorNomeLogin(PortalWeb.ContextoWebAtual.Configuracao.Imap.Usuario,false);

                            if (emailAttachment && (message.Subject.IndexOf('#') < 0 || message.Subject.ToCharArray().Count(item => item == '#') != 4))
                                message.MoveTo(client.Folders.Inbox);//move para caixa de entrada (não possui tag correta de email)
                            else
                            {
                                emailEntry = new MensagemEmail()
                                {
                                    De = message.From.Address,
                                    Assunto = message.Subject,
                                    Corpo = message.Body.HasHtml ? message.Body.Html : message.Body.HasText ? message.Body.Text : "",
                                    CorpoTexto = message.Body.HasHtml ? LimparCorpoEmail(message.Body.Html, usuarioEmail != null ? usuarioEmail.Email : usuario) :
                                        message.Body.HasText ? LimparCorpoEmail(message.Body.Text, usuarioEmail != null ? usuarioEmail.Email : usuario) : "",
                                };

                                #region [Download Anexo]
                                if (emailAttachment)
                                {
                                    emailEntry.Anexos = new List<MensagemEmailAnexo>();
                                    foreach (var anexo in message.Attachments)
                                    {
                                        try
                                        {
                                            anexo.Download();
                                            //Ocorre erro quando o arquivo está vazio. Não existe propriedade para identificar quando isso ocorre.
                                            emailEntry.Anexos.Add(new MensagemEmailAnexo
                                            {
                                                FileData = anexo.FileData,
                                                MediaType = anexo.ContentType.MediaType,
                                                FileName = anexo.FileName,
                                                FileSize = anexo.FileSize
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            new Log().Inserir("PortalDeFluxos.Core.Servicos.Utilitario.Email", "BuscarEmail - MensagemEmailAnexo (e-mail em branco)", ex);
                                            emailEntry.Anexos.Add(new MensagemEmailAnexo
                                            {
                                                FileData = null,
                                                MediaType = anexo.ContentType.MediaType,
                                                FileName = anexo.FileName,
                                                FileSize = 0
                                            });

                                        }
                                    }
                                } 
                                #endregion

                                //Marca como lido
                                message.Seen = true;

                                //message.Remove();
                                emails.Add(emailEntry);

                                //Se existir a pasta, move para a lixeira 
                                if (trash != null && !emailAttachment && (message.Attachments == null || message.Attachments.Length == 0))
                                    message.MoveTo(trash);
                                else if (emailAttachment)//Se for do tipo EmailAttachment remove da Caixa
                                    message.Remove();
                            }
                        }
                        catch (Exception ex)
                        {
                            new Log().Inserir("PortalDeFluxos.Core.Servicos.Utilitario.Email", "BuscarEmail - MensagemEmailAnexo (e-mail em branco)", ex);
                            if (emailEntry == null)
                                emailEntry = new MensagemEmail();
                            if (emailEntry.Anexos == null)
                                emailEntry.Anexos = new List<MensagemEmailAnexo>();
                            emailEntry.Anexos.Add(new MensagemEmailAnexo
                            {
                                FileData = null,
                                MediaType = "",
                                FileName = "",
                                FileSize = 0
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                new Log().Inserir("PortalDeFluxos.Core.Servicos.Utilitario.Email", "BuscarEmail - ImapX.ImapClient", ex);
            }
            return emails;
        }

        #endregion

        #region [Métodos Template e-mail]

        public static ListaSP_RaizenTemplateDeEmails ObterTemplateEmail(TipoTemplate tipoTemplate, String tituloEmail = "")
        {
            String tipoTemplateTitulo = tipoTemplate.GetTitle();
            List<ListaSP_RaizenTemplateDeEmails> templates = new ListaSP_RaizenTemplateDeEmails().Consultar(t => t.TipoTemplate == tipoTemplateTitulo);
            templates.CarregarDados();

            ListaSP_RaizenTemplateDeEmails template = templates.Find(t => t.Fluxo != null && t.Fluxo.Titulo == tituloEmail);
            template = template != null ? template : templates.Find(t => t.Fluxo == null) != null ? templates.Find(t => t.Fluxo == null) : templates.FirstOrDefault();

            return template;

        }

        public static void EnviarEmailCancelamento(InstanciaFluxo instanciaFluxo, List lista, TipoTemplate tipoTemplate)
        {
            ListaSP_RaizenTemplateDeEmails template = ObterTemplateEmail(tipoTemplate, lista.Title);

            if (template == null)
            {
                new Log().Inserir("NegocioEmail", "EnviarEmailCancelamento", new Exception(String.Format("Template não foi encontrado - Cancelar - Lista:{0}", lista.Title)));
                return;
            }
            //Neste caso não é enviado o email novamente (erro no fluxo foi reiniciado)
            if (instanciaFluxo.ErroCancelado != null && (Boolean)instanciaFluxo.ErroCancelado)
                return;
            EnviarEmailTemplate(template, instanciaFluxo);
        }

        public static void EnviarEmailRespostaDiscussao(Tarefa tarefaDiscussaoResposta, List lista = null)
        {
            String nomeLista = lista == null ? "" : lista.Title;
            ListaSP_RaizenTemplateDeEmails template = ObterTemplateEmail(TipoTemplate.DiscutirResposta, nomeLista);

            if (template ==  null)
            {
                new Log().Inserir("NegocioEmail", "EnviarEmailRespostaDiscussao", new Exception("Template não foi encontrado - Discutir Resposta"));
                return;
            }
            InstanciaFluxo instanciaFluxo = null;
            if (tarefaDiscussaoResposta.IdInstanciaFluxo > 0)
                instanciaFluxo = new InstanciaFluxo().Obter(tarefaDiscussaoResposta.IdInstanciaFluxo);
            MensagemEmail mensagem = ObterMensagemTemplate(template, instanciaFluxo, tarefaDiscussaoResposta.IdTarefa);

            Tarefa _discussaoPergunta = new Tarefa().Obter((Int32)tarefaDiscussaoResposta.IdTarefaPai);
            Tarefa _tarefa = new Tarefa().Obter(_discussaoPergunta.IdTarefa);
            mensagem.Para = _tarefa.EmailResponsavel;

            Enviar(mensagem);
        }

        public static void EnviarEmailTemplate(this ListaSP_RaizenTemplateDeEmails templateEmail, InstanciaFluxo instanciaFluxo = null, Int32 codigoTarefa = -1)
        {
            String destinatarios = ObterEmailDestinatarios(templateEmail, instanciaFluxo);
            if (destinatarios == String.Empty)
                return;

            #region [Mensagem]

            MensagemEmail mensagem = ObterMensagemTemplate(templateEmail, instanciaFluxo, codigoTarefa);
            mensagem.Para = destinatarios;
            mensagem.IdTarefa = codigoTarefa;

            CarregarPdf(mensagem);

            #endregion

            Enviar(mensagem);
        }

        public static MensagemEmail ObterMensagemTemplate(this ListaSP_RaizenTemplateDeEmails templateEmail, InstanciaFluxo instanciaFluxo = null, Int32 codigoTarefa = -1)
        {
            Dictionary<TipoTag, Object> objetos = new Dictionary<TipoTag, Object>();

            #region [Tags]

            if (instanciaFluxo != null)
            {
                instanciaFluxo.CarregarHistoricoWorkflow();
                NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Fluxo, instanciaFluxo);

                ListItem item = BaseSP.ObterItem(instanciaFluxo.CodigoLista, instanciaFluxo.CodigoItem);
                PortalWeb.ContextoWebAtual.SPClient.Load(item.ParentList);
                PortalWeb.ContextoWebAtual.SPClient.ExecuteQuery();

                NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Item, item);
                NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Lista, item.ParentList);

                String listaDescricaoUrlTarefa = String.Empty;
                Lista lista = new Lista().Consultar(l => l.CodigoLista == instanciaFluxo.CodigoLista).FirstOrDefault();
                if (lista != null)
                    listaDescricaoUrlTarefa = String.Format("{0}", new Uri(new Uri(PortalWeb.ContextoWebAtual.Url), lista.DescricaoUrlItem));
                Tarefa tarefa = new Tarefa();
                if (codigoTarefa > 0)
                {
                    tarefa = new Tarefa().Obter(codigoTarefa);
                    tarefa.DescricaoUrlTarefa = String.Format("{0}{1}&IdTarefa={2}", listaDescricaoUrlTarefa, item.Id.ToString(), tarefa.IdTarefa);
                }
                tarefa.DescricaoUrlItem = String.Format("{0}{1}", listaDescricaoUrlTarefa, item.Id.ToString());

                NegocioTradutorTags.PreencherObjetoTag(objetos, TipoTag.Tarefa, tarefa);
            }

            #endregion

            #region [Mensagem]

            String emailCorpo = PortalWeb.ContextoWebAtual.TraduzirTags(objetos, System.Web.HttpUtility.HtmlDecode(templateEmail.Corpo));
            String emailAssunto = PortalWeb.ContextoWebAtual.TraduzirTags(objetos, System.Web.HttpUtility.HtmlDecode(templateEmail.Assunto));

            MensagemEmail mensagem = new MensagemEmail();
            mensagem.Assunto = emailAssunto;
            mensagem.Corpo = emailCorpo;

            #endregion

            return mensagem;
        }

        private static String ObterEmailDestinatarios(ListaSP_RaizenTemplateDeEmails templateEmail, InstanciaFluxo instanciaFluxo = null)
        {
            String email = String.Empty;

            if (instanciaFluxo != null && templateEmail.DestinatariosItem != null)
            {
                ListItem item = BaseSP.ObterItem(instanciaFluxo.CodigoLista, instanciaFluxo.CodigoItem);
                if (item != null && !String.IsNullOrEmpty(templateEmail.DestinatariosItem))
                {
                    String[] propriedades = templateEmail.DestinatariosItem.Split(';');
                    foreach (String prop in propriedades)
                    {
                        Usuario usuario = PortalWeb.ContextoWebAtual.BuscarUsuarioPorPropriedadeItem(item, prop);
                        if (usuario != null && usuario.Email.IsEmail())
                            email += usuario.Email + ",";
                    }
                }
            }

            if (templateEmail.Destinatarios != null)
            {
                foreach (UsuarioGrupoBase usuarioGrupo in templateEmail.Destinatarios)
                {
                    if (usuarioGrupo is Usuario)
                    {
                        if (usuarioGrupo != null && ((Usuario)usuarioGrupo).Email.IsEmail())
                            email += ((Usuario)usuarioGrupo).Email + ",";
                    }
                    else if (usuarioGrupo is Grupo)
                    {
                        foreach (Usuario usuario in ((Grupo)usuarioGrupo).Usuarios)
                        {
                            if (usuario != null && usuario.Email.IsEmail())
                                email += usuario.Email + ",";
                        }
                    }
                }
            }

            return email.TrimEnd(',');
        }
        
        #endregion
    }
}