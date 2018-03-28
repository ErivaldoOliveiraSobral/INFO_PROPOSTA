using System;
using System.Collections.Generic;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class MensagemEmail
    {
        /// <summary>Id da Tarefa associada ao e-mail (Se houver)</summary>
        public int? IdTarefa { get; set; }

        /// <summary>Id do Lembrete assiciado ao e-mail (Se houver)</summary>
        public int? IdLembrete { get; set; }

        /// <summary>Id do item associado ao e-mail attachment (Se houver)</summary>
        public int? IdItemAttachment { get; set; }

        /// <summary>Id da lista associada ao e-mail attachment (Se houver)</summary>
        public Guid? GuidListaAttachment { get; set; }

        /// <summary>Se o e-mail attchment for para o ambiente 2013</summary>
        public Boolean Ambiente2013 { get; set; }

        /// <summary>Remetente (Se houver)</summary>
        public String De { get; set; }

        /// <summary>Destinatário do e-mail</summary>
        public String Para { get; set; }

        /// <summary>Destinatário / Cópia</summary>
        public String Copia { get; set; }

        /// <summary>Assunto</summary>
        public String Assunto { get; set; }

        /// <summary>Mensagem</summary>
        public String Corpo { get; set; }

        /// <summary>Mensagem original do email (Utilizado na leitura)</summary>
        public String CorpoTexto { get; set; }

        /// <summary>Se o e-mail foi enviado</summary>
        public Boolean Enviado { get; set; }

        /// <summary>Se for para enviar pdf</summary>
        public Boolean EnviarPdf { get; set; }

        /// <summary>Anexos</summary>
        public List<MensagemEmailAnexo> Anexos { get; set; }
    }
}
