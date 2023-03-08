using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Threading.Tasks;
using StartEx.Core.Interfaces;
using StartEx.Core.Models;

namespace StartEx.Windows.Implements;

/// <summary>
/// 通过开始菜单来获取APP资源库
/// </summary>
/// <remarks>
///	具体来说，就是读取C:\ProgramData\Microsoft\Windows\Start Menu\Programs中的文件
/// </remarks>
public class StartMenuAppLibraryLoader : IAppLibraryLoader {
	internal class StartMenuEnumerator : FileSystemEnumerator<LauncherViewItem>, IEnumerable<LauncherViewItem> {
		public StartMenuEnumerator() : 
			base(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs")) { }  // TODO

		protected override LauncherViewItem TransformEntry(ref FileSystemEntry entry) {
			if (entry.IsDirectory) {
				return new LauncherViewDirectoryItem(entry.ToFullPath()) {
					HorizontalSpan = 2,
					VerticalSpan = 2
				};
			}

			return new LauncherViewFileItem(entry.ToFullPath());
		}

		public IEnumerator<LauncherViewItem> GetEnumerator() => this;

		IEnumerator IEnumerable.GetEnumerator() => this;
	}

	public Task<List<LauncherViewItem>> Load() {
		return Task.Run(static () => new StartMenuEnumerator().ToList());
	}
}