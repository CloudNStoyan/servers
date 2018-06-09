using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpListenerServer
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpListener listener = null;
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:2905/");
                listener.Start();
                while (true)
                {
                    Console.WriteLine("Waiting for connection...");
                    HttpListenerContext context = listener.GetContext();
                    Console.WriteLine("Connected!");
                    string page = context.Request.RawUrl;

                    Console.WriteLine(page);
                    if (page == "/")
                    {
                        page = "/index.html";
                    } else if (page == "/cats")
                    {
                        page = "/cats.html";
                    }

                    string response = String.Empty;
                    if (File.Exists(@"C:\Projects\Servers\HttpListenerServer\WebSite" + page))
                    {
                        string post = string.Empty;
                        if (page == "/cats.html")
                        {
                            string request = GetRequest(context);
                            var arr  = request.Split('&');
                            post = (int.Parse(arr[0].Split('=')[1]) + int.Parse(arr[1].Split('=')[1])).ToString();
                        }
                        response = File.ReadAllText(@"C:\Projects\Servers\HttpListenerServer\WebSite\" + page);
                        if (post != string.Empty)
                        {
                            response = response.Replace("DefaultValue0", post);
                        }
                        context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(response);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }

                    using (Stream stream = context.Response.OutputStream)
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(response);
                            writer.Flush();
                        }
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Status);
            }
        }

        static string GetRequest(HttpListenerContext context)
        {
            var request = context.Request;
            string text;
            using (var reader = new StreamReader(request.InputStream,
                request.ContentEncoding))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }
    }
}
