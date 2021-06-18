using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace com.idpx
{
	public static class Security
	{
		public const string BeginCertPadding = "-----BEGIN CERTIFICATE-----";
		public const string EndCertPadding = "-----END CERTIFICATE-----";

		public static string Decrypt(string text)
		{
			return Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(text), new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, DataProtectionScope.LocalMachine));
		}

		public static string Encrypt(string text)
		{
			return Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(text), new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, DataProtectionScope.LocalMachine), Base64FormattingOptions.None);
		}

		public static string EncodeToBase64(string text)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
		}

		public static string DecodeFromBase64(string text)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(text));
		}

        public static string Serialize(object value)
        {
            return HttpUtility.UrlEncode(Encrypt(JsonConvert.SerializeObject(value)));
        }

        public static dynamic Deserialize(string value)
        {
            return JsonConvert.DeserializeObject<dynamic>(Decrypt(value));
        }

        public static X509Certificate2 ParseCertificate(this string data)
		{
			if (string.IsNullOrWhiteSpace(data))
			{
				return null;
			}

			try
			{
				// https://stackoverflow.com/questions/29337930/can-i-determine-whether-the-string-can-deserialize-by-newtonsoft
				var cert = JsonConvert.DeserializeAnonymousType(data.TrimCert(), new { base64 = (string)null, filename = (string)null, password = (string)null },
					new JsonSerializerSettings()
					{
						Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
						{
							//errors.Add(args.ErrorContext.Error.Message);
							args.ErrorContext.Handled = true;
						},
					});

				return new X509Certificate2(Convert.FromBase64String(cert?.base64 ?? data), cert?.password, X509KeyStorageFlags.MachineKeySet);
			}
			catch
			{
				return null;
			}
		}

		private static string TrimCert(this string cert)
		{
			if (string.IsNullOrWhiteSpace(cert))
			{
				return cert;
			}

			if (cert.StartsWith(BeginCertPadding) && cert.EndsWith(EndCertPadding))
			{
				cert = cert.Remove(0, BeginCertPadding.Length);
				cert = cert.Remove(cert.Length - EndCertPadding.Length, EndCertPadding.Length);
			}

			return cert;
		}
	}
}
