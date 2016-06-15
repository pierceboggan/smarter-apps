using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MicrosoftStore
{
	public partial class App : Application
	{
		public ObservableCollection<Inventory> Cart { get; set; }

		public App()
		{
			InitializeComponent();

			Cart = new ObservableCollection<Inventory>();

			MainPage = new NavigationPage(new InventoryPage())
			{
				BarBackgroundColor = Color.FromHex("00a1f1"),
				BarTextColor = Color.White
			};
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}

