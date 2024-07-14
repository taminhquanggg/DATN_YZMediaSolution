using System.Security.Claims;
using YzMediaAPI.BusinessObject;
using YzMediaAPI.BusinessService;

namespace YzMediaAPI.Helper
{
	public static class Request
	{
		public static UserInfo GetUserInfo(this HttpRequest httpRequest)
		{
			if (httpRequest.HttpContext.User.Identity == null || !httpRequest.HttpContext.User.Identity.IsAuthenticated)
				return new();
			var claims = ((ClaimsIdentity)httpRequest.HttpContext.User.Identity).Claims;
			var userIdClaim = claims.FirstOrDefault(x => x.Type == "UserID")?.Value;

			Int32.TryParse(userIdClaim ?? string.Empty, out var userPidValue);
			var user = LoginMemory.GetUserByUserID(userPidValue);

			if (user == null || String.IsNullOrEmpty(user.UserName))
			{
				return null;
			}

			return user;
		}
	}
}
