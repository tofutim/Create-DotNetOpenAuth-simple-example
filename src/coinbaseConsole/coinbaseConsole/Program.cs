using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace cbConsole
{
    class Program
    {
        const string _clientId = "xb5dd88908edba1e22b18e358df0607da85f41a77ae9b15bfcaaea87de5328dc";
        const string _clientSecret = "x8938d6e006e089b754007af7ac6fbb7e36fb43ef5fd41b041373d9de05efa37";
        private readonly CoinbaseConsumer _coinbase;

        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }

        public Program()
        {
            _coinbase = new CoinbaseConsumer(_clientId, _clientSecret);
        }

        void Run()
        {
            var url = _coinbase.BeginAuth();
            Process.Start(url);
            Console.Write("Enter code: ");
            var pin = Console.ReadLine();
            _coinbase.CompleteAuth(pin);

            var request = _coinbase.PrepareAuthorizedRequest(@"https://api.coinbase.com/v2/user");
            var response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            var stream = response.GetResponseStream();
            var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            Console.WriteLine(result);

            reader.Close();
            stream.Close();
            response.Close();

            Console.ReadLine();
        }
    }
}
