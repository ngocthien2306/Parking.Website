using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_ClientSoundMgt
    {
        public int? no { get; set; }
        public int soundNo { get; set; }
        public string soundName { get; set; }
        public string localFileLocation { get; set; }
        public string kioskFolderLocation { get; set; }
        public bool deployStatus { get; set; }
        public DateTime? registDate { get; set; }
        public string resitUser { get; set; }
        public string statusName { get; set; }
        public bool isActivity { get; set; }
        public DateTime? createdDate { get; set; }
        public string createdBy { get; set; }
        public string lastModifiedBy { get; set; }
        public DateTime? lastModified { get; set; }
        public string soundType { get; set; }
        public string typeName { get; set; }
        public int Version { get; set; }
    }
    public class ResultAudioDto
    {
        public bool Success { get; set; }
        public string SoundName { get; set; }
        public string SoundNo { get; set; }
        public string Message { get; set; }
        public byte[] Data { get; set; }


    } 
    public class SoundNoDto
    {
        public string soundNo { get; set; }
    }
    public class SoundDto
    {
        public string soundNo { get; set; }
        public string soundName { get; set; }
        public string localFileLocation { get; set; }
        public string soundType { get; set; }
    }
    public class DeployDto
    {
        public string soundNo { get; set; }
        public string soundType { get; set; }
        public string extension { get; set; }
        public byte[] source { get; set; }
    }
    public class tblTempDeploy
    {
        public int SoundNo { get; set; }
        public int StoreNo { get; set; }
        public int DeviceNo { get; set; }
        public int DeployNo { get; set; }
        public byte[] Source { get; set; }
    }
}
