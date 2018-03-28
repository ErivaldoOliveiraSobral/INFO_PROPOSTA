using Iteris;
using Iteris.SharePoint;
using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL.Dados;
using PortalDeFluxos.Core.BLL.Modelo;
using PortalDeFluxos.Core.BLL.Utilitario;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Winnovative;
using Winnovative.WnvHtmlConvert;

namespace PortalDeFluxos.Core.BLL.Negocio
{
    public static class NegocioPdf
    {

        /// <summary>
        /// Gera um pdf da url utilizando o PdfConverter
        /// </summary>
        /// <param name="urlPdf"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static Byte[] GetPdfFile(String urlPdf, Double width = 700, Double height = 0, Double delay = 15)
        {
            if (PortalWeb.ContextoWebAtual == null)
                throw new ArgumentNullException("PortalWeb.ContextoWebAtual", "ContextoWebAtual não pode ser nulo");

            if (String.IsNullOrEmpty(urlPdf))
                return null;

            Byte[] pdfContent = null;
            width = width != 700 ? width : 700;
            height = height != 0 ? height : 0;
            delay = delay != 15 ? delay : 15;

            try
            {
                PdfConverter configuracaoPdf = PortalWeb.ContextoWebAtual.ConfiguracaoPdf;
                configuracaoPdf.PageWidth = Convert.ToInt32(width);
                if (height > 0)
                    configuracaoPdf.PageHeight = Convert.ToInt32(height);
                configuracaoPdf.ConversionDelay = Convert.ToInt32(delay);
                pdfContent = configuracaoPdf.GetPdfBytesFromUrl(urlPdf);
            }
            catch (Exception ex)
            {
                new Log().Inserir("NegocioPdf", "GetPdfFile", ex);
            }

            return pdfContent;
        }

        /// <summary>
        /// Gera o pdf e Salva na pasta PDF do item
        /// </summary>
        /// <param name="nomeLista"></param>
        /// <param name="codigoItem"></param>
        /// <param name="nomePdf"></param>
        /// <param name="urlPdf"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="delay"></param>
        public static void GerarPdf(String nomeLista, Int32 codigoItem, String nomePdf, String urlPdf, Double width = 700, Double height = 0, Double delay = 15)
        {
            if (PortalWeb.ContextoWebAtual == null)
                throw new ArgumentNullException("PortalWeb.ContextoWebAtual", "ContextoWebAtual não pode ser nulo");

            try
            {
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = GetPdfFile(urlPdf, width, height, delay);
                newFile.Url = nomePdf;
                newFile.Overwrite = true;

                List lista = BaseSP.ObterLista(nomeLista);

                NegocioComum.UploadAnexo(nomeLista, String.Format("{0}/{1}", codigoItem, "PDF"), newFile, removerArquivos:true);
            }
            catch (Exception ex)
            {
                new Log().Inserir("NegocioPdf", "GerarPdf", ex);
            }
        }

        /// <summary>
        /// Retorna o ultimo pdf gerado do item e o nome do arquivo
        /// </summary>
        /// <param name="codigoLista"></param>
        /// <param name="codigoItem"></param>
        /// <returns></returns>
        public static KeyValuePair<String,FileInformation> Download(Guid codigoLista, Int32 codigoItem)
        {
            KeyValuePair<String, FileInformation> pdfInformation = new KeyValuePair<string, FileInformation>();

            if (PortalWeb.ContextoWebAtual == null)
                throw new ArgumentNullException("PortalWeb.ContextoWebAtual", "ContextoWebAtual não pode ser nulo");
            try
            {
                List listaOrigem = ComumSP.ObterLista(codigoLista);
                String nomeLista = listaOrigem.Title;
                pdfInformation = Download(nomeLista, codigoItem);
            }
            catch (Exception ex)
            {
                new Log().Inserir("NegocioPdf", "Download", ex);
            }

            return pdfInformation;
        }

        /// <summary>
        /// Retorna o ultimo pdf gerado do item e o nome do arquivo
        /// </summary>
        /// <param name="nomeLista"></param>
        /// <param name="codigoItem"></param>
        /// <returns></returns>
        public static KeyValuePair<String, FileInformation> Download(String nomeLista, Int32 codigoItem)
        {
            KeyValuePair<String, FileInformation> pdfInformation = new KeyValuePair<string, FileInformation>();

            FileInformation fileInformation = null;
            Anexo ultimoPdf = null;
            if (PortalWeb.ContextoWebAtual == null)
                throw new ArgumentNullException("PortalWeb.ContextoWebAtual", "ContextoWebAtual não pode ser nulo");
            try
            {
                
                using (PortalWeb pWebDocumento = new PortalWeb(PortalWeb.ContextoWebAtual.Configuracao.UrlRecordCenter))
                {
                    PortalWeb.ContextoWebAtual.ExecutarComPrivilegioElevado(() =>
                    {
                        List<Anexo> anexos = NegocioComum.ObterAnexos(nomeLista, String.Format("{0}/{1}", codigoItem, "PDF"));

                        ultimoPdf = anexos.OrderByDescending(_ => _.DataUpload).FirstOrDefault();
                        if (ultimoPdf != null)
                            fileInformation = File.OpenBinaryDirect(PortalWeb.ContextoWebAtual.SPClient, ultimoPdf.RelativeUrl);
                    });
                }
                
            }
            catch (Exception ex)
            {
                new Log().Inserir("NegocioPdf", "Download", ex);
            }

            if (ultimoPdf != null)
                pdfInformation = new KeyValuePair<string, FileInformation>(ultimoPdf.NomeArquivo, fileInformation);

            return pdfInformation;
        }
    }
}
