using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace YzMedia.Library.Common.BusinessObject
{
	[DataContract]
	public class ImageInsertInfo
	{
		[DataMember]
		public string? tableName { get; set; }

		[DataMember]
		public string? imageID { get; set; }
	}

	[DataContract]
	public class ImageDetectInfo
	{
		[DataMember]
		public string? imageDetectID { get; set; }

		[DataMember]
		public string? imageDetectPath { get; set; }
	}

	[DataContract]
	public class ImageSearchInfo
	{
		[DataMember]
		public string? tableName { get; set; }

		[DataMember]
		public IFormFile? image { get; set; }
	}

	[DataContract]
	public class ImageSearchResponse
	{
		[DataMember]
		public string? ImageID { get; set; }

		[DataMember]
		public string? ImageDetectID { get; set; }

		[DataMember]
		public string? ImageDetectPath { get; set; }

		[DataMember]
		public string? FaceDetectID { get; set; }

		[DataMember]
		public string? FaceDetectPath { get; set; }

		[DataMember]
		public float? Similarity { get; set; }

		[DataMember]
		public string? ImageSearchDetect { get; set; }
	}
}
