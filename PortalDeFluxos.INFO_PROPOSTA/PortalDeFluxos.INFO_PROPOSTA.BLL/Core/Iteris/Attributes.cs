using System;
using System.Globalization;
using System.Reflection;

namespace Iteris {

	[AttributeUsage(AttributeTargets.All)]
	public sealed class TitleAttribute : Attribute {
		// Fields
		private static TitleAttribute _default = new TitleAttribute();

		private String _title;

		// Methods
		public TitleAttribute()
			: this(String.Empty) {
		}

		public TitleAttribute(String title) {
			_title = title;
		}

		public override Boolean Equals(Object obj) {
			TitleAttribute attribute = null;

			if (obj == this)
				return true;

			attribute = obj as TitleAttribute;
			return ((attribute != null) && (attribute.Title == Title));
		}

		public override Int32 GetHashCode() {
			return Title.GetHashCode();
		}

		public override Boolean IsDefaultAttribute() {
			return Equals(_default);
		}

		// Properties
		public String Title {
			get {
				return TitleValue;
			}
		}

		private String TitleValue {
			get {
				return _title;
			}
			set {
				_title = value;
			}
		}
	}

    [AttributeUsage(AttributeTargets.Property)]
    public class ScaleAttribute : Attribute
    {
        // Fields
        private static ScaleAttribute _default = new ScaleAttribute();

        private Int32 _scale;

        // Methods
        public ScaleAttribute()
            : this(2)
        {
        }

        public ScaleAttribute(Int32 precision)
        {
            _scale = precision;
        }

        public override Boolean Equals(Object obj)
        {
            ScaleAttribute attribute = null;

            if (obj == this)
                return true;

            attribute = obj as ScaleAttribute;
            return ((attribute != null) && (attribute.Scale == Scale));
        }

        public override Int32 GetHashCode()
        {
            return Scale.GetHashCode();
        }

        public override Boolean IsDefaultAttribute()
        {
            return Equals(_default);
        }

        // Properties
        public Int32 Scale
        {
            get
            {
                return ScaleValue;
            }
        }

