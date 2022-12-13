using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace AzureDataStorages.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CosmosCoreController : ControllerBase
{
	private readonly string _connectionString;

	public CosmosCoreController(IOptions<AzureConnectionStrings> connectionStrings)
	{
		_connectionString = connectionStrings.Value.CosmosCore;
	}

	[HttpPost]
	public async Task<IActionResult> Post(Worker worker)
	{
		var container = await GetContainer();
		await container.CreateItemAsync(worker);
		return Accepted();
	}

	[HttpGet]
	public async Task<IActionResult> Get([FromQuery] string id, [FromQuery] string department)
	{
		var container = await GetContainer();
		var worker = await container.ReadItemAsync<Worker>(id, new PartitionKey(department));
		return Ok(worker);
	}

	[HttpPut]
	public async Task<IActionResult> Put(Worker worker)
	{
		var container = await GetContainer();
		await container.UpsertItemAsync(worker);
		return Ok();
	}

	[HttpGet("query")]
	public IActionResult GetByQuery()
	{
		var sqlQuery = "SELECT * FROM Employees e WHERE e.Department = 'IT'";
		var workers = GetWorkers(sqlQuery);
		return Ok(workers);
	}

	[HttpGet("linq")]
	public async Task<IActionResult> GetByQueryLinq()
	{
		var container = await GetContainer();
		var workers = container.GetItemLinqQueryable<Worker>(true).Where(w => w.Department == "IT").ToList();
		return Ok(workers);
	}

	private async IAsyncEnumerable<Worker> GetWorkers(string sqlQuery)
	{
		var container = await GetContainer();
		var workersFeed = container.GetItemQueryIterator<Worker>(sqlQuery);
		while (workersFeed.HasMoreResults)
		{
			var response = await workersFeed.ReadNextAsync();
			foreach (var item in response)
			{
				yield return item;
			}
		}
	}

	private async Task<Container> GetContainer()
	{
		var databaseName = "Employees";
		CosmosClient cosmosClient = new(_connectionString);
		Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
		return await database.CreateContainerIfNotExistsAsync(databaseName, "/Department", 400);
	}
}

public record Worker(string id, string Name, string Department, string City);
