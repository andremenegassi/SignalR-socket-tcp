using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Connections;


namespace SignalRTeste
{
    public class Handler : ConnectionHandler
    {
        private ConnectionList Connections { get; } = new ConnectionList();

        public override async Task OnConnectedAsync(ConnectionContext context)
        {
            

            Connections.Add(context);

            await Broadcast($"{context.ConnectionId} connected.");


            while (true)
            {
                var result = await context.Transport.Input.ReadAsync();
                var buffer = result.Buffer;

                try
                {
                    if (!buffer.IsEmpty)
                    {
                        string text = Encoding.ASCII.GetString(buffer.ToArray());
                        Console.WriteLine("RECEBIDO: " + text);

                        await context.Transport.Output.WriteAsync(Encoding.ASCII.GetBytes("RECEBIDO: " + text + "\n"));
                    }
                    else if (result.IsCompleted)
                    {
                        break;
                    }
                }
                finally
                {

                    context.Transport.Input.AdvanceTo(result.Buffer.End);
                }
            }


        }

        private Task Broadcast(string text)
        {
            return Broadcast(Encoding.ASCII.GetBytes(text));
        }

        private Task Broadcast(byte[] payload)
        {
            var tasks = new List<Task>(Connections.Count);
            foreach (var c in Connections)
            {
                tasks.Add(c.Transport.Output.WriteAsync(payload).AsTask());
            }

            return Task.WhenAll(tasks);
        }

        private Task SendMessage(string connectionId, string text)
        {
            byte[] payload = Encoding.ASCII.GetBytes(text);
            var c = Connections[connectionId];
            if (c != null)
                return c.Transport.Output.WriteAsync(payload).AsTask();
            return null;
        }
    }
}
