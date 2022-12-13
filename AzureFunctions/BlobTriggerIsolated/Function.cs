using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace BlobTriggerIsolated;

public class Function
{
	private readonly ILogger _logger;

	public Function(ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<Function>();
	}

	[Function("BlobTriggerFunction")]
	[BlobOutput("results/{name}", Connection = "AzureWebJobsStorage")]
	public byte[] Run([BlobTrigger("samples-workitems/{name}", Connection = "AzureWebJobsStorage")] byte[] myBlob, string name)
	{
		_logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {myBlob}");

		var resized = new MemoryStream();
		using var image = Image.Load(myBlob);
		image.Mutate(x => x.Resize(900, 600));
		image.Save(resized, new JpegEncoder());

		_logger.LogInformation($"{name} has been processed");
		return resized.ToArray();
	}

	[Function("CronTriggerFunction")]
	public void RunCronTrigger([TimerTrigger("*/2 * * * *")] TimerInfo timerInfo)
	{
		_logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
		_logger.LogInformation($"Next timer schedule at: {timerInfo.ScheduleStatus.Next}");
	}
}
