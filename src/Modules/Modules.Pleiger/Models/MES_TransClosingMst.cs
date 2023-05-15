namespace Modules.Pleiger.Models
{
    public class MES_TransClosingMst
    {
        public int No { get; set; }
        public string TransMonth { get; set; }
        public bool POTrans { get; set; }
        public bool PJTrans { get; set; }
        public bool ItemSlipTrans { get; set; }
        public bool InventoryClosedYN { get; set; }
        public string State { get; set; }
    }
}
