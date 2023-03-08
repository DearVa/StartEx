using System.Collections.Generic;
using System.Threading.Tasks;
using StartEx.Core.Models;

namespace StartEx.Core.Interfaces; 

/// <summary>
/// 加载APP资源库
/// </summary>
public interface IAppLibraryLoader {
	Task<List<LauncherViewItem>> Load();
}