﻿using InfrastructureCore.Common;
using System.Collections.Generic;

namespace Modules.Pleiger.CommonModels
{
    public class MES_ItemPartnerResult : HLCoreCrudModel
    {
        public List<MES_ItemPartner> listFullPartner { get; set; } 
        public List<MES_ItemPartner> listDistinctPartner { get; set; } 
    }
}