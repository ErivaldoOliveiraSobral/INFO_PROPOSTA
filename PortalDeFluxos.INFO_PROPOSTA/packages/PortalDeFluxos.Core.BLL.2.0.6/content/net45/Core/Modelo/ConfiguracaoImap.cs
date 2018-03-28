using System;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    [Serializable]
    public class ConfiguracaoImap
    {
        /// <summary>Nome do servidor</summary>
        public String Servidor { get; set; }
        
        /// <summary>Porta</summary>
        public Int32 Porta { get; set; }

        /// <summary>Deve usar SSL</summary>
        public Boolean SSL { get; set; }

        /// <summary>Usuário</summary>
        public String Usuario { get; set; }

        /// <summary>Senha</summary>
        public String Senha { get; set; }

        /// <summary>Se o remetente do e-mail deve ser validado</summary>
        public Boolean ValidarRemetente { get; set; }

        /// <summary>Quantidade de mensagens que devem ser lidas por execução</summary>
        public Int32 QuantidadeLote { get; set; }
    }
}
