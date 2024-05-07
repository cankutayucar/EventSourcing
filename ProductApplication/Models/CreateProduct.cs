namespace ProductApplication.Models
{
    public class CreateProduct
    {
        public string ProductName { get; set; }
        public int Count { get; set; }
        public bool IsAvaliable { get; set; }
        public decimal Price { get; set; }
    }
}
