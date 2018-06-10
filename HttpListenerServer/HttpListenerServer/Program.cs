using System;
using System.IO;
using System.Net;
using System.Text;

namespace HttpListenerServer
{
    class Program
    {
        static void Main()
        {
            try
            {
                var listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:2905/");
                listener.Start();
                while (true)
                {
                    Console.WriteLine("Waiting for connection...");
                    HttpListenerContext context = listener.GetContext();
                    try
                    {
                        Console.WriteLine("Connected!");
                        string page = context.Request.RawUrl;

                        Console.WriteLine(page);
                        if (page == "/")
                        {
                            page = "/index.html";
                        }
                        else if (page == "/cats")
                        {
                            page = "/cats.html";
                        }

                        string response = String.Empty;
                        if (File.Exists(@"C:\Projects\Servers\HttpListenerServer\WebSite" + page))
                        {
                            string post = string.Empty;
                            if (page == "/cats.html")
                            {
                                try
                                {
                                    string request = GetRequest(context);
                                    var arr = request.Split('&');
                                    post = (int.Parse(arr[0].Split('=')[1]) + int.Parse(arr[1].Split('=')[1]))
                                        .ToString();
                                }
                                catch (Exception e)
                                {
                                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                                    Console.WriteLine(e.Message);
                                    continue;
                                }
                            }

                            response = File.ReadAllText(@"C:\Projects\Servers\HttpListenerServer\WebSite\" + page);
                            if (post != string.Empty)
                            {
                                response = response.Replace("DefaultValue0", post);
                            }

                            context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(response);
                            context.Response.StatusCode = (int) HttpStatusCode.OK;


                        }
                        else
                        {
                            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                        }

                        using (Stream stream = context.Response.OutputStream)
                        {
                            using (StreamWriter writer = new StreamWriter(stream))
                            {
                                writer.Write(response);
                            }
                        }


                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
