using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AvaloniaToDoList.Models;

namespace AvaloniaToDoList.Services;

public static class ToDoListFileService
{
    // A hard coded filepath. Realistically this should be configurable but that is apparently beyond the scope of the tutorial I am following
    private static string _jsonFileName =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "AvaloniaToDoList", "MyToDoList.txt");

    /// <summary>
    /// Stores the given items into a file on the disc
    /// </summary>
    /// <param name="itemsToSave">the items to save</param>
    public static async Task SaveFileToAsync(IEnumerable<ToDoItem> itemsToSave)
    {
        // Ensure directories exist
        Directory.CreateDirectory(Path.GetDirectoryName(_jsonFileName)!);
        
        // Use filestream to write items to long term storage
        using (var toDoListSaver = File.Create(_jsonFileName))
        {
            await JsonSerializer.SerializeAsync(toDoListSaver, itemsToSave);
        }
    }

    /// <summary>
    /// Loads the file from the long term storage and returns the items stored inside
    /// </summary>
    /// <returns></returns>
    public static async Task<IEnumerable<ToDoItem>?> LoadFromFileAsync()
    {
        try
        {
            // We try to read the saved file and return the ToDoItemsList if successful
            using (var toDoListLoader = File.OpenRead(_jsonFileName))
            {
                return await JsonSerializer.DeserializeAsync<List<ToDoItem>>(toDoListLoader);
            }
        }
        catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
        {
            // in the case the file wasn't found, return null
            return null;
        }
    }
}