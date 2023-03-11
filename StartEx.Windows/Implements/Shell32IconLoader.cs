using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Microsoft.WindowsAPICodePack.Shell;
using StartEx.Core.Interfaces;
using PixelFormat = Avalonia.Platform.PixelFormat;

namespace StartEx.Windows.Implements; 

/// <summary>
/// 通过Shell32读取文件的图标
/// </summary>
internal class Shell32IconLoader : IIconLoader {
	public IImage? Load(string path) {
		if (!File.Exists(path)) {
			return null;
		}

		var shellThumbnail = ShellFile.FromFilePath(path).Thumbnail;
		if (shellThumbnail == null) {
			return null;
		}

		using var bitmap = shellThumbnail.Bitmap;
		var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
		var image = new WriteableBitmap(new PixelSize(bitmap.Width, bitmap.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul);

		// Bitmap不能处理Stride为负数的情况
		using (var buffer = image.Lock()) {
			var rowBytes = Math.Abs(data.Stride);
			unsafe {
				var src = (byte*)data.Scan0;
				var dst = (byte*)buffer.Address;
				for (var i = 0; i < bitmap.Height; i++) {
					Buffer.MemoryCopy(src, dst, buffer.RowBytes, rowBytes);
					src += data.Stride;
					dst += buffer.RowBytes;
				}
			}
		}
		
		bitmap.UnlockBits(data);
		return image;
	}
}
