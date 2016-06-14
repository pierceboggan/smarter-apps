using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace EmployeeDirectory
{
	public partial class EmployeesPage : ContentPage
	{
		public EmployeesPage()
		{
			InitializeComponent();

			BindingContext = new EmployeesViewModel();
		}
	}
}