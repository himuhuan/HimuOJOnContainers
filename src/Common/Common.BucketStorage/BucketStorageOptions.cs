namespace HimuOJ.Common.BucketStorage;

/// <summary>
/// Represents the options for configuring bucket storage.
/// </summary>
public class BucketStorageOptions
{
    /// <summary>
    /// The endpoint URL for the bucket storage.
    /// </summary>
    public string Endpoint { get; set; }
    
    /// <summary>
    /// The name of the bucket.
    /// </summary>
    public string BucketName { get; set; }
    
    /// <summary>
    /// The access key for the bucket storage.
    /// </summary>
    public string AccessKey { get; set; }
    
    /// <summary>
    /// The secret key for the bucket storage.
    /// </summary>
    public string SecretKey { get; set; }

    public bool UseSSL { get; set; } = false;
}