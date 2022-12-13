using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureDataStorages.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KeyVaultController : ControllerBase
{
	private readonly SecretClient _secretClient;

	public KeyVaultController(IOptions<AzureConnectionStrings> connectionStrings)
	{
		var secretClient = new SecretClient(new Uri(connectionStrings.Value.KeyVault), new DefaultAzureCredential());
		_secretClient = secretClient;
	}

	[HttpPost]
	public async Task<IActionResult> CreateSecretApiKey([FromQuery] string userId)
	{

		var secretName = $"api-key-{userId}";
		var secret = Guid.NewGuid().ToString();
		await _secretClient.SetSecretAsync(secretName, secret);
		return Ok();
	}

	[HttpGet]
	public async Task<IActionResult> GetSecretApiKey([FromQuery] string userId)
	{

		var secretName = $"api-key-{userId}";
		var secretResponse = await _secretClient.GetSecretAsync(secretName);
		return Ok(secretResponse.Value.Value);
	}

	[HttpDelete]
	public async Task<IActionResult> DeleteSecretApiKey([FromQuery] string userId)
	{

		var secretName = $"api-key-{userId}";
		var deleteOperation = await _secretClient.StartDeleteSecretAsync(secretName);
		await deleteOperation.WaitForCompletionAsync();
		return NoContent();
	}
}
