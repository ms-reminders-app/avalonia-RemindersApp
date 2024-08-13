using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using ReactiveUI;
using RemindersApp.Views;

namespace RemindersApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public static string Greeting => "Welcome to Avalonia!";

    public ICommand OpenWindowCommand { get; }

    private PixelPoint? WindowPosition { get; set; }

    private ReminderWindow? _reminderWindow;
    
    public MainWindowViewModel()
    {
        OpenWindowCommand = ReactiveCommand.Create(OpenWindow);
        Init();
    }

    private async Task Init()
    {
        await PeriodicAsync(OpenWindow, TimeSpan.FromSeconds(2));
    }
    

    private async Task PeriodicAsync(Func<Task> action, TimeSpan interval,
        CancellationToken cancellationToken = default)
    {
        using var timer = new PeriodicTimer(interval);
        while (true)
        {
            await action();
            await timer.WaitForNextTickAsync(cancellationToken);
        }
    }

    private async Task OpenWindow()
    {
        var reminderWindowVm = new ReminderWindowViewModel();

        var randomNumber = Random.Shared.Next();
        var probability = decimal.Divide(randomNumber, int.MaxValue);

        if (probability < 0.5M)
        {
            return;
        }
        
        // await Task.Delay(TimeSpan.FromSeconds(1));

        _reminderWindow?.Close();

        _reminderWindow = new ReminderWindow() { DataContext = reminderWindowVm };

        Console.WriteLine($"{DateTime.Now} - Showing Window");
        _reminderWindow.Show();
        Console.WriteLine($"{DateTime.Now} - Showed Window");


        if (WindowPosition != null)
        {
            _reminderWindow.Position = (PixelPoint)WindowPosition;
        }
        
        _reminderWindow.Closing += (sender, args) =>
        {
            Console.WriteLine($"Closing - Window Position is ({_reminderWindow.Position.X}, {_reminderWindow.Position.Y})");
            WindowPosition = _reminderWindow.Position;
        };
    }
}