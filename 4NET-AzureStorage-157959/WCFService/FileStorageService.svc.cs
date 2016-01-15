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
using WCFService.Utils;

namespace WCFService
{

    public class FileStorageService : IFileStorageService
    {
        public string ListRootFolders()
        {
            return ListContent("");
        }

        public string ListContent(string folderPath)
        {
            var directory = BlobUtils.getDirectory(folderPath);
            var blobs = directory.ListBlobs();

            var rList = new List<TreeItemMessage>();
            foreach (var blob in blobs)
            {
                string name;
                if (blob.GetType() == typeof (CloudBlobDirectory))
                {
                    name = ((CloudBlobDirectory) blob).Prefix;
                } else if (blob.GetType() == typeof (CloudBlockBlob))
                {
                    name = ((CloudBlockBlob) blob).Name;
                }
                else
                {
                    name = blob.Uri.LocalPath;
                }

                bool isDirectory = name.EndsWith("/");
                if (isDirectory) name = name.Substring(0, name.Length - 1);

                if (name.Contains("/")) name = name.Substring(name.LastIndexOf('/')+1);

                rList.Add(new TreeItemMessage
                {
                    Name = name,
                    IsDirectory = isDirectory
                });
            }

            return JsonConvert.SerializeObject(rList);
        }

        public void UploadFile(UploadFileMessage msg)
        {
            var directory = BlobUtils.getDirectory(msg.Metadata.Folder);
            CloudBlockBlob blob = directory.GetBlockBlobReference(msg.Metadata.FileName);
            blob.UploadFromStream(msg.FileData);
        }

        //Created to be used with WcfTestClient (Stream call not compatible with WcfTestClient)
        public void Uploadfile(string filename, string folderPath, byte[] data)
        {
            if(string.IsNullOrWhiteSpace(folderPath)) throw new ArgumentException("Can't add anything to root folder");

            UploadFile(new UploadFileMessage
            {
                FileData = new MemoryStream(data),
                Metadata = new UploadFileMetadata
                {
                    FileName = filename,
                    Folder = folderPath
                }
        });
        }

        public byte[] DownloadFile(string filename, string folderPath = "")
        {
            var directory = BlobUtils.getDirectory(folderPath);
            var blob = directory.GetBlobReference(filename);

            blob.FetchAttributes();
            long fileSize = blob.Properties.Length;

            byte[] fileData = new byte[fileSize];

            blob.DownloadToByteArray(fileData, 0);
            return fileData;
        }

        public void CompressFolder(string folderPath)
        {
            if(string.IsNullOrWhiteSpace(folderPath)) throw new ArgumentException("Folder path must be specified");

            var directory = BlobUtils.getDirectory(folderPath);
            var directoryName = directory.Prefix.Substring(0, directory.Prefix.Length - 1);

            using (var zip = ZipUtils.CompressBlobDirectory(directory))
            {
                var archivesDir = BlobUtils.getDirectory("archives");
                var zipBlob = archivesDir.GetBlockBlobReference(directoryName+".zip");

                ZipUtils.UploadZipToBlockBlob(zip, zipBlob);
            }
        }
    }
}
