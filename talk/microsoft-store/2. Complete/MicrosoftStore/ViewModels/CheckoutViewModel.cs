using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

using Recommendations;

namespace MicrosoftStore
{
	public class CheckoutViewModel : BaseViewModel
	{
		const string AccountKey = "799bcb7dd81a492286193ce3209ec0f6";
		const string BaseUri = "https://westus.api.cognitive.microsoft.com/recommendations/v4.0";
		const long BuildId = 1558644;
		const string ModelId = "a2d93304-457c-4c6c-9a23-4320a58fbe27";

		public ObservableCollection<Inventory> Cart { get; set; }	
		public ObservableCollection<Inventory> Recommendations { get; set; }

		public CheckoutViewModel()
		{
			Title = "Checkout";

			var app = (App)Application.Current;
			Cart = app.Cart;

			CheckoutRecommendations();
		}

		void CheckoutRecommendations()
		{
			var client = new RecommendationsApi(AccountKey, BaseUri);
			var recommendations = client.GetRecommendations(ModelId, BuildId, Cart[0].ItemId, 3);

			Recommendations = new ObservableCollection<Inventory>();
			foreach (var rec in recommendations.RecommendedItemSetInfo)
			{
				foreach (var item in rec.Items)
				{
					Recommendations.Add(new Inventory { ItemId = item.Id, Name = item.Name, Description = rec.Rating.ToString() });
				}
			}
		}
	}
}

