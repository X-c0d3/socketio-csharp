using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Socket.Io.Client.Core;
using Socket.Io.Client.Core.Model;
using Socket.Io.Client.Core.Model.SocketIo;

namespace SocketIOCleintSharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //optionally supply your own implementation of ILogger (default is NullLogger)
            var config = new SocketIoClientOptions().With(logger: new NullLogger<SocketIoClient>());
            using var client = new SocketIoClient(config);

            //no need to hold reference to subscription as it
            //will be automatically unsubscribed once client is disposed
            client.Events.OnOpen.Subscribe((_) =>
            {
                Console.WriteLine("Socket has been opened");
            });



            //Emit message to server

    
            if(client.State == ReadyState.Open)
            {
                //client.Emit("ESP", new { data = "some-data" }); //object data
                await client.Emit("ESP", "some-data"); //string data

                //with callback (acknowledgement)
                //it is always called only once, no need to unsubscribe/dispose
               client.Emit("ESP", "some-data").Subscribe(ack =>
                {
                    Console.WriteLine($"Callback with data: {ack.Data[0]}.");
                });
            }




            //Acknowledgements
            client.On("ESP").Subscribe(e =>
            {
                //always check before calling acknowledgement
                if (e.SupportsAcknowledgement)
                {
                    //without any data

                    e.Acknowledge();

                    //OR with any optional serializable data
                    e.Acknowledge("message has been processed");
                }
            });


            //subscribe to event with data
            client.Events.OnPacket.Subscribe(packet =>
            {
                Console.WriteLine($"Received packet: {packet}");
            });


            //var options = new SocketIoOpenOptions("custom-path");
            //await client.OpenAsync(new Uri("http://192.168.137.102:4000"), options);


            await client.OpenAsync(new Uri("http://192.168.137.102:4000"));
            Console.ReadLine();
        }
    }
}

//https://github.com/LadislavBohm/socket.io-client-core
//PM> Install-Package Socket.Io.Client.Core

