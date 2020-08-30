using Store.Model.CartService;

namespace CartService.ViewModel
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public bool ForBonusPoints { get; set; }

        public static explicit operator Product(ProductViewModel viewModel)
        {
            return new Product
            {
                Cost = viewModel.Cost,
                ForBonusPoints = viewModel.ForBonusPoints,
                Id = viewModel.Id,
                Name = viewModel.Name,
            };
        }

        public static explicit operator ProductViewModel(Product model)
        {
            return new ProductViewModel
            {
                Cost = model.Cost,
                ForBonusPoints = model.ForBonusPoints,
                Id = model.Id,
                Name = model.Name,
            };
        }
    }
}
