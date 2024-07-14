using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.BusinessService;
using static YzMedia.Library.Common.BusinessService.CloudImageApiService;

namespace YzMediaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ImageController : Controller
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[HttpPost("get-by-posts-id")]
		public IActionResult GetByPostID([FromBody] ImageService.ImageFilterInfo infoFilter)
		{
			try
			{
				if (infoFilter != null)
				{
					using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
					{
						List<ImageService.ImageInfo> listImageInfo = ImageService.GetInstance().GetImageByPostsID(connection, infoFilter);

						List<ImageService.ImageInfo> listResult = new List<ImageService.ImageInfo>();

						if (infoFilter.Offset.HasValue && infoFilter.PageSize.HasValue)
						{
							listResult = listImageInfo.Skip((int)infoFilter.Offset - 1).Take((int)infoFilter.PageSize).ToList();

							return Ok(new
							{
								jsondata = JsonConvert.SerializeObject(listResult),
								totalrows = listImageInfo.Count
							});
						}

						return Ok(new
						{
							jsondata = JsonConvert.SerializeObject(new List<PostService.PostInfo>()),
							totalrows = 0
						});
					}
				}
				else
				{
					return Ok(new
					{
						jsondata = JsonConvert.SerializeObject(new List<PostService.PostInfo>()),
						totalrows = 0
					});
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());
				return Json(new
				{
					jsondata = JsonConvert.SerializeObject(new List<PostService.PostInfo>()),
					err = ex.Message.ToString()
				});
			}
		}

		[HttpPost("get-presigned-url")]
		public async Task<IActionResult> GetPresignedUrl()
		{
			try
			{
				var url = await CloudImageApiService.GetInstance().GetPresignedUrl();

				return Ok(new
				{
					jsondata = url
				});
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());
				return Json(new
				{
					jsondata = String.Empty,
					err = ex.Message.ToString()
				});
			}
		}

		[HttpPost]
		public async Task<IActionResult> Insert(ImageService.ImageInfo infoInsert)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					var result = ImageService.GetInstance().InsertImage(connection, infoInsert);

					return Ok(new
					{
						code = result > 0,
						message = result > 0 ? "Tải ảnh thành công !" : "Tải ảnh không thành công !",
						jsondata = result
					});
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());
				return Ok(new
				{
					code = false,
					message = $"Tải ảnh không thành công. Lỗi: {ex.Message.ToString()}"
				});
			}
		}
	}
}
