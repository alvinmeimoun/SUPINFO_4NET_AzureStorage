using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using WCFService.Message;

namespace WCFService
{

    public class FileStorageService : IFileStorageService
    {
        private static readonly string ROOT_CONTAINER = "main-local-storage";

        public string ListFolders()
        {
            var blobs = getRootContainer().ListBlobs();

            var rList = new List<FolderMessage>();
            foreach(var blob in blobs)
            {
                var localPath = blob.Uri.LocalPath;
                var name = localPath.Substring(0, localPath.LastIndexOf('/'));
                name = name.Substring(name.LastIndexOf('/') + 1);

                rList.Add(new FolderMessage
                {
                    Name = name
                });
            }

            return JsonConvert.SerializeObject(rList);
        }

        public void UploadFile(UploadFileMessage msg)
        {
            var directory = getDirectory(msg.Metadata.Folder);
            CloudBlockBlob blob = directory.GetBlockBlobReference(msg.Metadata.FileName);
            blob.UploadFromStream(msg.FileData);
        }

        //Created to be used with WcfTestClient (Stream call not compatible with WcfTestClient)
        public void Uploadfile(string filename, string folder, byte[] data)
        {
            UploadFile(new UploadFileMessage
            {
                FileData = new MemoryStream(data),
                Metadata = new UploadFileMetadata
                {
                    FileName = filename,
                    Folder = folder
                }
        });
        }

        private CloudBlobClient getBlobClient()
        {
            var account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString"));
            return account.CreateCloudBlobClient();
        }

        private CloudBlobContainer getRootContainer()
        {
            return getBlobClient().GetContainerReference(ROOT_CONTAINER);
        }

        private CloudBlobDirectory getDirectory(string folderName)
        {
            return getRootContainer().GetDirectoryReference(folderName);
        }

    }
}
