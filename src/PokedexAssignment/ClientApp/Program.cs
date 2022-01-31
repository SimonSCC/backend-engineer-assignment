using SharedLibary.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Program pr = new();
            Thread.Sleep(2000);
            Console.WriteLine("Running client app...");
            pr.RunClient().Wait();
        }

        private async Task RunClient()
        {
            HTTPServices http = new();
            Console.WriteLine(await http.GETModel("https://192.168.0.46:57711/GatewayAPI?id=1"));

        }
    }
}
