public class TempFileService : ITempFileService
{
    private readonly ILogger<ITempFileService> _logger;

    public TempFileService(ILogger<ITempFileService> logger)
    {
        _logger = logger;
    }

    public void DeleteTempFiles()
    {
        string tempPath = Environment.GetEnvironmentVariable("TEMP")!;
        string windowsTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");

        _logger.LogInformation("Starting to delete files from temp directories...");
        _logger.LogInformation("Deleting files from user-specific Temp folder...");
       
        DeleteFilesFromDirectory(tempPath);
        
        _logger.LogInformation("Deleting files from Windows Temp folder...");
        
        try
        {
            DeleteFilesFromDirectory(windowsTempPath);
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Access denied to Windows Temp folder. Please run as administrator.");
        }
    }

    #region  Private
     private void DeleteFilesFromDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            try
            {
                // Delete files
                foreach (string file in Directory.GetFiles(directoryPath))
                {
                    try
                    {
                        File.Delete(file);
                        _logger.LogInformation("Deleted: {File}", file);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Could not delete file {File}", file);
                    }
                }

                //Delete empty subdirectories
                foreach (string dir in Directory.GetDirectories(directoryPath))
                {
                    try
                    {
                        Directory.Delete(dir, true); // true to delete recursively
                        _logger.LogInformation("Deleted directory: {Directory}", dir);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Could not delete directory {Directory}", dir);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing directory {DirectoryPath}", directoryPath);
            }
        }
        else
        {
            _logger.LogWarning("Directory does not exist: {DirectoryPath}", directoryPath);
        }
    }
    #endregion
}