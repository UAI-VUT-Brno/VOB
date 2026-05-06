using TaskBoard.Desktop.Domain;

namespace TaskBoard.Desktop.Data;

public interface ITaskRepository
{
    List<TaskItem> LoadAll();
    void SaveAll(List<TaskItem> tasks);
}
