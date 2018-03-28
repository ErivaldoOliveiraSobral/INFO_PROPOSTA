using System;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Globalization;
using System.Net;
using System.Security.Principal;

namespace Iteris.Utilities {

	public delegate void ExceptionHandler(Exception ex);

	public static class ADUtility {

		private enum SAMAccountType {
			SAM_DOMAIN_OBJECT = 0x0,
			SAM_GROUP_OBJECT = 0x10000000,
			SAM_NON_SECURITY_GROUP_OBJECT = 0x10000001,
			SAM_ALIAS_OBJECT = 0x20000000,
			SAM_NON_SECURITY_ALIAS_OBJECT = 0x20000001,
			SAM_USER_OBJECT = 0x30000000,
			SAM_NORMAL_USER_ACCOUNT = 0x30000000,
			SAM_MACHINE_ACCOUNT = 0x30000001,
			SAM_TRUST_ACCOUNT = 0x30000002,
			SAM_APP_BASIC_GROUP = 0x40000000,
			SAM_APP_QUERY_GROUP = 0x40000001,
			SAM_ACCOUNT_TYPE_MAX = 0x7fffffff
		}

		public static Collection<String> GetAuthorizationGroups(String logOnName, ExceptionHandler exceptionHandler) {
			Collection<String> groups = null;
			String[] logOnNameParts = null;

			if (logOnName.IsNullOrWhiteSpace() || logOnName.IndexOf('\\') < 0)
				return new Collection<String>();

			logOnNameParts = logOnName.Split('\\');
			String domain = logOnNameParts[0];
			String sAMAccountName = logOnNameParts[1];

			groups = new Collection<String>();

			// Get all the user's base AD groups using the App Pool account to query the Active Directory server.
			using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://" + domain)) {
				using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry, "(sAMAccountName=" + sAMAccountName + ")")) {
					directorySearcher.SearchScope = SearchScope.Subtree;
					SearchResult result = directorySearcher.FindOne();

					if (result == null)
						return new Collection<String>();

					using (DirectoryEntry user = result.GetDirectoryEntry()) {
						if (user == null)
							return new Collection<String>();

						user.RefreshCache(new String[] { "tokenGroups" });

						foreach (Object tokenGroup in user.Properties["tokenGroups"]) {
							SecurityIdentifier sid = new SecurityIdentifier((Byte[])tokenGroup, 0);
							// Check whether this is a valid domain group.
							using (DirectorySearcher groupDirectorySearcher = new DirectorySearcher(directoryEntry, "(&(objectSID=" + sid.Value + ")(samaccounttype=" + ((Int32)SAMAccountType.SAM_GROUP_OBJECT).ToString(CultureInfo.InvariantCulture) + "))")) {
								groupDirectorySearcher.SearchScope = SearchScope.Subtree;
								SearchResult groupResult = groupDirectorySearcher.FindOne();
								if (groupResult == null)
									continue;
							}
							NTAccount account = null;

							try {
								account = (NTAccount)sid.Translate(typeof(NTAccount));
							}
							catch (Exception ex) {
								// No collateral effect.
								if (exceptionHandler != null)
									exceptionHandler(new Exception("Error translating SID " + sid.Value + " into a NT Account.", ex));
								continue;
							}
							groups.Add(account.Value);
						}
					}
				}
			}
			return groups;
		}

		public static Collection<String> GetAuthorizationGroups(String logOnName) {
			return GetAuthorizationGroups(logOnName, (ExceptionHandler)null);
		}

		public static Collection<String> GetAuthorizationGroups(String sAMAccountName, NetworkCredential[] domains) {
			Collection<String> groups = null;

			if (String.IsNullOrEmpty(sAMAccountName))
				return groups;

			groups = new Collection<String>();

			// Get all the user's base AD groups.
			foreach (NetworkCredential domain in domains) {
				using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://" + domain.Domain, domain.UserName, domain.Password)) {
					using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry, "(sAMAccountName=" + sAMAccountName + ")")) {
						directorySearcher.SearchScope = SearchScope.Subtree;
						SearchResult result = directorySearcher.FindOne();

						if (result == null)
							return new Collection<String>();

						using (DirectoryEntry user = result.GetDirectoryEntry()) {
							if (user == null)
								continue;

							user.RefreshCache(new String[] { "tokenGroups" });

							foreach (Object tokenGroup in user.Properties["tokenGroups"]) {
								SecurityIdentifier sid = new SecurityIdentifier((Byte[])tokenGroup, 0);
								NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));

								if (!account.Value.StartsWith(domain.Domain, StringComparison.OrdinalIgnoreCase))
									continue;

								groups.Add(account.Value);
							}
						}
					}
				}
			}
			return groups;
		}

		public static String GetDomainName(String userLogOnName) {
			if (userLogOnName.IndexOf('\\') == -1)
				return null;
			return userLogOnName.Split('\\')[0];
		}
	}

}
