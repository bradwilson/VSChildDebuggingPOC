using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Xunit;

// The VSTest adapter should always know what environment it's running in, because
// it multi-targets net and netcoreapp. These two tests show the difference in the
// launch and attach behavior. The runner utility will be performing the launch,
// and the VS Test adapter will be performing the attach (when it detects that
// a debugger is already attached).
//
// This means we'll need a way to let the test runner know that we've launched
// a child process, so that it knows it needs to do this. Running v1 and v2 tests
// will not launch a child process.

public class Examples
{
	public Examples()
	{
		if (!Debugger.IsAttached)
			Assert.Fail("This test requires an attached debugger");
	}

	[Fact]
	public void DotNetCore()
	{
		var childFolder = FindAppFolder("ChildProcess", "netcoreapp3.1");
		var psi = new ProcessStartInfo
		{
			FileName = "dotnet",
			Arguments = "exec ChildProcess.dll",
			WorkingDirectory = childFolder
		};

		var child = Process.Start(psi);

		try
		{
			// Set breakpoint here, step over to see the attach succeeded
			AttachDebugger(child, Engines.DotNetCore);

			// Let the Thread.Sleep run so you can see Debug.WriteLine output from the child process
			Thread.Sleep(5000);
		}
		finally
		{
			child.Kill();
		}
	}

	[Fact]
	public void DotNetFramework()
	{
		var childFolder = FindAppFolder("ChildProcess", "net472");
		var psi = new ProcessStartInfo
		{
			FileName = Path.Combine(childFolder, "ChildProcess.exe"),
			WorkingDirectory = childFolder
		};

		var child = Process.Start(psi);

		try
		{
			// Set breakpoint here, step over to see the attach succeeded
			AttachDebugger(child, Engines.DotNetFramework);

			// Let the Thread.Sleep run so you can see Debug.WriteLine output from the child process
			Thread.Sleep(5000);
		}
		finally
		{
			child.Kill();
		}
	}

	void AttachDebugger(Process child, string engine)
	{
		var me = Process.GetCurrentProcess();

		VisualStudioUtil.AttachDebugger(me, child, engine);
	}

	public string FindAppFolder(string appName, string targetFramework)
	{
		var myFolder = Path.GetDirectoryName(typeof(Examples).Assembly.Location);
		var configuration = Path.GetFileName(Path.GetDirectoryName(myFolder));  // Debug or Release
		var rootFolder = FindRootFolder(myFolder);
		return Path.Combine(rootFolder, appName, "bin", configuration, targetFramework);
	}

	string FindRootFolder(string folder)
	{
		if (string.IsNullOrWhiteSpace(folder))
			throw new InvalidOperationException("Could not find .git folder!");

		var gitFolder = Path.Combine(folder, ".git");
		if (Directory.Exists(gitFolder))
			return folder;

		return FindRootFolder(Path.GetDirectoryName(folder));
	}
}
