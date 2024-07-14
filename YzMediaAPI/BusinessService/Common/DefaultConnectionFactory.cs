namespace YzMediaAPI.BusinessService
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
