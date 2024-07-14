using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Security.Policy;

namespace YzMedia.Library.Common.BusinessService
{
	public class ImageService : BaseService<ImageService>
	{
		[DataContract]
		public class ImageInfo
		{
			public int? PostID { get; set; }

			[DataMember]
			public int? ImageID { get; set; }

			[DataMember]
			public string? ImageCloudID { get; set; }

			[DataMember]
			public string? ImageName { get; set; }

			[DataMember]
			public string? ImagePath { get; set; }

			[DataMember]
			public string? ImageDetectID { get; set; }

			[DataMember]
			public string? ImageDetectPath { get; set; }

			[DataMember]
			public int? StatusTrain { get; set; }

			[DataMember]
			public int? TrainCount { get; set; }

			[DataMember]
			public string? Comment { get; set; }

			[DataMember]
			public string? UserI { get; set; }

			[DataMember]
			public DateTime? InTime { get; set; }

			[DataMember]
			public string? UserU { get; set; }

			[DataMember]
			public DateTime? UpdateTime { get; set; }

			[DataMember]
			public string? ImageSearchDetect { get; set; }

			[DataMember]
			public float? Similarity { get; set; }
		}

		[DataContract]
		public class ImageFilterInfo
		{
			[DataMember]
			public int? PostID { get; set; }

			[DataMember]
			public string? OrderBy { get; set; }

			[DataMember]
			public int? Offset { get; set; }

			[DataMember]
			public int? PageSize { get; set; }

			[DataMember]
			public IFormFile? Image { get; set; }

			[DataMember]
			public int? SearchType { get; set; }

		}

		public List<ImageInfo> GetImageByPostsID(SqlConnection connection, ImageFilterInfo infoFilter)
		{
			var result = new List<ImageInfo>();

			string strSQL = @"
							SELECT I.[ImageID]
								  ,I.[ImageCloudID]
								  ,I.[ImageName]
								  ,I.[ImagePath]
								  ,I.[ImageDetectID]
								  ,I.[ImageDetectPath]
								  ,I.[StatusTrain]
								  ,I.[TrainCount]
								  ,I.[Comment]
								  ,I.[UserI]
								  ,I.[InTime]
								  ,I.[UserU]
								  ,I.[UpdateTime]
							  FROM [dbo].[tbl_Image] I
							INNER JOIN tbl_PostImage P ON I.ImageID = P.ImageID 
							WHERE P.PostID = @PostID ORDER BY I." + infoFilter.OrderBy;

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@PostID", infoFilter.PostID, System.Data.SqlDbType.Int);

				WriteLogExecutingCommand(command);

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var item = new ImageInfo();
						item.ImageID = GetDbReaderValue<int>(reader["ImageID"]);
						item.ImageCloudID = GetDbReaderValue<string>(reader["ImageCloudID"]);
						item.ImageName = GetDbReaderValue<string>(reader["ImageName"]);
						item.ImagePath = GetDbReaderValue<string>(reader["ImagePath"]);
						item.ImageDetectID = GetDbReaderValue<string>(reader["ImageDetectID"]);
						item.ImageDetectPath = GetDbReaderValue<string>(reader["ImageDetectPath"]);
						item.StatusTrain = GetDbReaderValue<int>(reader["StatusTrain"]);
						item.TrainCount = GetDbReaderValue<int>(reader["TrainCount"]);
						item.Comment = GetDbReaderValue<string>(reader["Comment"]);
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

		public bool IsImageUnTrainByPostsID(SqlConnection connection, int postID)
		{
			string strSQL = @"
							SELECT I.[ImageID]
								  ,I.[ImageCloudID]
								  ,I.[ImageName]
								  ,I.[ImagePath]
								  ,I.[ImageDetectID]
								  ,I.[ImageDetectPath]
								  ,I.[StatusTrain]
								  ,I.[TrainCount]
								  ,I.[Comment]
								  ,I.[UserI]
								  ,I.[InTime]
								  ,I.[UserU]
								  ,I.[UpdateTime]
							  FROM [dbo].[tbl_Image] I
							INNER JOIN tbl_PostImage P ON I.ImageID = P.ImageID 
							WHERE P.PostID = @PostID and I.[StatusTrain] != 1";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@PostID", postID, System.Data.SqlDbType.Int);

				WriteLogExecutingCommand(command);

				using (var reader = command.ExecuteReader())
				{
					return reader.Read();
				}
			}
		}

		public ImageInfo GetImageByImageCloudID(SqlConnection connection, string imageCloudID)
		{
			var result = new ImageInfo();

			string strSQL = @"
							SELECT [ImageID]
								  ,[ImageCloudID]
								  ,[ImageName]
								  ,[ImagePath]
								  ,[ImageDetectID]
								  ,[ImageDetectPath]
								  ,[StatusTrain]
								  ,[TrainCount]
								  ,[Comment]
								  ,[UserI]
								  ,[InTime]
								  ,[UserU]
								  ,[UpdateTime]
							  FROM [dbo].[tbl_Image]
							WHERE ImageCloudID = @ImageCloudID";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@ImageCloudID", imageCloudID, System.Data.SqlDbType.VarChar);

				WriteLogExecutingCommand(command);

