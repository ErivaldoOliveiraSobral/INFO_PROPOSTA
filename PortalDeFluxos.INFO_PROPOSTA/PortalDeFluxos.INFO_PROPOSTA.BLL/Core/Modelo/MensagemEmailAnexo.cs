using System;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class MensagemEmailAnexo
    {
        public String FileName { get; set; }

        public byte[] FileData { get; set; }

        public String MediaType { get; set; }

        public long FileSize { get; set; }
    }
}
