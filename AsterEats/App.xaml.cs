namespace AsterEats;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// AppShell owns all navigation (Login -> Main tabs -> individual pages).
		return new Window(new AppShell());
	}
}
