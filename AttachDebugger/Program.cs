using System.Diagnostics;

class Program
{
	// Usage: <parentProcessId> <childProcessId> <engine>
	// Common engines:
	// - "{2E36F1D4-B23C-435D-AB41-18E608940038}" = .NET Core
	// - "{449EC4CC-30D2-4032-9256-EE18EB41B62B}" = .NET Framework
	static int Main(string[] args)
	{
		if (args.Length != 3)
			return 1;

		if (!int.TryParse(args[0], out var parentProcessId))
			return 2;

		if (!int.TryParse(args[1], out var childProcessId))
			return 3;

		var engine = args[2];

		var parentProcess = Process.GetProcessById(parentProcessId);
		if (parentProcess == null)
			return 4;

		var childProcess = Process.GetProcessById(childProcessId);
		if (childProcess == null)
			return 5;

		try
		{
			VisualStudioUtil.AttachDebugger(parentProcess, childProcess, engine);
		}
		catch
		{
			return 6;
		}

		return 0;
	}
}
