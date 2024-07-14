using SecurityLibrary;
using System.Data.SqlClient;
using YzMediaAPI.BusinessObject;

namespace YzMediaAPI.BusinessService
{
	public class SqlConnectionFactory : BaseMapInstance<SqlConnectionFactory>
	{
		public SqlConnection GetConnection()
		{
			SqlConnection connection = null;

			try
			{
				string sqlConnectionString = $@"Data Source={AppSettings.HostAddress};Initial Catalog={AppSettings.DatabaseName}
					;User ID={SecurityUtils.Decrypt(AppSettings.EncryptUser, AppSettings.ApplicationName, AppSettings.ApplicationId)}
					;Password={SecurityUtils.Decrypt(AppSettings.EncryptPass, AppSettings.ApplicationName, AppSettings.ApplicationId)}
					;Application Name={AppSettings.ApplicationName};";

				connection = new SqlConnection(sqlConnectionString);
				connection.Open();
			}
			catch (Exception ex)
			{
				_logger.Error($"Failed to connect to database " +
					$"server:{AppSettings.HostAddress} - " +
					$"database:{AppSettings.DatabaseName} - " +
					$"user:{SecurityUtils.Decrypt(AppSettings.EncryptUser, AppSettings.ApplicationName, AppSettings.ApplicationId)}",
					ex);
				throw;
			}

			return connection;
		}
	}

}
