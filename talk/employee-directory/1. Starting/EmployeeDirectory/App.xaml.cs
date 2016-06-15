using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace EmployeeDirectory
{
	public partial class App : Application
	{
        const string COGNITIVE_KEY = "22e49721a20e457880a32138afc9e027";

        public App()
		{
			InitializeComponent();

			MainPage = new NavigationPage(new EmployeesPage());
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

