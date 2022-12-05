namespace AzureDataStorages;

public class DatabaseMock
{
	private static readonly Dictionary<string, string> _countries = new()
	{
		{"pl","Poland" },
		{"es", "Spain" },
		{"it", "Italy" },
		{"nl", "Netherlands" },
		{"us", "United States" },
		{"en", "England" }
	};

	public async Task<Dictionary<string, string>> GetAllCountriesAsync()
	{
		await Task.Delay(1000);
		return _countries;
	}

	public void AddCountry(string code, string name)
	{
		_countries.Add(code, name);
	}
}
