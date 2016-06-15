using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace MicrosoftStore.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new App());

			var x = typeof(Xamarin.Forms.Pages.HeroImage);

			return base.FinishedLaunching(app, options);
		}
	}
}

