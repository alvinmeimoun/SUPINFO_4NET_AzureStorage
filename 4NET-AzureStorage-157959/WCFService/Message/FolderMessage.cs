using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFService.Message
{
    [DataContract]
    public class FolderMessage
    {
        private string name;

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}