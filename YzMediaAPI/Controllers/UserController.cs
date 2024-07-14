using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.BusinessService;

namespace YzMediaAPI.Controllers
{
	public class UserController : Controller
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[HttpGet]
		[Authorize]
		[Route("user/get-all")]
		public IActionResult GetListUser(string strSearch = null)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					List<UserService.UserInfo> listCompanyInfo = UserService.GetInstance().GetListUser(connection, strSearch);

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
					jsondata = JsonConvert.SerializeObject(new List<UserService.UserInfo>()),
					err = ex.Message.ToString()
				});
			}
		}

		[HttpPost]
		[Route("user/insert")]
		[Authorize]
		public IActionResult InsertUser([FromBody] UserService.UserInfo infoInsert)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					bool result = UserService.GetInstance().InsertUser(connection, infoInsert);

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
					err = ex.Message.ToString()
				});
			}
		}

		[HttpPost]
		[Authorize]
		[Route("user/update")]
		public IActionResult UpdateUser([FromBody] UserService.UserInfo infoUpdate)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					bool result = UserService.GetInstance().UpdateUser(connection, infoUpdate);

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
					err = ex.Message.ToString()
				});
			}
		}

		[HttpPost]
		[Authorize]
		[Route("user/delete")]
		public IActionResult DeleteUser([FromBody] string userName)
		{
			try
			{
				using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
				{
					bool result = UserService.GetInstance().DeleteUser(connection, userName);

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
					err = ex.Message.ToString()
				});
			}
		}

	}
}
