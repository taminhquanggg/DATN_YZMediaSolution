// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.BusinessService;
using System.Net.Http.Headers;

List<IFormFile> GetAllFilesAsFormFiles(string directoryPath)
{
	// List to hold IFormFile objects
	List<IFormFile> formFiles = new List<IFormFile>();

	// Get all file paths from the directory
	string[] filePaths = Directory.GetFiles(directoryPath);

	foreach (var filePath in filePaths)
	{
		// Read the file content
		byte[] fileBytes = File.ReadAllBytes(filePath);

		// Create a memory stream from the file bytes
		var stream = new MemoryStream(fileBytes);

		// Create a FormFile from the memory stream
		IFormFile formFile = new FormFile(stream, 0, stream.Length, Path.GetFileNameWithoutExtension(filePath), Path.GetFileName(filePath))
		{
			Headers = new HeaderDictionary(),
			ContentType = "application/octet-stream",
			ContentDisposition = new ContentDispositionHeaderValue("form-data")
			{
				Name = "files",
				FileName = Path.GetFileName(filePath)
			}.ToString()
		};


		formFiles.Add(formFile);
	}

	return formFiles;
}

async Task<bool> uploadAuto(string directoryPath, string postID)
{

	List<IFormFile> files = GetAllFilesAsFormFiles(directoryPath);

	Console.WriteLine($"-- start upload to {postID} with {files.Count} file --");

	if (files.Count > 0)
	{
		using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
		{
			foreach (var file in files)
			{
				bool r = await CloudImageApiService.GetInstance().AutoUploadImage(connection, file, Int32.Parse(postID));
				Console.WriteLine(r ? "success" : "fail");
				Thread.Sleep(3);
			}
		}
		return true;
	}
	return false;
}
Console.WriteLine("Hello, World!");

string postID;


string directoryPath;


do
{
	Console.Write("PostID = ");
	postID = Console.ReadLine();

	Console.Write("directoryPath = ");
	directoryPath = Console.ReadLine();
}
while (await uploadAuto(directoryPath, postID));



//string directoryPath = "D:\\PROJECT\\DownloadImg\\Images2";






Console.Read();