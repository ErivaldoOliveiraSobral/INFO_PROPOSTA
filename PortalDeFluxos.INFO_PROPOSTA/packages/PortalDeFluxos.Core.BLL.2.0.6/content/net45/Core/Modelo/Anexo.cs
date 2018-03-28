using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class Anexo : BasePainel
    {
        public Int32? IdAnexo { get; set; }
        public String NomeArquivo { get; set; }
        public String Usuario { get; set; }
        public String DataUploadString { get; set; }
		public DateTime DataUpload { get; set; }
        public String Url { get; set; }
        public String RelativeUrl { get; set; }
        public Anexo(File file)
        {
            this.IdAnexo = file.ListItemAllFields.Id;
            this.NomeArquivo = file.Name;
            this.Usuario = file.ListItemAllFields["Usuario"] != null ? ((Microsoft.SharePoint.Client.FieldLookupValue)(file.ListItemAllFields["Usuario"])).LookupValue : file.Author.Title;
			this.DataUploadString = file.TimeLastModified.ToLocalTime().ToString("dd/MM/yyyy");
			this.DataUpload = file.TimeLastModified;
            this.Url = (String)file.ListItemAllFields["EncodedAbsUrl"];
            this.RelativeUrl = file.ServerRelativeUrl;
        }
    }
}
