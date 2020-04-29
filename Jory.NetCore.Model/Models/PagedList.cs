using System.Collections.Generic;

namespace Jory.NetCore.Model.Models
{
    public class PagedList<T> where T : class, new()
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 30;
        public int PageCount { get; set; } = 0;
        public int RowCount { get; set; } = 0;
        public bool HasPaged { get; set; } = true;
        public List<T> DataList { get; set; }
    }
}
