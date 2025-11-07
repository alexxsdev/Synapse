namespace WebApi.Example
{
    public interface IProductService
    {
        Task<List<Product>> SearchProducts(string query);
    }
}
