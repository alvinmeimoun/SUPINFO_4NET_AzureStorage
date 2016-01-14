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
        string ListFolders();

        [OperationContract]
        void UploadFile(UploadFileMessage msg);

        [OperationContract]
        void Uploadfile(string filename, string folder, byte[] data);
    }
}
