using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace com.idpx
{
	public static class Web
	{
		public static string Referrer = "Referrer";
		public static string ReferrerUrl = "Url";

		public static string ReturnUrl = "/callback";
		public static string RootUrl = "";

		public static string GetReferrerUrl(HttpRequest request, string defaultValue = default)
		{
            return request.Query[ReferrerUrl].ToString() ?? request.Headers[Referrer].ToString() ?? defaultValue;
        }

		public static string GetReturnUrl(Uri requestUrl)
		{
			return DeriveUrl(requestUrl, ReturnUrl);
		}

		public static string GetRootUrl(Uri requestUrl)
		{
			return DeriveUrl(requestUrl, RootUrl);
		}

		public static string ConfirmUrl(string url, string defaultUrl = null, string errorMessageOnNullOrEmpty = null, params object[] parameters)
		{
			if (string.IsNullOrEmpty(url))
			{
				url = defaultUrl;

				if (string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(errorMessageOnNullOrEmpty))
				{
					throw new ApplicationException(errorMessageOnNullOrEmpty ?? string.Format("Null or empty url: {0}", string.Join(" | ", parameters)));
				}
			}

			return url;
		}

		public static string DeriveUrl(Uri requestUrl, string path)
		{
			return new Uri(requestUrl, path).ToString();
		}

		public static string GetAuthority(string url)
		{
			try
			{
				return new Uri(HttpUtility.UrlDecode(url)).GetLeftPart(UriPartial.Authority).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		public static string GetSamlUrl(string src, string binding, bool ignoreBindingQueryString)
		{
			if (!ignoreBindingQueryString)
			{
				return AppendToUrl(src, new Dictionary<string, string>() { { "binding", HttpUtility.UrlEncode(binding) } });
			}

			return src;
		}

		public static string AppendToUrl(string src, Dictionary<string, string> details)
		{
			var url = new UriBuilder(src);

			url.Query = AppendToQuery(url.Query, details);

			return url.ToString();
		}

		public static string AppendToQuery(string querystring, Dictionary<string, string> details)
		{
			var query = HttpUtility.ParseQueryString(querystring);

			foreach (var detail in details)
			{
				query[detail.Key] = detail.Value; //HttpUtility.UrlEncode(detail.Value);     // http://stackoverflow.com/questions/14517798/append-values-to-query-string
			}

			return query.ToString();
		}

		public static void RedirectTo(this HttpResponse response, string location)
		{
			response.StatusCode = (int)HttpStatusCode.Redirect;
			response.Headers.Add("Location", location);
		}
	}
}
