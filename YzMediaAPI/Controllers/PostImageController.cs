using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.BusinessService;

namespace YzMediaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PostImageController : Controller
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[HttpGet]
		public IActionResult GetListPostImage(string strSearch = null)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					List<PostImageService.PostImageInfo> listCompanyInfo = PostImageService.GetInstance().GetListPostImage(connection, strSearch);
					return Json(new
					{
						jsondata = JsonConvert.SerializeObject(listCompanyInfo)
					});
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());

				return Json(new
				{
					jsondata = JsonConvert.SerializeObject(new List<PostImageService.PostImageInfo>()),
					err = ex.Message.ToString()
				});
			}
		}

		[HttpPost]
		public IActionResult Insert([FromBody] PostImageService.PostImageInfo infoInsert)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					bool result = PostImageService.GetInstance().InsertPostImage(connection, infoInsert);

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

		//[HttpPost]
		//[Authorize]
		//[Route("post-image/update")]
		//public IActionResult UpdatePostImage([FromBody] PostImageService.PostImageInfo infoUpdate)
		//{
		//	try
		//	{
		//		using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
		//		{
		//			bool result = PostImageService.GetInstance().UpdatePostImage(connection, infoUpdate);

		//			return Json(new
		//			{
		//				code = result
		//			});
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.Error(ex.ToString());
		//		return Json(new
		//		{
		//			code = false,
		//			err = ex.Message.ToString()
		//		});
		//	}
		//}

		//[HttpPost]
		//[Authorize]
		//[Route("post-image/delete")]
		//public IActionResult DeletePostImage([FromBody] int PostImageID)
		//{
		//	try
		//	{
		//		using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
		//		{
		//			bool result = PostImageService.GetInstance().DeletePostImage(connection, PostImageID);

		//			return Json(new
		//			{
		//				code = result
		//			});
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.Error(ex.ToString());
		//		return Json(new
		//		{
		//			code = false,
		//			err = ex.Message.ToString()
		//		});
		//	}
		//}

	}
}
