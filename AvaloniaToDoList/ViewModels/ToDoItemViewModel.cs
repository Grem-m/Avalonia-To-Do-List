using AvaloniaToDoList.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaToDoList.ViewModels;

/// <summary>
/// This is a ViewModel which represents a <see cref="Models.ToDoItem"/>
/// </summary>
public partial class ToDoItemViewModel : ViewModelBase
{
    /// <summary>
    /// Creates a new blank ToDoItemViewModel
    /// </summary>
    public ToDoItemViewModel()
    {
        //empty boi
    }

    public ToDoItemViewModel(ToDoItem item)
    {
        //init the properties with the given values
        IsChecked = item.IsChecked;
        Content = item.Content;
    }

    /// <summary>
    /// Gets or sets the checked status of each item
    /// </summary>
    private bool _isChecked;

    public bool IsChecked
    {
        get { return _isChecked; }
        set { SetProperty(ref _isChecked, value); }
    }

    /// <summary>
    /// Gets or sets the content of the to-do item
    /// </summary>
    [ObservableProperty] 
    private string? _content;

    /// <summary>
    /// Gets a ToDoItem of this ViewModel
    /// </summary>
    /// <returns>The ToDoItem</returns>
    public ToDoItem GetToDoItem()
    {
        return new ToDoItem()
        {
            IsChecked = this.IsChecked,
            Content = this.Content
        };
    }

}