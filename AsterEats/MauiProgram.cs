using AsterEats.Services;
using AsterEats.ViewModels;
using AsterEats.Views;
using FFImageLoading.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AsterEats;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // =====================================================
        // MAUI APP
        // =====================================================

        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseFFImageLoading();


#if ANDROID

        // =====================================================
        // ANDROID STATUS BAR
        // =====================================================

        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddAndroid(android =>
            {
                // Apply after Android activity creation is complete.
                android.OnPostCreate(
                    (activity, bundle) =>
                    {
                        ConfigureTransparentStatusBar(activity);
                    });

                // Re-apply when the activity becomes active.
                android.OnResume(
                    activity =>
                    {
                        ConfigureTransparentStatusBar(activity);
                    });
            });
        });

#endif


        // =====================================================
        // SERVICES
        // =====================================================

        builder.Services.AddSingleton<MockDataService>();


        // =====================================================
        // VIEW MODELS
        // =====================================================

        builder.Services.AddTransient<LoginViewModel>();

        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<DeliveriesViewModel>();
        builder.Services.AddSingleton<EarningsViewModel>();
        builder.Services.AddSingleton<ProfileViewModel>();

        builder.Services.AddTransient<DeliveryRequestViewModel>();
        builder.Services.AddTransient<OrderDetailsViewModel>();
        builder.Services.AddTransient<ActiveDeliveryViewModel>();
        builder.Services.AddTransient<NotificationsViewModel>();


        // =====================================================
        // PAGES
        // =====================================================

        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<DeliveriesPage>();
        builder.Services.AddSingleton<EarningsPage>();
        builder.Services.AddSingleton<ProfilePage>();

        
        builder.Services.AddTransient<OrderDetailsPage>();
        builder.Services.AddTransient<ActiveDeliveryPage>();
        builder.Services.AddTransient<NotificationsPage>();


        // =====================================================
        // DEBUG LOGGING
        // =====================================================

#if DEBUG
        builder.Logging.AddDebug();
#endif


        return builder.Build();
    }


#if ANDROID

    // =========================================================
    // TRANSPARENT STATUS BAR
    // =========================================================

    private static void ConfigureTransparentStatusBar(
        Android.App.Activity activity)
    {
        var window = activity.Window;

        // Allow the window to draw the system-bar background.
        window.AddFlags(
            Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);

        // Remove Android's translucent status mode.
        window.ClearFlags(
            Android.Views.WindowManagerFlags.TranslucentStatus);

        // Let app content extend behind the status bar.
        window.SetFlags(
            Android.Views.WindowManagerFlags.LayoutNoLimits,
            Android.Views.WindowManagerFlags.LayoutNoLimits);

        // Fully transparent status bar.
        window.SetStatusBarColor(
            Android.Graphics.Color.Transparent);

        // Keep time, battery, Wi-Fi and other status-bar
        // indicators visible with dark icons.
        window.DecorView.SystemUiFlags =
            Android.Views.SystemUiFlags.LayoutStable |
            Android.Views.SystemUiFlags.LayoutFullscreen |
            Android.Views.SystemUiFlags.LightStatusBar;
    }

#endif
}