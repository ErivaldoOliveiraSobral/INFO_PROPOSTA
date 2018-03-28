using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalDeFluxos.Core.BLL.Utilitario;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    /// <summary>Classe para controle de grupo</summary>
    public partial class Grupo : UsuarioGrupoBase
    {
        public List<Usuario> Usuarios { get; set; }

        //Talvez implementar Superior nessa classe

        //Tomar cuidado de carregar as informações dos usuários do grupo ao fazer essa operação
        public static implicit operator Grupo(Group grupoSP)
        {
            if (grupoSP == null)
                return null;

            List<Usuario> membrosGrupo = new List<Usuario>();
            
            foreach(User usuario in grupoSP.Users)
            {
                membrosGrupo.Add(usuario);
            }

            return new Grupo()
            {
                Id = grupoSP.Id,
                Nome = grupoSP.Title,
                Usuarios = membrosGrupo,
            };
        }

        public Grupo(Group grupoSP)
        {
            List<Usuario> membrosGrupo = new List<Usuario>();
            foreach (User usuario in grupoSP.Users)
            {
                membrosGrupo.Add(usuario);
            }
            this.Usuarios = membrosGrupo;
            this.Id = grupoSP.Id;
            this.Nome = grupoSP.Title;
        }

        public Grupo()
        {

        }
    }
}
