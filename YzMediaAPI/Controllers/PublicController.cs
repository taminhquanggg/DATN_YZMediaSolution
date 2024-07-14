using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.BusinessService;
using YzMedia.Library.Common.Helper;
using static YzMedia.Library.Common.BusinessService.PostImageService;

namespace YzMedia.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class PublicController : Controller
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[HttpGet("get-all-posts")]
		public async Task<IActionResult> GetAllPosts()
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					List<PostService.PostInfo> listResult = PostService.GetInstance().GetAllPost(connection);

					return Ok(new
					{
						jsondata = JsonConvert.SerializeObject(listResult),
						totalrows = listResult.Count
					});
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());
				return Json(new
				{
					jsondata = JsonConvert.SerializeObject(new List<PostService.PostInfo>()),
					message = ex.Message.ToString()
				});
			}
		}

		[HttpGet("get-post-info/{postId}")]
		public async Task<IActionResult> GetPostInfo(int postId)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					PostService.PostInfo postInfo = PostService.GetInstance().GetByID(connection, postId);

					return Ok(new
					{
						jsondata = JsonConvert.SerializeObject(postInfo)
					});
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());

				return Ok(new
				{
					jsondata = JsonConvert.SerializeObject(new List<PostService.PostInfo>()),
					message = ex.Message.ToString()
				});
			}
		}

		[HttpPost("get-image-by-posts-id")]
		public async Task<IActionResult> GetImageByPostID([FromForm] ImageService.ImageFilterInfo infoFilter)
		{
			try
			{
				if (infoFilter != null)
				{
					using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
					{
						if (infoFilter.SearchType == 1)
						{

							List<ImageService.ImageInfo> listImageInfo = ImageService.GetInstance().GetImageByPostsID(connection, infoFilter);

							List<ImageService.ImageInfo> listResult = new List<ImageService.ImageInfo>();

							if (infoFilter.Offset.HasValue && infoFilter.PageSize.HasValue)
							{
								listResult = listImageInfo.Skip((int)infoFilter.Offset - 1).Take((int)infoFilter.PageSize).ToList();
							}

							return Ok(new
							{
								jsondata = JsonConvert.SerializeObject(listResult),
								totalrows = listImageInfo.Count
							});

						}
						else
						{
							var resImgSearch = await APICallingHelper.GetInstance().SearchImage(infoFilter.Image, "face" + infoFilter.PostID.ToString());

							if (resImgSearch.IsSuccess && resImgSearch.TotalRecords > 0)
							{
								List<ImageService.ImageInfo> listImgSearch = new List<ImageService.ImageInfo>();

								foreach (var item in resImgSearch.Data)
								{
									var imageInfo = ImageService.GetInstance().GetImageByImageCloudID(connection, item.ImageID);

									imageInfo.ImageDetectID = item.FaceDetectID;
									imageInfo.ImageDetectPath = item.FaceDetectPath;
									imageInfo.ImageSearchDetect = item.ImageSearchDetect;
									imageInfo.Similarity = (float)Math.Round((decimal)item.Similarity, 5);

									listImgSearch.Add(imageInfo);
								}

								var sortedListImgSearch = listImgSearch.OrderByDescending(img => img.Similarity).ToList();

								List<ImageService.ImageInfo> listResult = new List<ImageService.ImageInfo>();

								if (infoFilter.Offset.HasValue && infoFilter.PageSize.HasValue)
								{
									listResult = sortedListImgSearch.Skip((int)infoFilter.Offset - 1).Take((int)infoFilter.PageSize).ToList();
								}

								return Ok(new
								{
									jsondata = JsonConvert.SerializeObject(listResult),
									totalrows = sortedListImgSearch.Count
								});
							}
						}
					}

					return Ok(new
					{
						jsondata = JsonConvert.SerializeObject(new List<ImageService.ImageInfo>()),
						totalrows = 0
					});
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
	}
}
