using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace InfrastructureCore.Common
{
    public abstract class HLCoreCrudModel
    {
        [JsonProperty("__deleted__")]
        public bool __deleted__ { get; set; }

        [JsonProperty("__created__")]
        public bool __created__ { get; set; }

        [JsonProperty("__modified__")]
        public bool __modified__ { get; set; }

        public DataStatus getDataStatus()
        {
            if (__deleted__)
            {
                return DataStatus.DELETED;
            }

            if (__created__)
            {
                return DataStatus.CREATED;
            }

            if (__modified__)
            {
                return DataStatus.MODIFIED;
            }

            return DataStatus.ORIGIN;
        }
    }

    public enum DataStatus
    {
        CREATED,
        MODIFIED,
        DELETED,
        ORIGIN
    }
}
