using Microsoft.SharePoint.Client;
using System;
using PortalDeFluxos.Core.BLL.Utilitario;
using Microsoft.SharePoint.Client.Utilities;

namespace PortalDeFluxos.Core.BLL.Modelo
{
       /// <summary>Classe para controle de usuário</summary>
    public partial class UsuarioSP
    {
        public Boolean Inserido { get; set; }
        public Boolean Atualizado { get; set; }

        public UsuarioSP(Usuario usuario,Boolean ativo,Boolean inserido = false,Boolean atualizado = false)
        {
            this.Inserido = inserido;
            this.Atualizado = atualizado;
            this.IdUsuarioSP = usuario.Id;
            this.Nome = usuario.Nome;
            this.Login = usuario.Login;
            this.LoginComClaims = usuario.LoginComClaims;
            this.LookupValue = usuario.LookupValue;
            this.Email = usuario.Email;
            this.SiteAdmin = usuario.SiteAdmin;
            this.Ativo = ativo ;
        }
    }

    public partial class GrupoSP
    {
        public Boolean Inserido { get; set; }
        public Boolean Atualizado { get; set; }

        public GrupoSP(Grupo grupo, Boolean inserido = false, Boolean atualizado = false)
        {
            this.Inserido = inserido;
            this.Atualizado = atualizado;
            this.IdGrupoSP = grupo.Id;
            this.Nome = grupo.Nome;
        }
    }

    public partial class GrupoUsuariosSP
    {
        public Boolean Inserido { get; set; }
        public Boolean Atualizado { get; set; }

        public GrupoUsuariosSP(Grupo grupo, Int32 idUsuarioSP, Boolean inserido = false, Boolean atualizado = false)
        {
            this.Inserido = inserido;
            this.Atualizado = atualizado;
            this.IdGrupoSP = grupo.Id;
            this.IdUsuarioSP = idUsuarioSP;
        }

        public GrupoUsuariosSP()
        {

        }

    }

    public class UsuariosGruposSP
    {
        public Int32 Id {get;set;}

        public String Nome { get; set; }

        public String Login { get; set; }

        public String Tipo { get; set; }

        public String QtdUsuarios { get; set; }

        public Int32 QtdTarefa { get; set; }

        public Boolean Ativo { get; set; }

        public String AtivoDescricao { get { return this.Ativo ? "Sim" : "Não"; } }

        public Int32? TotalRecordCount { get; set; }
    }
}
