using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels
{
    public class MES_ProjectProdcnLinesStatusChart
    {
        public string ProjectCode { get; set; }
        public string ProdcnCode { get; set; }
        public int OrderQty { get; set; }
        public DateTime PlanDeLiveryDate { get; set; }
        public List<MES_ProjectProdcnLinesChart> MES_ProjectProdcnLinesChart { get; set; }

    }
    public class MES_ProjectProdcnLinesChart
    {
        public string ProjectCode { get; set; }
        public string ProdcnLineCode { get; set; }
        public int AssignedQty { get; set; }
        public DateTime ProdLineStartDate { get; set; }
        public List<MES_ProjectProdcnWorkResultsChart> MES_ProjectProdcnWorkResultsChart { get; set; }

    }

    public class MES_ProjectProdcnWorkResultsChart
    {
        public string ProjectCode { get; set; }
        public string ProdcnLineCode { get; set; }
        public int OriginalNumber { get; set; }
        
        public int AssignedQty { get; set; }
        public int ProdDoneQty { get; set; }
        public int WorkDoneQty { get; set; }

        public DateTime ProdLineDoneDate { get; set; }
        public DateTime CreateAt { get; set; }

    }

}
