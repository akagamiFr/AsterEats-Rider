using AsterEats.Services;
using AsterEats.ViewModels;
using AsterEats.Views;
using FFImageLoading.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AsterEats;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseFFImageLoading();

        builder.Services.AddSingleton<MockDataService>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<HomeViewModel>();

        builder.Services.AddTransient<DeliveryRequestViewModel>();
        builder.Services.AddTransient<OrderDetailsViewModel>();
        builder.Services.AddTransient<ActiveDeliveryViewModel>();

        builder.Services.AddTransient<DeliveriesViewModel>();
        builder.Services.AddTransient<EarningsViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<NotificationsViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<HomePage>();

        builder.Services.AddTransient<DeliveryRequestPage>();
        builder.Services.AddTransient<OrderDetailsPage>();
        builder.Services.AddTransient<ActiveDeliveryPage>();

        builder.Services.AddTransient<DeliveriesPage>();
        builder.Services.AddTransient<EarningsPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<NotificationsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}