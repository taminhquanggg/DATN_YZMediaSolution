using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace YzMediaAPI.Helper
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class FunctionAuthorizeFilterAttribute : AuthorizeAttribute, IAuthorizationFilter
	{
		public bool CheckRight { get; set; } = true;

		/// <summary>
		/// FunctionID trong bảng tbl_Function.
		/// </summary>
		public string FunctionID { get; set; } = string.Empty;
		/// <summary>
		/// Danh sách các FunctionID trong bảng tbl_Function.
		/// </summary>
		public string[] ListFunctionID { get; set; } = Array.Empty<string>();

		//
		public FunctionAuthorizeFilterAttribute(params string[] listFunctionID)
		{
			ListFunctionID = listFunctionID;
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

			// 
			if (CheckRight && !CheckRightByFuncCode(user.UserID, context))
			{
				context.Result = new ForbidResult();
				return;
			}
		}

		private bool CheckRightByFuncCode(int userID, AuthorizationFilterContext context)
		{
			var listUserFunction = LoginMemory.GetUserFunctionByUserId(userID);

			// 1. Ưu tiên check theo setup ở FuncCode trước.
			// 2. Nếu không có mới check ở FuncCodes.
			// 3. Cả 2 option check theo FuncCode không set thì check theo request path.
			if (!string.IsNullOrEmpty(FunctionID))
			{
				return listUserFunction?.Any(x => x.FunctionID?.Equals(FunctionID) == true) ?? false;
			}
			else if (ListFunctionID?.Length > 0)
			{
				return listUserFunction?.Any(x => !string.IsNullOrEmpty(x.FunctionID) && ListFunctionID.Contains(x.FunctionID)) ?? false;
			}
			else
			{
				//string reqPath = context.HttpContext.Request.Path.Value ?? string.Empty;

				//if (!string.IsNullOrEmpty(reqPath))
				//{
				//    return functionsHasRight?.Any(x => x.FuncUrl?.Equals(reqPath, StringComparison.OrdinalIgnoreCase) == true) ?? false;
				//}
			}

			return false;
		}
	}
}
