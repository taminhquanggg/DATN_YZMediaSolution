using YzMediaAPI.BusinessObject;

namespace YzMediaAPI.BusinessService
{
	public class LoginMemory
	{
		private static readonly object memoryLock = new();
		/// <summary>
		/// Key: UserID; Value: List User Function
		/// </summary>
		private static readonly Dictionary<int, List<UserFunctionInfo>> userFunctionMapping = new Dictionary<int, List<UserFunctionInfo>>();
		/// <summary>
		/// Key: UserID; Value: UserInfo
		/// </summary>
		private static readonly Dictionary<int, UserInfo> userMapping = new Dictionary<int, UserInfo>();
		/// <summary>
		/// Key: RefeshToken; Value: Info of RefeshToken
		/// </summary>
		private static readonly Dictionary<string, IdentityRefreshToken> refreshTokenMapping = new Dictionary<string, IdentityRefreshToken>();

		#region Hanle Refresh Token

		public static void SetRefreshToken(IdentityRefreshToken refreshToken)
		{
			lock (memoryLock)
			{
				var key = refreshToken.RefreshToken;
				if (refreshTokenMapping.ContainsKey(key))
				{
					refreshTokenMapping.Remove(key);
				}
				
				refreshTokenMapping.Add(key, refreshToken);
			}
		}

		public static void RemoveRefreshToken(string refreshToken)
		{
			lock (memoryLock)
			{
				if (refreshTokenMapping.ContainsKey(refreshToken))
				{
					refreshTokenMapping.Remove(refreshToken);
				}
			}
		}

		public static IdentityRefreshToken? GetRefreshToken(string refreshToken)
		{
			lock (memoryLock)
			{
				if (refreshTokenMapping.ContainsKey(refreshToken))
				{
					return refreshTokenMapping[refreshToken];
				}
			}

			return null;
		}

		#endregion

		#region Hanle User

		public static void AddUserMapping(UserInfo user)
		{
			lock (memoryLock)
			{
				var key = user.UserID;
				if (userMapping.ContainsKey(key))
				{
					userMapping.Remove(key);
				}
				userMapping.Add(key, user);
			}
		}

		public static void RemoveUserMapping(int userID)
		{
			lock (memoryLock)
			{
				if (userMapping.ContainsKey(userID))
				{
					userMapping.Remove(userID);
				}
			}
		}

		public static UserInfo? GetUserMapping(int userID)
		{
			lock (memoryLock)
			{
				if (userMapping.ContainsKey(userID))
				{
					return userMapping[userID];
				}
			}

			return null;
		}

		public static UserInfo? GetUserByUserID(int userID)
		{
			lock (memoryLock)
			{
				if (userMapping.Count > 0)
				{
					foreach (var item in userMapping.Values)
					{
						if (item == null)
						{
							continue;
						}

						if (string.Equals(item.UserID, userID))
						{
							return item;
						}
					}
				}
			}

			return null;
		}

		#endregion

		#region Hanle Functions Of User

		public static void AddUserFunctionMapping(int userID, List<UserFunctionInfo> listUserFunctionInfo)
		{
			lock (memoryLock)
			{
				if (userFunctionMapping.ContainsKey(userID))
				{
					userFunctionMapping.Remove(userID);
				}
				userFunctionMapping.Add(userID, listUserFunctionInfo);
			}
		}

		public static void RemoveUserFunctionMapping(int userID)
		{
			lock (memoryLock)
			{
				if (userFunctionMapping.ContainsKey(userID))
				{
					userFunctionMapping.Remove(userID);
				}
			}
		}

		public static List<UserFunctionInfo>? GetUserFunctionByUserId(int userID)
		{
			lock (memoryLock)
			{
				if (userFunctionMapping.ContainsKey(userID))
				{
					return userFunctionMapping[userID];
				}
			}
			return null;
		}

		#endregion
	}
}
