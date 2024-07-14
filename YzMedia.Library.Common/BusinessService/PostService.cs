using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.Helper;

namespace YzMedia.Library.Common.BusinessService
{
	public class PostService : BaseService<PostService>
	{
		[DataContract]
		public class PostInfo
		{
			[DataMember]
			public int PostID { get; set; }

			[DataMember]
			public string? PostTitle { get; set; }

			[DataMember]
			public string? PostDescription { get; set; }

			[DataMember]
			public int PostLogoImageID { get; set; }

			[DataMember]
			public IFormFile PostLogoImage { get; set; }

			[DataMember]
			public string? PostLogoImagePath { get; set; }

			[DataMember]
			public int PostCoverImageID { get; set; }

			[DataMember]
			public IFormFile PostCoverImage { get; set; }

			[DataMember]
			public string? PostCoverImagePath { get; set; }

			[DataMember]
			public bool StatusTrain { get; set; }

			[DataMember]
			public string? UserI { get; set; }

			[DataMember]
			public DateTime InTime { get; set; }

			[DataMember]
			public string? UserU { get; set; }

			[DataMember]
			public DateTime? UpdateTime { get; set; }

		}

		[DataContract]
		public class PostFilterInfo
		{
			[DataMember]
			public string? SearchText { get; set; }

			[DataMember]
			public DateTime? DateFrom { get; set; }

			[DataMember]
			public DateTime? DateTo { get; set; }

			[DataMember]
			public int? Offset { get; set; }

			[DataMember]
			public int? PageSize { get; set; }
		}

		#region Excute SQL Function

		public List<PostInfo> GetAllPost(SqlConnection connection)
		{
			var result = new List<PostInfo>();

			string strSQL = @"SELECT [PostID]
									  ,[PostTitle]
							  FROM [dbo].[tbl_Post]";

			using (var command = new SqlCommand(strSQL, connection))
			{
				WriteLogExecutingCommand(command);

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var item = new PostInfo();
						item.PostID = GetDbReaderValue<int>(reader["PostID"]);
						item.PostTitle = GetDbReaderValue<string>(reader["PostTitle"]);

						result.Add(item);
					}
				}
			}

			return result;
		}

		public List<PostInfo> GetAllPostUnTrain(SqlConnection connection)
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
							  FROM [dbo].[tbl_Post] WHERE StatusTrain = 0";

			using (var command = new SqlCommand(strSQL, connection))
			{
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

		public int InsertPost(SqlConnection connection, PostInfo infoInsert)
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
				 OUTPUT INSERTED.PostID
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
				AddSqlParameter(command, "@PostTitle", infoInsert.PostTitle, System.Data.SqlDbType.NText);
				AddSqlParameter(command, "@PostDescription", infoInsert.PostDescription, System.Data.SqlDbType.NText);
				AddSqlParameter(command, "@PostLogoImageID", infoInsert.PostLogoImageID, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@PostCoverImageID", infoInsert.PostCoverImageID, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@UserI", infoInsert.UserI, System.Data.SqlDbType.VarChar);

				WriteLogExecutingCommand(command);

				return (int)command.ExecuteScalar();
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
				AddSqlParameter(command, "@PostTitle", infoUpdate.PostTitle, System.Data.SqlDbType.NText);
				AddSqlParameter(command, "@PostDescription", infoUpdate.PostDescription, System.Data.SqlDbType.NText);
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
					var idImage = await CloudImageApiService.GetInstance().UploadImage(connection, infoInsert.PostLogoImage, infoInsert.UserI);
					infoInsert.PostLogoImageID = idImage;
				}

				if (infoInsert.PostCoverImage != null)
				{
					var idImage = await CloudImageApiService.GetInstance().UploadImage(connection, infoInsert.PostCoverImage, infoInsert.UserI);
					infoInsert.PostCoverImageID = idImage;
				}

				int postIdInsert = InsertPost(connection, infoInsert);

				if (postIdInsert > 0)
				{
					await APICallingHelper
						.GetInstance()
						.GetSingleResultFromAPI<string, SingleResponeMessage<string>>(null, $"http://localhost:8000/table/face{postIdInsert}");

					return true;
				}
			}

			return false;
		}
	}
}
