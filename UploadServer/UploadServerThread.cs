using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class UploadServerThread
{
    private Socket socket = null;

    public UploadServerThread(Socket socket)
    {
        this.socket = socket;
    }

    public void Run()
    {
        try
        {
            using (var networkStream = new NetworkStream(socket))
            using (var check = new StreamReader(networkStream, Encoding.UTF8))
            {
                var req = new HttpServletRequest(networkStream);
                var httpServlet = new UploadServlet();
                var baos = new MemoryStream();
                var res = new HttpServletResponse(baos);

                // Read the request type
                string inputLine = check.ReadLine();
                Console.WriteLine("Checking the request type..");

                // Process the request
                if (inputLine != null)
                {
                    if (inputLine.StartsWith("GET / "))
                    {
                        Console.WriteLine("Calling DoGet method");
                        httpServlet.DoGet(req, res);
                    }
                    else if (inputLine.StartsWith("POST /upload HTTP/1.1"))
                    {
                        Console.WriteLine("Calling DoPost method");
                        httpServlet.DoPost(req, res);
                    }

                    // Ensure any buffered data is written to the memory stream
                    res.GetWriter().Flush();
                    baos.Position = 0; // Reset the position to the beginning of the MemoryStream.

                    // Send the response
                    baos.CopyTo(networkStream); // This copies the MemoryStream contents to the NetworkStream.
                    Console.WriteLine("Response has been sent.");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            // Close the socket after the operation is complete
            try
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while closing the socket: " + e.Message);
            }
        }
    }




}
