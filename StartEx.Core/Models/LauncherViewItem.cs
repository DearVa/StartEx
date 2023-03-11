using System;
using System.Collections.ObjectModel;
using Avalonia.Media;
using DynamicData;
using ReactiveUI;
using StartEx.PhysicsEngine;

namespace StartEx.Core.Models;

/// <summary>
/// LauncherView中的一个项目
/// </summary>
public abstract class LauncherViewItem : PhysicsObject {
	
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
	public SourceList<LauncherViewFileItem> Items { get; } = new();

	public ReadOnlyObservableCollection<LauncherViewFileItem> ObservableItems { get; }

	public LauncherViewDirectoryItem(string folderPath) : base(folderPath) {
		Items.Connect().Bind(out var list).Subscribe();
		ObservableItems = list;
	}
}