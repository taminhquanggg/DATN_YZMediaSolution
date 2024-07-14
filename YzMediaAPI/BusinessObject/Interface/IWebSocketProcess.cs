using System.Net.WebSockets;

namespace YzMediaAPI.BusinessObject
{
	public interface IWebSocketProcess
	{
		Task Handle(Guid id, WebSocket websocket);
	}
}
