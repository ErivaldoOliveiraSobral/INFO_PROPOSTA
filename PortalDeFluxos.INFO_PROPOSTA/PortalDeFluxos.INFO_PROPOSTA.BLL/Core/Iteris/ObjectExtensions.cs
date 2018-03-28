using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Iteris
{

    /// <summary>
    /// This class contains all object extension methods of the Iteris library.
    /// </summary>
    public static class ObjectExtensions
    {

        /// <summary>
        /// Gets a property of an attribute of this object.
        /// </summary>
        /// <param name="attributeType">The attribute type.</param>
        /// <returns>The attribute value.</returns>
        public static T GetAttributeValue<T>(this Object value, Type attributeType)
        {
            Type objectType = null;
            Object[] attributes = null;
            String attributeName = null;

            if (value == null)
                throw new ArgumentNullException("value");

            if (attributeType == null)
                throw new ArgumentNullException("attributeType");

            objectType = value.GetType();
            attributeName = attributeType.Name.Left(attributeType.Name.Length - "Attribute".Length);

            if (value.GetType().BaseType == typeof(Enum))
                attributes = objectType.GetField(value.ToString()).GetCustomAttributes(attributeType, false);
            else
                attributes = objectType.GetCustomAttributes(attributeType, false);

            if (attributes.Length > 0)
                return (T)attributeType.GetProperty(attributeName).GetValue(attributes[0], null);

            return default(T);
        }

        /// <summary>
        /// Gets a string property of an attribute of this object.
        /// </summary>
        /// <param name="attributeType">The attribute type.</param>
        /// <returns>The attribute string value.</returns>
        public static String GetAttributeValue(this Object value, Type attributeType)
        {
            return GetAttributeValue<String>(value, attributeType);
        }


        /// <summary>
        /// Gets the title attribute of this object.
        /// </summary>
        /// <returns>The object's title.</returns>
        public static String GetTitle(this Object value)
        {
            return GetAttributeValue(value, typeof(TitleAttribute));
        }

        /// <summary>
        /// Gets the localizable title attribute of this object.
        /// </summary>
        /// <returns>The object's localizable title.</returns>
        public static LocalizableString GetLocalizableTitle(this Object value)
        {
            return GetAttributeValue<LocalizableString>(value, typeof(LocalizableTitleAttribute));
        }

        /// <summary>
        /// Gets the resource title attribute of this object.
        /// </summary>
        /// <returns>The object's resource title.</returns>
        public static String GetResourceTitle(this Object value)
        {
            return GetAttributeValue<String>(value, typeof(ResourceTitleAttribute));
        }

        /// <summary>
        /// Get the description attribute of this object.
        /// </summary>
        /// <returns>The object's description.</returns>
        public static String GetDescription(this Object value)
        {
            return GetAttributeValue(value, typeof(DescriptionAttribute));
        }

        /// <summary>
        /// Get the scale attribute of this object.
        /// </summary>
        /// <returns>The object's description.</returns>
        public static Int32 GetScale(this PropertyInfo value)
        {
            object[] titulo = value.GetCustomAttributes(typeof(ScaleAttribute), true);
            if (titulo == null || titulo.Length == 0)
                return 0;

            return ((ScaleAttribute)titulo.GetValue(0)).Scale;
        }

		/// <summary>
		/// Get the InfoExtra attribute of this object.
		/// </summary>
		/// <returns>The object's description.</returns>
		public static String GetInfoExtra(this Object value)
		{
			return GetAttributeValue(value, typeof(InfoExtraAttribute));
		}

        /// <summary>
        /// Returns null if the object is DBNull.
        /// </summary>
        /// <param name="value">The object to be tested.</param>
        /// <returns>The value null if the object value is equal to DBNull.Value</returns>
        public static Object NullIfDBNull(this Object value)
        {
            if (value is DBNull)
                return null;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj_"></param>
        /// <returns></returns>
        public static T Converter<T>(this Object obj_)
        {
            Type objectType = obj_.GetType();
            Type target = typeof(T);

            var x = Activator.CreateInstance(target, false);
            var z = from source in objectType.GetMembers().ToList()
                    where source.MemberType == MemberTypes.Property
                    select source;
            var d = from source in target.GetMembers().ToList()
                    where source.MemberType == MemberTypes.Property
                    select source;

            List<MemberInfo> members = d.Where(memberInfo => d.Select(c => c.Name).ToList().Contains(memberInfo.Name)).ToList();
            PropertyInfo propertyInfo;
            object value;

            foreach (var memberInfo in members)
            {
                propertyInfo = typeof(T).GetProperty(memberInfo.Name);
                value = obj_.GetType().GetProperty(memberInfo.Name).GetValue(obj_, null);

                propertyInfo.SetValue(x, value, null);
            }

            return (T)x;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="obj_"></param>
        /// <returns></returns>
        public static T Converter<T, U>(this Object obj_)
        {
            Type objectType = obj_.GetType();
            Type target = typeof(T);

            var x = Activator.CreateInstance(target, false);
            var z = from source in objectType.GetMembers().ToList()
                    where source.MemberType == MemberTypes.Property
                    select source;
            var d = from source in target.GetMembers().ToList()
                    where source.MemberType == MemberTypes.Property
                    select source;

            List<MemberInfo> members = d.Where(memberInfo => d.Select(c => c.Name).ToList().Contains(memberInfo.Name)).ToList();
            PropertyInfo propertyInfo;
            object value;

            foreach (var memberInfo in members)
            {
                propertyInfo = typeof(T).GetProperty(memberInfo.Name);
                value = obj_.GetType().GetProperty(memberInfo.Name).GetValue(obj_, null);

                try
                {
                    propertyInfo.SetValue(x, value, null);
                }
                catch (ArgumentException)
                {
                    value = obj_.GetType().GetProperty(memberInfo.Name).GetValue(obj_, null).Converter<U>();
                    propertyInfo.SetValue(x, value, null);
                }
                
            }

            return (T)x;
        }
    }
}
