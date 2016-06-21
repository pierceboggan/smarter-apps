using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Acr.UserDialogs;

using Microsoft.ProjectOxford.Face;

namespace EmployeeDirectory
{
	public class EmployeesViewModel : BaseViewModel
	{
		string personGroupId;

		public EmployeesViewModel()
		{
			Title = "Employees";

			Employees = new ObservableCollection<Employee>
			{
				new Employee { Name = "Nat Friedman", Title = "CEO", PhotoUrl = "http://static4.businessinsider.com/image/559d359decad04574c42a3c4-480/xamarin-nat-friedman.jpg" },
				new Employee { Name = "Miguel de Icaza", Title = "CTO", PhotoUrl = "http://images.techhive.com/images/idge/imported/article/nww/2011/03/031111-deicaza-100272676-orig.jpg" },
				new Employee { Name = "Joseph Hill", Title = "VP of Developer Relations", PhotoUrl = "https://www.gravatar.com/avatar/f763ec6935726b7f7715808828e52223.jpg?s=256" },
				new Employee { Name = "James Montemagno", Title = "Developer Evangelist", PhotoUrl = "http://www.gravatar.com/avatar/7d1f32b86a6076963e7beab73dddf7ca?s=256" },
				new Employee { Name = "Pierce Boggan", Title = "Software Engineer", PhotoUrl = "https://avatars3.githubusercontent.com/u/1091304?v=3&s=460" },
			};
		}

		ObservableCollection<Employee> employees;
		public ObservableCollection<Employee> Employees
		{
			get { return employees; }
			set { employees = value; OnPropertyChanged("Employees"); }
		}

		Command findSimilarFaceCommand;
		public Command FindSimilarFaceCommand
		{
			get { return findSimilarFaceCommand ?? (findSimilarFaceCommand = new Command(async () => await ExecuteFindSimilarFaceCommandAsync())); }
		}

		async Task ExecuteFindSimilarFaceCommandAsync()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				
			}
			catch (Exception ex)
			{
				UserDialogs.Instance.ShowError(ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
