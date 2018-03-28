using Microsoft.SharePoint.Client;
using System;
using PortalDeFluxos.Core.BLL.Utilitario;
using System.Collections.Generic;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    /// <summary>Classe para controle de usuário Teste</summary>
    public class UsuariosTeste
    {
        public Int32 IndexUsuarioAtivo { get; set; }

        public Boolean Ativo { get; set; }

        public List<UsuarioTeste> Usuarios { get; set; }
    }

    public class UsuarioTeste
    {
        public String Login { get; set; }

        public String Nome { get; set; }

        public String Email { get; set; }

        public String Password { get; set; }

    }
}
