using StartEx.Core.Interfaces;
using StartEx.Core.Models;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using DynamicData;

namespace StartEx.Core.ViewModels;

public class MainWindowViewModel : BusyViewModelBase {
	public ObservableCollection<LauncherViewItem> Items { get; } = new();

	private readonly IAppLibraryLoader appLibraryLoader;

	public MainWindowViewModel(IAppLibraryLoader appLibraryLoader) {
		this.appLibraryLoader = appLibraryLoader;
	}

	protected override Task OnActivatedAsync(CompositeDisposable disposables) {
		return LoadItems();
	}

	public async Task LoadItems() {
		IsBusy = true;

		try {
			Items.AddRange(await appLibraryLoader.Load());
		} finally {
			IsBusy = false;
		}
	}
}