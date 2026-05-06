using TaskBoard.Desktop.Domain;

namespace TaskBoard.Desktop.Data;

public class FileTaskRepository : ITaskRepository
{
    public List<TaskItem> LoadAll()
    {
        var list = new List<TaskItem>();

        try
        {
            if (!File.Exists(AppConfig.StoragePath))
            {
                return list;
            }

            foreach (var line in File.ReadAllLines(AppConfig.StoragePath))
            {
                var parts = line.Split('|');
                list.Add(new TaskItem
                {
                    Id = int.Parse(parts[0]),
                    Title = parts[1],
                    IsDone = bool.Parse(parts[2])
                });
            }
        }
        catch
        {
        }

        return list;
    }

    public void SaveAll(List<TaskItem> tasks)
    {
        var lines = tasks.Select(t => $"{t.Id}|{t.Title}|{t.IsDone}").ToArray();
        File.WriteAllLines(AppConfig.StoragePath, lines);
    }
}
