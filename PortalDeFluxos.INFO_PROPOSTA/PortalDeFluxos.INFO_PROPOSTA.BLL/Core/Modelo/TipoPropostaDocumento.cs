//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Iteris;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    using System;
    using System.Collections.Generic;
    
    public partial class TipoPropostaDocumento : EntidadeDB, IEntidadeDBCore
    {
    	
        public int IdTipoPropostaDocumento { get; set; }
    	
        public int IdItem { get; set; }
    	
        public Nullable<int> IdTipoProposta { get; set; }
    	
        public Nullable<int> IdDocumento { get; set; }
    	
        public Nullable<bool> Tem { get; set; }
    	
        public Nullable<bool> Atende { get; set; }
    	
        public Nullable<bool> Excecao { get; set; }
    	
        public Nullable<bool> Dispensado { get; set; }
    
        public virtual Documento Documento { get; set; }
        public virtual TipoProposta TipoProposta { get; set; }
    }
}
