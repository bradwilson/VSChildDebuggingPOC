// This code was adapted from the VsixTesting project at https://github.com/josetr/VsixTesting
// Copyright (c) 2018 Jose Torres, licensed under Apache 2.0
// License: https://github.com/josetr/VsixTesting/blob/dc9e38b20ae66be9849e068a6e556cdd9cbb5b73/LICENSE.md

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

static class Ole32
{
	[DllImport("ole32.dll")]
	public static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);
}
