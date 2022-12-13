namespace WebApiFunction;

internal class ToDo
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Description { get; set; } = string.Empty;
	public bool IsFinished { get; set; }
}
