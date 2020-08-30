using Common.Interfaces;
using Dapper;
using Npgsql;
using Store.DTO.CartReportService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Store.DAL.Repositories
{
    public class CartReportServiceRepository : BaseNpgsqlRepository, ICartReportServiceRepository
    {
        protected override string SchemaName => "public";

        private readonly IDateTimeService _dateTimeService;

        public CartReportServiceRepository(IDateTimeService dateTimeService, string connectionString) : base(connectionString)
        {
            _dateTimeService = dateTimeService;
        }


        public async Task<CartReportDto> GetReport()
        {
            //public int TenDaysCarts { get; set; }
            //public int TwentyDaysCarts { get; set; }
            //public int ThirtyDaysCarts { get; set; }

            //public decimal AverrageCartSum { get; set; }

            var totalCartsClause = 
                $"SELECT COUNT(DISTINCT c.buyer_id) FROM {SchemaName}.carts c;";

            var bonusPointsCartsClause =
                $"SELECT COUNT(DISTINCT c.buyer_id) {n}" +
                $"FROM {SchemaName}.carts c {n}" +
                $"JOIN {SchemaName}.products p {n}" +
                $"  ON p.id = c.product_id {n}" +
                $"WHERE p.for_bonus_points = true {n};";

            await using var connection = new NpgsqlConnection(ConnectionString);

            var multi = await connection.QueryMultipleAsync(
                totalCartsClause + bonusPointsCartsClause,
                commandTimeout: SqlTimeout);

            var totalCarts = await multi.ReadFirstOrDefaultAsync<int>();
            var bonusPointsCarts = await multi.ReadFirstOrDefaultAsync<int>();

            return new CartReportDto
            {
                TotalCarts = totalCarts,
                BonusPointsCarts = bonusPointsCarts,
            };
        }
    }
}
