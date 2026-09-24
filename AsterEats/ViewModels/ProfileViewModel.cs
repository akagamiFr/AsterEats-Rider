using System.Windows.Input;
using AsterEats.Models;
using AsterEats.Services;

namespace AsterEats.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    private readonly MockDataService _dataService;

    public Rider Rider { get; }

    public int UnreadNotificationCount =>
        _dataService.GetUnreadNotificationCount();

    public ICommand NotificationsCommand { get; }

    public ProfileViewModel(MockDataService dataService)
    {
        _dataService = dataService;

        Rider = _dataService.GetCurrentRider();

        NotificationsCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("notifications");
        });
    }
}