using PortalDeFluxos.Core.BLL.Atributos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalDeFluxos.Core.BLL.Modelo
{
    [InternalName("Lookup")]
    public class Lookup : EntidadeSP
    {
        [InternalName("Gerente")]
        public String Gerente { get; set; }
    }

    [InternalName("TesteLookup")]
    public class TesteLookup :EntidadeSP
    {
        [InternalName("ColunaA")]
        public Lookup ColunaA { get; set; }

        [InternalName("ColunaB")]
        public List<Lookup> ColunaB { get; set; }
    }
}
