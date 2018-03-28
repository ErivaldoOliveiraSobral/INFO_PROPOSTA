using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Iteris
{
    /// <summary>
    /// This class contains all string extension methods of the Iteris library.
    /// </summary>
	public static class StringExtensions
	{

		/// <summary>
		/// Appends a string value at the end of this string only if this string doesn't ends with the value passed as parameter.
		/// </summary>
		/// <param name="value">The string reference.</param>
		/// <param name="defaultValue">The value to be appended to the string.</param>
		/// <returns>If the string already ends with the value passed as parameter, the string itself. Otherwise, the string with the value appended to it.</returns>
		public static String AppendIfMissing(this String value, String defaultValue)
		{
			if (!String.IsNullOrEmpty(value))
				if (!value.EndsWith(defaultValue, StringComparison.OrdinalIgnoreCase))
					return String.Concat(value, defaultValue);

			return value;
		}

		/// <summary>
		/// Cut a string by getting the leftmost characters of it appending chars to the end.
		/// </summary>
		/// <param name="length">The length of the returned string.</param>
		/// <param name="charactersToAppendAfter">The characters to be appended to the end of it.</param>
		/// <returns>The cutted string.</returns>
		public static String Cut(this String value, Int32 length, String charactersToAppendAfter)
		{
			return String.Concat(Left(value, length), charactersToAppendAfter);
		}

		/// <summary>
		/// Cut a string by getting the leftmost characters of it.
		/// </summary>
		/// <param name="length">The length of the returned string.</param>
		/// <returns>The cutted string.</returns>
		public static String Cut(this String value, Int32 length)
		{
			return Cut(value, length, null);
		}

		/// <summary>
		/// Gets the leftmost characters of the string.
		/// </summary>
		/// <param name="length">The length of the string to be returned.</param>
		/// <returns>The left portion of the string.</returns>
		public static String Left(this String value, Int32 length)
		{
			if (String.IsNullOrEmpty(value))
				return value;

			return value.Substring(0, length);
		}

		/// <summary>
		/// Returns the rightmost characters of the string.
		/// </summary>
		/// <param name="length">The length of the string to be returned.</param>
		/// <returns>The right portion of the string.</returns>
		public static String Right(this String value, Int32 length)
		{
			if (String.IsNullOrEmpty(value))
				return value;

			return value.Substring(value.Length - length, length);
		}

		/// <summary>
		/// Returns null if this instance is equals to String.Empty.
		/// </summary>
		/// <returns>null if this instance is equals to String.Empty; otherwise, the previous string value.</returns>
		public static String NullIfEmptyOrWhiteSpace(this String value)
		{
			if (value.IsNullOrWhiteSpace())
				return null;
			return value;
		}

		/// <summary>
		/// Returns the specified string when the context string is null or empty.
		/// </summary>
		/// <returns>The specified string when the context string is null or empty. Otherwise, the context string itself.</returns>
		public static String IfNullOrEmptyOrWhiteSpace(this String value, String returnValue)
		{
			if (value.IsNullOrWhiteSpace())
				return returnValue;
			return value;
		}

		/// <summary>
		/// Gets String.Empty if this instance is null.
		/// </summary>
		/// <returns>String.Empty if this instance is null; otherwise, keeps the previous string value.</returns>
		public static String EmptyIfNull(this String value)
		{
			return value ?? String.Empty;
		}

		/// <summary>
		/// Converts a string to the title case accordingly to the culture passed as parameter.
		/// </summary>
		/// <param name="cultureInfo">The culture info to use the title rules of.</param>
		/// <returns>The converted string.</returns>
		public static String ToTitleCase(this String value, CultureInfo cultureInfo)
		{
			if (cultureInfo == null)
				return value;

			return cultureInfo.TextInfo.ToTitleCase(value);
		}

		/// <summary>
		/// Converts a string to the title case accordingly to the culture passed as parameter.
		/// </summary>
		/// <returns>The converted string.</returns>
		public static String ToTitleCase(this String value)
		{
			return ToTitleCase(value, CultureInfo.InvariantCulture);
		}


		/// <summary>
		/// Converts a string into a camel case string.
		/// </summary>
		/// <param name="value">The string to be converted.</param>
		/// <returns>The passed string in camel case.</returns>
		public static String ToCamelCase(this String value)
		{
			return value.Substring(0, 1).ToLowerInvariant() + value.Substring(1, value.Length - 1);
		}

		/// <summary>
		/// Tries to parse this string into a byte.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		/// <returns>The parsed byte when it contains a valid value; otherwise, the default value.</returns>
		public static Byte TryParseToByte(this String value, Byte defaultValue)
		{
			Byte result = 0;
			if (Byte.TryParse(value, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into a byte.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="style">The number style.</param>
		/// <param name="provider">The number format provider</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		public static Byte TryParseToByte(this String value, NumberStyles style, IFormatProvider provider, Byte defaultValue)
		{
			Byte result = 0;
			if (Byte.TryParse(value, style, provider, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into a short integer.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		/// <returns>The parsed byte when it contains a valid value; otherwise, the default value.</returns>
		public static Int16 TryParseToInt16(this String value, Int16 defaultValue)
		{
			Int16 result = 0;
			if (Int16.TryParse(value, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into a short integer.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="style">The number style.</param>
		/// <param name="provider">The number format provider</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		public static Int16 TryParseToInt16(this String value, NumberStyles style, IFormatProvider provider, Int16 defaultValue)
		{
			Int16 result = 0;
			if (Int16.TryParse(value, style, provider, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into an integer.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		/// <returns>The parsed byte when it contains a valid value; otherwise, the default value.</returns>
		public static Int32 TryParseToInt32(this String value, Int32 defaultValue)
		{
			Int32 result = 0;
			if (Int32.TryParse(value, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into an integer.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="style">The number style.</param>
		/// <param name="provider">The number format provider</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		public static Int32 TryParseToInt32(this String value, NumberStyles style, IFormatProvider provider, Int32 defaultValue)
		{
			Int32 result = 0;
			if (Int32.TryParse(value, style, provider, out result))
				return result;
			else
			{
				Double valorDouble = 0;
				if (Double.TryParse(value, style, provider, out valorDouble))
					return Convert.ToInt32(valorDouble);
			}

			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into a long integer.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		/// <returns>The parsed byte when it contains a valid value; otherwise, the default value.</returns>
		public static Int64 TryParseToInt64(this String value, Int64 defaultValue)
		{
			Int64 result = 0;
			if (Int64.TryParse(value, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into a long integer.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="style">The number style.</param>
		/// <param name="provider">The number format provider</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		public static Int64 TryParseToInt64(this String value, NumberStyles style, IFormatProvider provider, Int64 defaultValue)
		{
			Int64 result = 0;
			if (Int64.TryParse(value, style, provider, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into a float.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		/// <returns>The parsed byte when it contains a valid value; otherwise, the default value.</returns>
		public static Single TryParseToSingle(this String value, Single defaultValue)
		{
			Single result = 0;
			if (Single.TryParse(value, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into a float.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="style">The number style.</param>
		/// <param name="provider">The number format provider</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		public static Single TryParseToSingle(this String value, NumberStyles style, IFormatProvider provider, Single defaultValue)
		{
			Single result = 0;
			if (Single.TryParse(value, style, provider, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into a Decimal.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		/// <returns>The parsed byte when it contains a valid value; otherwise, the default value.</returns>
		public static Decimal TryParseToDecimal(this String value, Decimal defaultValue)
		{
			Decimal result = 0;
			if (Decimal.TryParse(value, out result))
				return result;
			return defaultValue;
		}

		public static Decimal TryParseToDecimal(this String value, NumberStyles style, IFormatProvider provider, Decimal defaultValue)
		{
			Decimal result = 0;
			if (Decimal.TryParse(value, style, provider, out result))
				return result;
			return defaultValue;
		}

		public static Decimal? TryParseToDecimal(this String value, Decimal? defaultValue)
		{
			Decimal result = 0;
			if (Decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into a double.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		/// <returns>The parsed byte when it contains a valid value; otherwise, the default value.</returns>
		public static Double TryParseToDouble(this String value, Double defaultValue)
		{
			Double result = 0;
			if (Double.TryParse(value, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into a double.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="style">The number style.</param>
		/// <param name="provider">The number format provider</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		public static Double TryParseToDouble(this String value, NumberStyles style, IFormatProvider provider, Double defaultValue)
		{
			Double result = 0;
			if (Double.TryParse(value, style, provider, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into a boolean.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		public static Boolean TryParseToBoolean(this String value, Boolean defaultValue)
		{
			Boolean result = false;
			if (Boolean.TryParse(value, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Removes all diacritic marks (accents) from then string.
		/// </summary>
		/// <param name="string">The string to be processed.</param>
		/// <returns>The string without any diacritic marks.</returns>
		public static String ToNormalizedString(this String value)
		{
			StringBuilder normalizedTitle = null;

			if (value == null)
				return null;

			normalizedTitle = new StringBuilder();

			foreach (Char character in value.Normalize(NormalizationForm.FormD))
				if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
					normalizedTitle.Append(character);

			return normalizedTitle.ToString();
		}

		/// <summary>
		/// Creates a Pascal-case name without any diacritic marks (accents) or spaces.
		/// </summary>
		/// <param name="string">The string to be used in the name generation.</param>
		/// <returns>The string in Pascal-case without any diacritic marks (accents) or spaces)</returns>
		public static String ToFileName(this String value, String extension)
		{
			return String.Format("{0}.{1}"
				, Regex.Replace(value, @"[^\w\s]*", "").ToNormalizedString().ToTitleCase(CultureInfo.CurrentCulture).Replace("  ", "_").Replace(" ", "_").Replace("-", String.Empty)
				, extension);
		}

		/// <summary>
		/// Creates a object name without any diacritic marks (accents) or spaces.
		/// </summary>
		/// <param name="string">The string to be used in the name generation.</param>
		/// <returns>The string in Pascal-case without any diacritic marks (accents) or spaces)</returns>
		public static String ToObjectName(this String value)
		{
			String retorno = "";
			value = ToNormalizedString(Regex.Replace(value, @"[^\w\s]*", "")).Replace("-", String.Empty);
			string[] words = value.Split(' ');
			foreach (String word in words)
			{
				String wordTratado = word.Replace(" ", String.Empty);
				if (wordTratado != String.Empty)
					if (wordTratado.Length > 1)
						retorno += wordTratado.First().ToString().ToUpper() + wordTratado.Substring(1);
					else
						retorno += wordTratado.First().ToString().ToUpper();
			}

			return retorno;
		}

		/// <summary>
		/// Encloses the string within quotes.
		/// </summary>
		/// <param name="string">The string to be enclosed with quotes.</param>
		/// <param name="ignoreWhenEmpty">The flag indicating whether the string should be ignored when empty.</param>
		/// <returns></returns>
		public static String EncloseInQuotes(this String value, Boolean ignoreWhenEmpty)
		{
			if (String.IsNullOrEmpty(value) && ignoreWhenEmpty)
				return value;
			return String.Concat("'", value, "'");
		}

		/// <summary>
		/// Encloses the string within quotes.
		/// </summary>
		/// <param name="string">The string to be enclosed with quotes.</param>
		/// <returns></returns>
		public static String EncloseInQuotes(this String value)
		{
			return EncloseInQuotes(value, false);
		}

		/// <summary>
		/// Encloses the string within double quotes.
		/// </summary>
		/// <param name="string">The string to be enclosed with double quotes.</param>
		/// <param name="ignoreWhenEmpty">The flag indicating whether the string should be ignored when empty.</param>
		/// <returns></returns>
		public static String EncloseInDoubleQuotes(this String value, Boolean ignoreWhenEmpty)
		{
			if (String.IsNullOrEmpty(value) && ignoreWhenEmpty)
				return value;
			return String.Concat(@"""", value, @"""");
		}

		/// <summary>
		/// Escape Double Quote
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static String EscapeQuotes(this String value)
		{
			if (String.IsNullOrEmpty(value))
				return value;

			return value.Replace("\"", "\\\"");
		}

		/// <summary>
		/// Encloses the string within double quotes.
		/// </summary>
		/// <param name="string">The string to be enclosed with double quotes.</param>
		/// <returns></returns>
		public static String EncloseInDoubleQuotes(this String value)
		{
			return EncloseInDoubleQuotes(value, false);
		}

		public static Boolean IsNullOrWhiteSpace(this String value)
		{
			if (String.IsNullOrEmpty(value))
				return true;
			return String.IsNullOrEmpty(value.Trim());
		}

		/// <summary>
		/// Tries to parse this string into an Datetime.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		/// <returns>The parsed byte when it contains a valid value; otherwise, the default value.</returns>
		public static DateTime TryParseToDateTime(this String value, DateTime defaultValue)
		{
			DateTime result;
			if (DateTime.TryParse(value, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Tries to parse this string into an DateTime.
		/// </summary>
		/// <param name="value">The string instance.</param>
		/// <param name="style">The number style.</param>
		/// <param name="provider">The number format provider</param>
		/// <param name="defaultValue">The default value when the parsing is not possible to be done.</param>
		public static DateTime TryParseToDateTime(this String value, DateTimeStyles style, IFormatProvider provider, DateTime defaultValue)
		{
			DateTime result;
			if (DateTime.TryParse(value, provider, style, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Convert string value to decimal ignore the culture.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Decimal value.</returns>
		public static decimal? ToDecimal(this String value)
		{
			if (String.IsNullOrEmpty(value))
				return null;

			decimal number;
			value = value.Trim();
			string tempValue = value;

			if (String.IsNullOrEmpty(tempValue) || tempValue.Contains("#"))
				return null;

			if (tempValue.Contains("%"))
				tempValue = tempValue.Replace("%", "");

			Boolean neg = tempValue.Contains("-");
			tempValue = tempValue.Replace("-", "");

			if (String.IsNullOrEmpty(tempValue))
				return null;

			var punctuation = tempValue.Where(x => char.IsPunctuation(x)).Distinct();
			int count = punctuation.Count();

			NumberFormatInfo format = CultureInfo.InvariantCulture.NumberFormat;
			switch (count)
			{
				case 0:
					break;
				case 1:
					tempValue = tempValue.Replace(".", "");
					tempValue = tempValue.Replace(",", ".");
					break;
				case 2:
					if (punctuation.ElementAt(0) == '.')
						tempValue = tempValue.SwapChar('.', ',');
					break;
				default:
					throw new InvalidCastException();
			}

			number = decimal.Parse(tempValue, format);
			if (neg)
				number = number * -1;
			return number;
		}

		/// <summary>
		/// Convert string value to int ignore the culture.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Decimal value.</returns>
		public static Int32? ToInt32(this String value)
		{
			if (String.IsNullOrEmpty(value))
				return null;

			Int32 number = 0;
			value = value.Trim();
			string tempValue = value;

			if (String.IsNullOrEmpty(tempValue) || tempValue.Contains("#"))
				return null;

			if (tempValue.Contains("%"))
				tempValue = tempValue.Replace("%", "");

			Boolean neg = tempValue.Contains("-");
			tempValue = tempValue.Replace("-", "");

			if (String.IsNullOrEmpty(tempValue))
				return null;

			var punctuation = tempValue.Where(x => char.IsPunctuation(x)).Distinct();
			int count = punctuation.Count();

			NumberFormatInfo format = CultureInfo.InvariantCulture.NumberFormat;
			switch (count)
			{
				case 0:
					break;
				case 1:
					tempValue = tempValue.Replace(",", "").Replace(".", "");
					break;
				case 2:
					tempValue = tempValue.Split(punctuation.ElementAt(1))[0];
					tempValue = tempValue.Replace(",", "").Replace(".", "");
					break;
				default:
					throw new InvalidCastException();
			}

			Boolean notNumber = false;
			try
			{
				number = Int32.Parse(tempValue, format);
			}
			catch
			{
				notNumber = true;

			}
			if (neg && !notNumber)
				number = number * -1;
			return notNumber ? (Int32?)null : number;
		}

		/// <summary>
		/// Convert string value to int ignore the culture.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Decimal value.</returns>
		public static Int64? ToInt(this String value)
		{
			Int64 number = 0;
			value = value.Trim();
			string tempValue = value;

			if (String.IsNullOrEmpty(tempValue))
				return null;

			Boolean neg = tempValue.Contains("-");
			tempValue = tempValue.Replace("-", "");
			value = value.Replace("-", "");

			var punctuation = value.Where(x => char.IsPunctuation(x)).Distinct();
			int count = punctuation.Count();

			NumberFormatInfo format = CultureInfo.InvariantCulture.NumberFormat;
			switch (count)
			{
				case 0:
					break;
				case 1:
					tempValue = value.Replace(",", "").Replace(".", "");
					break;
				case 2:
					tempValue = value.Split(punctuation.ElementAt(1))[0];
					tempValue = tempValue.Replace(",", "").Replace(".", "");
					break;
				default:
					throw new InvalidCastException();
			}

			Boolean notNumber = false;
			try
			{
				number = Int64.Parse(tempValue, format);
			}
			catch
			{
				notNumber = true;

			}
			if (neg && !notNumber)
				number = number * -1;
			return notNumber ? (Int64?)null : number;
		}

		/// <summary>
		/// Convert string value to datetime 
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>DateTime value. Caso não consiga converter retorna null.</returns>
		public static DateTime? ToDateTime(this String value)
		{
			DateTime date = DateTime.MinValue;
			return DateTime.TryParse(value, out date) ? date : (DateTime?)null;
		}

		/// <summary>
		/// Convert string value to datetime 
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>DateTime value. Caso não consiga converter retorna null.</returns>
		public static Boolean? ToBoolean(this String value)
		{
			Boolean finalValue = false;
			return Boolean.TryParse(value, out finalValue) ? finalValue : (Boolean?)null;
		}

		/// <summary>
		/// Convert string value to Guid 
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Guid value. Caso não consiga converter retorna null.</returns>
		public static Guid? ToGuid(this String value)
		{
			Guid finalValue = Guid.Empty;
			return Guid.TryParse(value, out finalValue) ? finalValue : (Guid?)null;
		}

		/// <summary>
		/// Swaps the char.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <returns></returns>
		public static String SwapChar(this String value, char from, char to)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			StringBuilder builder = new StringBuilder();

			foreach (var item in value)
			{
				char c = item;
				if (c == from)
					c = to;
				else if (c == to)
					c = from;

				builder.Append(c);
			}
			return builder.ToString();
		}

		public static String ToNumberMask(this object valor, Int32 casasDecimais)
		{
			return GetNumeroFormatado(valor, casasDecimais);
		}

		public static String GetNumeroFormatado(object valor, Int32 casasDecimais = 2)
		{
			String retorno = "";

			if (valor is Decimal || valor is float || valor is double)
				retorno = valor != null ? String.Format(ObterMascaraDecimal(casasDecimais), valor).Replace(",", ".") : String.Empty;
			else if (valor is Int32 || valor is Int64 || valor is int || valor is Int16)
				retorno = valor != null ? valor.ToString() : String.Empty;

			if (!String.IsNullOrEmpty(retorno))
			{
				String vInt = retorno.Split('.')[0];
				string vNeg = vInt.IndexOf('-') >= 0 ? "-" : "";
				vInt = vInt.Replace("-", "");
				while (vInt.Split('.')[0].Length > 3)
				{
					vInt = Regex.Replace(vInt, @"(\d*)(\d{3})", m => String.Format("{0}{1}{2}"
						, m.Groups[1].Value, m.Groups[1].Value != "" ? "." : "", m.Groups[2].Value));
				}

				if (casasDecimais > 0 && retorno.Split('.').Length > 1)
				{
					String vDec = retorno.Split('.')[1];
					retorno = vNeg + vInt + "," + vDec;
				}
				else
					retorno = vNeg + vInt;
			}

			return retorno;
		}

		public static String ObterMascaraDecimal(Int32 casasDecimais = 2)
		{
			String numeroCasas = "0";
			numeroCasas = numeroCasas.PadRight(casasDecimais, '0');
			String formatacao = "{0:0." + numeroCasas + "}";
			return formatacao;
		}

		public static String ToJScriptString(this String value)
		{
			if (String.IsNullOrEmpty(value))
				return "";
			return value.Replace(@"""", @"\""").Replace(@"'", @"\'").Replace(@"`", @"\`").Replace(Environment.NewLine, "\\n");
		}

		public static Int32 IndexColumnExcel(this String coluna)
		{
			Int32 index = 0;
			Char[] charColunas = coluna.ToCharArray();
			for (int i = charColunas.Length - 1; i >= 0; i--)
				index += (Int32)charColunas[i] * (Int32)Math.Pow(10, charColunas.Length - 1 - i);
			return index;
		}

		public static String StripDomain(this String value)
		{
			return value.Split('\\').Last();
		}
	}
}