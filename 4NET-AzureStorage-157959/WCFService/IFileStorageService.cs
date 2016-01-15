using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WCFService.Message;

namespace WCFService
{
    [ServiceContract]
    public interface IFileStorageService
    {

        [OperationContract]
        string ListRootFolders();

        [OperationContract]
        string ListContent(string folderPath);

        [OperationContract]
        void UploadFile(UploadFileMessage msg);

        [OperationContract]
        void Uploadfile(string filename, string folderPath, byte[] data);

        [OperationContract]
        byte[] DownloadFile(string filename, string folderPath = "");

        [OperationContract]
        void CompressFolder(string folderPath);
    }
}
