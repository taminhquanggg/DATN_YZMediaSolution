using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using YzMedia.Library.Common.BusinessObject;

namespace YzMediaAPI.Controllers
{
	[Route("api/websocket")]
	public class WebSocketController : Controller
	{
		public IWebSocketProcess WebsocketHandler { get; }

		public WebSocketController(IWebSocketProcess websocketHandler)
		{
			WebsocketHandler = websocketHandler;
		}

		[HttpGet]
		public async Task Get(string username)
		{
			var context = ControllerContext.HttpContext;

			try
			{
				if (username != "hello")
				{
					return;
				}

				var isSocketRequest = context.WebSockets.IsWebSocketRequest;

				if (isSocketRequest)
				{
					WebSocket websocket = await context.WebSockets.AcceptWebSocketAsync();

					await WebsocketHandler.Handle(Guid.NewGuid(), websocket);
				}
				else
				{
					context.Response.StatusCode = 400;
				}
			}
			catch
			{
				context.Response.StatusCode = 400;
			}
		}
	}
}
