using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WCFService.Utils
{
    public class ZipUtils
    {
        public static void UploadZipToBlockBlob(ZipFile zip, CloudBlockBlob blockBlob)
        {
            using (var zipStream = new MemoryStream())
            {
                zip.Save(zipStream);
                zipStream.Seek(0, SeekOrigin.Begin);

                blockBlob.UploadFromStream(zipStream);
            }
        }

        public static ZipFile CompressBlobDirectory(CloudBlobDirectory directory)
        {
            using (ZipFile zip = new ZipFile())
            {
                AddBlobDirectoryToZip(directory, zip);
                return zip;
            }
        }

        private static void AddBlobDirectoryToZip(CloudBlobDirectory directory, ZipFile zip, string currentPath = "")
        {
            if (!string.IsNullOrWhiteSpace(currentPath)) zip.AddDirectoryByName(currentPath);
            
            var blobs = directory.ListBlobs();
            foreach (var blob in blobs)
            {
                if (blob is CloudBlobDirectory)
                {
                    var subDirName = directory.Prefix.Substring(0, directory.Prefix.Length - 1);
                    string subDirPath;
                    if (string.IsNullOrWhiteSpace(currentPath)) subDirPath = subDirName;
                    else subDirPath = string.Format("{0}/{1}", currentPath, subDirName);

                    AddBlobDirectoryToZip((CloudBlobDirectory) blob, zip, subDirPath);
                }
                else if (blob is CloudBlockBlob)
                {
                    var blockBlob = (CloudBlockBlob) blob;
                    var blobStream = new MemoryStream();
                    blockBlob.DownloadToStream(blobStream, AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions(), new OperationContext());

                    var zippedBlobStream = new Ionic.Zlib.GZipStream(blobStream, Ionic.Zlib.CompressionMode.Compress);

                    zip.AddEntry(blockBlob.Name, (name, stream) => zippedBlobStream.CopyTo(stream));
                }
            }
        }

        public static byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        public static Stream DecompressStream(Stream compressedStream)
        {
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream;
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
