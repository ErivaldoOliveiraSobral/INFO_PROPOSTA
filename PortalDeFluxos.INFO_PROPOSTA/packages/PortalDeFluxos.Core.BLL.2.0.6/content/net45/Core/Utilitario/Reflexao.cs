using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Globalization;

namespace PortalDeFluxos.Core.BLL.Utilitario
{
	/// <summary>
	/// Métodos para reflexão de código
	/// </summary>
	public static class Reflexao
	{
		public static string DllFullName()
		{
			return Assembly.GetExecutingAssembly().FullName;
		}

		/// <summary>
		/// Efetua a cópia de objetos
		/// </summary>
		/// <typeparam name="T">Tipo do objeto a ser copiado</typeparam>
		/// <param name="objetoFonte">Origem</param>
		/// <param name="objetoDestino">Destino</param>
		public static void CopiarObjeto<T>(T objetoFonte, T objetoDestino)
		{
			foreach (PropertyInfo property in typeof(T).GetProperties())
				if (property.PropertyType.Namespace == "System"
					|| property.PropertyType.Namespace == "System.Xml.Linq"
					|| property.PropertyType.Namespace == "PortalDeFluxos.Core.BLL.Modelo")
					property.SetValue(objetoDestino, property.GetValue(objetoFonte, null), null);
		}

		/// <summary>Verifica se existe a propriedade no objeto</summary>
		/// <param name="objeto_">Objeto</param>
		/// <param name="propriedade_">Propriedade</param>
		/// <returns>Se existe</returns>
		public static void DefinePropriedade(object objeto_, string propriedade_, object valor_, CultureInfo culture = null)
		{
			Type myType = objeto_.GetType();

			//Busca os métodos
			PropertyInfo p = myType.GetProperty(propriedade_);

			if (p != null)
			{
				if (valor_ != null && p.PropertyType != valor_.GetType() && valor_.GetType() == typeof(string))
				{
					valor_ = ChangeType(p.PropertyType, valor_, culture);
				}
				if (valor_ != null || (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
					p.SetValue(objeto_, valor_, null);
				else if (p.PropertyType == typeof(string))
					p.SetValue(objeto_, "", null);
			}

		}


		/// <summary>Verifica se existe a propriedade no objeto</summary>
		/// <param name="objeto_">Objeto</param>
		/// <param name="propriedade_">Propriedade</param>
		/// <returns>Se existe</returns>
		public static void DefinePropriedade(object objeto_, PropertyInfo propertyInfo, object valor_, CultureInfo culture = null)
		{
			if (valor_ != null && propertyInfo.PropertyType != valor_.GetType() && valor_.GetType() == typeof(string))
			{
				valor_ = ChangeType(propertyInfo.PropertyType, valor_, culture);
			}
			if (valor_ != null || (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
				propertyInfo.SetValue(objeto_, valor_, null);
			else if (propertyInfo.PropertyType == typeof(string))
				propertyInfo.SetValue(objeto_, "", null);
		}

		public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
		{
			return (propertyExpression.Body as MemberExpression).Member.Name;
		}

		public static bool IsVirtualProperty(Type tipo, string nomePropriedade)
		{
			bool retorno = false;
			PropertyInfo[] properties = tipo.GetProperties().Where(p => p.GetMethod.IsVirtual).ToArray();
			foreach (PropertyInfo propriedade in properties)
			{
				if (propriedade.Name == nomePropriedade)
					retorno = true;
			}
			return retorno;
		}

		/// <summary>Retorna o valor da propriedade</summary>
		/// <param name="objeto_">Objeto</param>
		/// <param name="propriedade_">Propriedade</param>
		/// <returns>Valor</returns>
		public static object BuscarValorPropriedade(object objeto_, string propriedade_)
		{
			Type myType = objeto_.GetType();
			//Busca os métodos
			PropertyInfo p = myType.GetProperty(propriedade_);
			if (p != null)
				return p.GetValue(objeto_, null);
			else
				return null;
		}


		private static Object ChangeType(Type toType, object o, CultureInfo culture = null)
		{
			Type conversionType = Nullable.GetUnderlyingType(toType) ?? toType;
			object originalValue = o;
			try
			{
				o = culture != null ? Convert.ChangeType(o, conversionType, culture) : Convert.ChangeType(o, conversionType);
			}
			catch
			{
				o = originalValue;
				if (o.ToString() == "")
					return null;
				try
				{
					if (culture == null)
						return o;
					if (conversionType == typeof(int))
						o = (Int32)Convert.ToDecimal(o.ToString(), culture);
				}
				catch
				{ }
			}

			return o;
		}

		public static String GetPropertyName(LambdaExpression expression)
		{
			var lambda = expression as LambdaExpression;
			MemberExpression memberExpression;
			if (lambda.Body is UnaryExpression)
			{
				var unaryExpression = lambda.Body as UnaryExpression;
				memberExpression = unaryExpression.Operand as MemberExpression;
			}
			else
				memberExpression = lambda.Body as MemberExpression;

			if (memberExpression != null)
			{
				var propertyInfo = memberExpression.Member as PropertyInfo;
				return propertyInfo.Name;
			}

			return null;
		}
	}

	#region [ Compara usando lambda ]
	public class LambdaComparer<T> : IEqualityComparer<T>
	{
		private readonly Func<T, T, bool> _lambdaComparer;
		private readonly Func<T, int> _lambdaHash;

		public LambdaComparer(Func<T, T, bool> lambdaComparer) :
			this(lambdaComparer, o => 0)
		{
		}

		public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
		{
			if (lambdaComparer == null)
				throw new ArgumentNullException("lambdaComparer");
			if (lambdaHash == null)
				throw new ArgumentNullException("lambdaHash");

			_lambdaComparer = lambdaComparer;
			_lambdaHash = lambdaHash;
		}

		public bool Equals(T x, T y)
		{
			return _lambdaComparer(x, y);
		}

		public int GetHashCode(T obj)
		{
			return _lambdaHash(obj);
		}
	}
	#endregion
}
