using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.BusinessService;
using YzMedia.Library.Common.Helper;

namespace YzMediaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PostsController : Controller
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[HttpGet("{postID}")]
		public async Task<IActionResult> GetPostByID(int postID)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					PostService.PostInfo postInfo = PostService.GetInstance().GetByID(connection, postID);

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

		[HttpPost("search")]
		public async Task<IActionResult> SearchPost([FromBody] PostService.PostFilterInfo infoFilter)
		{
			try
			{
				if (infoFilter != null)
				{
					using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
					{
						List<PostService.PostInfo> listPostInfo = PostService.GetInstance().SearchPosts(connection, infoFilter);

						List<PostService.PostInfo> listResult = new List<PostService.PostInfo>();

						if (infoFilter.Offset.HasValue && infoFilter.PageSize.HasValue)
						{
							listResult = listPostInfo.Skip((int)infoFilter.Offset - 1).Take((int)infoFilter.PageSize).ToList();
						}

						return Ok(new
						{
							jsondata = JsonConvert.SerializeObject(listResult),
							totalrows = listPostInfo.Count
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
					message = ex.Message.ToString()
				});
			}
		}

		[HttpPost]
		public async Task<IActionResult> InsertPost([FromForm] PostService.PostInfo infoInsert)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					bool result = await PostService.GetInstance().InsertPostHandle(connection, infoInsert);

					return Ok(new
					{
						code = result,
						message = result ? "Thêm sự kiện thành công !" : "Thêm sự kiện không thành công !"
					});
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());
				return Ok(new
				{
					code = false,
					message = $"Thêm sự kiện không thành công. Lỗi: {ex.Message.ToString()}"
				});
			}
		}

		[HttpPut]
		public IActionResult UpdatePost([FromForm] PostService.PostInfo infoUpdate)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					bool result = PostService.GetInstance().UpdatePost(connection, infoUpdate);

					return Json(new
					{
						code = result
					});
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());
				return Json(new
				{
					code = false,
					message = ex.Message.ToString()
				});
			}
		}

		[HttpDelete("{postID}")]
		public async Task<IActionResult> DeletePost(int postID)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					bool result = PostService.GetInstance().DeletePost(connection, postID);

					if (result)
					{
						await APICallingHelper
						.GetInstance()
						.GetSingleResultFromAPI<string, SingleResponeMessage<string>>(null, $"http://localhost:8000/table/face{postID}", "DELETE");
					}

					return Json(new
					{
						code = result
					});
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());
				return Json(new
				{
					code = false,
					message = ex.Message.ToString()
				});
			}
		}
	}
}
