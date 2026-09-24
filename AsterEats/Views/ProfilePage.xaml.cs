using AsterEats.ViewModels;

namespace AsterEats.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}