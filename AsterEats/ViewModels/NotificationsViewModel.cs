using System.Collections.ObjectModel;
using System.Windows.Input;
using AsterEats.Models;
using AsterEats.Services;

namespace AsterEats.ViewModels;

public class NotificationsViewModel : BaseViewModel
{
    private readonly MockDataService _dataService;

    public ObservableCollection<Notification> Notifications { get; } = new();

    public ICommand MarkAllReadCommand { get; }

    public int UnreadNotificationCount =>
        _dataService.GetUnreadNotificationCount();

    public NotificationsViewModel(MockDataService dataService)
    {
        _dataService = dataService;

        MarkAllReadCommand = new Command(MarkAllRead);

        LoadNotifications();
    }

    public void LoadNotifications()
    {
        Notifications.Clear();

        foreach (var notification in _dataService.GetNotifications())
        {
            Notifications.Add(notification);
        }

        OnPropertyChanged(nameof(UnreadNotificationCount));
    }

    private void MarkAllRead()
    {
        _dataService.MarkAllNotificationsAsRead();

        LoadNotifications();
    }
}