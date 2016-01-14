using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFService.Message
{
    [MessageContract]
    public class UploadFileMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public UploadFileMetadata Metadata;

        [MessageBodyMember(Order = 1)]
        public Stream FileData;
    }
}
