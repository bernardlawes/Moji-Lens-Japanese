# 🇯🇵 Japanese Flashcard Learning App

A fast, lightweight C# application designed to help learners build and retain Japanese vocabulary using flashcards, spaced repetition, and dictionary lookups.

## ✨ Features

- 🔤 **Kanji, Kana, and English Support**  
  Practice recognition and recall across multiple writing systems.

- 🧠 **Spaced Repetition Algorithm**  
  Boost retention using an intelligent scheduling system inspired by Anki/SRS techniques.

- 📘 **Integrated Dictionary Lookup**  
  Uses parsed data from JMdict/EDICT2 to support word definitions, readings, and kanji breakdowns.

- 📊 **Progress Tracking**  
  Track your learning stats over time and visualize mastery levels.

- 📁 **Local-First Design**  
  Fully functional offline; stores flashcards and progress using SQLite for speed and simplicity.

## 🏗️ Tech Stack

- **Language**: C# (.NET 6 or later)
- **Database**: SQLite
- **UI Framework**: WinForms (or WPF, depending on final implementation)
- **Dictionary Data**: [JMdict/EDICT2](https://www.edrdg.org/wiki/index.php/JMdict-EDICT_Dictionary_Project)

## 🚧 Current Modules

- `FlashcardEngine.cs` — Manages review scheduling and user interactions.
- `DictionaryParser.cs` — Parses and caches JMdict or EDICT2 entries.
- `DataStore.cs` — Interfaces with SQLite for card progress, user data, and word lists.
- `MainForm.cs` — Primary WinForms UI for card review and lookup.

## 📦 Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/JapaneseFlashcards.git
