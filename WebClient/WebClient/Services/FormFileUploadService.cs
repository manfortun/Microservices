using WebClient.Services.Interfaces;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebClient.Services;

public class FormFileUploadService : IUploadService<IFormFile>
{
    private readonly string[] _validFileExts = ["jpg", "jpeg", "png"];
    public readonly string Directory;

    public FormFileUploadService(IHostingEnvironment webHost)
    {
        Directory = Path.Combine(webHost.WebRootPath, "images");
    }

    public string GetAvailableFileName(string fileName)
    {
        // check if the filename is already used
        int counter = 1;
        string cleanFileName = Path.GetFileNameWithoutExtension(fileName);
        string finalizedFileName = cleanFileName;

        while (IsExisting(finalizedFileName, Directory))
        {
            counter++;
            finalizedFileName = $"{cleanFileName}({counter})";
        }

        finalizedFileName += Path.GetExtension(fileName);

        return finalizedFileName;
    }

    public bool IsValidFormat(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return false;
        }

        if (!file.ContentType.StartsWith("image"))
        {
            return false;
        }

        if (!_validFileExts.Any(file.ContentType.EndsWith))
        {
            return false;
        }

        return true;
    }

    public async Task<bool> Upload(IFormFile file)
    {
        if (!IsValidFormat(file))
        {
            return false;
        }

        string finalizedFileName = GetAvailableFileName(file.FileName);

        using (var stream = new FileStream(Path.Combine(Directory, finalizedFileName), FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return true;
    }

    /// <summary>
    /// Checks if there is already an existing file in the <paramref name="directory"/> with the same file name as <paramref name="fileName"/>
    /// </summary>
    /// <param name="fileName">Filename to check</param>
    /// <param name="directory">Directory to check</param>
    /// <returns><c>true</c> if there is already an existing file name. Otherwise, <c>false</c>.</returns>
    private bool IsExisting(string fileName, string directory)
    {
        var existingFiles = System.IO.Directory.GetFiles(directory).Select(Path.GetFileNameWithoutExtension);
        return existingFiles.Contains(Path.GetFileNameWithoutExtension(fileName));
    }
}
