using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class HttpServletRequest
{
    // Private members
    private Stream inputStream = null;
    private Dictionary<string, object> paramsDictionary;

    // Constructor
    public HttpServletRequest(Stream inputStream)
    {
        this.inputStream = inputStream;
        this.paramsDictionary = new Dictionary<string, object>();
    }

    public Stream GetInputStream()
    {
        return inputStream;
    }

    // This method is only for returning a string value
    public string GetParameter(string name)
    {
        if (paramsDictionary.TryGetValue(name, out object value) && value is string)
        {
            return (string)value;
        }
        else
        {
            return null;
        }
    }

    // This method is only for returning file object
    public byte[] GetFileParameter(string name)
    {
        if (paramsDictionary.TryGetValue(name, out object value) && value is byte[])
        {
            return (byte[])value;
        }
        else
        {
            return null;
        }
    }

    // This method will store the following key-value pairs in dictionary
    // Caption, Date, Boundary, Filename, File data (in bytes)
    public void ParseInputStream()
    {
        Console.WriteLine("Parsing input data in request now:");

        try
        {
            Console.WriteLine("Entered try");
            using (StreamReader reader = new StreamReader(inputStream, Encoding.UTF8))
            {
                // To store the file content
                using (MemoryStream fileContentStream = new MemoryStream())
                {
                    // Variables to store form fields
                    string boundary = null;
                    string caption = null;
                    string date = null;
                    string originalFilename = null;
                    string line = null;

                    Console.WriteLine("Starting to read request now");
                    // First search for the boundary
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        if (line.StartsWith("------"))
                        {
                            Console.WriteLine("Boundary is found");
                            boundary = line;
                            break;
                        }
                    }
                    paramsDictionary["boundary"] = boundary;
                    Console.WriteLine("Boundary placed in dictionary is: " + boundary);

                    // This is just a flag to indicate whether we are currently reading the file data or not
                    bool isFileData = false;

                    // Read form data
                    while ((line = reader.ReadLine()) != null && !line.Equals(boundary + "--"))
                    {
                        // Check if the file field is found
                        if (line.Contains("Content-Disposition: form-data; name=\"file\""))
                        {
                            originalFilename = ExtractFilename(line);
                            paramsDictionary["filename"] = originalFilename;
                            Console.WriteLine("Filename placed in dictionary is: " + originalFilename);
                            reader.ReadLine(); // Skip the Content-Type line (if it exists)
                            reader.ReadLine(); // Empty line before file content
                            isFileData = true;
                            continue;
                        }
                        else if (line.Contains("Content-Disposition: form-data; name=\"caption\""))
                        {
                            reader.ReadLine(); // Empty line before caption
                            caption = reader.ReadLine();
                            paramsDictionary["caption"] = caption;
                            Console.WriteLine("Caption placed in dictionary is: " + caption);
                        }
                        else if (line.Contains("Content-Disposition: form-data; name=\"date\""))
                        {
                            reader.ReadLine(); // Empty line before date
                            date = reader.ReadLine();
                            paramsDictionary["date"] = date;
                            Console.WriteLine("Date placed in dictionary is: " + date);
                        }

                        // This means we are still in the file data and have not reached the end
                        if (isFileData && !line.Equals("--" + boundary))
                        {
                            byte[] lineBytes = Encoding.UTF8.GetBytes(line + Environment.NewLine);
                            fileContentStream.Write(lineBytes, 0, lineBytes.Length);
                        }
                        else if (line.Equals("--" + boundary))
                        {
                            isFileData = false;
                        }
                    }

                    // Convert the byte stream to byte array and store in our dictionary
                    paramsDictionary["fileData"] = fileContentStream.ToArray();
                    Console.WriteLine("Placed file data in dictionary");
                }
            }
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    // Method that will extract the string filename based on an input stream line
    private static string ExtractFilename(string contentDisposition)
    {
        Console.WriteLine("Extracting filename method is called");
        string[] parts = contentDisposition.Split(';');
        foreach (string part in parts)
        {
            if (part.Trim().StartsWith("filename"))
            {
                string filename = part.Split('=')[1].Trim('"');
                Console.WriteLine("Extracting filename method returned: " + filename);
                return filename;
            }
        }
        Console.WriteLine("Extracting filename method returned null");
        return null;
    }
}
