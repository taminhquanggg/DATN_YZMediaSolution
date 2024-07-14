using System.Net.WebSockets;

namespace YzMedia.Library.Common.BusinessObject
{
	public interface IWebSocketProcess
	{
		Task Handle(Guid id, WebSocket websocket);
	}
}
