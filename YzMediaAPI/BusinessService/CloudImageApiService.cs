using Newtonsoft.Json;
using RestSharp;
using System.Data.SqlClient;
using YzMediaAPI.BusinessObject;

namespace YzMediaAPI.BusinessService
{
	public class CloudImageApiService
	{
		public class Result
		{
			public List<ResultItem> images { get; set; }
		}

		public class CloudflareGetResponse
		{
			public Result result { get; set; }
			public bool success { get; set; }
			public List<object> errors { get; set; }
			public List<object> messages { get; set; }
		}

		public class ResultItem
		{
			public string id { get; set; }
			public string filename { get; set; }
			public DateTime uploaded { get; set; }
			public bool requireSignedURLs { get; set; }
			public List<string> variants { get; set; }
			public string uploadURL { get; set; }
		}

		public class CloudflareResponse
		{
			public ResultItem result { get; set; }
			public bool success { get; set; }
			public List<object> errors { get; set; }
			public List<object> messages { get; set; }
		}

		public static async Task<int> UploadImage(SqlConnection connection, IFormFile file, string userI)
		{
			var client = new RestClient("https://api.cloudflare.com");
			var request = new RestRequest($"/client/v4/accounts/{AppSettings.Cloud_AccountId}/images/v1", Method.Post);
			request.AddHeader("Authorization", $"Bearer {AppSettings.Cloud_TokenUpload}");

			using (var ms = new MemoryStream())
			{
				file.CopyTo(ms);
				var fileBytes = ms.ToArray();
				request.AddFile("file", fileBytes, file.FileName, file.ContentType);
			}

			RestResponse response = await client.ExecuteAsync(request);

			var cloudflareResponse = JsonConvert.DeserializeObject<CloudflareResponse>(response.Content);

			if (cloudflareResponse.success)
			{
				ImageInfo imageInfo = new ImageInfo
				{
					ImageCloudID = cloudflareResponse.result.id,
					ImageName = file.FileName.Split('.')[0],
					ImagePath = cloudflareResponse.result.variants[0],
					UserI = userI
				};

				return ImageService.GetInstance().InsertImage(connection, imageInfo);
			}


			else return 0;
		}

		public static async Task<string> GetPresignedUrl()
		{
			var client = new RestClient("https://api.cloudflare.com");
			var request = new RestRequest($"/client/v4/accounts/{AppSettings.Cloud_AccountId}/images/v2/direct_upload", Method.Post);

			request.AddHeader("Authorization", $"Bearer {AppSettings.Cloud_TokenUpload}");

			var expiryTime = DateTime.UtcNow.AddMinutes(5).ToString("o");
			request.AddParameter("expiry", expiryTime);

			RestResponse response = await client.ExecuteAsync(request);
			var cloudflareResponse = JsonConvert.DeserializeObject<CloudflareResponse>(response.Content);

			if (cloudflareResponse.success)
			{
				return cloudflareResponse.result.uploadURL;
			}

			return null;

		}
	}
}
