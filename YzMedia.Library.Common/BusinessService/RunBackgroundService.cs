using Microsoft.Extensions.Hosting;
using System.Reflection;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.Helper;

namespace YzMedia.Library.Common.BusinessService
{
	public class RunBackgroundService : IHostedService
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public Task StartAsync(CancellationToken cancellationToken)
		{
			Task.Run(AutoTrainImage, cancellationToken);

			Task.Run(AutoUpdateStatusPostsTrain, cancellationToken);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public async Task AutoTrainImage()
		{
			while (true)
			{
				try
				{
					using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
					{
						var listImageUntrain = ImageService.GetInstance().GetImageUnTrain(connection);

						foreach (var image in listImageUntrain)
						{
							try
							{
								ImageInsertInfo imageInsertInfo = new ImageInsertInfo
								{
									tableName = "face" + image.PostID.ToString(),
									imageID = image.ImageCloudID
								};

								var resImgInsert = await APICallingHelper
									.GetInstance()
									.GetSingleResultFromAPI<ImageInsertInfo, SingleResponeMessage<ImageDetectInfo>>
									(
										imageInsertInfo,
										$"http://localhost:8000/face-api/insert-image",
										"POST"
									);

								if (resImgInsert.IsSuccess)
								{
									image.ImageDetectID = resImgInsert.Item.imageDetectID;
									image.ImageDetectPath = resImgInsert.Item.imageDetectPath;
									image.StatusTrain = 1;
									image.UserU = "AutoTrain";
								}
								else
								{
									if (image.TrainCount > 5)
									{
										image.StatusTrain = 2;
									}

									image.TrainCount += 1;
									image.Comment = resImgInsert.Err.MsgString;
									image.UserU = "AutoTrain";
								}
							}
							catch (Exception ex)
							{
								if (image.TrainCount > 5)
								{
									image.StatusTrain = 2;
								}

								image.TrainCount += 1;
								image.Comment = ex.Message;
								image.UserU = "AutoTrain";
							}
							finally
							{
								ImageService.GetInstance().UpdateImage(connection, image);
							}
						}
					}
				}
				catch (Exception ex)
				{
					_logger.Error(ex);
				}

				Thread.Sleep(5000);
			}
		}

		public async Task AutoUpdateStatusPostsTrain()
		{
			while (true)
			{
				try
				{
					using (var connection = DefaultConnectionFactory.YzMedia.GetConnection())
					{
						var listPostUnTrain = PostService.GetInstance().GetAllPostUnTrain(connection);

						foreach (var post in listPostUnTrain)
						{
							post.StatusTrain = true;
							post.UserU = "AutoTrain";

							if (ImageService.GetInstance().IsImageUnTrainByPostsID(connection, post.PostID))
							{
								post.StatusTrain = false;
							}

							PostService.GetInstance().UpdatePost(connection, post);
						}
					}
				}
				catch (Exception ex)
				{
					_logger.Error(ex);
				}

				Thread.Sleep(5000);
			}
		}
	}
}
