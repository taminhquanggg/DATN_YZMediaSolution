using System.Reflection;
using YzMedia.Library.Common.BusinessObject;

namespace YzMediaAPI.Config
{
	public class ReadAppSettings
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static void ReadAppSettingsInfo(IConfigurationRoot config)
		{
			try
			{
				AppSetting.ApplicationId = config["ApplicationId"]?.ToString();
				AppSetting.ApplicationName = config["ApplicationName"]?.ToString();
				AppSetting.HostAddress = config["HostAddress"]?.ToString();
				AppSetting.DatabaseName = config["DatabaseName"]?.ToString();
				AppSetting.EncryptUser = config["EncryptUser"]?.ToString();
				AppSetting.EncryptPass = config["EncryptPass"]?.ToString();
				AppSetting.Cloud_AccountId = config["Cloud_AccountId"]?.ToString();
				AppSetting.Cloud_TokenUpload = config["Cloud_TokenUpload"]?.ToString();

				AppSetting.TimeOutLogin = Convert.ToInt32(config["TimeOutLogin"]?.ToString());

				AppSetting.JWT_Key = config["JWT_Key"]?.ToString();
				AppSetting.JWT_Issuer = config["JWT_Issuer"]?.ToString();

			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
		}
	}
}
