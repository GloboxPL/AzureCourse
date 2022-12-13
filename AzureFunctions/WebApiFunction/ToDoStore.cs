namespace WebApiFunction;

internal class ToDoStore
{
	private readonly List<ToDo> _toDoList = new List<ToDo>()
	{
		new ToDo()
		{
			Description="Learn Azure"
		}
	};

	public List<ToDo> GetToDos() => _toDoList;

	public void AddToDo(string toDoDescription)
	{
		var toDo = new ToDo()
		{
			Description = toDoDescription
		};
		_toDoList.Add(toDo);
	}

	public void MarkComplited(Guid id)
	{
		var toDo = _toDoList.Find(x => x.Id == id);
		toDo.IsFinished = true;
	}
}
