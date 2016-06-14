using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MicrosoftStore
{
	// Model ID: a2d93304-457c-4c6c-9a23-4320a58fbe27
	// Build ID: 1558644

	public class InventoryViewModel : BaseViewModel
	{
		public ObservableCollection<Inventory> Inventory { get; set; }

		public InventoryViewModel()
		{
			Title = "Microsoft Store";

			Inventory = new ObservableCollection<Inventory>
			{
				new Inventory { ItemId = "FKF-00908", Name = "Titanfall Collector's Edition (Xbox One)", Price = "$49.99", PhotoUrl = "http://ecx.images-amazon.com/images/I/91WJmvCW5gL._SX385_.jpg", Description="Prepare for Titanfall. Crafted by one of the co-creators of Call of Duty and other key developers behind the Call of Duty franchise, Titanfall is an all-new universe juxtaposing small vs. giant, natural vs. industrial and man vs. machine. The visionaries at Respawn have drawn inspiration from their proven experiences in first-person action and with Titanfall are focused on bringing something exciting the next generation of multiplayer gaming." },
				new Inventory { ItemId = "FKF-00962", Name = "EA Madden NFL 15 (Xbox One)", Price = "$29.99", PhotoUrl = "http://i5.walmartimages.com/dfw/dce07b8c-7960/k2-_ce3b93d5-8df1-4ebb-8635-a6f28373d32c.v3.jpg", Description="Bring the Heat Utilize a new set of pass rush tools to beat your blocker and disrupt the backfield. New mechanics to jump the snap, shed blocks and steer offensive linemen put your in control and make defensive linemen more dangerous than ever." },
				new Inventory { ItemId = "FKF-00644", Name = "EA UFC (Xbox One)", Price = "$19.99", PhotoUrl = "http://gk4.me/14996-thickbox_default/ea-sports-ufc-xbox-one.jpg", Description="EA SPORTS UFC 2 innovates with stunning character likeness and animation, adds an all new Knockout Physics System and authentic gameplay features, and invites all fighters to step back into the Octagon to experience the thrill of finishing the fight. From the walkout to the knockout, EA SPORTS UFC 2 delivers a deep, authentic, and exciting experience." },
				new Inventory { ItemId = "44Z-00001", Name = "Minecraft (Xbox One)", Price = "$49.99", PhotoUrl = "http://i5.walmartimages.com/dfw/dce07b8c-30cb/k2-_e17d2280-c66b-4558-8fb0-56b6d1d76453.v1.jpg", Description="Build with your imagination! Minecraft, one of the best-selling games on Xbox 360, is now available on Xbox One. Create and explore your very own world where the only limit is what you can imagine - just be sure to build a shelter before night comes to keep yourself safe from monsters." }
			};
		}

		Inventory selectedInventory;
		public Inventory SelectedInventory
		{
			get { return selectedInventory; }
			set
			{
				selectedInventory = value;
				OnPropertyChanged("SelectedItem");

				if (selectedInventory != null)
				{
					var navigation = Application.Current.MainPage as NavigationPage;
					navigation.PushAsync(new InventoryDetailPage(SelectedInventory));
				}
			}
		}

		// NavigateToCart
		Command navigateToCartCommand;
		public Command NavigateToCartCommand
		{
			get { return navigateToCartCommand ?? (navigateToCartCommand = new Command(async () => await ExecuteAddToCartCommandAsync())); }
		}

		async Task ExecuteAddToCartCommandAsync()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				var navigation = Application.Current.MainPage as NavigationPage;
				await navigation.PushAsync(new CheckoutPage());
			}
			catch (Exception ex)
			{
				Acr.UserDialogs.UserDialogs.Instance.ShowError(ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}

