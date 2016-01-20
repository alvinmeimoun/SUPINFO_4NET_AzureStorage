using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using WCFService.Utils;

namespace BackupsRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Définir le nombre maximum de connexions simultanées
            ServicePointManager.DefaultConnectionLimit = 12;

            // Pour plus d'informations sur la gestion des modifications de configuration
            // consultez la rubrique MSDN à l'adresse http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("BackupsRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("BackupsRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("BackupsRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var delay = 60000; //TODO get this property from config file

            while (!cancellationToken.IsCancellationRequested)
            {
                doBackups();

                await Task.Delay(delay);
            }
        }

        private void doBackups()
        {
            var directory = BlobUtils.getDirectory("");
            var zipFileName = "backup-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";

            using (var zip = ZipUtils.CompressBlobDirectory(directory, excludedRootDirectories: new List<string>{"backups"}))
            {
                var archivesDir = BlobUtils.getDirectory("backups");
                var zipBlob = archivesDir.GetBlockBlobReference(zipFileName);

                ZipUtils.UploadZipToBlockBlob(zip, zipBlob);
            }
        }
    }
}
