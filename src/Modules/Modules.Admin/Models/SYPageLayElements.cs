using System.Collections.Generic;

namespace Modules.Admin.Models
{
    public class SYPageLayElements
    {
        public long ID { get; set; }
        public int PAG_ID { get; set; }
        public string PEL_ID { get; set; }
        public string PEL_ID_REAL { get; set; }
        public bool IS_KEY { get; set; }
        public string PEL_LBL { get; set; }
        public string PEL_TYP { get; set; }
        public string PAGING_TYP { get; set; }
        public string PEL_PRN { get; set; }
        public string PEL_DAT_TYPE { get; set; }
        public int PEL_LEN { get; set; }
        public int PEL_COL { get; set; }
        public int PEL_ROW { get; set; }
        public int PEL_CSPN { get; set; }
        public int PEL_RSPN { get; set; }
        public int PEL_WDT { get; set; }
        public int PEL_HGT { get; set; }
        public bool PEL_VIS { get; set; }
        public bool PEL_MAPYN { get; set; }
        public bool PEL_BINDYN { get; set; }
        public int PEL_SEQ { get; set; }
        public string PEL_DFVALUE { get; set; }
        public int PEL_XS { get; set; }
        public int PEL_SM { get; set; }
        public int PEL_MD { get; set; }
        public int PEL_LG { get; set; }
        public bool PEL_FIX { get; set; }
        public string PEL_FORL { get; set; }
        public string PEL_ALGN { get; set; }
        public int? PEL_CLICK { get; set; }
        public int? PEL_DBLCLICK { get; set; }
        public int PEL_REF_PAG_ID { get; set; }
        //public List<DataMapInfo> listDataMap { get; set; }
        public List<SYDataMap> listDataMap { get; set; }
        public List<SYPageLayElements> listElementChild { get; set; }
        public List<SYPageActions> listAction { get; set; }
        public List<SYPageLayElementReference> listReference { get; set; }
        public string PAG_KEY { get; set; }
        //--2020-04-17: MINH: Parse GroupCode value to render view
        //Center = 0,
        //Left = 1,
        //Right = 2
        public int ALIGN_COLUMN_PARSE { get; set; }
        public bool IS_EDIT { get; set; }
        // QUAN ADD 2021-03-07
        public bool IS_CREATE { get; set; }

        public bool IS_UPDATE { get; set; }

        public bool IS_DELETE { get; set; }     
        
        // END
        public string EDIT_TYPE { get; set; }
        public int EDIT_ACT { get; set; }
        public string PEL_EXP_TEXT { get; set; } // Math expression, JS render function
        public string GRP_CD { get; set; } // Group code for render select box 
        public string GRP_CD_CUSTOM { get; set; } // Group code for render select box with custom SP
        public string SP_CUSTOM_REFER { get; set; } // Custom SP to get data for another column display

        public bool PEL_IS_REQUIRED { get; set; } // Element required
        public string CONNECTION_NM { get; set; } // Conenction for fet group code, sp combobox, radio checkbox
        public string CUSTM_VIEW { get; set; } // Custom view .cshtml file
        public string PEL_VALIDATE_RULE_API { get; set; } // API Check validate
        public string PEL_VALIDATE_RULE_API_MSG { get; set; } // API Check validate return Messsage
        public string PEL_VALIDATE_REGULAR_EXP { get; set; } // Validate rugular exp custom
        public string PEL_VALIDATE_REGULAR_EXP_MSG { get; set; } // Validate rugular exp custom return Messsage
        public string VAL_FRM_PEL_ID { get; set; }
        public string GRID_MODE_EDIT { get; set; } // Grid mode edit cell/ batch/ row
        // list pel element refer item parent, using for autocomplete, combobox. Select parent to show another data
        public string LST_PEL_REFER { get; set; } 
        public SYPageLayElements()
        {
            //this.listDataMap = new List<DataMapInfo>();
            this.listElementChild = new List<SYPageLayElements>();
            this.listAction = new List<SYPageActions>();
            this.listDataMap = new List<SYDataMap>();
        }
    }
}
