using _25dh110089_MyStore.Models;
using System.Collections.Generic;

namespace _25dh110089_MyStore.Models // Sửa lại namespace theo tên project của bạn
{
    public class MyOrderViewModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public string StatusName { get; set; } // Tên trạng thái hiển thị
        public int TotalItems { get; set; } // Tổng số lượng sản phẩm
    }
}