using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using System.Data.SqlClient;
using System.Net;
using System.Reflection;
using System.Text;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.BusinessService;
using static YzMedia.Library.Common.BusinessService.CloudImageApiService;

namespace YzMedia.Library.Common.Helper
{
	public class APICallingHelper : BaseService<APICallingHelper>
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public async Task<TResponse> GetSingleResultFromAPI<TRequest, TResponse>(TRequest requestObj, string sendOrderURL, string method = WebRequestMethods.Http.Post, string contentType = "application/json")
		{
			TResponse result = default(TResponse);
			var requestContent = JsonConvert.SerializeObject(requestObj, Formatting.Indented);

			_logger.Debug(string.Format("GetListResult: url:{0} - requestContent:{1}", sendOrderURL, requestContent));

			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(sendOrderURL);
			httpWebRequest.Method = method;
			httpWebRequest.Headers.Clear();
			httpWebRequest.Accept = "text/json";
			httpWebRequest.ContentType = contentType;

			if (method != WebRequestMethods.Http.Get)
			{
				using (Stream stream = httpWebRequest.GetRequestStream())
				{
					var bytes = Encoding.UTF8.GetBytes(requestContent);
					stream.Write(bytes, 0, bytes.Length);
				}
			}

			HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
			string responseContent = string.Empty;
			using (Stream stream = response.GetResponseStream())
			{
				using (StreamReader sr = new StreamReader(stream))
				{
					responseContent = sr.ReadToEnd();
				}
			}
			var messNotice = $"Kết quả gọi API: {sendOrderURL}. StatusCode: {response.StatusCode} - contentResponse:{responseContent}";
			_logger.Debug(messNotice);


			var data = JsonConvert.DeserializeObject<TResponse>(responseContent);
			_logger.Debug($"Data DeserializeObject: {JsonConvert.SerializeObject(data)}");

			if (data != null)
			{
				result = data;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception(responseContent);
				}
			}
			response.Close();
			return result;
		}

		public async Task<ListResponeMessage<ImageSearchResponse>> SearchImage(IFormFile file, string tableName)
		{
			var client = new RestClient("http://localhost:8000");
			var request = new RestRequest($"/face-api/search-face", Method.Post);

			using (var ms = new MemoryStream())
			{
				file.CopyTo(ms);
				var fileBytes = ms.ToArray();
				request.AddFile("image", fileBytes, file.FileName, file.ContentType);
			}

			request.AddParameter("tableName", tableName);

			RestResponse response = await client.ExecuteAsync(request);

			var searchImgResult = JsonConvert.DeserializeObject<ListResponeMessage<ImageSearchResponse>>(response.Content);

			return searchImgResult;
		}
	}

}
