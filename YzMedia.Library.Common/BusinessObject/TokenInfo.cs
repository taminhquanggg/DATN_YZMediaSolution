using YzMedia.Library.Common.BusinessService;

namespace YzMedia.Library.Common.BusinessObject
{
	public class TokenRequest : UserService.UserInfo
	{
		public string Type { get; set; } = string.Empty;
		public string Refresh { get; set; } = string.Empty;
	}

	public class IdentityRefreshToken
	{
		public string Identity { get; set; }
		public string RefreshToken { get; set; }
		public string AccessToken { get; set; }
		public DateTime IssueTimeUtc { get; set; }
		public DateTime ExpiryTimeUtc { get; set; }
		public bool IsExpired { get { return DateTime.UtcNow >= ExpiryTimeUtc; } }
	}
}
