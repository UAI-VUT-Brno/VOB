using System;
using System.Windows.Forms;
using TaskBoard.Desktop.UI;

namespace TaskBoard.Desktop;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
