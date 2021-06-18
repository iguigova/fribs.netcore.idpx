using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace com.idpx.core
{
	public interface IResponseHandler
    {
        Task ProcessAsync(HttpResponse response);
    }
}
