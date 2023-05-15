using AutoMapper;
using Modules.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.FileUpload.Mappers
{
    public class FileMapConfig : Profile
    {
        public FileMapConfig()
        {
            CreateMap<ChunkMetadata,SYFileUpload>();            
        }
    }

    //public class ChunkMetadata
    //{
    //    public int Index { get; set; }
    //    public int TotalCount { get; set; }
    //    public int FileSize { get; set; }
    //    public string FileName { get; set; }
    //    public string FileType { get; set; }
    //    public string FileGuid { get; set; }
    //}
}
