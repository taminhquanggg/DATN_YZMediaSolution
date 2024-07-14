using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace YzMedia.Library.Common.BusinessObject
{
	public class AppSetting
	{
		public static string ApplicationId = "YzMedia.1.0";
		public static string ApplicationName = "YzMedia";
		public static string HostAddress = "MINHQUANG3008\\SQLEXPRESS";
		public static string DatabaseName = "YzMedia";
		public static string EncryptUser = "8jtwjt01920YyAHcVeVQBw==";
		public static string EncryptPass = "3Gx+ICYgWRO75fMOXYz4jA==";
		public static string Cloud_AccountId = "ebb60230b2dffcfca922a56003de2224";
		public static string Cloud_TokenUpload = "S72zl_MJq4hap-5G58c1n3n9c7wIEgiszYWqsyZO";
		public static int TimeOutLogin = 36;

		public static string JWT_Key = "YZ MEDIA WEB API SIGNATURE COPYRIGHT BY QUANGKAR";
		public static string JWT_Issuer = "yzgroup.com.vn";
	}

	public class ReadAppSetting
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
