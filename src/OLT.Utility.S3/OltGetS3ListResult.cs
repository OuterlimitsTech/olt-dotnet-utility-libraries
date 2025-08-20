namespace OLT.Utility.S3    
{
    /// <summary>
    /// Represents the response from listing objects in an S3 bucket.
    /// </summary>
    public record OltGetS3ListResult
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// The HTTP status code returned by the S3 operation.
        /// </summary>
        public System.Net.HttpStatusCode? StatusCode { get; }

        /// <summary>
        /// The list of S3 object responses.
        /// </summary>
        public List<OltGetS3Result> Objects { get; } = new List<OltGetS3Result>();
    }

}
