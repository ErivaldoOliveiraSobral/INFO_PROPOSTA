using Microsoft.SharePoint.Client;
using System;
using PortalDeFluxos.Core.BLL.Utilitario;
using Microsoft.SharePoint.Client.Utilities;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public partial class UsuarioGrupoBase
    {
        public Int32 Id { get; set; }

        public String Nome { get; set; }
    }
    /// <summary>Classe para controle de usuário</summary>
    public partial  class Usuario : UsuarioGrupoBase
    {
        private String _loginComClaims = String.Empty;
        public String LoginComClaims
        {
            get
            {
                return _loginComClaims;
            }
            set
            {
                _loginComClaims = value;
            }
        }

        public String Login
        {
            get
            {
                return _loginComClaims.RemoverClaims();
            }
        }

        public String LookupValue
        {
            get
            {
                return string.Format("{0};#{1}", Id, Nome);
            }
        }

        public Boolean SiteAdmin { get; set; }

        public String Email { get; set; }

        public static implicit operator Usuario(User usuarioSP)
        {
            if (usuarioSP == null)
                return null;

            return new Usuario()
            {
                Id = usuarioSP.Id,
                Nome = usuarioSP.Title,
                LoginComClaims = usuarioSP.LoginName,
                Email = usuarioSP.Email,
                SiteAdmin = usuarioSP.IsSiteAdmin
            };
        }
    }

    public partial class Usuario
    {
        public Int32? TotalRecordCount { get; set; }
    }
}
