using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace UploadMultipleFilesInMVC.Services
{

    public class StorageDataModel
    {
        public string FileStorageConnectionString { get; set; }
        public string FileReference { get; set; }
        public string FileStorageName { get; set; }
        public byte[] FileData { get; set; }
    }

    public class StorageAPIService
    {

        public async Task<bool> DoUploadSourceToAzureFileStorage(StorageDataModel storageDataModel)
        {
            try
            {
                var storageAccountConnectionString_File = CloudStorageAccount.Parse(storageDataModel.FileStorageConnectionString);
                CloudFileClient cloudFileClient = storageAccountConnectionString_File.CreateCloudFileClient();
                CloudFileShare cloudFileShare = cloudFileClient.GetShareReference(storageDataModel.FileReference);
                if (cloudFileShare.Exists())
                {
                    CloudFileDirectory rootDir = cloudFileShare.GetRootDirectoryReference();
                    CloudFile fileSas = rootDir.GetFileReference(storageDataModel.FileStorageName);
                    fileSas.UploadFromStream(new MemoryStream(storageDataModel.FileData));
                }
            }
            catch(Exception ex)
            {
                return await Task.FromResult<bool>(false);
            }
            return await Task.FromResult<bool>(true);
        }
    }
}