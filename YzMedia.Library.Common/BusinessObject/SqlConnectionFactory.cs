using SecurityLibrary;
using System.Data.SqlClient;
using YzMedia.Library.Common.BusinessService;

namespace YzMedia.Library.Common.BusinessObject
{
	public class SqlConnectionFactory : BaseMapInstance<SqlConnectionFactory>
	{
		public SqlConnection GetConnection()
		{
			SqlConnection connection = null;

			try
			{
				string sqlConnectionString = $@"Data Source={AppSetting.HostAddress};Initial Catalog={AppSetting.DatabaseName}
					;User ID={SecurityUtils.Decrypt(AppSetting.EncryptUser, AppSetting.ApplicationName, AppSetting.ApplicationId)}
					;Password={SecurityUtils.Decrypt(AppSetting.EncryptPass, AppSetting.ApplicationName, AppSetting.ApplicationId)}
					;Application Name={AppSetting.ApplicationName};";

				connection = new SqlConnection(sqlConnectionString);
				connection.Open();
			}
			catch (Exception ex)
			{
				_logger.Error($"Failed to connect to database " +
					$"server:{AppSetting.HostAddress} - " +
					$"database:{AppSetting.DatabaseName} - " +
					$"user:{SecurityUtils.Decrypt(AppSetting.EncryptUser, AppSetting.ApplicationName, AppSetting.ApplicationId)}",
					ex);
				throw;
			}

			return connection;
		}
	}

}