        private Int32 ScaleValue
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }
    }

	[AttributeUsage(AttributeTargets.All)]
	public sealed class LocalizableTitleAttribute : Attribute {
		// Fields
		private static LocalizableTitleAttribute _default = new LocalizableTitleAttribute();

		private LocalizableString _title;

		// Methods
		public LocalizableTitleAttribute()
			: this(new LocalizableString()) {
		}

		public LocalizableTitleAttribute(LocalizableString title) {
			_title = title;
		}

		public override Boolean Equals(Object obj) {
			TitleAttribute attribute = null;

			if (obj == this)
				return true;

			attribute = obj as TitleAttribute;
			return ((attribute != null) && (attribute.Title == LocalizableTitle));
		}

		public override Int32 GetHashCode() {
			return LocalizableTitle.GetHashCode();
		}

		public override Boolean IsDefaultAttribute() {
			return Equals(_default);
		}

		// Properties
		public LocalizableString LocalizableTitle {
			get {
				return LocalizableTitleValue;
			}
		}

		private LocalizableString LocalizableTitleValue {
			get {
				return _title;
			}
			set {
				_title = value;
			}
		}
	}

	[AttributeUsage(AttributeTargets.All)]
	public class ResourceTitleAttribute : Attribute {
		// Properties
		public String ResourceTitle {
			get;
			private set;
		}

		public ResourceTitleAttribute(Type resourceType, String resourceName) {
			if (resourceType == null)
				throw new ArgumentNullException("resourceType");

			if (resourceName.IsNullOrWhiteSpace())
				throw new ArgumentNullException("resourceName");

			String localizedResourceTypeName = resourceType.FullName + "_" + CultureInfo.CurrentUICulture.Name.Replace('-', '_');
			Type localizedResourceType = Assembly.GetExecutingAssembly().GetType(localizedResourceTypeName, false);

			if (localizedResourceType == null)
				localizedResourceType = resourceType;

			if (localizedResourceType.GetProperty(resourceName, BindingFlags.Static | BindingFlags.NonPublic) == null)
				throw new InvalidOperationException("Could not find the resource " + resourceName + " in " + localizedResourceType.FullName);

			ResourceTitle = localizedResourceType.GetProperty(resourceName, BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null) as String;
		}

		public override Boolean Equals(Object obj) {
			ResourceTitleAttribute attribute = null;

			if (obj == this)
				return true;

			attribute = obj as ResourceTitleAttribute;
			return ((attribute != null) && (attribute.ResourceTitle == ResourceTitle));
		}

		public override Int32 GetHashCode() {
			return ResourceTitle.GetHashCode();
		}

		public override Boolean IsDefaultAttribute() {
			return false;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class MaxLengthAttribute : Attribute
	{
		// Fields
		private static MaxLengthAttribute _default = new MaxLengthAttribute();

		private Int32 _length;

		// Methods
		public MaxLengthAttribute()
			: this(250)
		{
		}

		public MaxLengthAttribute(Int32 length)
		{
			_length = length;
		}

		public override Boolean Equals(Object obj)
		{
			MaxLengthAttribute attribute = null;

			if (obj == this)
				return true;

			attribute = obj as MaxLengthAttribute;
			return ((attribute != null) && (attribute.MaxLength == MaxLength));
		}

		public override Int32 GetHashCode()
		{
			return MaxLength.GetHashCode();
		}

		public override Boolean IsDefaultAttribute()
		{
			return Equals(_default);
		}

		// Properties
		public Int32 MaxLength
		{
			get
			{
				return MaxLengthValue;
			}
		}

		private Int32 MaxLengthValue
		{
			get
			{
				return _length;
			}
			set
			{
				_length = value;
			}
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class PrecisionAttribute : Attribute
	{
		// Fields
		private static PrecisionAttribute _default = new PrecisionAttribute();

		private Int32 _precision;

		// Methods
		public PrecisionAttribute()
			: this(18)
		{
		}

		public PrecisionAttribute(Int32 precision)
		{
			_precision = precision;
		}

		public override Boolean Equals(Object obj)
		{
			PrecisionAttribute attribute = null;

			if (obj == this)
				return true;

			attribute = obj as PrecisionAttribute;
			return ((attribute != null) && (attribute.Precision == Precision));
		}

		public override Int32 GetHashCode()
		{
			return Precision.GetHashCode();
		}

		public override Boolean IsDefaultAttribute()
		{
			return Equals(_default);
		}

		// Properties
		public Int32 Precision
		{
			get
			{
				return PrecisionValue;
			}
		}

		private Int32 PrecisionValue
		{
			get
			{
				return _precision;
			}
			set
			{
				_precision = value;
			}
		}
	}

	[AttributeUsage(AttributeTargets.All)]
	public sealed class InfoExtraAttribute : Attribute
	{
		// Fields
		private static InfoExtraAttribute _default = new InfoExtraAttribute();

		private String _infoExtra;

		// Methods
		public InfoExtraAttribute()
			: this(String.Empty)
		{
		}

		public InfoExtraAttribute(String infoExtra)
		{
			_infoExtra = infoExtra;
		}

		public override Boolean Equals(Object obj)
		{
			InfoExtraAttribute attribute = null;

			if (obj == this)
				return true;

			attribute = obj as InfoExtraAttribute;
			return ((attribute != null) && (attribute.InfoExtra == InfoExtra));
		}

		public override Int32 GetHashCode()
		{
			return InfoExtra.GetHashCode();
		}

		public override Boolean IsDefaultAttribute()
		{
			return Equals(_default);
		}

		// Properties
		public String InfoExtra
		{
			get
			{
				return InfoExtraValue;
			}
		}

		private String InfoExtraValue
		{
			get
			{
				return _infoExtra;
			}
			set
			{
				_infoExtra = value;
			}
		}
	}
}
