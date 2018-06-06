using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        static async Task Main()
        {
            try
            {
                var listener = new TcpListener(IPAddress.Any, 2905);

                listener.Start();

                while (true)
                {
                    Console.WriteLine("Waiting for connection...");

                    using (var client = listener.AcceptTcpClient())
                    {
                        using (var stream = client.GetStream())
                        {
                            string request = ReadRequest(stream);

                            string[] tokens = request.Split(' ');

                            Console.WriteLine(request.Split('\n')[0]);

                            string postVariables = string.Empty;

                            string page = tokens[1];

                            if (page == "/")
                            {
                                page = @"\index.html";
                            }
                            else if (page == "/cats")
                            {
                                page = @"\cats.html";
                                postVariables = request.Split('\n')[request.Split('\n').Length - 1];
                            }

                            string fileName = @"C:\Projects\TPCServerStuff\TPCServer\Website" + page;


                            var writer = new StreamWriter(stream);

                            if (File.Exists(fileName))
                            {
                                var fileContents = File.ReadAllBytes(fileName);

                                writer.Write("HTTP/1.1 200 OK\r\n\r\n");
                                if (page == @"\cats.html")
                                {
                                    int number1 = int.Parse(postVariables.Split('&')[0].Split('=')[1]);
                                    int number2 = int.Parse(postVariables.Split('&')[1].Split('=')[1]);
                                    writer.Write(Encoding.UTF8.GetString(fileContents).Replace("DefaultValue0",(number1 + number2).ToString()));
                                    writer.Flush();
                                }
                                else
                                {
                                    writer.Write(Encoding.UTF8.GetString(fileContents));
                                    writer.Flush();
                                }

                            }
                            else
                            {
                                writer.Write("HTTP/1.1 404\r\n\r\n");
                                writer.Write("<h1>404</h1>");
                                writer.Flush();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static string ReadRequest(NetworkStream stream)
        {
            var builder = new StringBuilder();

            var buffer = new byte[500 * 1024];

            while (true)
            {
                int read = stream.Read(buffer, 0, buffer.Length);

                builder.Append(Encoding.UTF8.GetString(buffer, 0, read));

                if (read < buffer.Length)
                {
                    break;
                }
            }

            return builder.ToString();
        }
 
    }
}
