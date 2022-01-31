using SharedLibary.Services;
using System;
using System.Threading.Tasks;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Program pr = new();
            pr.RunClient().Wait();
        }

        private async Task RunClient()
        {
            HTTPServices http = new();
            Console.WriteLine(await http.GETModel("https://localhost:44328/GatewayAPI?id=1"));
        }
    }
}
