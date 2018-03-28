using System;
using System.Data.Entity;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    public partial class PortalDeFluxoEntities : DbContext
    {
        public PortalDeFluxoEntities(String connectionString)
            : base(connectionString)
        {
        }
    }
}
