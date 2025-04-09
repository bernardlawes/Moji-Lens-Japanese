using csharp_dictionary_japanese;
using csharp_peripherals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using csharp_journal;

namespace MainProject
{
    class Utilities
    {
        // Instantiate the object with user input
        static KanjiDict2_Reader _kanjiDictionary;
        static EDict2_Reader _edictDictionary;
        static Kanji_API_Client _kanjiAPIClient;
        static ChronoLogger _logger_kanji;

        static string userInput = string.Empty;

        public static void MeasureTime(string label, Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            Console.WriteLine($"{label} took {sw.ElapsedMilliseconds} ms");

            // Usage:
            /*
            Measure("Lookup process", () =>
            {
                var matches = edict.LookupFirstMatches(segments);
            });
            */
        }

        public static bool InitializeDictionaryAndTools(string kanjiPath, string edictPath)
        {


            if (!ValidateDictionaryFiles(kanjiPath, edictPath)) return false;

            Program.logger_event.Log($"{Emoji.Clock} | Dictionaries Files Found" );

            _logger_kanji = new ChronoLogger(Path.Combine(Program.logsPath,"kanji.log"));

            string kanjifilename = System.IO.Path.GetFileName(kanjiPath);
            // Load the kanjidic2.xml file
            _kanjiDictionary = new KanjiDict2_Reader(kanjiPath);
            string msg = (_kanjiDictionary.RecordCount == 0) ? $"Error: No records found in the {kanjifilename} file." : $"Loaded {_kanjiDictionary.RecordCount} entries from the {kanjifilename} file.";
            if (_kanjiDictionary.RecordCount == 0) return false;
            Program.logger_event.Log($"{Emoji.Clock} | {msg}");
            Console.WriteLine(msg);

            string edictfilename = System.IO.Path.GetFileName(edictPath);
            // Load the edict2 file
            _edictDictionary = new EDict2_Reader(edictPath);
            msg = (_edictDictionary.RecordCount == 0) ? $"Error: No records found in the {edictfilename} file." : $"Loaded {_edictDictionary.RecordCount} entries from the {edictfilename} file.";
            if (_edictDictionary.RecordCount == 0) return false;
            Program.logger_event.Log($"{Emoji.Clock} | {msg}");
            Console.WriteLine(msg);


            // Initialize the Kanji API client
            _kanjiAPIClient = new Kanji_API_Client();

            

            return true;

        }

        static bool ValidateDictionaryFiles(string kanjiPath, string edictPath)
        {
            string kanjifilename = System.IO.Path.GetFileName(kanjiPath);
            // Check if the kanjidic file exists
            Console.WriteLine(File.Exists(kanjiPath) ? $"{kanjifilename} Dictionary Found." : $"\n{kanjifilename} file not found at {kanjiPath}");
            if (!File.Exists(kanjiPath)) return false;

            string edictfilename = System.IO.Path.GetFileName(edictPath);
            // Check if the edict file exists
            Console.WriteLine(File.Exists(edictPath) ? $"{edictfilename} Dictionary Found." : $"\n{edictfilename} file not found at {edictPath}");
            if (!File.Exists(edictPath)) return false;

            return true;

            // Load the files or perform any other operations here
        }

        static string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            string userinput = Console.ReadLine() ?? "";

            Console.WriteLine((string.IsNullOrWhiteSpace(userinput)) ? "Input cannot be empty. Please try again." : $"\nYou entered: {userinput.Trim()}");
            if (string.IsNullOrWhiteSpace(userinput)) return GetUserInput(prompt); ;

            return userinput.Trim();
        }

        public static void RunInConsoleClipboard()
        {

            bool quiet = true;

            ClipboardMonitorHost.Start(text =>
            {
                Console.Clear();
                if (!quiet)  Console.WriteLine($"Search Item: {text}");

                var stopwatch = Stopwatch.StartNew();
                long segmentTime = 0;
                long lookupTime = 0;
                long loggingTime = 0;

                int kanjiCount = 0, wordCount = 0;

                if (!string.IsNullOrWhiteSpace(text))
                {                 
                    var kanjisegments = _kanjiDictionary.SegmentKanji(text);
                    var kanjis = _kanjiDictionary.LookupUniqueKanjiDefinitions(kanjisegments);
                    if (kanjis != null && kanjis.Count > 0)
                    {
                        foreach (KanjiEntry res in kanjis)
                        {
                            if (!quiet) Console.WriteLine($"{ res.Literal} : {string.Join(", ", res.Meanings)}");
                            _logger_kanji.Log($"{Emoji.Info} {res.Literal} | {string.Join(", ", res.Readings)} | {string.Join(", ", res.Meanings)} | Grade: {res.Grade} | JLPT: {res.JLPT} | Strokes: {res.StrokeCount}"); // <-- 💾 this line saves to file
                            kanjiCount++;
                        }

                    }

                    Console.WriteLine("\n===========================================\n");

                    Console.WriteLine($"Characters : {text.Length.ToString()}");
                    Console.WriteLine($"Kanji Count : {kanjisegments.Count.ToString()}");
                    Console.WriteLine($"Matched Kanji : {kanjis.Count.ToString()}");

                    Console.WriteLine("\n===========================================\n");

                    var segments    = _edictDictionary.SegmentJapanese(text, false);

                    segmentTime = stopwatch.ElapsedMilliseconds;

                    var definitions = _edictDictionary.LookupDefinitions(segments, false);

                    lookupTime = stopwatch.ElapsedMilliseconds;

                    Console.WriteLine("\n===========================================\n");
                    foreach (string def in definitions)
                    {
                        if(!quiet) Console.WriteLine($"{def}");

                        _logger_kanji.Log($"{Emoji.Fire} {def}"); // <-- 💾 this line saves to file
                        wordCount++;
                    }

                    loggingTime = stopwatch.ElapsedMilliseconds;

                } else
                {
                    Console.WriteLine("Input term cannot be null or whitespace. Please try again.");
                }

                stopwatch.Stop();
                Console.WriteLine($"Unique Kanji: {kanjiCount}\nUnique Kanji Words: {wordCount}");

                Console.WriteLine("\n===========================================\n");

                Console.WriteLine($"Segment: {segmentTime} ms");
                Console.WriteLine($"Lookup : {lookupTime} ms");
                Console.WriteLine($"Logging: {loggingTime} ms");
                Console.WriteLine($"Process: {stopwatch.ElapsedMilliseconds} ms");

                Console.WriteLine("\n===========================================\n");


                Program.logger_event.Log($"{Emoji.Check} | Characters : {text.Length.ToString()} | Unique Kanji: {kanjiCount} | Unique Words: {wordCount} | Process Time: {loggingTime}");

            });

        }

