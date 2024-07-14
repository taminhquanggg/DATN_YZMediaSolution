namespace YzMediaAPI.BusinessObject
{
	public class ImageInfo
	{
		public int ImageID { get; set; }
		public string ImageCloudID { get; set; }
		public string ImageName { get; set; }
		public string ImagePath { get; set; }
		public int StatusTrain { get; set; }
		public int TrainCount { get; set; }
		public string Comment { get; set; }
		public string UserI { get; set; }
		public DateTime InTime { get; set; }
		public string UserU { get; set; }
		public DateTime? UpdateTime { get; set; }
	}

	public class ImageFilterInfo
	{
		public int PostID { get; set; }
		public string OrderBy { get; set; }
		public int Offset { get; set; }
		public int PageSize { get; set; }
	}
}
