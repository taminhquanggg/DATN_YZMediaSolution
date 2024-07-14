using System.IdentityModel.Tokens.Jwt;
using YzMediaAPI.BusinessObject;
using YzMediaAPI.BusinessService;

namespace YzMediaAPI.Helper
{
	public static class JWTHelper
	{
		public static UserInfo? GetUserInfo(string token)
		{
			if (string.IsNullOrEmpty(token))
			{
				return null;
			}

			var audienceConfig = AppSettings.JWT_Issuer;
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
