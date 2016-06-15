using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MicrosoftStore
{
	public partial class CheckoutPage : ContentPage
	{
		public CheckoutPage()
		{
			InitializeComponent();

			BindingContext = new CheckoutViewModel();
		}
	}
}

