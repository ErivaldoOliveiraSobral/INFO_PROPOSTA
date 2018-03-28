using PortalDeFluxos.Core.BLL.Atributos;
using System;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    [Serializable]
    public class EntidadeSP : Entidade
    {
        /// <summary>ID do item</summary>
		[InternalName("ID")]
		public int ID { get; set; }

        /// <summary>Título do item na lista</summary>
		[InternalName("Title")]
		public String Titulo { get; set; }

        /// <summary>Data que o item foi incluído</summary>
        [InternalName("Created", ReadOnly = true)]
        public DateTime DataInclusao { get; set; }

        /// <summary>Retorna o nome do usuário que efetuou a Inclusão</summary>
        [InternalName("Author", ReadOnly = true)]
        public String UsuarioInclusao { get; set; }

        /// <summary>Data que o item foi modificado</summary>
        [InternalName("Modified", ReadOnly = true)]
        public DateTime DataAlteracao { get; set; }

        /// <summary>Retorna o nome do usuário que efetuou a alteração</summary>
        [InternalName("Editor", ReadOnly = true)]
        public String UsuarioAlteracao { get; set; }
	}
}
