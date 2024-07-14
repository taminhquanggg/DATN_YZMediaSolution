using System.Data.SqlClient;
using YzMediaAPI.BusinessObject;

namespace YzMediaAPI.BusinessService
{
	public class PostService : BaseService<PostService>
	{
		#region Excute SQL Function

		public List<PostInfo> GetListPost(SqlConnection connection, string strSearch = null)
		{
			var result = new List<PostInfo>();

			string strSQL = @"SELECT [PostID]
									  ,[PostTitle]
									  ,[PostDescription]
									  ,[PostLogoImageID]
									  ,[PostCoverImageID]
									  ,[StatusTrain]
									  ,[UserI]
									  ,[InTime]
									  ,[UserU]
									  ,[UpdateTime]
							  FROM [dbo].[tbl_Post] WHERE 1=1";

			using (var command = new SqlCommand(strSQL, connection))
			{
				if (!String.IsNullOrEmpty(strSearch))
				{
					command.CommandText += " and InputID like @strSearch ";
					AddSqlParameter(command, "@strSearch", "%" + strSearch + "%", System.Data.SqlDbType.NVarChar);
				}

				WriteLogExecutingCommand(command);

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var item = new PostInfo();
						item.PostID = GetDbReaderValue<int>(reader["PostID"]);
						item.PostTitle = GetDbReaderValue<string>(reader["PostTitle"]);
						item.PostDescription = GetDbReaderValue<string>(reader["PostDescription"]);
						item.PostLogoImageID = GetDbReaderValue<int>(reader["PostLogoImageID"]);
						item.PostCoverImageID = GetDbReaderValue<int>(reader["PostCoverImageID"]);
						item.StatusTrain = GetDbReaderValue<bool>(reader["StatusTrain"]);
						item.UserI = GetDbReaderValue<string>(reader["UserI"]);
						item.InTime = GetDbReaderValue<DateTime>(reader["InTime"]);
						item.UserU = GetDbReaderValue<string>(reader["UserU"]);
						item.UpdateTime = GetDbReaderValue<DateTime?>(reader["UpdateTime"]);

						result.Add(item);
					}
				}
			}

