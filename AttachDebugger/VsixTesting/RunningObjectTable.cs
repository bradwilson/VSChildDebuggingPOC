// This code was adapted from the VsixTesting project at https://github.com/josetr/VsixTesting
// Copyright (c) 2018 Jose Torres, licensed under Apache 2.0
// License: https://github.com/josetr/VsixTesting/blob/dc9e38b20ae66be9849e068a6e556cdd9cbb5b73/LICENSE.md

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;

static class RunningObjectTable
{
	const int S_OK = 0;

	public static IEnumerable<object> GetRunningObjects(string namePattern = ".*")
	{
		IEnumMoniker? enumMoniker = null;
		IRunningObjectTable? rot = null;
		IBindCtx? bindCtx = null;

		try
		{
			Marshal.ThrowExceptionForHR(Ole32.CreateBindCtx(0, out bindCtx));
			bindCtx.GetRunningObjectTable(out rot);
			rot.EnumRunning(out enumMoniker);

			var moniker = new IMoniker[1];
			var fetched = IntPtr.Zero;

			while (enumMoniker.Next(1, moniker, fetched) == S_OK)
			{
				object? runningObject = null;

				try
				{
					var roMoniker = moniker[0];
					if (roMoniker is null)
						continue;
					roMoniker.GetDisplayName(bindCtx, null, out var name);
					if (!Regex.IsMatch(name, namePattern))
						continue;
					Marshal.ThrowExceptionForHR(rot.GetObject(roMoniker, out runningObject));
				}
				catch (UnauthorizedAccessException)
				{
					continue;
				}

				if (runningObject != null)
					yield return runningObject;
			}
		}
		finally
		{
			if (enumMoniker != null)
				Marshal.ReleaseComObject(enumMoniker);
			if (rot != null)
				Marshal.ReleaseComObject(rot);
			if (bindCtx != null)
				Marshal.ReleaseComObject(bindCtx);
		}
	}
}
