using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WCFService
{

    public class FileStorageService : IFileStorageService
    {
        public string GetData(int value)
        {
            var localResource = RoleEnvironment.GetLocalResource("MainLocalStorage");
            var rootPath = localResource.RootPath;
            return string.Format("Test: {0}", rootPath);
        }

        //public CompositeType GetDataUsingDataContract(CompositeType composite)
        //{
        //    if (composite == null)
        //    {
        //        throw new ArgumentNullException("composite");
        //    }
        //    if (composite.BoolValue)
        //    {
        //        composite.StringValue += "Suffix";
        //    }
        //    return composite;
        //}
    }
}
