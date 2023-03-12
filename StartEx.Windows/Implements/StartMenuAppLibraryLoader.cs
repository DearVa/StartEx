using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using DynamicData;
using StartEx.Core.Interfaces;
using StartEx.Core.Models;
using StartEx.PhysicsEngine.DataTypes;

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
			var fullPath = entry.ToFullPath();

			if (entry.IsDirectory) {
				var item = new LauncherViewDirectoryItem(fullPath) {
					TargetSize = new Vector3(2, 2)
				};

				item.Items.AddRange(Directory
					.EnumerateFiles(fullPath)
					.Take(9)
					.Select(filePath => new LauncherViewFileItem(filePath) {
					Icon = AvaloniaLocator.Current.GetRequiredService<IIconLoader>().Load(filePath),
					PositionElasticity = 600,
					PositionDamping = 30
				}));

				return item;
			}

			return new LauncherViewFileItem(fullPath) {
				Icon = AvaloniaLocator.Current.GetRequiredService<IIconLoader>().Load(fullPath)
			};
		}

		public IEnumerator<LauncherViewItem> GetEnumerator() => this;

		IEnumerator IEnumerable.GetEnumerator() => this;
	}

	public Task<List<LauncherViewItem>> Load() {
		return Task.Run(static () => new StartMenuEnumerator().ToList());
	}
}