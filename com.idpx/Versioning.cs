using System;
using System.Reflection;

namespace com.idpx
{
	public static class Versioning
	{
		public static string GenericErrorMsg = "There was an error! Please look in the logs for more information. Thank you. {0} Timestamp: {1} Context: {2}";
		public static string VersionTemplate = "Version: {0}";

		public static string VersionedGenericErrorMsg(string stateId = null)
		{
			return string.Format(GenericErrorMsg, Version, DateTime.UtcNow, stateId ?? string.Empty);
		}

		private static string _version; 
		public static string Version
		{
			get
			{
				if (string.IsNullOrEmpty(_version))
				{
					_version = string.Format(VersionTemplate, Assembly.GetCallingAssembly().GetName().Version); //GetAssemblyVersion(Keys.AssemblyName));
				}

				return _version;
			}
		}

		//private static Version GetAssemblyVersion(string name)
		//{
		//	var assemblyName = GetReferencedAssemblyName(name);

		//	return (assemblyName != null) ? assemblyName.Version : null;
		//}

		//private static AssemblyName GetReferencedAssemblyName(string name)
		//{
		//	var referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
		//	foreach (var assembly in referencedAssemblies)
		//	{
		//		if (assembly.Name == name)
		//		{
		//			return assembly;
		//		}
		//	}

		//	return null;
		//}
	}
}
