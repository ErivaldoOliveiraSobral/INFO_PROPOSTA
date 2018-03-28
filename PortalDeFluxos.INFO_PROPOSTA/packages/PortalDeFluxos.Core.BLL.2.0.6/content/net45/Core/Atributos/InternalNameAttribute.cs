using Iteris;
using System;

namespace PortalDeFluxos.Core.BLL.Atributos
{
    /// <summary>Tipo o campor</summary>
    public enum FieldType
    {
        [Title("Não definido")]
        Undefined = 0,
        [Title("Inteiro")]
        Number = 1,
        [Title("Decimal")]
        Percentage = 2,
        [Title("Double")]
        Money = 3,
        [Title("ReiniciarWF")]
        ReiniciarWF = 4,
        [Title("ReiniciarWF2")]
        ReiniciarWF2 = 5
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
	public class InternalNameAttribute: Attribute
	{
		public String Name  { get; set; }
        public FieldType Type { get; set; }
        public Boolean ReadOnly { get; set; }

		public InternalNameAttribute() { }

		public InternalNameAttribute(String name)
		{
            Name = name;
		}

        public InternalNameAttribute(String value, FieldType type)
        {
            Name = value;
            Type = type;
        }
	}
}
