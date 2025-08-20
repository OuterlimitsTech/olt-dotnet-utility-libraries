using Amazon.S3.Model;

namespace OLT.Utility.S3
{
    /// <summary>
    /// Represents an S3 object and its metadata.
    /// </summary>
    public record OltS3Object
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OltS3Object"/> class from a GetObjectResponse.
        /// </summary>
        /// <param name="getObjectResponse">The response from S3 GetObject operation.</param>
        public OltS3Object(GetObjectResponse? getObjectResponse)
        {
            BucketName = getObjectResponse?.BucketName;
            ObjectKey = getObjectResponse?.Key;
            ContentType = getObjectResponse?.Headers.ContentType;
            Metadata = getObjectResponse?.Metadata.Keys.ToDictionary(key => key, key => getObjectResponse.Metadata[key]);
            ContentLength = getObjectResponse?.ContentLength;

            if (getObjectResponse?.LastModified != null)
            {
                LastModified = new DateTime(getObjectResponse.LastModified.Ticks, DateTimeKind.Utc);
            }

            if (getObjectResponse?.Headers.ContentType != null && getObjectResponse?.ContentLength > 0)
            {
                var parts = getObjectResponse?.Key.Split('/');                
                FileName = parts?.LastOrDefault();
                if (FileName != null && !FileName.Contains("."))
                {
                    FileName = null;
                }
            }
        }

        /// <summary>
        /// The file name of the S3 object, if available.
        /// </summary>
        public string? FileName { get; init; }

        /// <summary>
        /// The content type of the S3 object.
        /// </summary>
        public string? ContentType { get; init; }

        /// <summary>
        /// The key of the S3 object.
        /// </summary>
        public string? ObjectKey { get; init; }

        /// <summary>
        /// The name of the S3 bucket.
        /// </summary>
        public string? BucketName { get; init; }

        /// <summary>
        /// The metadata associated with the S3 object.
        /// </summary>
        public Dictionary<string, string>? Metadata { get; init; }

        /// <summary>
        /// The content length of the S3 object.
        /// </summary>
        public long? ContentLength { get; init; }

        /// <summary>
        /// The last modified date of the S3 object.
        /// </summary>
        public DateTime? LastModified { get; init; }
    }

}
