using System.Text;
using TaskBoard.Desktop.Services;

namespace TaskBoard.Desktop.UI;

public class MainForm : Form
{
    private readonly TaskService _service = new();

    private readonly ListBox _tasks = new() { Dock = DockStyle.Fill };
    private readonly TextBox _input = new() { Dock = DockStyle.Top };
    private readonly Button _add = new() { Dock = DockStyle.Top, Text = "Add" };
    private readonly Button _toggle = new() { Dock = DockStyle.Top, Text = "Toggle Selected" };

    public MainForm()
    {
        Text = "TaskBoard";
        Width = 600;
        Height = 400;

        Controls.Add(_tasks);
        Controls.Add(_toggle);
        Controls.Add(_add);
        Controls.Add(_input);

        // Intentional issue: async void-like event chain with blocking work in UI thread.
        _add.Click += (_, _) => AddClicked();
        _toggle.Click += (_, _) => ToggleClicked();

        RefreshList();
    }

    private void AddClicked()
    {
        _service.AddTask(_input.Text);
        _input.Clear();
        RefreshList();
    }

    private void ToggleClicked()
    {
        if (_tasks.SelectedItem is null)
        {
            return;
        }

        var selected = _tasks.SelectedItem.ToString() ?? string.Empty;
        var idText = selected.Split(':')[0];
        _service.Toggle(int.Parse(idText));
        RefreshList();
    }

    private void RefreshList()
    {
        _tasks.Items.Clear();
        foreach (var t in _service.GetTasks())
        {
            _tasks.Items.Add($"{t.Id}: {(t.IsDone ? "[x]" : "[ ]")} {t.Title}");
        }
    }
}
