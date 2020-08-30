using Common;
using Dapper;
using Npgsql;
using Store.DTO.CartService;
using Store.Model.CartService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.DAL
{
    public class CartServiceRepository : BaseNpgsqlRepository, ICartServiceRepository
    {
        protected override string SchemaName => "public";

        public CartServiceRepository(string connectionString) : base(connectionString)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<CartDto> GetCart(string buyerId, int? page = null, int? pageSize = null, string orderBy = "")
        {
            var limitClause = BuildLimitClause(page, pageSize);

            var orderByClause = BuildOrderingClause<Product>(orderBy);

            var whereClause = $" WHERE buyer_id = '{buyerId}' {n}";

            var selectCartAggregatedDataClause =
                $"SELECT {n}" +
                $"	MAX(c.created) as created, {n}" +
                $"	MIN(c.edited) as edited, {n}" +
                $"  c.buyer_id {n}" +
                $"FROM {SchemaName}.carts c {n}" +
                $"{whereClause} {n}" +
                $"GROUP BY c.buyer_id {n}" +
                $"; {n}";


            var selectProductsClause =
                $"SELECT {n}" +
                $"    c.created,  {n}" +
                $"    c.edited,  {n}" +
                $@"	  c.""number"", {n}" +
                $"	  c.product_id, {n}" +
                $"	  c.buyer_id, {n}" +
                $"	  p.name as product_name, {n}" +
                $"	  p.cost, {n}" +
                $"	  p.for_bonus_points {n}" +
                $"FROM {SchemaName}.carts c {n}" +
                $"JOIN {SchemaName}.products p {n}" +
                $"    ON p.id = c.product_id {n}" +
                $"{whereClause} {n}" +
                $"    {orderByClause} {n}" +
                $"    {limitClause}; {n}" +
                $"; {n}";

            var countClause =
                $"SELECT {n}" +
                $"  COUNT(*) {n}" +
                $"FROM {SchemaName}.carts c {n}" +
                $"JOIN {SchemaName}.products p {n}" +
                $"    ON p.id = c.product_id {n}" +
                $"{whereClause}; {n}";


            await using var connection = new NpgsqlConnection(ConnectionString);

            var multi = await connection.QueryMultipleAsync(
                selectCartAggregatedDataClause + countClause + selectProductsClause,
                commandTimeout: SqlTimeout);

            var cart = await multi.ReadFirstOrDefaultAsync<CartDto>();

            if (cart != null)
            {
                var totalItems = await multi.ReadFirstOrDefaultAsync<Int64>();

                cart.Products = new PaginationWrapper<CartProductDto>(await multi.ReadAsync<CartProductDto>())
                {
                    Page = page ?? 0,
                    PageSize = page > 0
                    ? pageSize ?? 0
                    : 0,
                    TotalItems = (int)totalItems,
                };
            }

            return cart;
        }

        public async Task AddProducts(string buyerId, IEnumerable<AddProductToCartDto> products)
        {
            if (products?.Any() != true)
            {
                throw new ArgumentNullException($"{nameof(products)} is null or empty.");
            }

            var insertClause = string.Empty;

            foreach (var product in products)
            {
                insertClause +=
                    $"INSERT INTO {SchemaName}.carts " +
                    $"( {n}" +
                    $@" ""number"",  {n}" +
                    $"  product_id,  {n}" +
                    $"  buyer_id {n}" +
                    $") {n}" +
                    $"VALUES" +
                    $"( {n}" +
                    $"  {product.Number},  {n}" +
                    $"  {product.ProductId},  {n}" +
                    $"  '{buyerId}' {n}" +
                    $") {n}" +
                    $"ON CONFLICT(product_id, buyer_id) {n}" +
                    $"DO UPDATE SET {n}" +
                    $@" ""number"" = {SchemaName}.carts.""number"" + {product.Number}, {n}" +
                    $"  edited = now()" +
                    $"WHERE {SchemaName}.carts.buyer_id = '{buyerId}'; {n}";
            }

            await using var connection = new NpgsqlConnection(ConnectionString);
            try
            {
                connection.Open();
                await StartTransaction(connection);

                await connection.QueryAsync(
                    insertClause,
                    commandTimeout: SqlTimeout);

                await Commit(connection);
            }
            catch
            {
                await Rollback(connection);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task RemoveProducts(string buyerId, IEnumerable<AddProductToCartDto> products)
        {
            if (products?.Any() != true)
            {
                throw new ArgumentNullException($"{nameof(products)} is null or empty.");
            }

            var updateClause = string.Empty;

            foreach (var product in products)
            {
                updateClause +=
                    $"UPDATE {SchemaName}.carts  {n}" +
                    $"SET  {n}" +
                    $@" ""number"" = {SchemaName}.carts.""number"" - {product.Number}, {n}" +
                    $"  edited = now()  {n}" +
                    $"WHERE " +
                    $"  {SchemaName}.carts.buyer_id = '{buyerId}'  {n}" +
                    $"  AND {SchemaName}.carts.product_id = {product.ProductId}; {n}";
            }

            updateClause +=
                $"DELETE FROM {SchemaName}.carts " +
                $@"WHERE {SchemaName}.carts.""number"" < 1;";

            await using var connection = new NpgsqlConnection(ConnectionString);
            try
            {
                connection.Open();
                await StartTransaction(connection);

                await connection.QueryAsync(
                    updateClause,
                    commandTimeout: SqlTimeout);

                await Commit(connection);
            }
            catch
            {
                await Rollback(connection);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task Subscribe(IEnumerable<string> urls, string buyerId = "")
        {
            if (urls?.Any() != true)
            {
                throw new ArgumentNullException($"{nameof(urls)} is null or empty.");
            }

            var updateClause = string.Empty;

            foreach (var url in urls)
            {
                updateClause += $"INSERT INTO public.webhooks(buyer_id, url) {n}" +
                    $"VALUES({(string.IsNullOrWhiteSpace(buyerId) ? "null" : $"'{buyerId}'")}, '{url}') {n}" +
                    $"ON CONFLICT DO NOTHING;  {n}";
            }

            await using var connection = new NpgsqlConnection(ConnectionString);
            try
            {
                connection.Open();
                await StartTransaction(connection);

                await connection.QueryAsync(
                    updateClause,
                    commandTimeout: SqlTimeout);

                await Commit(connection);
            }
            catch
            {
                await Rollback(connection);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task Unsubscribe(IEnumerable<string> urls, string buyerId = "")
        {
            if (urls?.Any() != true)
            {
                throw new ArgumentNullException($"{nameof(urls)} is null or empty.");
            }

            var deleteClause = $"DELETE FROM public.webhooks w {n}";

            var urlsConditions = urls.Select(q => $" w.url = '{q}' ");
            var urlsConditionsString = string.Join(" OR ", urlsConditions);

            var buyerClause = (string.IsNullOrWhiteSpace(buyerId) ? "w.buyer_id IS NULL" : $"w.buyer_id = '{buyerId}'");

            var whereClause =
                $"WHERE {buyerClause} {n} " +
                $"  AND ({urlsConditionsString})";

            deleteClause += whereClause;

            await using var connection = new NpgsqlConnection(ConnectionString);
            try
            {
                connection.Open();
                await StartTransaction(connection);

                await connection.QueryAsync(
                    deleteClause,
                    commandTimeout: SqlTimeout);

                await Commit(connection);
            }
            catch
            {
                await Rollback(connection);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