				using (var reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						result.ImageID = GetDbReaderValue<int>(reader["ImageID"]);
						result.ImageCloudID = GetDbReaderValue<string>(reader["ImageCloudID"]);
						result.ImageName = GetDbReaderValue<string>(reader["ImageName"]);
						result.ImagePath = GetDbReaderValue<string>(reader["ImagePath"]);
						result.ImageDetectID = GetDbReaderValue<string>(reader["ImageDetectID"]);
						result.ImageDetectPath = GetDbReaderValue<string>(reader["ImageDetectPath"]);
						result.StatusTrain = GetDbReaderValue<int>(reader["StatusTrain"]);
						result.TrainCount = GetDbReaderValue<int>(reader["TrainCount"]);
						result.Comment = GetDbReaderValue<string>(reader["Comment"]);
						result.UserI = GetDbReaderValue<string>(reader["UserI"]);
						result.InTime = GetDbReaderValue<DateTime>(reader["InTime"]);
						result.UserU = GetDbReaderValue<string>(reader["UserU"]);
						result.UpdateTime = GetDbReaderValue<DateTime?>(reader["UpdateTime"]);
					}
				}
			}

			return result;
		}

		public int InsertImage(SqlConnection connection, ImageInfo infoInsert)
		{
			string strSQL = @"
			INSERT INTO [dbo].[tbl_Image]
						([ImageCloudID]
						,[ImageName]
						,[ImagePath]
						,[UserI])
					OUTPUT INSERTED.ImageID
					VALUES
						(@ImageCloudID
						,@ImageName
						,@ImagePath
						,@UserI)";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@ImageCloudID", infoInsert.ImageCloudID, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@ImageName", infoInsert.ImageName, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@ImagePath", infoInsert.ImagePath, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@UserI", infoInsert.UserI, System.Data.SqlDbType.VarChar);

				WriteLogExecutingCommand(command);

				return (int)command.ExecuteScalar();
			}
		}

		public bool UpdateImage(SqlConnection connection, ImageInfo infoUpdate)
		{
			string strSQL = @"
			UPDATE [dbo].[tbl_Image]
				   SET [ImageCloudID] = @ImageCloudID
					  ,[ImageName] = @ImageName
					  ,[ImagePath] = @ImagePath
					  ,[ImageDetectID] = @ImageDetectID
					  ,[ImageDetectPath] = @ImageDetectPath
					  ,[StatusTrain] = @StatusTrain
					  ,[TrainCount] = @TrainCount
					  ,[Comment] = @Comment
					  ,[UserU] = @UserU
					  ,[UpdateTime] = GETDATE()
			WHERE ImageID = @ImageID";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@ImageID", infoUpdate.ImageID, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@ImageCloudID", infoUpdate.ImageCloudID, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@ImageName", infoUpdate.ImageName, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@ImagePath", infoUpdate.ImagePath, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@ImageDetectID", infoUpdate.ImageDetectID, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@ImageDetectPath", infoUpdate.ImageDetectPath, System.Data.SqlDbType.VarChar);
				AddSqlParameter(command, "@StatusTrain", infoUpdate.StatusTrain, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@TrainCount", infoUpdate.TrainCount, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@Comment", infoUpdate.Comment, System.Data.SqlDbType.NVarChar);
				AddSqlParameter(command, "@UserU", infoUpdate.UserU, System.Data.SqlDbType.VarChar);

				WriteLogExecutingCommand(command);
				return command.ExecuteNonQuery() > 0;
			}
		}

		//public bool DeleteImage(SqlConnection connection, int ImageID)
		//{
		//	string strSQL = @"DELETE FROM [dbo].[tbl_Image] WHERE ImageID = @ImageID";

		//	using (var command = new SqlCommand(strSQL, connection))
		//	{
		//		AddSqlParameter(command, "@ImageID", ImageID, System.Data.SqlDbType.Int);
		//		WriteLogExecutingCommand(command);

		//		return command.ExecuteNonQuery() > 0;
		//	}
		//}

		public List<ImageInfo> GetImageUnTrain(SqlConnection connection)
		{
			var result = new List<ImageInfo>();

			string strSQL = @"
							SELECT P.[PostID]
								  ,I.[ImageID]
								  ,I.[ImageCloudID]
								  ,I.[ImageName]
								  ,I.[ImagePath]
								  ,I.[ImageDetectID]
								  ,I.[ImageDetectPath]
								  ,I.[StatusTrain]
								  ,I.[TrainCount]
								  ,I.[Comment]
								  ,I.[UserI]
								  ,I.[InTime]
								  ,I.[UserU]
								  ,I.[UpdateTime]
							  FROM [dbo].[tbl_Image] I
							INNER JOIN tbl_PostImage P ON I.ImageID = P.ImageID 
							WHERE I.[StatusTrain] = 0 and I.[TrainCount] < 6 ORDER BY I.Intime";

			using (var command = new SqlCommand(strSQL, connection))
			{
				WriteLogExecutingCommand(command);

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var item = new ImageInfo();
						item.PostID = GetDbReaderValue<int>(reader["PostID"]);
						item.ImageID = GetDbReaderValue<int>(reader["ImageID"]);
						item.ImageCloudID = GetDbReaderValue<string>(reader["ImageCloudID"]);
						item.ImageName = GetDbReaderValue<string>(reader["ImageName"]);
						item.ImagePath = GetDbReaderValue<string>(reader["ImagePath"]);
						item.ImageDetectID = GetDbReaderValue<string>(reader["ImageDetectID"]);
						item.ImageDetectPath = GetDbReaderValue<string>(reader["ImageDetectPath"]);
						item.StatusTrain = GetDbReaderValue<int>(reader["StatusTrain"]);
						item.TrainCount = GetDbReaderValue<int>(reader["TrainCount"]);
						item.Comment = GetDbReaderValue<string>(reader["Comment"]);
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
	}

}
