using Azure.Storage.Queues;
using AzureDataStorages.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AzureDataStorages.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QueueStorageController : ControllerBase
{
	private readonly string _connectionString;

	public QueueStorageController(IOptions<AzureConnectionStrings> connectionStrings)
	{
		_connectionString = connectionStrings.Value.QueueStorage;
	}

	[HttpPost("publish")]
	public async Task<IActionResult> Publish(ReturnDTO returnDto)
	{
		var queueName = "returns";
		var queueClient = new QueueClient(_connectionString, queueName);

		await queueClient.CreateIfNotExistsAsync();

		var serializedMessage = JsonSerializer.Serialize(returnDto);
		await queueClient.SendMessageAsync(serializedMessage);

		return Ok();
	}
}
