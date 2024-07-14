using System.Data.SqlClient;

namespace YzMedia.Library.Common.BusinessService
{
	public class UserService : BaseService<UserService>
	{
		public class UserInfo
		{
			public int UserID { get; set; }
			public string UserName { get; set; }
			public string Password { get; set; }
			public string FullName { get; set; }
		}

		public List<UserInfo> GetListUser(SqlConnection connection, string strSearch = null)
		{
			var result = new List<UserInfo>();

			string strSQL = @"
			SELECT [UserID]
				,[UserName]
				,[Password]
				,[FullName]
			FROM [dbo].[tbl_User] WHERE 1=1";

			using (var command = new SqlCommand(strSQL, connection))
			{
				if (!String.IsNullOrEmpty(strSearch))
				{
					command.CommandText += " and UserName like @strSearch ";
					AddSqlParameter(command, "@strSearch", "%" + strSearch + "%", System.Data.SqlDbType.VarChar);
				}

				WriteLogExecutingCommand(command);

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var item = new UserInfo();
						item.UserID = GetDbReaderValue<int>(reader["UserID"]);
						item.UserName = GetDbReaderValue<string>(reader["UserName"]);
						item.Password = GetDbReaderValue<string>(reader["Password"]);
						item.FullName = GetDbReaderValue<string>(reader["FullName"]);

						result.Add(item);
					}
				}
			}

			return result;
		}

		public UserInfo GetUser(SqlConnection connection, string userName, string password)
		{
			var result = new UserInfo();

			string strSQL = @"
			SELECT [UserID]
				,[UserName]
				,[Password]
				,[FullName]
			FROM [dbo].[tbl_User] WHERE UserName = @UserName and Password = @Password";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@UserName", userName, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@Password", password, System.Data.SqlDbType.VarChar);

				WriteLogExecutingCommand(command);

				using (var reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						result.UserID = GetDbReaderValue<int>(reader["UserID"]);
						result.UserName = GetDbReaderValue<string>(reader["UserName"]);
						result.Password = GetDbReaderValue<string>(reader["Password"]);
						result.FullName = GetDbReaderValue<string>(reader["FullName"]);
					}
				}
			}

			return result;
		}


		public bool InsertUser(SqlConnection connection, UserInfo infoInsert)
		{
			string strSQL = @"
			INSERT INTO [dbo].[tbl_User]
				   ([UserName]
				   ,[Password]
				   ,[FullName])
			 VALUES
				   (@UserName
				   ,@Password
				   ,@FullName)";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@UserName", infoInsert.UserName, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@Password", infoInsert.Password, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@FullName", infoInsert.FullName, System.Data.SqlDbType.NVarChar);

				WriteLogExecutingCommand(command);

				return command.ExecuteNonQuery() > 0;
			}
		}

		public bool UpdateUser(SqlConnection connection, UserInfo infoUpdate)
		{
			string strSQL = @"
			UPDATE [dbo].[tbl_User]
			   SET [Password] = @Password
				  ,[FullName] = @FullName
			 WHERE [UserName] = @UserName";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@UserName", infoUpdate.UserName, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@Password", infoUpdate.Password, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@FullName", infoUpdate.FullName, System.Data.SqlDbType.NVarChar);

				WriteLogExecutingCommand(command);
				return command.ExecuteNonQuery() > 0;
			}
		}

		public bool DeleteUser(SqlConnection connection, string userName)
		{
			string strSQL = @"DELETE FROM [dbo].[tbl_User] WHERE UserName = @UserName";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@UserName", userName, System.Data.SqlDbType.VarChar);
				WriteLogExecutingCommand(command);

				return command.ExecuteNonQuery() > 0;
			}
		}

	}

}
