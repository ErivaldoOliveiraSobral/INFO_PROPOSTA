using System;
using System.Web.Script.Serialization;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public class Entidade {
        private PortalWeb _contexto = null;
        
		/// <summary>Contexto Web atual</summary>
		[ScriptIgnore]
		public PortalWeb Contexto { 
            get {
                if (_contexto != null)
                    return _contexto;

                if (PortalWeb.ContextoWebAtual != null) { 
                    this._contexto = PortalWeb.ContextoWebAtual;
                    return this._contexto;
                }

                //Contexto não implementado
                throw new NullReferenceException("PortalWeb não implementado. Implementar a classe de contexto PortalWeb.");
            }
            set { _contexto = value; }
        }

		public Entidade Clone()
        {
            return (Entidade)this.MemberwiseClone();
        }

    }
}
