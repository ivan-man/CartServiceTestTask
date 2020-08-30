using System;
using System.Collections.Generic;
using System.Text;

namespace Store.DTO.CartReportService
{
    public class CartReportDto
    {
        public int TotalCarts { get; set; }
        public int BonusPointsCarts { get; set; }

        public int TenDaysCarts { get; set; }
        public int TwentyDaysCarts { get; set; }
        public int ThirtyDaysCarts { get; set; }

        public decimal AverrageCartSum { get; set; }
    }
}
