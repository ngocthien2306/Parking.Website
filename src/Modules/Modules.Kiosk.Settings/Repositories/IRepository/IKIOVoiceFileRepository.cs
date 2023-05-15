using InfrastructureCore;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Management.Repositories.IRepository
{
    public interface IKIOVoiceFileRepository
    {
        public List<KIO_ClientSoundMgt> GetListAudioFile(string id, string name);
        public List<KIO_StoreDeployHistory> GetSoundDeployHist(string soundId);

        public Result UpdateVersionAudio(string soundNo);
        public Result SaveTempDeploy(string storeNo, string deviceNo, string soundNo, byte[] source);
        public Result UpdateTempDeploy(string soundNo);

        public Result SaveAudioFile(KIO_ClientSoundMgt clientSoundMgt, string userId);
        public Result DeleteAudioFile(string soundId);
    }
}
