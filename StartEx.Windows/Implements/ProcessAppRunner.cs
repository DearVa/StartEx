using StartEx.Core.Interfaces;
using System.Diagnostics;

namespace StartEx.Windows.Implements; 

internal class ProcessAppRunner : IAppRunner {
	public void Run(string filePath) {
		Process.Start(new ProcessStartInfo(filePath) {
			UseShellExecute = true
		});
	}
}