using System;
using System.IO;
using System.Text;

public class HttpServletResponse
{
    // Private members
    private Stream outputStream = null;
    private StreamWriter writer;

    // Constructor
    public HttpServletResponse(Stream outputStream)
    {
        this.outputStream = outputStream;
        // Set leaveOpen to true so the underlying stream is not closed when the writer is disposed
        this.writer = new StreamWriter(outputStream, new UTF8Encoding(false, true), 1024, true);
    }


    public Stream GetOutputStream()
    {
        return outputStream;
    }

    public StreamWriter GetWriter()
    {
        return writer;
    }

    // Set the content type of the response being sent back
    public void SetContentType(string type, byte[] content)
    {
        string httpResponse = "HTTP/1.1 200 OK\r\n" +
                              "Content-Type: " + type + "; charset=UTF-8\r\n" +
                              "Content-Length: " + content.Length + "\r\n" +
                              "\r\n";
        writer.Write(httpResponse);
        writer.Flush(); // Flush the headers to the stream
        outputStream.Write(content, 0, content.Length); // Write the content to the stream
        outputStream.Flush(); // Ensure the content is sent to the network stream
    }


    public void Flush()
    {
        writer.Flush();
    }
}
