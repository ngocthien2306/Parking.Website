using Modules.Admin.Models;

namespace Modules.UIRender.Models
{
    public class ColumnGridModel
    {
        public SYPageLayElements ColData { get; set; }
        public DevExtreme.AspNet.Mvc.Factories.CollectionFactory<DevExtreme.AspNet.Mvc.Builders.DataGridColumnBuilder<object>> ColInfo { get; set; }
    }
}
