
namespace Iteris {

	public enum CultureLCID : int {
		[Title("None")]
		None = 0,
		[Title("ar")]
		Arabic = 1,
		[Title("bg")]
		Bulgarian = 2,
		[Title("ca")]
		Catalan = 3,
		[Title("zh-CHS")]
		ChineseSimplified = 4,
		[Title("cs")]
		Czech = 5,
		[Title("da")]
		Danish = 6,
		[Title("de")]
		German = 7,
		[Title("el")]
		Greek = 8,
		[Title("en")]
		English = 9,
		[Title("es")]
		Spanish = 10,
		[Title("fi")]
		Finnish = 11,
		[Title("fr")]
		French = 12,
		[Title("he")]
		Hebrew = 13,
		[Title("hu")]
		Hungarian = 14,
		[Title("is")]
		Icelandic = 15,
		[Title("it")]
		Italian = 16,
		[Title("ja")]
		Japanese = 17,
		[Title("ko")]
		Korean = 18,
		[Title("nl")]
		Dutch = 19,
		[Title("no")]
		Norwegian = 20,
		[Title("pl")]
		Polish = 21,
		[Title("pt")]
		Portuguese = 22,
		[Title("ro")]
		Romanian = 24,
		[Title("ru")]
		Russian = 25,
		[Title("hr")]
		Croatian = 26,
		[Title("sk")]
		Slovak = 27,
		[Title("sq")]
		Albanian = 28,
		[Title("sv")]
		Swedish = 29,
		[Title("th")]
		Thai = 30,
		[Title("tr")]
		Turkish = 31,
		[Title("ur")]
		Urdu = 32,
		[Title("id")]
		Indonesian = 33,
		[Title("uk")]
		Ukrainian = 34,
		[Title("be")]
		Belarusian = 35,
		[Title("sl")]
		Slovenian = 36,
		[Title("et")]
		Estonian = 37,
		[Title("lv")]
		Latvian = 38,
		[Title("lt")]
		Lithuanian = 39,
		[Title("fa")]
		Persian = 41,
		[Title("vi")]
		Vietnamese = 42,
		[Title("hy")]
		Armenian = 43,
		[Title("az")]
		Azeri = 44,
		[Title("eu")]
		Basque = 45,
		[Title("mk")]
		Macedonian = 47,
		[Title("af")]
		Afrikaans = 54,
		[Title("ka")]
		Georgian = 55,
		[Title("fo")]
		Faroese = 56,
		[Title("hi")]
		Hindi = 57,
		[Title("ms")]
		Malay = 62,
		[Title("kk")]
		Kazakh = 63,
		[Title("ky")]
		Kyrgyz = 64,
		[Title("sw")]
		Kiswahili = 65,
		[Title("uz")]
		Uzbek = 67,
		[Title("tt")]
		Tatar = 68,
		[Title("pa")]
		Punjabi = 70,
		[Title("gu")]
		Gujarati = 71,
		[Title("ta")]
		Tamil = 73,
		[Title("te")]
		Telugu = 74,
		[Title("kn")]
		Kannada = 75,
		[Title("mr")]
		Marathi = 78,
		[Title("sa")]
		Sanskrit = 79,
		[Title("mn")]
		Mongolian = 80,
		[Title("gl")]
		Galician = 86,
		[Title("kok")]
		Konkani = 87,
		[Title("syr")]
		Syriac = 90,
		[Title("dv")]
		Divehi = 101,
		[Title("")]
		InvariantLanguageInvariantCountry = 127,
		[Title("ar-SA")]
		ArabicSaudiArabia = 1025,
		[Title("bg-BG")]
		BulgarianBulgaria = 1026,
		[Title("ca-ES")]
		CatalanCatalan = 1027,
		[Title("zh-TW")]
		ChineseTaiwan = 1028,
		[Title("cs-CZ")]
		CzechCzechRepublic = 1029,
		[Title("da-DK")]
		DanishDenmark = 1030,
		[Title("de-DE")]
		GermanGermany = 1031,
		[Title("el-GR")]
		GreekGreece = 1032,
		[Title("en-US")]
		EnglishUnitedStates = 1033,
		[Title("fi-FI")]
		FinnishFinland = 1035,
		[Title("fr-FR")]
		FrenchFrance = 1036,
		[Title("he-IL")]
		HebrewIsrael = 1037,
		[Title("hu-HU")]
		HungarianHungary = 1038,
		[Title("is-IS")]
		IcelandicIceland = 1039,
		[Title("it-IT")]
		ItalianItaly = 1040,
		[Title("ja-JP")]
		JapaneseJapan = 1041,
		[Title("ko-KR")]
		KoreanKorea = 1042,
		[Title("nl-NL")]
		DutchNetherlands = 1043,
		[Title("nb-NO")]
		NorwegianBokmlNorway = 1044,
		[Title("pl-PL")]
		PolishPoland = 1045,
		[Title("pt-BR")]
		PortugueseBrazil = 1046,
		[Title("ro-RO")]
		RomanianRomania = 1048,
		[Title("ru-RU")]
		RussianRussia = 1049,
		[Title("hr-HR")]
		CroatianCroatia = 1050,
		[Title("sk-SK")]
		SlovakSlovakia = 1051,
		[Title("sq-AL")]
		AlbanianAlbania = 1052,
		[Title("sv-SE")]
		SwedishSweden = 1053,
		[Title("th-TH")]
		ThaiThailand = 1054,
		[Title("tr-TR")]
		TurkishTurkey = 1055,
		[Title("ur-PK")]
		UrduIslamicRepublicOfPakistan = 1056,
		[Title("id-ID")]
		IndonesianIndonesia = 1057,
		[Title("uk-UA")]
		UkrainianUkraine = 1058,
		[Title("be-BY")]
		BelarusianBelarus = 1059,
		[Title("sl-SI")]
		SlovenianSlovenia = 1060,
		[Title("et-EE")]
		EstonianEstonia = 1061,
		[Title("lv-LV")]
		LatvianLatvia = 1062,
		[Title("lt-LT")]
		LithuanianLithuania = 1063,
		[Title("fa-IR")]
		PersianIran = 1065,
		[Title("vi-VN")]
		VietnameseVietnam = 1066,
		[Title("hy-AM")]
		ArmenianArmenia = 1067,
		[Title("az-Latn-AZ")]
		AzeriLatinAzerbaijan = 1068,
		[Title("eu-ES")]
		BasqueBasque = 1069,
		[Title("mk-MK")]
		MacedonianFormerYugoslavRepublicOfMacedonia = 1071,
		[Title("af-ZA")]
		AfrikaansSouthAfrica = 1078,
		[Title("ka-GE")]
		GeorgianGeorgia = 1079,
		[Title("fo-FO")]
		FaroeseFaroeIslands = 1080,
		[Title("hi-IN")]
		HindiIndia = 1081,
		[Title("ms-MY")]
		MalayMalaysia = 1086,
		[Title("kk-KZ")]
		KazakhKazakhstan = 1087,
		[Title("ky-KG")]
		KyrgyzKyrgyzstan = 1088,
		[Title("sw-KE")]
		KiswahiliKenya = 1089,
		[Title("uz-Latn-UZ")]
		UzbekLatinUzbekistan = 1091,
		[Title("tt-RU")]
		TatarRussia = 1092,
		[Title("pa-IN")]
		PunjabiIndia = 1094,
		[Title("gu-IN")]
		GujaratiIndia = 1095,
		[Title("ta-IN")]
		TamilIndia = 1097,
		[Title("te-IN")]
		TeluguIndia = 1098,
		[Title("kn-IN")]
		KannadaIndia = 1099,
		[Title("mr-IN")]
		MarathiIndia = 1102,
		[Title("sa-IN")]
		SanskritIndia = 1103,
		[Title("mn-MN")]
		MongolianCyrillicMongolia = 1104,
		[Title("gl-ES")]
		GalicianGalician = 1110,
		[Title("kok-IN")]
		KonkaniIndia = 1111,
		[Title("syr-SY")]
		SyriacSyria = 1114,
		[Title("dv-MV")]
		DivehiMaldives = 1125,
		[Title("ar-IQ")]
		ArabicIraq = 2049,
		[Title("zh-CN")]
		ChinesePeoplesRepublicOfChina = 2052,
		[Title("de-CH")]
		GermanSwitzerland = 2055,
		[Title("en-GB")]
		EnglishUnitedKingdom = 2057,
		[Title("es-MX")]
		SpanishMexico = 2058,
		[Title("fr-BE")]
		FrenchBelgium = 2060,
		[Title("it-CH")]
		ItalianSwitzerland = 2064,
		[Title("nl-BE")]
		DutchBelgium = 2067,
		[Title("nn-NO")]
		NorwegianNynorskNorway = 2068,
		[Title("pt-PT")]
		PortuguesePortugal = 2070,
		[Title("sr-Latn-CS")]
		SerbianLatinSerbiaAndMontenegroFormer = 2074,
		[Title("sv-FI")]
		SwedishFinland = 2077,
		[Title("az-Cyrl-AZ")]
		AzeriCyrillicAzerbaijan = 2092,
		[Title("ms-BN")]
		MalayBruneiDarussalam = 2110,
		[Title("uz-Cyrl-UZ")]
		UzbekCyrillicUzbekistan = 2115,
		[Title("ar-EG")]
		ArabicEgypt = 3073,
		[Title("zh-HK")]
		ChineseHongKongSar = 3076,
		[Title("de-AT")]
		GermanAustria = 3079,
		[Title("en-AU")]
		EnglishAustralia = 3081,
		[Title("es-ES")]
		SpanishSpain = 3082,
		[Title("fr-CA")]
		FrenchCanada = 3084,
		[Title("sr-Cyrl-CS")]
		SerbianCyrillicSerbiaAndMontenegroFormer = 3098,
		[Title("ar-LY")]
		ArabicLibya = 4097,
		[Title("zh-SG")]
		ChineseSingapore = 4100,
		[Title("de-LU")]
		GermanLuxembourg = 4103,
		[Title("en-CA")]
		EnglishCanada = 4105,
		[Title("es-GT")]
		SpanishGuatemala = 4106,
		[Title("fr-CH")]
		FrenchSwitzerland = 4108,
		[Title("ar-DZ")]
		ArabicAlgeria = 5121,
		[Title("zh-MO")]
		ChineseMacaoSar = 5124,
		[Title("de-LI")]
		GermanLiechtenstein = 5127,
		[Title("en-NZ")]
		EnglishNewZealand = 5129,
		[Title("es-CR")]
		SpanishCostaRica = 5130,
		[Title("fr-LU")]
		FrenchLuxembourg = 5132,
		[Title("ar-MA")]
		ArabicMorocco = 6145,
		[Title("en-IE")]
		EnglishIreland = 6153,
		[Title("es-PA")]
		SpanishPanama = 6154,
		[Title("fr-MC")]
		FrenchPrincipalityOfMonaco = 6156,
		[Title("ar-TN")]
		ArabicTunisia = 7169,
		[Title("en-ZA")]
		EnglishSouthAfrica = 7177,
		[Title("es-DO")]
		SpanishDominicanRepublic = 7178,
		[Title("ar-OM")]
		ArabicOman = 8193,
		[Title("en-JM")]
		EnglishJamaica = 8201,
		[Title("es-VE")]
		SpanishVenezuela = 8202,
		[Title("ar-YE")]
		ArabicYemen = 9217,
		[Title("en-029")]
		EnglishCaribbean = 9225,
		[Title("es-CO")]
		SpanishColombia = 9226,
		[Title("ar-SY")]
		ArabicSyria = 10241,
		[Title("en-BZ")]
		EnglishBelize = 10249,
		[Title("es-PE")]
		SpanishPeru = 10250,
		[Title("ar-JO")]
		ArabicJordan = 11265,
		[Title("en-TT")]
		EnglishTrinidadAndTobago = 11273,
		[Title("es-AR")]
		SpanishArgentina = 11274,
		[Title("ar-LB")]
		ArabicLebanon = 12289,
		[Title("en-ZW")]
		EnglishZimbabwe = 12297,
		[Title("es-EC")]
		SpanishEcuador = 12298,
		[Title("ar-KW")]
		ArabicKuwait = 13313,
		[Title("en-PH")]
		EnglishRepublicOfThePhilippines = 13321,
		[Title("es-CL")]
		SpanishChile = 13322,
		[Title("ar-AE")]
		ArabicUnitedArabEmirates = 14337,
		[Title("es-UY")]
		SpanishUruguay = 14346,
		[Title("ar-BH")]
		ArabicBahrain = 15361,
		[Title("es-PY")]
		SpanishParaguay = 15370,
		[Title("ar-QA")]
		ArabicQatar = 16385,
		[Title("es-BO")]
		SpanishBolivia = 16394,
		[Title("es-SV")]
		SpanishElSalvador = 17418,
		[Title("es-HN")]
		SpanishHonduras = 18442,
		[Title("es-NI")]
		SpanishNicaragua = 19466,
		[Title("es-PR")]
		SpanishPuertoRico = 20490,
		[Title("zh-CHT")]
		ChineseTraditional = 31748,
		[Title("sr")]
		Serbian = 31770,
		[Title("am-ET")]
		AmharicEthiopia = 1118,
		[Title("tzm-Latn-DZ")]
		TamazightLatinAlgeria = 2143,
		[Title("iu-Latn-CA")]
		InuktitutLatinCanada = 2141,
		[Title("sma-NO")]
		SamiSouthernNorway = 6203,
		[Title("mn-Mong-CN")]
		MongolianTraditionalMongolianPeoplesRepublicOfChina = 2128,
		[Title("gd-GB")]
		ScottishGaelicUnitedKingdom = 1169,
		[Title("en-MY")]
		EnglishMalaysia = 17417,
		[Title("prs-AF")]
		DariAfghanistan = 1164,
		[Title("bn-BD")]
		BengaliBangladesh = 2117,
		[Title("wo-SN")]
		WolofSenegal = 1160,
		[Title("rw-RW")]
		KinyarwandaRwanda = 1159,
		[Title("qut-GT")]
		KicheGuatemala = 1158,
		[Title("sah-RU")]
		YakutRussia = 1157,
		[Title("gsw-FR")]
		AlsatianFrance = 1156,
		[Title("co-FR")]
		CorsicanFrance = 1155,
		[Title("oc-FR")]
		OccitanFrance = 1154,
		[Title("mi-NZ")]
		MaoriNewZealand = 1153,
		[Title("ga-IE")]
		IrishIreland = 2108,
		[Title("se-SE")]
		SamiNorthernSweden = 2107,
		[Title("br-FR")]
		BretonFrance = 1150,
		[Title("smn-FI")]
		SamiInariFinland = 9275,
		[Title("moh-CA")]
		MohawkMohawk = 1148,
		[Title("arn-CL")]
		MapudungunChile = 1146,
		[Title("ii-CN")]
		YiPeoplesRepublicOfChina = 1144,
		[Title("dsb-DE")]
		LowerSorbianGermany = 2094,
		[Title("ig-NG")]
		IgboNigeria = 1136,
		[Title("kl-GL")]
		GreenlandicGreenland = 1135,
		[Title("lb-LU")]
		LuxembourgishLuxembourg = 1134,
		[Title("ba-RU")]
		BashkirRussia = 1133,
		[Title("nso-ZA")]
		SesothoSaLeboaSouthAfrica = 1132,
		[Title("quz-BO")]
		QuechuaBolivia = 1131,
		[Title("yo-NG")]
		YorubaNigeria = 1130,
		[Title("ha-Latn-NG")]
		HausaLatinNigeria = 1128,
		[Title("fil-PH")]
		FilipinoPhilippines = 1124,
		[Title("ps-AF")]
		PashtoAfghanistan = 1123,
		[Title("fy-NL")]
		FrisianNetherlands = 1122,
		[Title("ne-NP")]
		NepaliNepal = 1121,
		[Title("se-NO")]
		SamiNorthernNorway = 1083,
		[Title("iu-Cans-CA")]
		InuktitutSyllabicsCanada = 1117,
		[Title("sr-Latn-RS")]
		SerbianLatinSerbia = 9242,
		[Title("si-LK")]
		SinhalaSriLanka = 1115,
		[Title("sr-Cyrl-RS")]
		SerbianCyrillicSerbia = 10266,
		[Title("lo-LA")]
		LaoLaoPdr = 1108,
		[Title("km-KH")]
		KhmerCambodia = 1107,
		[Title("cy-GB")]
		WelshUnitedKingdom = 1106,
		[Title("bo-CN")]
		TibetanPeoplesRepublicOfChina = 1105,
		[Title("sms-FI")]
		SamiSkoltFinland = 8251,
		[Title("as-IN")]
		AssameseIndia = 1101,
		[Title("ml-IN")]
		MalayalamIndia = 1100,
		[Title("en-IN")]
		EnglishIndia = 16393,
		[Title("or-IN")]
		OriyaIndia = 1096,
		[Title("bn-IN")]
		BengaliIndia = 1093,
		[Title("tk-TM")]
		TurkmenTurkmenistan = 1090,
		[Title("bs-Latn-BA")]
		BosnianLatinBosniaandHerzegovina = 5146,
		[Title("mt-MT")]
		MalteseMalta = 1082,
		[Title("sr-Cyrl-ME")]
		SerbianCyrillicMontenegro = 12314,
		[Title("se-FI")]
		SamiNorthernFinland = 3131,
		[Title("zu-ZA")]
		IsiZuluSouthAfrica = 1077,
		[Title("xh-ZA")]
		IsiXhosaSouthAfrica = 1076,
		[Title("tn-ZA")]
		SetswanaSouthAfrica = 1074,
		[Title("hsb-DE")]
		UpperSorbianGermany = 1070,
		[Title("bs-Cyrl-BA")]
		BosnianCyrillicBosniaandHerzegovina = 8218,
		[Title("tg-Cyrl-TJ")]
		TajikCyrillicTajikistan = 1064,
		[Title("sr-Latn-BA")]
		SerbianLatinBosniaandHerzegovina = 6170,
		[Title("smj-NO")]
		SamiLuleNorway = 4155,
		[Title("rm-CH")]
		RomanshSwitzerland = 1047,
		[Title("smj-SE")]
		SamiLuleSweden = 5179,
		[Title("quz-EC")]
		QuechuaEcuador = 2155,
		[Title("quz-PE")]
		QuechuaPeru = 3179,
		[Title("hr-BA")]
		CroatianLatinBosniaandHerzegovina = 4122,
		[Title("sr-Latn-ME")]
		SerbianLatinMontenegro = 11290,
		[Title("sma-SE")]
		SamiSouthernSweden = 7227,
		[Title("en-SG")]
		EnglishSingapore = 18441,
		[Title("ug-CN")]
		UyghurPeoplesRepublicOfChina = 1152,
		[Title("sr-Cyrl-BA")]
		SerbianCyrillicBosniaandHerzegovina = 7194,
		[Title("es-US")]
		SpanishUnitedStates = 21514,
	}

}
