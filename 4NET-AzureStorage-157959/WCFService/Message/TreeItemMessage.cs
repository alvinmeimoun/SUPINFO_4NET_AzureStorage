using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WCFService.Message
{
    [DataContract]
    public class TreeItemMessage
    {
        private string _name;
        private bool _isDirectory;

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DataMember]
        public bool IsDirectory
        {
            get { return _isDirectory; }
            set { _isDirectory = value; }
        }
    }
}
