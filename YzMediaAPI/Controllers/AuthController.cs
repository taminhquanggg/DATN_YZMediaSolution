using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.BusinessService;
using YzMedia.Library.Common.Helper;

namespace YzMediaAPI.Controllers
{
	public class AuthController : Controller
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[AllowAnonymous]
		[HttpPost]
		[Route("api/auth")]
		public async Task<IActionResult> Auth([FromBody] TokenRequest parameters)
		{
			try
			{
				if (parameters.Type == "password")
				{
					return await DoCheckPassword(parameters);
				}
				else if (parameters.Type == "refresh_token")
				{
					return await DoRefreshToken(parameters);
				}
				else if (parameters.Type == "invalidate_token")
				{
					return DoInvalidateToken(parameters);
				}
				else
				{
					return Ok(new { code = -1, message = "Invalid grant type." });
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
				return Ok(new { code = -1, message = ex.ToString() });
			}
		}

		[Authorize]
		[HttpGet]
		[FunctionAuthorizeFilter(CheckRight = false)]
		[Route("api/checkalive")]
		public IActionResult CheckAlive()
		{
			var userInfo = Request.GetUserInfo();
			if (userInfo != null && userInfo.UserID > 0) return Ok();
			else return Unauthorized();
		}

		[AllowAnonymous]
		[HttpPost("api/logout")]
		public IActionResult Logout(string refresh_token)
		{
			try
			{
				if (!string.IsNullOrEmpty(refresh_token))
				{
					LoginMemory.RemoveRefreshToken(refresh_token);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
				return BadRequest(new
				{
					code = -1,
					message = ex.Message.ToString()
				});
			}
			return Ok(new { code = 1, message = "_" });
		}

		private async Task<IActionResult> DoCheckPassword(TokenRequest parameters)
		{
			if (String.IsNullOrEmpty(parameters.UserName))
			{
				return Ok(new { code = -1, message = "User không được để trống" });
			}

			if (parameters.Password == null || parameters.Password == "")
			{
				return Ok(new { code = -1, message = "Password không được để trống" });
			}

			_logger.Debug($"begin login UserName: {parameters.UserName} | Password: {parameters.Password}");

			var user = new UserService.UserInfo();

			using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
			{
				user = UserService.GetInstance().GetUser(connection, parameters.UserName, parameters.Password);
			}

			if (user != null && !String.IsNullOrEmpty(user.UserName))
			{
				_logger.Debug($"done login UserName: {parameters.UserName} | password: {parameters.Password}");

				var now = DateTime.UtcNow;
				var refresh_token = new IdentityRefreshToken
				{
					Identity = user.UserID.ToString(),
					RefreshToken = Guid.NewGuid().ToString("N"),
					IssueTimeUtc = DateTime.UtcNow,
					ExpiryTimeUtc = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1),
				};

				var returnvalue = await GetJwtAsync(refresh_token.RefreshToken, user);

				//store the refresh_token
				refresh_token.AccessToken = returnvalue.Item2;
				LoginMemory.SetRefreshToken(refresh_token);

				//store the sauser
				LoginMemory.AddUserMapping(user);

				return Ok(returnvalue.Item1);
			}
			else
			{
				_logger.Debug("NULL login UserName " + parameters.UserName + " login password " + parameters.Password);

				return Ok(new { code = -1, message = "Tài khoản hoặc mật khẩu không đúng" });
			}
		}

		private async Task<IActionResult> DoRefreshToken(TokenRequest parameters)
		{
			var authorizations = Request.Headers.Authorization;
			string accessToken = string.Empty;
			if (authorizations.Count > 0)
			{
				accessToken = (authorizations[0] ?? string.Empty).Replace("Bearer ", "");
			}

			var token = LoginMemory.GetRefreshToken(parameters.Refresh);

			if (token == null)
			{
				return BadRequest(new
				{
					code = -1,
					message = "Refresh Token not found."
				});
			}

			if (token.IsExpired)
			{
				// Remove refresh token if expired
				LoginMemory.RemoveRefreshToken(token.RefreshToken);

				return BadRequest(new
				{
					code = -1,
					message = "Refresh Token has expired."
				});
			}

			if (!string.Equals(accessToken, token.AccessToken))
			{
				return BadRequest(new
				{
					Code = -1,
					Message = "Refresh Token and Access Token do not match."
				});
			}

			//
			var user = JWTHelper.GetUserInfo(accessToken);
			if (user == null)  //|| user.AutoId <= 0
			{
				return BadRequest(new
				{
					code = -1,
					message = "User not logged on."
				});
			}
			//
			LoginMemory.RemoveRefreshToken(token.RefreshToken);

			var now = DateTime.UtcNow;
			var refresh_token = new IdentityRefreshToken
			{
				Identity = user.UserID.ToString(),
				RefreshToken = Guid.NewGuid().ToString("N"),
				IssueTimeUtc = DateTime.UtcNow,
				ExpiryTimeUtc = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1),
			};

			var returnValue = await GetJwtAsync(refresh_token.RefreshToken, user);
			refresh_token.AccessToken = returnValue.Item2;

			LoginMemory.SetRefreshToken(refresh_token);

			return Ok(returnValue.Item1);
		}

		private IActionResult DoInvalidateToken(TokenRequest parameters)
		{
			var token = LoginMemory.GetRefreshToken(parameters.Refresh);

			if (token == null)
			{
				return Ok(new { code = -1, message = "Token in valid." });
			}

			if (token.IsExpired)
			{
				LoginMemory.RemoveRefreshToken(token.RefreshToken);
			}

			return Ok(new { code = 1, message = "Token valid." });
		}

		private async Task<Tuple<string, string>> GetJwtAsync(string Refresh_Token, UserService.UserInfo user)
		{
			var now = DateTime.UtcNow;

			var claims = new Claim[]
			{
				new Claim("UserID", user.UserID.ToString()),
				new Claim(ClaimTypes.GivenName, user.UserName, ClaimValueTypes.String),
				new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString(), ClaimValueTypes.String),
			};

			var expires = now.AddHours(AppSetting.TimeOutLogin);
			string jwt_key = AppSetting.JWT_Key;
			string jwt_issuer = AppSetting.JWT_Issuer;
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_key));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				jwt_issuer,
				jwt_issuer,
				claims,
				notBefore: now,
				expires: expires,
				signingCredentials: credentials);


			var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

			var response = new
			{
				UserID = user.UserID,
				AccessToken = accessToken,
				ExpiryTime = expires,
				Refresh_Token,
				UserName = user.UserName,
				FullName = user.FullName,
			};

			return new Tuple<string, string>(JsonConvert.SerializeObject(response), accessToken);
		}
	}
}
