using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Program
{
    public static void Main(string[] args)
    {
        TcpListener server = null;
        try
        {
            // Specify the port number for connection
            int port = 8082;
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine("Server is running on port " + port + "...");

            while (true)
            {
                // Accept incoming connection
                Socket clientSocket = server.AcceptSocket();

                // Call UploadServerThread
                UploadServerThread serverThread = new UploadServerThread(clientSocket);

                // Start the thread
                new Thread(serverThread.Run).Start();
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Could not listen on port: 8082.");
            Console.Error.WriteLine(e.Message);
        }
        finally
        {
            // Stop listening for new clients.
            server?.Stop();
        }
    }
}
