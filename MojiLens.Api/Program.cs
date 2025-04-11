﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using csharp_dictionary_japanese;


string libFolderPath = @"D:\Data\Dictionary\Japanese\";
string jmdict_m_filename = @"JMdict.xml";
string jmdict_e_filename = @"JMdict_e.xml";
string jmdict_Path = System.IO.Path.Combine(libFolderPath, jmdict_e_filename);

// assuming JMDict_Reader and models are in your solution
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var reader = new JMDict_Reader(jmdict_Path, false);
reader.LoadDictionary(boolHashLookup: false);




// JSON settings for cleaner output
var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = false,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    //Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};

app.MapGet("/", () => "JMdict API is running 🚀");

// GET /lookup?word=猫
app.MapGet("/lookup", (string word) =>
{
    if (reader.Records.TryGetValue(word, out var entry))
        return Results.Json(entry, jsonOptions);

    return Results.NotFound();
});

// GET /search?query=化&kanjiOnly=true
app.MapGet("/search", (string query, bool kanjiOnly = false) =>
{
    var results = reader.SearchWithScoring(query, kanjiOnly)
        .Select(r => new
        {
            score = r.Score,
            entSeq = r.Entry.EntSeq,
            kebs = r.Entry.KanjiElements.Select(k => k.Keb),
            rebs = r.Entry.ReadingElements.Select(r => r.Reb),
            glosses = r.Entry.Senses.SelectMany(s => s.Glosses).Where(g => g.Language == "eng").Select(g => g.Text)
        });

    return Results.Json(results, jsonOptions);
});

// Optional: GET /entry/123456
app.MapGet("/entry/{entSeq}", (int entSeq) =>
{
    var match = reader.Records.Values.FirstOrDefault(e => e.EntSeq == entSeq);
    return match != null ? Results.Json(match, jsonOptions) : Results.NotFound();
});

app.Run();
