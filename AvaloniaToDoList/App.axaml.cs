using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using AvaloniaToDoList.Services;
using AvaloniaToDoList.ViewModels;
using AvaloniaToDoList.Views;

namespace AvaloniaToDoList;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    // This is a reference to the MainWindowViewModel which is used to save the list on shutdown. Dependency Injection could also be used (if I knew what that was)
    private readonly MainWindowViewModel _mainWindowViewModel = new MainWindowViewModel();

    public override async void OnFrameworkInitializationCompleted()
    {
        // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
        // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
        DisableAvaloniaDataAnnotationValidation();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _mainWindowViewModel // Changed to private reference
            };
            
            // Listen to the ShotDownRequested-event
            desktop.ShutdownRequested += DesktopOnShutdownRequested;
        }

        base.OnFrameworkInitializationCompleted();
        
        // init the MainViewModel
        await InitMainViewModelAsync();
    }
    
    // We want to save our ToDoList before we actually shut down the App. As file I/O is async, we need to wait until file is closed 
    // before we can actually close this window

    private bool _canClose; // this flag is used to check if window is allowed to close

    private async void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        e.Cancel  = !_canClose; // Cancel closing even the first time

        if (!_canClose)
        {
            // To save the items, we map them to the ToDoItem-Model which is better suited for I/O operations
            var itemsToSave = _mainWindowViewModel.ToDoItems.Select(item => item.GetToDoItem());

            await ToDoListFileService.SaveFileToAsync(itemsToSave);
            
            // Set _canClose to true and Close this window again
            _canClose = true;
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
    
    // Load data from disc
    private async Task InitMainViewModelAsync()
    {
        // get the items to load
        var itemsLoaded = await ToDoListFileService.LoadFromFileAsync();

        if (itemsLoaded is not null)
        {
            foreach (var item in itemsLoaded)
            {
                _mainWindowViewModel.ToDoItems.Add(new ToDoItemViewModel(item));
            }
        }
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}