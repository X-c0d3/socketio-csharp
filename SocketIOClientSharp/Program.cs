using System.Threading.Tasks;
using System;
using SocketIOClient;
using System.Collections.Generic;
using System.Net.Http;

namespace SocketIOCleintSharp {
    class Program {
        static async Task Main(string[] args) {
            // dont use /socket.io/  or /?EIO in url 
            var socket = new SocketIO(new Uri("https://192.168.137.35:8000/acid"), new SocketIOOptions {
                Query = new Dictionary<string, string>
                {
                    {"token", "V2" }
                },
                Transport = SocketIOClient.Transport.TransportProtocol.Polling,
                EIO = 3,
                //Path = "/acid"
            });


            socket.HttpClient = new HttpClient(new HttpClientHandler {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            });

            socket.OnConnected += Socket_OnConnected;
            socket.OnPing += (sender, e) => Console.WriteLine(" Ping");
            socket.OnPong += (sender, e) => Console.WriteLine(" Pong: " + e.TotalMilliseconds);
            socket.OnDisconnected += (sender, e) => Console.WriteLine(" disconnect: " + e);
            socket.OnReconnectAttempt += (sender, e) => Console.WriteLine($"{DateTime.Now} Reconnecting: attempt = {e}");
            socket.OnAny((name, response) => {
                Console.WriteLine(name);
                Console.WriteLine(response);
            });

            socket.On("planningstock", response => {
                // Console.WriteLine(response.ToString());
                Console.WriteLine(" =======> 1 " + response.GetValue<string>());
            });

            socket.On("watchlist-room", response => {
                // Console.WriteLine(response.ToString());
                Console.WriteLine(" =======> 2 " + response.GetValue<string>());
            });

            socket.On("reflux-subscribe", response => {
                // Console.WriteLine(response.ToString());
                Console.WriteLine(" =======> 3 " + response.GetValue<string>());
            });

            socket.On("room", response => {
                // Console.WriteLine(response.ToString());
                Console.WriteLine(" =======> 4 " + response.GetValue<string>());
            });


            socket.On("ESP", response => {
                // Console.WriteLine(response.ToString());
                Console.WriteLine(" =======> 5 " + response.ToString());
            });

            await socket.ConnectAsync();

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        private static async void Socket_OnConnected(object sender, EventArgs e) {
            Console.WriteLine("Socket_OnConnected");
            var socket = sender as SocketIO;
            Console.WriteLine("Socket.Id:" + socket.Id);

            while (true) {
                await Task.Delay(1000);

                var dto = new { Id = 123, Name = "bob" };
                if (socket.Connected)
                    await socket.EmitAsync("ESP", dto);

            }
        }
    }
}


