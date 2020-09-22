// This code was adapted from the VsixTesting project at https://github.com/josetr/VsixTesting
// Copyright (c) 2018 Jose Torres, licensed under Apache 2.0
// License: https://github.com/josetr/VsixTesting/blob/dc9e38b20ae66be9849e068a6e556cdd9cbb5b73/LICENSE.md

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE80;
using DTE = EnvDTE.DTE;

static partial class VisualStudioUtil
{
	static DTE? GetDTE(Process process)
	{
		var namePattern = $@"!VisualStudio\.DTE\.(\d+\.\d+):{process.Id}";

		return (DTE?)RunningObjectTable.GetRunningObjects(namePattern).FirstOrDefault();
	}

	static IEnumerable<DTE> GetRunningDTEs()
	{
		foreach (var process in Process.GetProcessesByName("devenv"))
			if (GetDTE(process) is DTE dte)
				yield return dte;
	}

	static DTE? GetDTEFromDebuggedProcess(Process process)
	{
		foreach (var dte in GetRunningDTEs())
			try
			{
				if (dte.Debugger.CurrentMode != EnvDTE.dbgDebugMode.dbgDesignMode)
					foreach (Process2 debuggedProcess in dte.Debugger.DebuggedProcesses)
						if (debuggedProcess.ProcessID == process.Id)
							return dte;
			}
			catch (COMException) { }

		return null;
	}

	public static void AttachDebugger(Process parentProcess, Process childProcess, string engine)
	{
		var dte = GetDTEFromDebuggedProcess(parentProcess);
		if (dte == null)
			return;

		var targetDTEProcess = dte.Debugger.LocalProcesses.Cast<Process2>().FirstOrDefault(p => p.ProcessID == childProcess.Id);
		if (targetDTEProcess != null)
			targetDTEProcess.Attach2(engine);
	}
}
