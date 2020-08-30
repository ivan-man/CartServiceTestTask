using Store.DTO.CartReportService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Store.DAL
{
    public interface ICartReportServiceRepository
    {
        public Task<CartReportDto> GetReportData();
    }
}
