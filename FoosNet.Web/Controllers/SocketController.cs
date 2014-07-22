using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FoosNet.Web.Handlers;
using Microsoft.Web.WebSockets;

namespace FoosNet.Web.Controllers
{
    public class SocketController : ApiController
    {
        public HttpResponseMessage Get()
        {
            if (HttpContext.Current.IsWebSocketRequest || HttpContext.Current.IsWebSocketRequestUpgrading)
            {
                HttpContext.Current.AcceptWebSocketRequest(new SocketHandler());
                return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }
    }
}
