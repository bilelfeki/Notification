using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Text.Json;

namespace Notification.Services.RabbitMQ
{
    public class Receiver
    {
        public List<string> list { get; set; }
        public Receiver()
        {
            list = new List<string>();

        }
        private class User
        {
            public string Name { get; set; }
            public int id { get; set; }
        }

        public void  Connexion()
        {
            
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    User user = JsonSerializer.Deserialize<User>(message);

                    System.Diagnostics.Debug.WriteLine(user);

                    Console.WriteLine(" [x] Received {0}", message);
                    System.Diagnostics.Debug.WriteLine(message);  
                };
                channel.BasicConsume(queue: "hello",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");

                Console.ReadLine();
            }
        }
    }
}
