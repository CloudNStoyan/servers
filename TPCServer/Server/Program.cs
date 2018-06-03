using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 2905);
                listener.Start();
                Console.WriteLine("Server started...");
                while (true)
                {
                    Console.WriteLine("Waiting for client connection...");
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    client.Close();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                }
            }
        }
    }
}
