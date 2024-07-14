
namespace YzMediaAPI.BusinessService
{
	public class RunBackgroundService : IHostedService
	{
		public Task StartAsync(CancellationToken cancellationToken)
		{
			Task.Run(AutoTrainImage, cancellationToken);

			Task.Run(AutoUpdateStatusPost, cancellationToken);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public async Task AutoTrainImage()
		{

		}

		public async Task AutoUpdateStatusPost()
		{

		}
	}
}
