using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using YzMedia.Library.Common.BusinessObject;

namespace YzMedia.Library.Common.BusinessService
{
	public class WebSocketProcess : IWebSocketProcess
	{
		public class WebSocketConnection
		{
			public Guid id { get; set; }
			public WebSocket websocket { get; set; }
		}
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static List<WebSocketConnection> listWebsocketConnection = new List<WebSocketConnection>();

		public async Task Handle(Guid id, WebSocket websocket)
		{
			lock (listWebsocketConnection)
			{
				listWebsocketConnection.Add(new WebSocketConnection
				{
					id = id,
					websocket = websocket
				});
			}

			while (websocket.State == WebSocketState.Open)
			{
				var message = await ReceiveMessage(id, websocket);
				if (message != null)
					await SendMessageToSockets(message);
			}
		}

		private async Task<string> ReceiveMessage(Guid id, WebSocket websocket)
		{
			var arraySegment = new ArraySegment<byte>(new byte[4096]);
			var receivedMessage = await websocket.ReceiveAsync(arraySegment, CancellationToken.None);
			if (receivedMessage.MessageType == WebSocketMessageType.Text)
			{
				var message = Encoding.Default.GetString(arraySegment).TrimEnd('\0');
				if (!string.IsNullOrWhiteSpace(message))
					return $"<b>{id}</b>: {message}";
			}

			return null;
		}

		private async Task SendMessageToSockets(string message)
		{
			IEnumerable<WebSocketConnection> toSentTo;

			lock (listWebsocketConnection)
			{
				toSentTo = listWebsocketConnection.ToList();
			}

			var tasks = toSentTo.Select(async websocketConnection =>
			{
				if (websocketConnection.websocket.State == WebSocketState.Open)
				{
					var bytes = Encoding.Default.GetBytes(message);
					var arraySegment = new ArraySegment<byte>(bytes);
					await websocketConnection.websocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
				}
			});
			await Task.WhenAll(tasks);
		}

		private void CleanUpTask()
		{
			Task.Run(async () =>
			{
				while (true)
				{
					IEnumerable<WebSocketConnection> listSocketOpen;
					IEnumerable<WebSocketConnection> listSocketClosed;

					lock (listWebsocketConnection)
					{
						listSocketOpen = listWebsocketConnection.Where(
							x => x.websocket.State == WebSocketState.Open ||
							x.websocket.State == WebSocketState.Connecting);
						listSocketClosed = listWebsocketConnection.Where(
							x => x.websocket.State != WebSocketState.Open &&
							x.websocket.State != WebSocketState.Connecting);

						listWebsocketConnection = listSocketOpen.ToList();
					}

					await Task.Delay(5000);
				}

			});
		}

		private static void PushNoti()
		{
			while (true)
			{
				try
				{
					string message = "connected socket | time: " + DateTime.Now.ToString();
					BroardCastMessage(message);
					Thread.Sleep(3000);
				}
				catch (Exception ex)
				{
					_logger.Error(ex.ToString());
				}
			}
		}

		public static void BroardCastMessage(string msg)
		{
			try
			{
				IEnumerable<WebSocketConnection> listSendTo;

				lock (listWebsocketConnection)
				{
					listSendTo = listWebsocketConnection.ToList();
				}

				foreach (var item in listSendTo)
				{
					var bytes = Encoding.Default.GetBytes(msg);
					var arraySegment = new ArraySegment<byte>(bytes);
					item.websocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());
			}
		}
	}

}
