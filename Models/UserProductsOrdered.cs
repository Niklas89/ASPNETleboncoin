namespace lebonanimal.Models
{
    public class UserProductsOrdered
    {
        public Order OrderVm { get; set; }
        public User UserVm { get; set; }
        public Product ProductVm { get; set; }
    }
}
