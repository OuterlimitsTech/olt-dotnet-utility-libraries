namespace OLT.Utility.S3    
{
    /// <summary>
    /// Represents the response for a single S3 object retrieval that includes a stream.
    /// </summary>
    public record OltGetS3StreamResult : OltGetS3Result, IDisposable
    {
        private bool _disposed;
        private Stream? _responseStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="OltGetS3StreamResult"/> class.
        /// </summary>
        public OltGetS3StreamResult()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OltGetS3StreamResult"/> class with an S3 object and response stream.
        /// </summary>
        /// <param name="s3Object">The S3 object.</param>
        /// <param name="stream">The response stream.</param>
        public OltGetS3StreamResult(OltS3Object s3Object, Stream? stream) : base(s3Object)
        {
            _responseStream = stream;
        }

        /// <summary>
        /// Gets the response stream containing the S3 object's data.
        /// </summary>
        public Stream? ResponseStream => _responseStream;

        /// <summary>
        /// Releases the resources used by the <see cref="OltGetS3StreamResult"/> instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            if (!_disposed)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="OltGetS3StreamResult"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ResponseStream?.Dispose();
                }

                _responseStream = null;
                _disposed = true;
            }
        }
    }
}
