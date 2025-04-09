using csharp_peripherals;

namespace MainProject;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        this.Load += Form1_Load;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        if (!Program.bool_App_Loaded)
        {
            button_RunInConsole.Enabled = false;
        }
        else
        {
            // Initialize the form or perform any other setup here
            this.Text = "Clipboard Dictionary";
        }
    }

    private void button_RunInConsole_Click(object sender, EventArgs e)
    {

        ConsoleHelper.ShowConsole();
        Thread.Sleep(100); // Let the console spin up
        ConsoleHelper.ReinitializeConsole();
        Thread.Sleep(100); // Let the console spin up

        Utilities.RunInConsole(true);
    }

    private void button_RunInConsoleClipboard_Click(object sender, EventArgs e)
    {

        ConsoleHelper.ShowConsole();
        Thread.Sleep(100); // Let the console spin up
        ConsoleHelper.ReinitializeConsole();
        Thread.Sleep(100); // Let the console spin up

        Utilities.RunInConsoleClipboard();
    }
}
