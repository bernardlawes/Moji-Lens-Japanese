using csharp_dictionary_japanese;
using csharp_peripherals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using csharp_journal;
using Microsoft.AspNetCore.Builder;
using System.Text.Json;


using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace MainProject
{
    class Utilities
    {
        // Instantiate the object with user input
        static KanjiDict2_Reader _kanjiDictionary;
        static JMDict_Reader _jmeDictionary;
        static EDict2_Reader _edictDictionary;
        static Kanji_API_Client _kanjiAPIClient;
        static ChronoLogger _logger_kanji;
        
        static bool useHashLookup = false;
        static string userInput = string.Empty;


        public static void API_Start()
        {
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            var reader = new JMDict_Reader(Program.jmdict_Path, false);
            reader.LoadDictionary(false);

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                //Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            app.MapGet("/lookup", (string word) =>
            {
                return reader.Records.TryGetValue(word, out var entry)
                    ? Results.Json(entry, jsonOptions)
                    : Results.NotFound();
            });

            app.Run();
        }

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

        public static bool InitializeDictionaryAndTools(string kanjiPath,  string edictPath, string jmdict_Path)
        {


            if (!ValidateDictionaryFiles(kanjiPath, edictPath, jmdict_Path)) return false;

            Program.logger_event.Log($"{Emoji.Clock} | Dictionaries Files Found" );

            _logger_kanji = new ChronoLogger(Path.Combine(Program.logsPath,"kanji.log"));

            string kanjifilename = System.IO.Path.GetFileName(kanjiPath);
            // Load the kanjidic2.xml file
            _kanjiDictionary = new KanjiDict2_Reader(kanjiPath);
            string msg = (_kanjiDictionary.RecordCount == 0) ? $"Error: No records found in the {kanjifilename} file." : $"Loaded {_kanjiDictionary.RecordCount} entries from the {kanjifilename} file.";
            if (_kanjiDictionary.RecordCount == 0) return false;
            Program.logger_event.Log($"{Emoji.Clock} | {msg}");
            Console.WriteLine(msg);

            // Load the JMDict.xml file
            if (File.Exists(jmdict_Path))
            {
                string jmdict_filename = System.IO.Path.GetFileName(jmdict_Path);
                _jmeDictionary = new JMDict_Reader(jmdict_Path, useHashLookup);
                msg = (_jmeDictionary.RecordCount == 0) ? $"Error: No records found in the {jmdict_filename} file." : $"JMDict Records Loaded: {_jmeDictionary.RecordCount} entries.";
                if (_jmeDictionary.RecordCount == 0) return false;
                Program.logger_event.Log($"{Emoji.Clock} | {msg}");
                Console.WriteLine(msg);
            }
            else
            {
                Console.WriteLine($"JMDict file not found at {jmdict_Path}");
            }
            
            
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

        static bool ValidateDictionaryFiles(string kanjiPath, string edictPath, string jmdictPath)
        {
            string kanjifilename = System.IO.Path.GetFileName(kanjiPath);
            // Check if the kanjidic file exists
            Console.WriteLine(File.Exists(kanjiPath) ? $"{kanjifilename} Dictionary Found." : $"\n{kanjifilename} file not found at {kanjiPath}");
            if (!File.Exists(kanjiPath)) return false;

            if (!string.IsNullOrWhiteSpace(jmdictPath))
            {
                string jmdictfilename = System.IO.Path.GetFileName(jmdictPath);
                // Check if the edict file exists
                Console.WriteLine(File.Exists(edictPath) ? $"{jmdictfilename} Dictionary Found." : $"\n{jmdictfilename} file not found at {jmdictPath}");
                if (!File.Exists(jmdictPath)) return false;
            }
            

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

            ClipboardMonitorHost.Start(text =>
            {
                Console.Clear();
                //if (Program.DebugMode)  Console.WriteLine($"Search Item: {text}");

                var stopwatch = Stopwatch.StartNew();
                long segmentTime = 0, jm_segmentandLookupTime=0;
                long lookupTime = 0, jm_lookupTime = 0;
                long loggingTime = 0,jm_loggingTime = 0;

                int kanjiCount = 0, wordCount = 0;

                var kanjis = new List<KanjiEntry>();
                var definitions = new List<string>();
                var jm_definitions = new List<JMDictEntry>();


                if (!string.IsNullOrWhiteSpace(text))
                {
                    var kanjisegments = _kanjiDictionary.SegmentKanji(text);
                    kanjis = _kanjiDictionary.LookupUniqueKanjiDefinitions(kanjisegments);
                    if (kanjis != null && kanjis.Count > 0)
                    {
                        foreach (KanjiEntry res in kanjis)
                        {
                            _logger_kanji.Log($"{Emoji.Info} {res.Literal} | {string.Join(", ", res.Readings)} | {string.Join(", ", res.Meanings)} | Grade: {res.Grade} | JLPT: {res.JLPT} | Strokes: {res.StrokeCount}"); // <-- 💾 this line saves to file
                            kanjiCount++;
                        }

                    }

                    Console.WriteLine("\n===========================================\n");

                    Console.WriteLine($"Characters    : {text.Length.ToString()}");
                    Console.WriteLine($"Kanji Count   : {kanjisegments.Count.ToString()}");
                    // Console.WriteLine($"Defined Kanji : {kanjis.Count.ToString()}");

                    Console.WriteLine("\n================= BY DIRECT ORIGINAL English Only, Only Words with Kanji ==========================\n");

                    List<(string, JMDictEntry)> results = null;
                    Dictionary<string, JMDictEntry> dict = null;

                    // List of matched segments and entries (no duplicates, English only)
                    var s3 = Stopwatch.StartNew();
                    if (_jmeDictionary != null) results = _jmeDictionary.GetEntriesFor(text,true,true,false,true,true) as List<(string Segment, JMDictEntry Entry)>;
                    int s3time = (int)s3.ElapsedMilliseconds;

                    if (_jmeDictionary != null)
                    {
                        foreach (var (segment, entry) in results)
                        {
                            var glosses = entry.Senses.SelectMany(s => s.Glosses).Select(g => g.Text).ToList();
                            var formatted = FormatEntry(entry, segment);
                            //Console.WriteLine(formatted);
                            // Do something with the English glosses
                        }
                    }

                    // Dictionary of segment → entry ---> make this an option, because it's really not needed here
                    //if (_jmeDictionary != null) dict = _jmeDictionary.GetEntriesFor(text, asDictionary: true) as Dictionary<string, JMDictEntry>;

                    // Search with Scoring
                    var matches = _jmeDictionary.SearchWithScoring("がく", kanjiOnly: false);
                    foreach (var match in matches)
                    {
                        Console.WriteLine($"Score: {match.Score} — {string.Join("/", match.Entry.KanjiElements.Select(k => k.Keb))}");
                    }


                    Console.WriteLine("\n================== BY DIRECT NEW (Only words with Kanji) =========================\n");

                    var s = Stopwatch.StartNew();
                    if (_jmeDictionary != null) results = _jmeDictionary.SegmentAndLookupDirect(text, onlyWithKanji: true, allowPartialFallback: true);
                    int stime = (int)s.ElapsedMilliseconds;

                    if (_jmeDictionary != null)
                    {
                        foreach (var (segment, entry) in results)
                        {
                            var glosses = entry.Senses.SelectMany(s => s.Glosses).Select(g => g.Text).ToList();
                            var formatted = FormatEntry(entry, segment);
                            //Console.WriteLine(formatted);
                            // Do something with the English glosses
                        }
                    }

                    Console.WriteLine("\n================== BY DIRECT OLD (All characters incl single kana) =========================\n");

                    var s0 = Stopwatch.StartNew();
                    if (_jmeDictionary != null) results = _jmeDictionary.GetEntriesFor(text, distinct: true, englishOnly: true,onlyWithKanji:true) as List<(string Segment, JMDictEntry Entry)>;
                    int s0time = (int)s0.ElapsedMilliseconds;

                    if (_jmeDictionary != null)
                        foreach (var (segment, entry) in results)
                        {
                            var glosses = entry.Senses.SelectMany(s => s.Glosses).Select(g => g.Text).ToList();
                            var formatted = FormatEntry(entry, segment);
                            //Console.WriteLine(formatted);
                            // Do something with the English glosses
                        }


                    if (_jmeDictionary != null)
                    { 
                        if (useHashLookup)
                        {

                            Console.WriteLine("\n================== BY HASH =========================\n");

                            var s1 = Stopwatch.StartNew();
                            if (_jmeDictionary != null) results = _jmeDictionary.GetEntriesForByHash(text, distinct: true, englishOnly: true) as List<(string Segment, JMDictEntry Entry)>;
                            int s1time = (int)s1.ElapsedMilliseconds;

                            foreach (var (segment, entry) in results)
                            {
                                var glosses = entry.Senses.SelectMany(s => s.Glosses).Select(g => g.Text).ToList();
                                var formatted = FormatEntry(entry, segment);
                                //Console.WriteLine(formatted);
                                // Do something with the English glosses
                            }
                            Console.WriteLine($"\nWordLookup Hash Time: {s1time} ms");
                        }
                    }

                    Console.WriteLine($"\nDirect English Only: {s3time} ms");
                    Console.WriteLine($"Words with Kanji Only: {stime} ms");
                    Console.WriteLine($"Direct Old - all kana: {s0time} ms");


                    //jm_definitions = _jmeDictionary.LookupSegments(jm_segments, false);
                    stopwatch = Stopwatch.StartNew();
                    var segments    = _edictDictionary.SegmentJapanese(text, false);

                    segmentTime = stopwatch.ElapsedMilliseconds;

                    definitions = _edictDictionary.LookupDefinitions(segments, false);

                    lookupTime = stopwatch.ElapsedMilliseconds;

                    Console.WriteLine("\n===========================================\n");

                    foreach (string def in definitions)
                    {
                        _logger_kanji.Log($"{Emoji.Fire} {def}");   // <-- 💾 this line saves to file
                        wordCount++;
                    }

                    loggingTime = stopwatch.ElapsedMilliseconds;

                    foreach (string def in definitions)
                    {
                        _logger_kanji.Log($"{Emoji.Fire} {def}");   // <-- 💾 this line saves to file
                        wordCount++;
                    }

                    loggingTime = stopwatch.ElapsedMilliseconds;

                } else
                {
                    Console.WriteLine("Input term cannot be null or whitespace. Please try again.");
                }

                stopwatch.Stop();
                
                Console.WriteLine($"Unique Kanji: {kanjiCount}\nUnique Words: {wordCount}");

                Console.WriteLine("\n===========================================\n");

                Console.WriteLine($"Segment: {segmentTime} ms");
                Console.WriteLine($"Lookup : {lookupTime}  ms");
                Console.WriteLine($"Logging: {loggingTime} ms");
                Console.WriteLine($"Process: {stopwatch.ElapsedMilliseconds} ms");

                Console.WriteLine("\n===========================================\n");
                

                Program.logger_event.Log($"{Emoji.Check} | Characters : {text.Length.ToString()} | Unique Kanji: {kanjiCount} | Unique Words: {wordCount} | Process Time: {loggingTime}");


                /*
                if (Program.DebugMode)
                {
                    if (kanjis != null && kanjis.Count > 0)
                    {
                        Console.WriteLine("\n================= KANJI ===================\n");
                        foreach (KanjiEntry res in kanjis)  { Console.WriteLine($"{res.Literal} : {string.Join(", ", res.Meanings)}"); }
                        Console.WriteLine("\n================= WORDS ===================\n");
                        foreach (string def in definitions) { Console.WriteLine($"{def}"); }
                    }
                    // Determine the single to compound ratio : hint 
                    // Console.WriteLine($"Single Kanjis: {singles} \n Kanji Words: {compounds}");
                }
                */

            });

        }

        public static string FormatEntry(JMDictEntry entry, string? segmentOverride = null)
        {
            var kanji = entry.KanjiElements.Select(k => k.Keb).ToList();
            var readings = entry.ReadingElements.Select(r => r.Reb).ToList();
            var glosses = entry.Senses
                .SelectMany(s => s.Glosses)
                .Where(g => g.Language == "eng")
                .Select(g => g.Text)
                .Distinct()
                .ToList();

            string headword = segmentOverride ?? (kanji.FirstOrDefault() ?? readings.FirstOrDefault() ?? "???");
            string readingText = readings.Count > 0 ? $"（{string.Join("・", readings)}）" : "";
            string glossText = "- " + string.Join("\n- ", glosses);

            return $"{headword}{readingText}\n{glossText}";
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
