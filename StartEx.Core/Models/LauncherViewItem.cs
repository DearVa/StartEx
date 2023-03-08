using Avalonia.Media;
using ReactiveUI;

namespace StartEx.Core.Models;

/// <summary>
/// LauncherView中的一个项目
/// </summary>
public abstract class LauncherViewItem : ReactiveObject {
	/// <summary>
	/// 在开始菜单中占横向的多少格
	/// </summary>
	public int HorizontalSpan {
		get => horizontalSpan;
		set => this.RaiseAndSetIfChanged(ref horizontalSpan, value);
	}

	private int horizontalSpan = 1;

	/// <summary>
	/// 在开始菜单中占纵向的多少格
	/// </summary>
	public int VerticalSpan {
		get => verticalSpan;
		set => this.RaiseAndSetIfChanged(ref verticalSpan, value);
	}

	private int verticalSpan = 1;
}


/// <summary>
/// LauncherView中的一个项目，表示一个文件夹或者文件
/// </summary>
public abstract class LauncherViewFileSystemEntryItem : LauncherViewItem {
	public string FullPath { get; set; }

	protected LauncherViewFileSystemEntryItem(string fullPath) {
		FullPath = fullPath;
	}
}

public class LauncherViewFileItem : LauncherViewFileSystemEntryItem {
	/// <summary>
	/// 文件图标
	/// </summary>
	public IImage? Icon {
		get => icon;
		set => this.RaiseAndSetIfChanged(ref icon, value);
	}

	private IImage? icon;


	public LauncherViewFileItem(string filePath) : base(filePath) { }
}

public class LauncherViewDirectoryItem : LauncherViewFileSystemEntryItem {
	public LauncherViewDirectoryItem(string folderPath) : base(folderPath) { }
}