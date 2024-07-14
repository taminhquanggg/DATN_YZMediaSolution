using System.Data.SqlClient;
using YzMediaAPI.BusinessObject;

namespace YzMediaAPI.BusinessService
{
	public class ImageService : BaseService<ImageService>
	{
		public List<ImageInfo> GetImageByPostsID(SqlConnection connection, ImageFilterInfo infoFilter)
		{
			var result = new List<ImageInfo>();

			string strSQL = @"
							SELECT I.[ImageID]
								  ,I.[ImageCloudID]
								  ,I.[ImageName]
								  ,I.[ImagePath]
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
	}
}
