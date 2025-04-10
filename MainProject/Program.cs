using csharp_dictionary_japanese;
using csharp_peripherals;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using csharp_journal;
using System.IO;

namespace MainProject;


/* Dynamically attach a console window at runtime - allowing me to keep the .csproj's OutputType as WinExe (so the app doesn't flash a console window on launch), but can still optionally use a console */
public static class ConsoleHelper
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();

    public static void ShowConsole()
    {
        AllocConsole();

    }
    public static void HideConsole()
    {
        FreeConsole();
        Thread.Sleep(100); // Let the console spin up
    }

    public static void ReinitializeConsole()
    {
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
        Console.SetIn(new StreamReader(Console.OpenStandardInput()));

        // Optional, but safe: set UTF-8 encoding
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
    }

}

class Program
{

    public static bool bool_App_Loaded = false;

    static string libFolderPath = @"D:\Data\Dictionary\Japanese\";
    static string kanjifilename = @"kanjidic2.xml";
    static string jmdict_m_filename = @"JMdict.xml";
    static string jmdict_e_filename = @"JMdict_e.xml";
    static string edictfilename = @"edict2";
    static string kanjiPath = System.IO.Path.Combine(libFolderPath, kanjifilename);
    static string edictPath = System.IO.Path.Combine(libFolderPath, edictfilename);
    static string jmdict_Path = System.IO.Path.Combine(libFolderPath, jmdict_e_filename);
    
    public static string basePath    = AppContext.BaseDirectory;
    //public static string projPath    = string.Empty; // Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
    public static string logsPath = @"D:\Logs\";
    public static ChronoLogger logger_error;
    public static ChronoLogger logger_event;
    public static bool DebugMode = true; // Set to true for debugging purposes

    public static class AppPathResolver
    {
        public static string GetBaseDirectory()
        {
            string path = AppContext.BaseDirectory;
            string? current = path;

            // Look up the tree for a .csproj file (or some custom marker file)
            while (current != null)
            {
                if (Directory.EnumerateFiles(current, " *.csproj").Any())
                {
                    return current; // Project folder detected
                }

                current = Directory.GetParent(current)?.FullName;
            }

            // Fallback to runtime folder (e.g. for production)
            return path;
        }
    }



    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {

        ConsoleHelper.ShowConsole();
        Console.WriteLine("Console is now visible!");

        // Enable the console to be used for input and output of Multi-byte characters
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        
        basePath = AppPathResolver.GetBaseDirectory();
        //logsPath = Path.Combine(basePath, "Logs");
        Directory.CreateDirectory(logsPath);
        

        logger_error = new ChronoLogger(Path.Combine(logsPath,"error.log"));
        logger_event = new ChronoLogger(Path.Combine(logsPath, "event.log"));

        logger_event.Log($"{Emoji.Clock} | ============================= Application Started ===================================");

        bool_App_Loaded = ApplicationConfiguration_Initialize() ? true : MessageBox.Show(" Application Configuration Failed.", "Configuration Error") == DialogResult.OK && false;

        if (bool_App_Loaded) ConsoleHelper.HideConsole();

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());

    }


    static bool ApplicationConfiguration_Initialize()
    {
        // This method is called to initialize application configuration settings.
        // You can set high DPI settings, default font, and other configurations here.
        // For example:
        // Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        // Application.SetDefaultFont(new Font("Arial", 9.75F));
        Console.WriteLine("Application Configuration Initialized.");

        //jmdict_Path = string.Empty;

        if (!Utilities.InitializeDictionaryAndTools(kanjiPath, edictPath, jmdict_Path)) return false;

        return true;
    }



}