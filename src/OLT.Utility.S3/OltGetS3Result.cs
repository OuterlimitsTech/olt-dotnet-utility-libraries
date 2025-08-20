using Amazon.S3.Model;

namespace OLT.Utility.S3    
{


    /// <summary>
    /// Represents the response for a single S3 object retrieval.
    /// </summary>
    public record OltGetS3Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OltGetS3Result"/> class.
        /// </summary>
        public OltGetS3Result()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OltGetS3Result"/> class with an S3 object.
        /// </summary>
        /// <param name="s3Object">The S3 object.</param>
        public OltGetS3Result(OltS3Object s3Object)
        {
            S3Object = s3Object;
        }


        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Indicates whether the S3 object exists.
        /// </summary>
        public bool Exists => S3Object?.ObjectKey != null;

        /// <summary>
        /// The S3 object associated with the response.
        /// </summary>
        public OltS3Object? S3Object { get; init; }

        /// <summary>
        /// The HTTP status code returned by the S3 operation.
        /// </summary>
        public System.Net.HttpStatusCode? StatusCode { get; init; }

        /// <summary>
        /// The exception thrown during the S3 operation, if any.
        /// </summary>
        public Exception? Exception { get; init; }
    }

}
