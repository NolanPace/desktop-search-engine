using DesktopSearch.Core;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
namespace DesktopSearch;
public partial class MainWindow:Window {
 private readonly Indexer indexer=new();
 public MainWindow(){InitializeComponent();}
 private void Browse_Click(object sender,RoutedEventArgs e){
  var dialog=new OpenFolderDialog{Title="Choose a folder of text documents"};
  if(dialog.ShowDialog()==true){try{Folder.Text=dialog.FolderName;int count=indexer.IndexDirectory(dialog.FolderName);Status.Text=$"{count} documents indexed";Search();}catch(Exception ex){Status.Text=ex.Message;}}
 }
 private void Search_Click(object sender,RoutedEventArgs e)=>Search();
 private void Query_KeyUp(object sender,KeyEventArgs e){if(e.Key==Key.Enter)Search();}
 private void Search(){Results.ItemsSource=indexer.Search(Query.Text);Status.Text=$"{indexer.Count} indexed";Preview.Text="";}
 private void Results_SelectionChanged(object sender,SelectionChangedEventArgs e){
  if(Results.SelectedItem is SearchHit hit)Preview.Text=$"{hit.Path}\n\nScore: {hit.Score:F3}\n\n{hit.Snippet}";
 }
}