using InfrastructureCore.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Models.WidgetModels
{
    public class SYWidgetElementDetail : HLCoreCrudModel
    {
        public string WidgetNumber { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public bool IsExport { get; set; }
        public int Row { get; set; }
        public int Widget_xs { get; set; }
        public int Widget_sm { get; set; }
        public int Widget_md { get; set; }
        public int Widget_lg { get; set; }
        public bool IsUse { get; set; }
        public int NumberOfSeries { get; set; }
        //
        public string ValueField1 { get; set; }
        public string ValueField2 { get; set; }
        public string ValueField3 { get; set; }
        public string ValueField4 { get; set; }
        public string NameField1 { get; set; }
        public string NameField2 { get; set; }
        public string NameField3 { get; set; }
        public string NameField4 { get; set; }
        //
        public DateTime CREATED_AT { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime UPDATED_AT { get; set; }
        public string UPDATED_BY { get; set; }

    }
}
