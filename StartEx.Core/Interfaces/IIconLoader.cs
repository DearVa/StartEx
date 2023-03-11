using Avalonia.Media;

namespace StartEx.Core.Interfaces;

/// <summary>
/// 加载一个指定路径的图标
/// </summary>
public interface IIconLoader {
	IImage? Load(string path);
}