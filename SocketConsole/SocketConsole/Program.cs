using CommandLine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketConsole
{
    class Program
    {
        class Options
        {
            [Option('m', "message", Required = true, HelpText = "Message for send to socket.")]
            public string Message { get; set; }

            [Option('p',"port",Required =true,
              HelpText = "Port Socket.")]
            public int Port { get; set; }

            [Option('s',"server",Required =true,
              HelpText = "Server.")]
            public string Server { get; set; }

            [Option('t', "timeout", Required = false,
                Default = 30,
              HelpText = "Time out in second unit.")]
            public int Timeout { get; set; }

            [Option('v', "verbose", Default = false,
         HelpText = "Prints all messages to standard output.")]
            public bool Verbose { get; set; }
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);
        }
        static void RunOptions(Options opts)
        {
            

            bool isSendSuccess;
            DateTime startDate = DateTime.UtcNow;
            //handle options
            string pathLog = ConfigurationManager.AppSettings["PATH_LOG"];
            
            TCP_Socket tcpSocket = new TCP_Socket(opts.Server, opts.Port);
            if (opts.Verbose)
            {
                Console.WriteLine("Send Message: {0}", opts.Message);
                Console.WriteLine("To: {0}", opts.Server);
                Console.WriteLine("Port: {0}", opts.Port);
            }

            tcpSocket.Connect();
            string responseMessage;
            isSendSuccess = tcpSocket.SendData(opts.Message,out responseMessage, true, opts.Timeout*1000, opts.Timeout*1000);
            tcpSocket.Disconnect();

            if (opts.Verbose)
            {
                Console.WriteLine("Status: {0}", isSendSuccess);
                Console.WriteLine("Response Message: {0}", responseMessage);
            }

            DateTime endDate = DateTime.UtcNow;
            double takeTimeMilli = (endDate - startDate).TotalMilliseconds;
            if (opts.Verbose)
            {
                Console.WriteLine("Take Time: {0} millisecond", takeTimeMilli);
            }
            KeepLog log = new KeepLog();
            log.write(opts.Server, string.Format("message: {0} status: {1} takeTime: {2}",opts.Message,isSendSuccess,takeTimeMilli), pathLog, true);

            
            
        }

        private static string GetVersion()
        {
            string ver = ConfigurationManager.AppSettings["VERSION"];
            return ver;
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }
    }
}
