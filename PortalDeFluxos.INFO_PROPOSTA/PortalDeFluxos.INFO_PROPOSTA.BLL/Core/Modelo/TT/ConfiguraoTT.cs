using System;
using System.Collections.Generic;

namespace PortalDeFluxos.Core.BLL.Modelo.T4
{
    public class ConfiguraoTT
    {
        /// <summary>
        /// Url do Site de desenvolvimento
        /// </summary>
        public String UrlSite { get; set; }

        /// <summary>
        /// Display name da lista  automatico
        /// </summary>
        public String NomeControle { get; set; }


        /// <summary>
        /// Menus
        /// </summary>
        public List<MenuTT> Menus { get; set; }

         /// <summary>
        /// Menus
        /// </summary>
        public String NomeLista { get; set; }

         /// <summary>
        /// Entidade referente a lista alvo
        /// </summary>
        public String EntidadeLista { get; set; }
        
        
        /// <summary>
        /// Campos não obrigatórios  - Client
        /// </summary>
        public List<String> NaoObrigatoriosLista { get; set; }

        /// <summary>
        /// EntidadesBD presentes no formulário
        /// </summary>
        public List<String> EntidadesBD { get; set; }
        
    }

    public class MenuTT
    {
        /// <summary>
        /// Id do Menu
        /// </summary>
        public String IdMenu { get; set; }

        /// <summary>
        /// Id do Placehoder - caso seja necessário
        /// </summary>
        public String IdPlaceHolder { get; set; }

        /// <summary>
        /// Nome do Menu
        /// </summary>
        public String NomeMenu { get; set; }

        public Boolean Selecionado { get; set; }

    }
}
