using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace com.idpx.core
{
	public interface IRequestHandler
	{
		Task RedirectAsync(HttpResponse response);
	}
}
