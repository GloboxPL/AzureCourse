using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureDataStorages.Controllers;

[ApiController]
[Route("[controller]")]
public class BlobStorageController : ControllerBase
{
	private readonly ILogger<BlobStorageController> _logger;
	private readonly string _connectionString;

	public BlobStorageController(ILogger<BlobStorageController> logger, IOptions<AzureConnectionStrings> connectionStrings)
	{
		_logger = logger;
		_connectionString = connectionStrings.Value.BlobStorage;
	}

	[HttpPost("upload")]
	public async Task<IActionResult> Upload(IFormFile file)
	{
		BlobServiceClient blobServiceClient = new(_connectionString);

		var containerName = "documents";

		var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		await blobContainerClient.CreateIfNotExistsAsync();

		var blobClient = blobContainerClient.GetBlobClient(file.FileName);

		var blodHttpHeaders = new BlobHttpHeaders()
		{
			ContentType = file.ContentType
		};

		await blobClient.UploadAsync(file.OpenReadStream(), blodHttpHeaders);

		_logger.LogInformation($"File {file.FileName} was created in blob storage");

		return Ok();
	}

	[HttpGet("download")]
	public async Task<IActionResult> Download([FromQuery] string blobName)
	{
		BlobServiceClient blobServiceClient = new(_connectionString);

		var containerName = "documents";

		var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		await blobContainerClient.CreateIfNotExistsAsync();

		var blobClient = blobContainerClient.GetBlobClient(blobName);
		var downloadResponse = await blobClient.DownloadContentAsync();
		var content = downloadResponse.Value.Content.ToStream();
		var contentType = blobClient.GetProperties().Value.ContentType;

		return File(content, contentType, fileDownloadName: blobName);
	}
}