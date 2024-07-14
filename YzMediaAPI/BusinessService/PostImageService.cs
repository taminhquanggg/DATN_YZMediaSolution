using System.Data.SqlClient;
using YzMediaAPI.BusinessObject;

namespace YzMediaAPI.BusinessService
{
	public class PostImageService : BaseService<PostImageService>
	{
		public List<PostImageInfo> GetListPostImage(SqlConnection connection, string strSearch = null)
		{
			var result = new List<PostImageInfo>();

			string strSQL = @"
			SELECT [AutoID]
				  ,[PostID]
				  ,[ImageID]
			  FROM [dbo].[tbl_PostImage] WHERE 1=1";

			using (var command = new SqlCommand(strSQL, connection))
			{
				if (!String.IsNullOrEmpty(strSearch))
				{
					command.CommandText += " and ImageID like @strSearch ";
					AddSqlParameter(command, "@strSearch", "%" + strSearch + "%", System.Data.SqlDbType.Int);
				}

				WriteLogExecutingCommand(command);

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var item = new PostImageInfo();
						item.AutoID = GetDbReaderValue<int>(reader["AutoID"]);
						item.PostID = GetDbReaderValue<int>(reader["PostID"]);
						item.ImageID = GetDbReaderValue<int>(reader["ImageID"]);

						result.Add(item);
					}
				}
			}

			return result;
		}

		public bool InsertPostImage(SqlConnection connection, PostImageInfo infoInsert)
		{
			string strSQL = @"
			INSERT INTO [dbo].[tbl_PostImage]
				   ([PostID]
					,[ImageID])
			 VALUES
				   (@PostID
				   ,@ImageID)";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@PostID", infoInsert.PostID, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@ImageID", infoInsert.ImageID, System.Data.SqlDbType.Int);

				WriteLogExecutingCommand(command);

				return command.ExecuteNonQuery() > 0;
			}
		}

		public bool UpdatePostImage(SqlConnection connection, PostImageInfo infoUpdate)
		{
			string strSQL = @"
			UPDATE [dbo].[tbl_PostImage]
			   SET [PostID] = @PostID
				  ,[ImageID] = @ImageID
			 WHERE [AutoID] = @AutoID";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@AutoID", infoUpdate.AutoID, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@PostID", infoUpdate.PostID, System.Data.SqlDbType.Int);
				AddSqlParameter(command, "@ImageID", infoUpdate.ImageID, System.Data.SqlDbType.Int);

				WriteLogExecutingCommand(command);
				return command.ExecuteNonQuery() > 0;
			}
		}

		public bool DeletePostImage(SqlConnection connection, int imageID)
		{
			string strSQL = @"DELETE FROM [dbo].[tbl_PostImage] WHERE [AutoID] = @AutoID";

			using (var command = new SqlCommand(strSQL, connection))
			{
				AddSqlParameter(command, "@AutoID", imageID, System.Data.SqlDbType.Int);
				WriteLogExecutingCommand(command);

				return command.ExecuteNonQuery() > 0;
			}
		}

	}
}
