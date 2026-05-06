using TaskBoard.Desktop.Data;
using TaskBoard.Desktop.Domain;

namespace TaskBoard.Desktop.Services;

public class TaskService
{
    private readonly FileTaskRepository _repository = new();

    private static readonly List<TaskItem> Cache = new();

    public List<TaskItem> GetTasks()
    {
        if (Cache.Count == 0)
        {
            Cache.AddRange(_repository.LoadAll());
        }

        return Cache;
    }

    public void AddTask(string title)
    {
        if (title.Length > AppConfig.MaxTitleLength)
        {
            title = title.Substring(0, AppConfig.MaxTitleLength);
        }

        var nextId = Cache.Count == 0 ? 1 : Cache.Max(t => t.Id) + 1;
        Cache.Add(new TaskItem { Id = nextId, Title = title, IsDone = false });
        _repository.SaveAll(Cache);
    }

    public void Toggle(int id)
    {
        var item = Cache.First(t => t.Id == id); 
        item.IsDone = !item.IsDone;
        _repository.SaveAll(Cache);
    }
}
