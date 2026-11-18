using System.Text.RegularExpressions;
namespace DesktopSearch.Core;
public record SearchHit(string Path,string Title,double Score,string Snippet);
public sealed class Indexer {
 private sealed record Document(string Path,string Text,Dictionary<string,int> Counts);
 private readonly List<Document> documents=new();
 private readonly Dictionary<string,int> documentFrequency=new();
 private static readonly Regex Tokens=new(@"[\p{L}\p{N}]{2,}",RegexOptions.Compiled);
 public int Count=>documents.Count;
 public static string[] Tokenize(string text)=>Tokens.Matches(text.ToLowerInvariant()).Select(x=>x.Value).ToArray();
 public void Add(string path,string content){
  var counts=Tokenize(content).GroupBy(x=>x).ToDictionary(g=>g.Key,g=>g.Count());
  documents.Add(new Document(path,content,counts));
  foreach(var word in counts.Keys)documentFrequency[word]=documentFrequency.GetValueOrDefault(word)+1;
 }
 public void Clear(){documents.Clear();documentFrequency.Clear();}
 public int IndexDirectory(string root){
  if(!Directory.Exists(root))throw new DirectoryNotFoundException(root);
  Clear();
  foreach(var path in Directory.EnumerateFiles(root,"*",SearchOption.AllDirectories)){
   if(!new[]{".txt",".md",".csv"}.Contains(Path.GetExtension(path),StringComparer.OrdinalIgnoreCase))continue;
   try{
    var info=new FileInfo(path);if(info.Length>2_000_000)continue;
    Add(path,File.ReadAllText(path));
   }catch(IOException){}catch(UnauthorizedAccessException){}
  }
  return Count;
 }
 public IReadOnlyList<SearchHit> Search(string query,int limit=30){
  var terms=Tokenize(query).Distinct().ToArray();
  if(terms.Length==0)return Array.Empty<SearchHit>();
  var results=new List<SearchHit>();
  foreach(var doc in documents){
   double score=0;
   foreach(var t in terms){
    if(!doc.Counts.TryGetValue(t,out int tf))continue;
    double idf=Math.Log(1+(documents.Count-documentFrequency[t]+.5)/(documentFrequency[t]+.5));
    score+=tf/(tf+1.5)*idf;
   }
   if(score<=0)continue;
   int match=terms.Select(t=>doc.Text.IndexOf(t,StringComparison.OrdinalIgnoreCase)).Where(i=>i>=0).DefaultIfEmpty(0).Min();
   int start=Math.Max(0,match-55),length=Math.Min(190,doc.Text.Length-start);
   results.Add(new SearchHit(doc.Path,System.IO.Path.GetFileName(doc.Path),score,doc.Text.Substring(start,length).Replace("\r"," ").Replace("\n"," ")));
  }
  return results.OrderByDescending(r=>r.Score).Take(Math.Clamp(limit,1,100)).ToArray();
 }
}