namespace YzMediaAPI.BusinessObject
{
	public class PostInfo
	{
		public int PostID { get; set; }
		public string PostTitle { get; set; }
		public string PostDescription { get; set; }
		public int PostLogoImageID { get; set; }
		public IFormFile PostLogoImage { get; set; }
		public string PostLogoImagePath { get; set; }
		public int PostCoverImageID { get; set; }
		public IFormFile PostCoverImage { get; set; }
		public string PostCoverImagePath { get; set; }
		public bool StatusTrain { get; set; }
		public string UserI { get; set; }
		public DateTime InTime { get; set; }
		public string UserU { get; set; }
		public DateTime? UpdateTime { get; set; }
	}

	public class PostFilterInfo
	{
		public string SearchText { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
		public int Offset { get; set; }
		public int PageSize { get; set; }
	}
}
