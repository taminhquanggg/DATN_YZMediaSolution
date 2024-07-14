using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace YzMedia.Library.Common.Helper
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class FunctionAuthorizeFilterAttribute : AuthorizeAttribute, IAuthorizationFilter
	{
		public bool CheckRight { get; set; } = true;

		//
		public FunctionAuthorizeFilterAttribute()
		{

		}

		//
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var user = context.HttpContext.Request.GetUserInfo();
			if (user == null || user.UserID <= 0)
			{
				context.Result = new UnauthorizedResult();
				return;
			}
		}
	}

}
