using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AzureDataStorages.Controllers;

[ApiController]
[Route("[controller]")]
public class CosmosMongoController : ControllerBase
{
	private readonly string _connectionString;

	public CosmosMongoController(IOptions<AzureConnectionStrings> connectionStrings)
	{
		_connectionString = connectionStrings.Value.CosmosMongo;
	}

	[HttpPost]
	public IActionResult Post([FromBody] Product product)
	{
		var products = GetProductsCollection();

		products.InsertOne(product);
		return Accepted();
	}

	[HttpGet]
	public async Task<IActionResult> GetById([FromQuery] string id)
	{
		var products = GetProductsCollection();

		var product = (await products.FindAsync(p => p.Id == id)).First();
		return Ok(product);
	}

	[HttpGet("linq")]
	public IActionResult GetByCategory([FromQuery] string category)
	{
		var products = GetProductsCollection();

		var results = products.AsQueryable()
			.Where(p => p.Category == category)
			.OrderByDescending(p => p.Id)
			.ToList();
		return Ok(results);
	}

	[HttpDelete]
	public async Task<IActionResult> Delete([FromQuery] string id)
	{
		var products = GetProductsCollection();

		await products.DeleteOneAsync(p => p.Id == id);
		return NoContent();
	}

	private IMongoCollection<Product> GetProductsCollection()
	{
		MongoClient client = new(_connectionString);
		var database = client.GetDatabase("ProductsDb");
		var products = database.GetCollection<Product>("products");
		return products;
	}
}

public record Product(string Id, string Category, string Name, int Quantity, bool Sale);
