using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.DynamoDBv2.DocumentModel;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace group_13_YenTing_Favour__Lab_3.Services
{
    public class AWSUtil
    {
        public readonly static IAmazonS3 s3Client;
        private static readonly IConfiguration _config;
        public readonly static string bucketName;
        public readonly static AmazonDynamoDBClient dynamoClient;
        public readonly static Table userTable;

        // static constructor runs once, the first time any member of Helper is accessed
        static AWSUtil()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(AppContext.BaseDirectory)
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _config = builder.Build();
            s3Client = GetS3Client();
            bucketName = _config["AWS:BucketName"];
            dynamoClient = GetDynamoClient();
            userTable = LoadUserTable(_config["AWS:DynamoDBName"]); // <-- change to your DynamoDB table name
        }
        
        private static IAmazonS3 GetS3Client()
        {
            var accessId = _config["AWS:AccessId"];
            var secretKey = _config["AWS:SecretKey"];
            var region = _config["AWS:Region"];

            return new AmazonS3Client(accessId, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
        }

        private static AmazonDynamoDBClient GetDynamoClient()
        {
            var credentials = new BasicAWSCredentials(
                _config["AWS:AccessId"],
                _config["AWS:SecretKey"]);
            return new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);
        }

        private static Table LoadUserTable(string tableName)
        {
            // loads metadata for the given DynamoDB table
            return Table.LoadTable(dynamoClient, tableName, DynamoDBEntryConversion.V2);
        }
    }
}
