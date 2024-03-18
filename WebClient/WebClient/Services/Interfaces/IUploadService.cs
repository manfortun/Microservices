namespace WebClient.Services.Interfaces;

public interface IUploadService<in T>
{
    /// <summary>
    /// Checks if the file is valid file.
    /// </summary>
    /// <param name="file">File</param>
    /// <returns><c>true</c> if the file is valid file. Otherwise, <c>false</c></returns>
    bool IsValidFormat(T file);

    /// <summary>
    /// Sends <paramref name="file"/> to root destination.
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <returns></returns>
    Task<bool> Upload(T file);

    /// <summary>
    /// Modifies a filename to ensure its availability within the specified directory. If the provided file path already exists,
    /// the method appends a numerical suffix in parentheses to the filename and increments the number until a unique filename
    /// is obtained.
    /// Example:
    /// <code>myImage.jpg -> myImage(2).jpg</code>
    /// </summary>
    /// <param name="fileName">Original filename</param>
    /// <returns>A string representing an available filename within the same directory as the original file path.</returns>
    string GetAvailableFileName(string fileName);
}
