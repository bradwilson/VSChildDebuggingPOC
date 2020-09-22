using System.Diagnostics;
using System.Threading;

class Program
{
	static void Main()
	{
		while (true)
		{
			Thread.Sleep(1000);
			Debug.WriteLine("Hello...");
		}
	}
}