        public static void RunInConsole(bool init = false, bool verbose = false)
        {
            // This method is where the main logic of your program will run.
            // You can add your code here to perform tasks, initialize components, etc.



            if (!Console.IsOutputRedirected)
            {
                //try { Console.Clear(); } catch { /* Ignore */ }
            }

            Console.WriteLine((init) ? "\n====================================== NEW SEARCH ================================================\n" : "");

            userInput = GetUserInput("\nEnter a Kanji to lookup: ");

            if (string.IsNullOrWhiteSpace(userInput)) return;
            if (userInput == "-close")
            {
                ConsoleHelper.HideConsole();
                return;
            }
            else if (userInput == "-clear")
            {
                Console.Clear();
                RunInConsole(init);
            }
            else if (userInput == "-help")
            {
                Console.WriteLine("\n======================= HELP =======================\n");
                Console.WriteLine("Type '_exit' to close the console.");
                Console.WriteLine("Type '_clear' to clear the console.");
                Console.WriteLine("Type '_help' to see this help message again.");
                RunInConsole(init);
            } else
            {
                LookupKanji(userInput);
                LookupKotoba(userInput, verbose);
                LookupKanjiByAPI(userInput);
            }

                

            RunInConsole();
            // For example, you could load data, initialize UI components, etc.
            // Add your code here as needed.
        }

        static List<KanjiEntry> LookupKanji(string input, bool quiet=false)
        {

            return new List<KanjiEntry>();
            /*
            Console.WriteLine("\n================ KanjiDict ================\n");
            

            List<KanjidicEntry> results = new List<KanjidicEntry>();

            foreach (char kanji in input)
            {
                var res = _kanjiDictionary.SearchByKanji(kanji);
                if (res != null)
                {
                    if (!quiet)
                    {
                        Console.WriteLine("____________________________________________\n");
                        Console.WriteLine($"Kanji: {res.Literal}");
                        Console.WriteLine($"Readings: {string.Join(", ", res.Readings)}");
                        Console.WriteLine($"Meaning(s): {string.Join(", ", res.Meanings)}");
                        Console.WriteLine($"Strokes: {res.StrokeCount}");
                        Console.WriteLine($"Grade: {res.Grade}");
                        Console.WriteLine($"JLPT: {res.JLPT}");
                    }

                    results.Add(res);
                }
                else
                {
                    try
                    {
                        if (!quiet)
                        {
                            Console.WriteLine("____________________________________________\n");
                            Console.WriteLine($"Kanji: {kanji.ToString()} / No Kanji matches found.");
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            return results;
            */
        }
        

        static List<string> LookupKotoba(string input="", bool verbose = false)
        {
            
            
            Console.WriteLine("\n================== EDict2 =================\n");

            var edictResults = _edictDictionary.LookupDefinitionsBasic(input);

            if (edictResults.Count > 0)
            {
                foreach (var res in edictResults)
                {
                    Console.WriteLine(res + "\n");
                }
            }
            else
            {
                Console.WriteLine("No matches found.");
            }

            return edictResults;
            
            
        }

        async static void LookupKanjiByAPI(string input)
        {

            Console.WriteLine("\n============== KanjiDict2 API ==============\n");
            Console.WriteLine("API is Processing asynchronously.... Waiting...");

            foreach (char kanji in input)
            {
                KanjiData result = await _kanjiAPIClient.GetKanjiDataAsync(kanji.ToString());

                if (result != null)
                {
                    try
                    {
                        Console.WriteLine("____________________________________________\n");
                        Console.WriteLine($"Kanji: {result.Kanji}");
                        Console.WriteLine($"Meanings: {string.Join(", ", result.Meanings)}");
                        Console.WriteLine($"Kunyomi: {string.Join(", ", result.Kun_readings)}");
                        Console.WriteLine($"Onyomi: {string.Join(", ", result.On_readings)}");
                    }
                    catch (Exception ex)
                    {
                        return;

                    }
                }
                else
                {
                    Console.WriteLine("No matches found.");
                }
            }

        }
    }
}
