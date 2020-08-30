using Common.Interfaces;
using Dapper;
using Npgsql;
using Store.Model.CartService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.DAL
{
    public class CartTimeoutServiceRepository : BaseNpgsqlRepository, ICartTimeoutServiceRepository
    {
        private readonly IDateTimeService _dateTimeService;

        protected override string SchemaName => "public";

        public CartTimeoutServiceRepository(IDateTimeService dateTimeService, string connectionString) : base(connectionString)
        {
            _dateTimeService = dateTimeService;
        }

        public async Task RemoveOldCarts(int daysLimit)
        {
            if (daysLimit < 1)
            {
                throw new ArgumentOutOfRangeException($"{nameof(daysLimit)} must to be above zero.");
            }

            var thirtyDaysClause = _dateTimeService.Now().AddDays(-daysLimit).ToString("yyyyMMdd");

            var deleteClause = 
                $"DELETE FROM public.carts c  {n}" +
                $"WHERE c.buyer_id in  {n}" +
                $"( {n}" +
                $"  SELECT DISTINCT buyer_id {n}" +
                $"  FROM public.carts c {n}" +
                $"  WHERE c.created < '{thirtyDaysClause}' {n}" +
                $"); {n}";

            await using var connection = new NpgsqlConnection(ConnectionString);

            await connection.QueryAsync(deleteClause, commandTimeout: SqlTimeout);
        }

        public async Task<List<Webhook>> GetHooks(int daysLimit)
        {
            var selectClause =
                $"SELECT * FROM public.webhooks w  {n}" +
                $"WHERE w.buyer_id in  {n}" +
                $"( {n}" +
                $"  SELECT DISTINCT buyer_id {n}" +
                $"  FROM public.carts c {n}" +
                $"  WHERE c.created < '{_dateTimeService.Now().AddDays(-daysLimit):yyyyMMdd}' {n}" +
                $");";

            await using var connection = new NpgsqlConnection(ConnectionString);

            return (await connection.QueryAsync<Webhook>(selectClause, commandTimeout: SqlTimeout))?.ToList();
        }
    }
}
