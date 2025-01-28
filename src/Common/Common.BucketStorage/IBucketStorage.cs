namespace HimuOJ.Common.BucketStorage;

/// <summary>
/// Interface for bucket storage operations.
/// </summary>
public interface IBucketStorage
{
    /// <summary>
    /// Uploads a file to the bucket storage.
    /// </summary>
    /// <param name="stream">The file stream to upload.</param>
    /// <param name="fileName">The name of the file to be uploaded.</param>
    /// <param name="size">The size of the file to be uploaded. </param>
    /// <param name="contentType">The content type of the file to be uploaded.</param>
    Task<bool> UploadAsync(Stream stream, string fileName, long size, string contentType);

    /// <summary>
    /// Downloads a file from the bucket storage.
    /// </summary>
    /// <param name="fileName">The name of the file to be downloaded.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the file stream.</returns>
    Task<Stream> DownloadAsync(string fileName);

    /// <summary>
    /// Deletes a file from the bucket storage.
    /// </summary>
    /// <param name="fileName">The name of the file to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the deletion was successful.</returns>
    Task<bool> DeleteAsync(string fileName);
    Task<bool> IsFileExits(string fileName);
}