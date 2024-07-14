namespace YzMedia.Library.Common.BusinessObject
{
	public static class DefaultConnectionFactory
	{
		public static SqlConnectionFactory YzMedia
		{
			get
			{
				return SqlConnectionFactory.GetInstance("YzMedia");
			}
		}
	}
}
