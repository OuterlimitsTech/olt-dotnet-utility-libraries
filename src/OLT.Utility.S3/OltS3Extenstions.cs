using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace OLT.Utility.S3
{
    /// <summary>
    /// Provides extension methods for Amazon S3 operations used in OLT.Utility.S3.
    /// </summary>
    public static class OltS3Extenstions
    {
        /// <summary>
        /// Creates an S3 bucket if it does not exist. Only executes if the debugger is attached.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client.</param>
        /// <param name="bucketName">The name of the bucket to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task<OltCreateBucketResult> CreateBucketIfNotExistsAsync(this IAmazonS3 s3Client, string? bucketName)
        {
            
            ArgumentNullException.ThrowIfNullOrEmpty(bucketName);

            if (await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName))
            {
                return new OltCreateBucketResult { Success = true };
            }

            try
            {
                // Create the bucket if it doesn't exist
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                };

                await s3Client.PutBucketAsync(putBucketRequest);
                return new OltCreateBucketResult { Success = true, Created = true };
            }
            catch (Exception ex)
            {
                return new OltCreateBucketResult { Exception = ex };
            }
        }

        /// <summary>
        /// Retrieves an object from S3 by bucket name and storage ID.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="storageId">The key of the object to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="OltGetS3Result"/> containing the result of the operation.</returns>
        public static async Task<OltGetS3Result> GetAsync(this IAmazonS3 s3Client, string? bucketName, string? storageId, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(bucketName);
            ArgumentNullException.ThrowIfNullOrEmpty(storageId);

            try
            {
                await CreateBucketIfNotExistsAsync(s3Client, bucketName);

                // Create a GetObject request
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = storageId
                };

                using var response = await s3Client.GetObjectAsync(request, cancellationToken);
                return new OltGetS3Result(new OltS3Object(response)) { Success = true };

            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new OltGetS3Result { Success = true, StatusCode = ex.StatusCode };
            }
            catch (AmazonS3Exception ex)
            {
                return new OltGetS3Result { Success = false, StatusCode = ex.StatusCode, Exception = ex };
            }
            catch (Exception ex)
            {
                return new OltGetS3Result { Success = false, Exception = ex };
            }
        }

        /// <summary>
        /// Retrieves all objects from an S3 bucket with the specified prefix.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="pathPrefix">The prefix to filter objects.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="OltGetS3ListResult"/> containing the list of objects.</returns>
        public static async Task<OltGetS3ListResult> GetAllAsync(this IAmazonS3 s3Client, string? bucketName, string? pathPrefix, CancellationToken cancellationToken = default)
        {
            var successResult = new OltGetS3ListResult { Success = true };

            ArgumentNullException.ThrowIfNullOrEmpty(bucketName);
            ArgumentNullException.ThrowIfNullOrEmpty(pathPrefix);

            try
            {
                await CreateBucketIfNotExistsAsync(s3Client, bucketName);

                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = pathPrefix,
                };

                bool isTruncated;

                ListObjectsV2Response? listResponse;
                do
                {
                    if (cancellationToken.IsCancellationRequested) return new OltGetS3ListResult { Success = false };

                    listResponse = await s3Client.ListObjectsV2Async(request, cancellationToken);

                    foreach (var obj in listResponse.S3Objects)
                    {
                        try
                        {
                            var response = await GetAsync(s3Client, obj.BucketName, obj.Key, cancellationToken);
                            if (response.S3Object?.ObjectKey != pathPrefix)
                            {
                                successResult.Objects.Add(response);
                            }
                        }
                        catch
                        {
                        }
                    }

                    isTruncated = listResponse.IsTruncated;
                    request.ContinuationToken = listResponse.NextContinuationToken;
                }
                while (isTruncated);

            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new OltGetS3ListResult { Success = false };
            }

            return successResult;
        }

        /// <summary>
        /// Lists all directories (common prefixes) under a specified root prefix in an S3 bucket.
        /// </summary>
        /// <param name="s3Client">The Amazon S3 client.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="rootPrefix">The root prefix to start listing directories.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of directory prefixes found under the root prefix.</returns>
        public static async Task<List<string>> ListDirectoriesAsync(this IAmazonS3 s3Client, string bucketName, string rootPrefix, CancellationToken cancellationToken = default)
        {
            var directories = new List<string>();
            var stack = new Stack<string>();
            stack.Push(rootPrefix);

            while (stack.Count > 0)
            {
                if (cancellationToken.IsCancellationRequested) return new List<string>();
                var currentPrefix = stack.Pop();

                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = currentPrefix,
                    Delimiter = "/",
                };

                bool isTruncated;

                ListObjectsV2Response? response;
                do
                {
                    if (cancellationToken.IsCancellationRequested) return new List<string>();
                    response = await s3Client.ListObjectsV2Async(request, cancellationToken);

                    if (response.CommonPrefixes != null)
                    {
                        foreach (var commonPrefix in response.CommonPrefixes)
                        {
                            directories.Add(commonPrefix);
                            stack.Push(commonPrefix);
                        }
                    }

                    isTruncated = response.IsTruncated;
                    request.ContinuationToken = response.NextContinuationToken;
                }
                while (isTruncated);
            }

            return directories.Where(p => p.Length > 0).ToList();
        }
    }

}
