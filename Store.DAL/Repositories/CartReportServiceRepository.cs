using Common.Interfaces;
using Dapper;
using Npgsql;
using Store.DTO.CartReportService;
using System;
using System.Threading.Tasks;

namespace Store.DAL
{
    public class CartReportServiceRepository : BaseNpgsqlRepository, ICartReportServiceRepository
    {
        protected override string SchemaName => "public";

        private readonly IDateTimeService _dateTimeService;

        public CartReportServiceRepository(IDateTimeService dateTimeService, string connectionString) : base(connectionString)
        {
            _dateTimeService = dateTimeService;
        }

        public async Task<CartReportDto> GetReportData()
        {
            var totalCartsClause =
                $"SELECT COUNT(DISTINCT c.buyer_id) FROM {SchemaName}.carts c;";

            var bonusPointsCartsClause =
                $"SELECT COUNT(DISTINCT c.buyer_id) {n}" +
                $"FROM {SchemaName}.carts c {n}" +
                $"JOIN {SchemaName}.products p {n}" +
                $"  ON p.id = c.product_id {n}" +
                $"WHERE p.for_bonus_points = true {n};";

            var daysReportClausePattern =
                $"SELECT COUNT(DISTINCT buyer_id) {n}" +
                $"FROM public.carts c {n}" +
                "WHERE c.created > '{0}'; " + $" {n}";

            var tenDaysDateClause = _dateTimeService.Now().AddDays(-10).ToString("yyyyMMdd");
            var tenDaysCartsClause = string.Format(daysReportClausePattern, tenDaysDateClause);

            var twentyDaysDateClause = _dateTimeService.Now().AddDays(-20).ToString("yyyyMMdd");
            var twentyDaysCartsClause = string.Format(daysReportClausePattern, twentyDaysDateClause);

            var thirtyDaysDateClause = _dateTimeService.Now().AddDays(-30).ToString("yyyyMMdd");
            var thirtyDaysCartsClause = string.Format(daysReportClausePattern, thirtyDaysDateClause);

            var avgCartClause =
                $"SELECT AVG(t.check) {n}" +
                $"FROM {n}" +
                $"( {n}" +
                $"    SELECT {n}" +
                $"        SUM(p.cost* c.number) as check {n}" +
                $"   FROM public.carts c {n}" +
                $"   JOIN public.products p {n}" +
                $"      ON p.id = c.product_id {n}" +
                $"   GROUP BY c.buyer_id {n}" +
                $") t ;{n}";

            await using var connection = new NpgsqlConnection(ConnectionString);

            var multi = await connection.QueryMultipleAsync(
                totalCartsClause
                + bonusPointsCartsClause
                + tenDaysCartsClause
                + twentyDaysCartsClause
                + thirtyDaysCartsClause
                + avgCartClause,
                commandTimeout: SqlTimeout);

            var totalCarts = (int)await multi.ReadFirstOrDefaultAsync<Int64>();
            var bonusPointsCarts = (int)await multi.ReadFirstOrDefaultAsync<Int64>();
            var tenDaysCarts = (int)await multi.ReadFirstOrDefaultAsync<Int64>();
            var twentyDaysCarts = (int)await multi.ReadFirstOrDefaultAsync<Int64>();
            var thirtyDaysCarts = (int)await multi.ReadFirstOrDefaultAsync<Int64>();
            var avgCart = await multi.ReadFirstOrDefaultAsync<decimal>();

            return new CartReportDto
            {
                TotalCarts = totalCarts,
                BonusPointsCarts = bonusPointsCarts,
                TenDaysCarts = tenDaysCarts,
                TwentyDaysCarts = twentyDaysCarts,
                ThirtyDaysCarts = thirtyDaysCarts,
                AverrageCartSum = avgCart,
            };
        }
    }
}
