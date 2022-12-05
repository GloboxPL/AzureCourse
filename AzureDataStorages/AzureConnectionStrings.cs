namespace AzureDataStorages;

public class AzureConnectionStrings
{
	public string BlobStorage { get; set; } = string.Empty;
	public string QueueStorage { get; set; } = string.Empty;
	public string TableStorage { get; set; } = string.Empty;
	public string CosmosCore { get; set; } = string.Empty;
	public string CosmosMongo { get; set; } = string.Empty;
	public string RedisCahce { get; set; } = string.Empty;
	public string KeyVault { get; set; } = string.Empty;
}
