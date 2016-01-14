using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WCFService.Utils
{
    public class BlobUtils
    {
        private static readonly string ROOT_CONTAINER = "main-local-storage";

        public static CloudBlobClient getBlobClient()
        {
            var account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString"));
            return account.CreateCloudBlobClient();
        }

        public static CloudBlobContainer getRootContainer()
        {
            return getBlobClient().GetContainerReference(ROOT_CONTAINER);
        }

        public static CloudBlobDirectory getDirectory(string folderPath)
        {
            return getRootContainer().GetDirectoryReference(folderPath);
        }
    }
}
