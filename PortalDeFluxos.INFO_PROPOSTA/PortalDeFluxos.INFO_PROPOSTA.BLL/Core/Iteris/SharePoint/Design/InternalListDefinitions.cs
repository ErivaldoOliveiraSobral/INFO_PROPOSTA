using System;
using System.ComponentModel;

namespace Iteris.SharePoint.Design {

	[Flags]
	public enum InternalListDefinitions {
		[Title("Configuracao")]
		[Description("Armazena a configuração da aplicação.")]
		Configurations = 1
	};

}
