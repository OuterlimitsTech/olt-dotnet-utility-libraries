<img src="https://user-images.githubusercontent.com/1365728/127748628-47575d74-a2fb-4539-a31e-74d8b435fc21.png" width="30%" >

# .NET Utility Library for S3

<code>OltS3Extenstions</code> provides simple extenstions for common S3 tasks

[![Nuget](https://img.shields.io/nuget/v/OLT.Utility.S3)](https://www.nuget.org/packages/OLT.Utility.S3)
[![CI](https://github.com/OuterlimitsTech/olt-dotnet-utility-libraries/actions/workflows/build.yml/badge.svg)](https://github.com/OuterlimitsTech/olt-dotnet-utility-libraries/actions/workflows/build.yml) 


### Installation

Add the S3 Extensions source files to your .NET project or include it as a class library.

```bash
dotnet add package OLT.Utility.S3
```


### Usage

Create Bucket Example

```csharp
using OLT.Utility.S3

using (var s3Client = new AmazonS3Client(Credentials, Config))
{
    var bucketName = "MyStuff";
    var result = await s3Client.CreateBucketIfNotExistsAsync(bucketName);

    if (result.Success) 
    {
     // Code Here
    }
}
```


Get All Objects in path

```csharp
using OLT.Utility.S3

using (var s3Client = new AmazonS3Client(Credentials, Config))
{
    var bucketName = "MyStuff";
    var rootPrefix = "/Test";
    var result = await s3Client.GetAllAsync(bucketName, rootPrefix, cancellationToken);

    if (result.Success) 
    {
     // Code Here
    }
}
```

Get Object
```csharp
using OLT.Utility.S3

using (var s3Client = new AmazonS3Client(Credentials, Config))
{
    var bucketName = "MyStuff";
    var objectKey = "/Test/MyFile.txt";
    var result = await s3Client.GetAsync(bucketName, objectKey, cancellationToken);

    
    if (result.Success) 
    {
     // Code Here
    }
}
```
