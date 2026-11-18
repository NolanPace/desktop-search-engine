# Desktop Search Engine

A **C# / .NET 8** local-file search engine with a Windows **WPF** graphical interface. Index UTF-8 text/Markdown/CSV files from a chosen directory, search using token matching with TF-IDF-inspired scoring, and preview matching excerpts.

## Run

On Windows with the .NET 8 SDK:
```powershell
dotnet run --project desktop/DesktopSearch.csproj
```
The GUI indexes `.txt`, `.md`, and `.csv` documents recursively from an operator-selected directory. No files are uploaded; processing happens locally.

Cross-platform test of the indexing engine:
```bash
dotnet run --project tests/SearchEngine.Tests.csproj
```

**Safety:** This is a local demo and reads only operator-selected paths. Do not index confidential directories unless you are authorized. Very large files are skipped (2 MB each); binary files are not indexed.

MIT licensed.
