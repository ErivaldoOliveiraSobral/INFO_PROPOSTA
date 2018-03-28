using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace PortalDeFluxos.Core.Wrapper
{
    public class ControleAnoContratual : ControleBase
    {
		public ControleAnoContratual(Control control)
		{
			_controle = control;
		}

		public Dictionary<Int32, Decimal?> ObterValores(Boolean anoZero = false)
		{
			object valores = _controle.GetType().GetMethod("ObterValores").Invoke(_controle, new object[] { anoZero });
			if (valores is Dictionary<Int32, Decimal?>)
				return (Dictionary<Int32, Decimal?>)valores;
			else
				return new Dictionary<Int32, Decimal?>();
		}

		public void CarregarValores(Dictionary<Int32, Decimal?> valoresAnosContratuais_, Int32 numeroAnos_, String nomeControle_, Boolean readOnly = false, String customClass = "decimal", String lblLegenda = "", Boolean anoZero = false, String mensageToolTip = "")
		{
			_controle.GetType().GetMethod("CarregarValores").Invoke(_controle, new object[] { valoresAnosContratuais_, numeroAnos_, nomeControle_, readOnly, customClass, lblLegenda, anoZero, mensageToolTip });
		}

		public void CarregarValoresAnos(Dictionary<Int32, Decimal?> valoresAnosContratuais_, Int32 numeroAnos_, String nomeControle_, Boolean readOnly = false, String customClass = "decimal", String lblLegenda = "", Boolean anoZero = false, Int32 qtdAnosLegenda = 10, String mensageToolTip = "")
		{
			_controle.GetType().GetMethod("CarregarValoresAnos").Invoke(_controle, new object[] { valoresAnosContratuais_, numeroAnos_, nomeControle_, readOnly, customClass, lblLegenda, qtdAnosLegenda, anoZero, mensageToolTip });
		}

		public void DefinirCamposSomenteLeitura()
		{
			_controle.GetType().GetMethod("DefinirCamposSomenteLeitura").Invoke(_controle, new object[] { });
		}

		public void DefinirCamposRequired(int? qntCamposHabilitados = null)
		{
			_controle.GetType().GetMethod("DefinirCamposRequired").Invoke(_controle, new object[] { qntCamposHabilitados });
		}

		public void DefinirAcompanhamento10Anos(String nomeControle_, String mensageToolTip = "")
		{
			_controle.GetType().GetMethod("DefinirAcompanhamento10Anos").Invoke(_controle, new object[] { nomeControle_, mensageToolTip });
		}

		public void DefinirAcompanhamento15Anos(String nomeControle_, String mensageToolTip = "")
		{
			_controle.GetType().GetMethod("DefinirAcompanhamento15Anos").Invoke(_controle, new object[] { nomeControle_, mensageToolTip });
		}

		public void DefinirAcompanhamento20Anos(String nomeControle_, String lblLegenda = "", String mensageToolTip = "")
		{
			_controle.GetType().GetMethod("DefinirAcompanhamento20Anos").Invoke(_controle, new object[] { nomeControle_, lblLegenda, mensageToolTip });
		}

		public void DefinirAcompanhamento20AnoZero(String nomeControle_, String lblLegenda = "", Boolean anoZero = false, String mensageToolTip = "")
		{
			_controle.GetType().GetMethod("DefinirAcompanhamento20AnoZero").Invoke(_controle, new object[] { nomeControle_, lblLegenda, anoZero, mensageToolTip });
		}

		public void DefinirAcompanhamentoAnos(String nomeControle_, String lblLegenda = "", Int32 qtdAnosCampos = 10, Int32 qtdAnosLegenda = 10, Boolean anoZero = false)
		{
			_controle.GetType().GetMethod("DefinirAcompanhamentoAnos").Invoke(_controle, new object[] { nomeControle_, lblLegenda, qtdAnosCampos, qtdAnosLegenda, anoZero });
		}
    }
}
