using Azure.Data.Tables;
using AzureDataStorages.Models.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureDataStorages.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TableStorageController : ControllerBase
{
	private readonly TableClient _tableClient;

	public TableStorageController(IOptions<AzureConnectionStrings> connectionStrings)
	{
		TableServiceClient tableServiceClient = new(connectionStrings.Value.TableStorage);
		_tableClient = tableServiceClient.GetTableClient("employee");
	}

	[HttpPost]
	public async Task<IActionResult> Post(Employee employee)
	{

		await _tableClient.CreateIfNotExistsAsync();
		await _tableClient.AddEntityAsync(employee);

		return Accepted();
	}

	[HttpGet]
	public async Task<IActionResult> Get([FromQuery] string partitionKey, [FromQuery] string rowKey)
	{
		var employee = await _tableClient.GetEntityAsync<Employee>(partitionKey, rowKey);
		return Ok(employee);
	}

	[HttpGet("query")]
	public async Task<IActionResult> Query([FromQuery] string partitionKey, [FromQuery] string rowKey)
	{
		var employees = _tableClient.Query<Employee>(e => e.PartitionKey == "IT");
		return Ok(employees);
	}
}
