using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class UploadServlet : HttpServlet
{
    // Directory to save uploaded files
    private static readonly string UploadDir = "uploads";

    // This method sends back a form as a response, which will call DoPost
    public override void DoGet(HttpServletRequest request, HttpServletResponse response)
    {
        Console.WriteLine("Entered DoGet");
        string htmlContent = "<html><body>" +
                             "<h1>File Upload Form</h1>" +
                             "<form method='POST' action='/upload' enctype='multipart/form-data'>" +
                             "<input type='file' name='file' /><br />" +
                             "<input type='text' name='caption' placeholder='Caption' /><br />" +
                             "<input type='text' name='date' placeholder='Date' /><br />" +
                             "<input type='submit' value='Upload' />" +
                             "</form>" +
                             "</body></html>";

        byte[] htmlBytes = Encoding.UTF8.GetBytes(htmlContent);
        response.SetContentType("text/html", htmlBytes);
        // The content is written in the SetContentType method.
    }


    // This method will save the file passed in from form and then send back
    // list of sorted files as a response

    public override void DoPost(HttpServletRequest request, HttpServletResponse response)
    {
        Console.WriteLine("Calling request parse input method");
        request.ParseInputStream();

        // Initialize uploadsDir only once
        DirectoryInfo uploadsDir = new DirectoryInfo(UploadDir);
        if (!uploadsDir.Exists)
        {
            uploadsDir.Create();
        }

        // Get all the values
        string caption = request.GetParameter("caption");
        string date = request.GetParameter("date");
        string boundary = request.GetParameter("boundary");
        string filename = request.GetParameter("filename");
        byte[] fileData = request.GetFileParameter("fileData");
        Console.WriteLine("Now back in UploadServlet:");
        Console.WriteLine("Caption is " + caption + " Date is " + date);
        Console.WriteLine("Filename is " + filename + " Boundary is " + boundary);

        // Save the file
        string filePath = Path.Combine(UploadDir, $"{caption}_{date}_{filename}");
        try
        {
            if (fileData != null)
            {
                File.WriteAllBytes(filePath, fileData);
                Console.WriteLine("File uploaded successfully");
            }
            else
            {
                Console.WriteLine("File data was null, could not be uploaded");
            }
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }

        // Get and sort the list of files
        FileInfo[] files = uploadsDir.GetFiles().OrderBy(f => f.Name).ToArray();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<html><body>");
        sb.AppendLine("<h1>Uploaded Files</h1>");
        sb.AppendLine("<ul>");

        // Get and sort the list of files only once
        foreach (var file in files)
        {
            string listItem = file.Attributes.HasFlag(FileAttributes.Directory)
                              ? $"<li><button type=\"button\">{file.Name}</button></li>"
                              : $"<li>{file.Name}</li>";
            sb.AppendLine(listItem);
        }

        sb.AppendLine("</ul>");
        sb.AppendLine("</body></html>");

        // Convert the StringBuilder content to a byte array
        string htmlContent = sb.ToString();
        byte[] htmlBytes = Encoding.UTF8.GetBytes(htmlContent);

        // Set the headers and write the content
        response.SetContentType("text/html", htmlBytes);
        // Note: You will need to implement the SetContentType method to take a byte array and set the content length header
    }

}