			return result;
		}

		public List<PostInfo> SearchPosts(SqlConnection connection, PostFilterInfo infoFilter)
		{
			var result = new List<PostInfo>();

			string strSQL = @"SELECT [PostID]
									  ,[PostTitle]
									  ,[PostDescription]
									  ,[PostLogoImageID]
									  ,[PostCoverImageID]
									  ,[StatusTrain]
									  ,[UserI]
									  ,[InTime]
									  ,[UserU]
									  ,[UpdateTime]
							  FROM [dbo].[tbl_Post] WHERE 1=1";

			using (var command = new SqlCommand(strSQL, connection))
			{
				if (!String.IsNullOrEmpty(infoFilter.SearchText))
				{
					command.CommandText += " AND (PostTitle LIKE @strSearch OR PostDescription LIKE @strSearch) ";
					AddSqlParameter(command, "@strSearch", "%" + infoFilter.SearchText + "%", System.Data.SqlDbType.NVarChar);
				}

				if (infoFilter.DateFrom.HasValue)
				{
					command.CommandText += " AND (InTime >= @DateFrom) ";
					AddSqlParameter(command, "@DateFrom", infoFilter.DateFrom, System.Data.SqlDbType.DateTime);
				}

				if (infoFilter.DateTo.HasValue)
				{
					command.CommandText += " AND (InTime <= @DateTo) ";
					AddSqlParameter(command, "@DateTo", infoFilter.DateTo, System.Data.SqlDbType.DateTime);
				}

				command.CommandText += " ORDER BY InTime DESC ";

				WriteLogExecutingCommand(command);

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var item = new PostInfo();
						item.PostID = GetDbReaderValue<int>(reader["PostID"]);
						item.PostTitle = GetDbReaderValue<string>(reader["PostTitle"]);
						item.PostDescription = GetDbReaderValue<string>(reader["PostDescription"]);
						item.PostLogoImageID = GetDbReaderValue<int>(reader["PostLogoImageID"]);
						item.PostCoverImageID = GetDbReaderValue<int>(reader["PostCoverImageID"]);
						item.StatusTrain = GetDbReaderValue<bool>(reader["StatusTrain"]);
						item.UserI = GetDbReaderValue<string>(reader["UserI"]);
						item.InTime = GetDbReaderValue<DateTime>(reader["InTime"]);
						item.UserU = GetDbReaderValue<string>(reader["UserU"]);
						item.UpdateTime = GetDbReaderValue<DateTime?>(reader["UpdateTime"]);

						result.Add(item);
					}
				}
			}

			return result;
		}

		public PostInfo GetByID(SqlConnection connection, int postID)
		{
			var result = new PostInfo();

			string strSQL = @"SELECT P.[PostID]
									  ,P.[PostTitle]
									  ,P.[PostDescription]
									  ,P.[PostLogoImageID]
									  ,ILogo.ImagePath as PostLogoImagePath
									  ,P.[PostCoverImageID]
									  ,ICover.ImagePath as PostCoverImagePath
									  ,P.[StatusTrain]
									  ,P.[UserI]
									  ,P.[InTime]
									  ,P.[UserU]
									  ,P.[UpdateTime]
							  FROM [dbo].[tbl_Post] P
							LEFT JOIN [tbl_Image] ILogo ON ILogo.ImageID = PostLogoImageID
							LEFT JOIN [tbl_Image] ICover ON ICover.ImageID = PostCoverImageID
							WHERE PostID = @PostID";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@PostID", postID, System.Data.SqlDbType.Int);

				WriteLogExecutingCommand(command);

				using (var reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						result.PostID = GetDbReaderValue<int>(reader["PostID"]);
						result.PostTitle = GetDbReaderValue<string>(reader["PostTitle"]);
						result.PostDescription = GetDbReaderValue<string>(reader["PostDescription"]);
						result.PostLogoImageID = GetDbReaderValue<int>(reader["PostLogoImageID"]);
						result.PostLogoImagePath = GetDbReaderValue<string>(reader["PostLogoImagePath"]);
						result.PostCoverImageID = GetDbReaderValue<int>(reader["PostCoverImageID"]);
						result.PostCoverImagePath = GetDbReaderValue<string>(reader["PostCoverImagePath"]);
						result.StatusTrain = GetDbReaderValue<bool>(reader["StatusTrain"]);
						result.UserI = GetDbReaderValue<string>(reader["UserI"]);
						result.InTime = GetDbReaderValue<DateTime>(reader["InTime"]);
						result.UserU = GetDbReaderValue<string>(reader["UserU"]);
						result.UpdateTime = GetDbReaderValue<DateTime?>(reader["UpdateTime"]);
					}
				}
			}

			return result;
		}

		public bool InsertPost(SqlConnection connection, PostInfo infoInsert)
		{
			string strSQL = @"
			INSERT INTO [dbo].[tbl_Post]
					   ([PostTitle]
					   ,[PostDescription]
					   ,[PostLogoImageID]
					   ,[PostCoverImageID]
					   ,[StatusTrain]
					   ,[UserI]
					   ,[InTime])
				 VALUES
					   (@PostTitle
					   ,@PostDescription
					   ,@PostLogoImageID
					   ,@PostCoverImageID
					   ,0
					   ,@UserI
					   ,GETDATE())";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@PostTitle", infoInsert.PostTitle, System.Data.SqlDbType.NVarChar);
				AddSqlParameter(command, "@PostDescription", infoInsert.PostDescription, System.Data.SqlDbType.NVarChar);
				AddSqlParameter(command, "@PostLogoImageID", infoInsert.PostLogoImageID, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@PostCoverImageID", infoInsert.PostCoverImageID, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@UserI", infoInsert.UserI, System.Data.SqlDbType.VarChar);

				WriteLogExecutingCommand(command);

				return command.ExecuteNonQuery() > 0;
			}
		}

		public bool UpdatePost(SqlConnection connection, PostInfo infoUpdate)
		{
			string strSQL = @"
			UPDATE [dbo].[tbl_Post]
			SET [PostTitle] = @PostTitle
				,[PostDescription] = @PostDescription
				,[PostLogoImageID] = @PostLogoImageID
				,[PostCoverImageID] = @PostCoverImageID
				,[StatusTrain] = @StatusTrain
				,[UserU] = @UserU
				,[UpdateTime] = GETDATE()
			WHERE PostID = @PostID";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@PostID", infoUpdate.PostID, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@PostTitle", infoUpdate.PostTitle, System.Data.SqlDbType.NVarChar);
				AddSqlParameter(command, "@PostDescription", infoUpdate.PostDescription, System.Data.SqlDbType.NVarChar);
				AddSqlParameter(command, "@PostLogoImageID", infoUpdate.PostLogoImageID, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@PostCoverImageID", infoUpdate.PostCoverImageID, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@StatusTrain", infoUpdate.StatusTrain, System.Data.SqlDbType.Bit);
				AddSqlParameter(command, "@UserU", infoUpdate.UserU, System.Data.SqlDbType.VarChar);

				WriteLogExecutingCommand(command);
				return command.ExecuteNonQuery() > 0;
			}
		}

		public bool DeletePost(SqlConnection connection, int postID)
		{
			string strSQL = @"DELETE FROM [dbo].[tbl_Post] WHERE PostID = @PostID";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@PostID", postID, System.Data.SqlDbType.Int);
				WriteLogExecutingCommand(command);

				return command.ExecuteNonQuery() > 0;
			}
		}

		#endregion

		public async Task<bool> InsertPostHandle(SqlConnection connection, PostInfo infoInsert)
		{
			if (infoInsert != null)
			{
				if (infoInsert.PostLogoImage != null)
				{
					var idImage = await CloudImageApiService.UploadImage(connection, infoInsert.PostLogoImage, infoInsert.UserI);
					infoInsert.PostLogoImageID = idImage;
				}

				if (infoInsert.PostCoverImage != null)
				{
					var idImage = await CloudImageApiService.UploadImage(connection, infoInsert.PostCoverImage, infoInsert.UserI);
					infoInsert.PostCoverImageID = idImage;
				}

				return InsertPost(connection, infoInsert);
			}

			return false;
		}
	}
}
