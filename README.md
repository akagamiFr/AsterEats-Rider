# AsterEats — Rider App (Phase 1)

A **delivery rider** application prototype (not a customer ordering app), built with
.NET 9 MAUI, using the AsterTrek visual identity. This is Phase 1 of a 4‑phase build.

## What's in Phase 1

```
Solution structure, Models, AsterTrek theme, Shell navigation,
MockDataService, Login page, Rider Dashboard (Home page)
```

Rider taps **Log In** → lands on the **Dashboard**, which shows:

- Greeting + star rating, both read from a `Rider` object (nothing hardcoded)
- A **GO ONLINE / GO OFFLINE** toggle bound to `Rider.IsOnline`
- Today's delivery count & earnings, computed from a list of `Earning` records
- A "Current Delivery" card (empty placeholder for now — wired up in Phase 2)

## Project layout

```
AsterEats.sln
AsterEats/                      .NET 9 MAUI app
├── Models/                     Rider, Restaurant, DeliveryOrder, OrderItem, Earning, Notification
├── Services/                   MockDataService (in-memory "backend" for now)
├── ViewModels/                 BaseViewModel, LoginViewModel, HomeViewModel
├── Views/                      LoginPage, HomePage
├── Converters/                 InverseBoolConverter
├── Resources/Styles/           Colors.xaml, Styles.xaml (AsterTrek theme)
├── Resources/Images/           appicon.svg, appicon_fg.svg, splash.svg, icon_home.svg
├── App.xaml / App.xaml.cs
├── AppShell.xaml / AppShell.xaml.cs
└── MauiProgram.cs              DI registrations
```

## How to run it (Visual Studio 2022, 17.9+)

1. Install the **.NET Multi-platform App UI development** workload
   (Visual Studio Installer → Modify → check ".NET MAUI").
2. Open `AsterEats.sln`.
3. Set the startup project to `AsterEats`.
4. Pick a target: **Windows Machine** is the fastest way to see it running while
   developing; Android Emulator works too once an emulator image is installed.
5. Press **F5**.
6. On the Login page, type anything into both fields and tap **LOG IN** — this
   is a prototype login, it doesn't check against a real account yet.
7. On the Dashboard, tap **GO ONLINE** and watch the button and status dot change
   colour — that's the `Rider.IsOnline` binding at work, not hardcoded text.

### Command line, if you prefer

```bash
dotnet workload install maui
dotnet build AsterEats/AsterEats.csproj -f net9.0-windows10.0.19041.0
```//)
(swap the `-f` target for `net9.0-android` / `net9.0-ios` / `net9.0-maccatalyst` as needed)

## Required NuGet packages (Phase 1)

Already referenced in `AsterEats.csproj` — Visual Studio restores these automatically:

- `Microsoft.Maui.Controls` (9.0.0)
- `Microsoft.Extensions.Logging.Debug` (9.0.0)

No third-party MVVM package is used on purpose — `BaseViewModel`/`ObservableObject`
are ~20 lines of plain `INotifyPropertyChanged`, and commands use MAUI's built-in
`Command` class, so there's nothing "magic" to learn before you can read the code.

## Assumptions made in Phase 1

- Login is a prototype: any non-empty phone/email + password logs in. Real
  authentication is added in Phase 4 against `AsterEats.API`.
- Bottom navigation currently shows only the **Home** tab. Adding empty
  "Deliveries / Earnings / Profile" tabs now would just be dead ends — they're
  added in Phase 3 once those pages exist.
- All rupee amounts use the **৳** symbol directly in `StringFormat`, matching the
  example screens in the spec.
- Dates/times for mock earnings are generated relative to "today" (UTC) each time
  the app starts, so the dashboard always has something to show.
- Android's manifest requests location permission ahead of time, since Phase 2's
  restaurant/customer navigation will need it — nothing prompts for it yet.

## Fixed after first build attempt

- **Duplicate output filename (`appicon`)**: `MauiImage` was wildcarding the whole
  `Resources\Images` folder, which re-caught `appicon.svg`/`splash.svg` that
  `MauiIcon`/`MauiSplashScreen` already claim. It now only includes `icon_home.svg`.
- **Missing AppxManifest for the Windows target**: the Windows platform head
  (`Platforms\Windows\App.xaml`, `App.xaml.cs`, `Package.appxmanifest`, `app.manifest`)
  was missing entirely. Added it, along with the matching `UseWinUI` /
  `EnableMsixTooling` / `WindowsPackageType` / `ApplicationManifest` csproj settings.
- Also added the **Android** (`MainActivity`, `MainApplication`, `AndroidManifest.xml`)
  and **iOS/MacCatalyst** (`AppDelegate`, `Program`, `Info.plist`, MacCatalyst
  `Entitlements.plist`) platform heads, which were likewise missing from the first
  zip and would have blocked those targets too.


## Roadmap (next phases)

- **Phase 2** — Delivery Request (bottom sheet), Order Details, Active Delivery
  state machine (Accepted → Preparing → Ready → Picked Up → On the Way → Arrived → Delivered).
- **Phase 3** — Delivery History, Earnings page, Profile page, Notifications,
  and the remaining bottom-nav tabs.
- **Phase 4** — `AsterEats.API` (ASP.NET Core Web API, .NET 9), EF Core + SQL
  Server (`AsterEatsDB`), and swapping `MockDataService` for a real `ApiService`.
