namespace OLT.Utility.S3    
{
    /// <summary>
    /// Represents the response for creating a bucket if it doesn't exist.
    /// </summary>
    public record OltCreateBucketResult
    {
        /// <summary>
        /// Indicates whether the bucket was created (true) or already existed (false).
        /// </summary>
        public bool Created { get; init; }

        /// <summary>
        /// Indicates whether the bucket creation operation was successful.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// The exception thrown during the S3 operation, if any.
        /// </summary>
        public Exception? Exception { get; init; }
    }

}
