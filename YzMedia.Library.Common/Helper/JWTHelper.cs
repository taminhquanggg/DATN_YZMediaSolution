using System.IdentityModel.Tokens.Jwt;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.BusinessService;

namespace YzMedia.Library.Common.Helper
{
	public static class JWTHelper
	{
		public static UserService.UserInfo? GetUserInfo(string token)
		{
			if (string.IsNullOrEmpty(token))
			{
				return null;
			}

			var audienceConfig = AppSetting.JWT_Issuer;
			var tokenHandler = new JwtSecurityTokenHandler();

			if (tokenHandler.CanReadToken(token))
			{
				var tokenRead = tokenHandler.ReadJwtToken(token);

				var userPid = tokenRead.Claims.FirstOrDefault(x => x.Type == "UserID")?.Value;

				Int32.TryParse(userPid ?? string.Empty, out var userPidValue);

				return LoginMemory.GetUserMapping(userPidValue);
			}

			return null;
		}

	}
}
