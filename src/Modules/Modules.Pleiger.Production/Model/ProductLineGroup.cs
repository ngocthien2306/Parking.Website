using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Model
{
    public class ProductLineGroup
    {
        public int PlanRequestQty { get; set; }
        public string ProductionProjectCode { get; set; }
        public int GroupLine { get; set; }
        private List<MES_ProjectProdcnLines> prodcnLines = new List<MES_ProjectProdcnLines>();
        public List<MES_ProjectProdcnLines> ProdcnLines { get { return prodcnLines; } }
        public bool btnSave { get; set; }
        public int GroupDoneQty { get; set; }
        public bool isCompleted { get; set; }
    }
}
