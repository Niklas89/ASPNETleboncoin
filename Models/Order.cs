using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lebonanimal.Models;

    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("idUser")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Orders))]
        public User UserIdNavigation { get; set; }

        [Column("idProduct")]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty(nameof(Product.Orders))]
        public Product ProductIdNavigation { get; set; }
    }

