using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace VoiceDictation
{
	public class NotesViewModel : BaseViewModel
	{
		public ObservableCollection<Note> Notes { get; set; }

		public NotesViewModel()
		{
			Title = "Notes";

			Notes = new ObservableCollection<Note>
			{
				new Note { Title = "Xamarin.Forms Overview", Text = "Xamarin.Forms allows me to build UIs for iOS, Android, and Windows from a single, shared codebase. You can define UIs using either C# or XAML.", Created = DateTime.Now.AddDays(-7) },
				new Note { Title = "Mobile First, Cloud First", Text = "To help power cross-platform apps, we need powerful backends that scale to consumer demand and are easy to build. That's where Microsoft Azure comes in!", Created = DateTime.Now.AddDays(-3) },
				new Note { Title = "VS Live Boston", Text = "Talk is on Microsoft Cognitive Services. Be sure to show how it can help to make your apps smarter, especially when combined with powerful tools like Xamarin.", Created = DateTime.Now.AddDays(-1) },
				new Note { Title = "Sample Applications", Text = "Moments is a Snapchat clone built using Xamarin.Forms and Microsoft Azure. Let people know they can get the bits on GitHub.", Created = DateTime.Now.AddHours(-5) },
				new Note { Title = "Personal Blog Posts", Text = "Be sure to write a few blog posts about your talk at VS Live, and how Microsoft Cognitive Services helped make for some awesome demos.", Created = DateTime.Now.AddHours(-1) },
				new Note { Title = "Evolve 2016", Text = "So much new awesome! Included in Visual Studio, open source, improvements to Xamarin.Forms, Previewer, dark theme for Xamarin Studio, iOS Simulator Remoting, and so much more. Don't forget!", Created = DateTime.Now },
			};

			SubscribeToMessages();
		}

		Note selectedNote;
		public Note SelectedNote
		{
			get { return selectedNote; }
			set
			{
				selectedNote = value;
				OnPropertyChanged("SelectedItem");

				if (selectedNote != null)
				{
					var navigation = Application.Current.MainPage as NavigationPage;
					navigation.PushAsync(new NoteDetailPage(SelectedNote));
					SelectedNote = null;
				}
			}
		}

		Command addNewNoteCommand;
		public Command AddNewNoteCommand
		{
			get { return addNewNoteCommand ?? (addNewNoteCommand = new Command(async () => await ExecuteAddNewNoteCommandAsync())); }
		}

		async Task ExecuteAddNewNoteCommandAsync()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				await Application.Current.MainPage.Navigation.PushAsync(new NoteDetailPage());
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

		void SubscribeToMessages()
		{
			MessagingCenter.Subscribe<NoteDetailViewModel, Note>(this, "ItemsChanged", (sender, arg) =>
			{
				Notes.Add(arg);
			});
		}
	}
}

