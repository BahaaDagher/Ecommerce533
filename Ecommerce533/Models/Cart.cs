using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce533.Models
{
    [PrimaryKey(nameof(ApplicationUserId) , nameof(ProductId))]
    public class Cart
    {
        public string  ApplicationUserId {  get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser {  get; set; }
        public int ProductId {  get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product {  get; set; }
        public int Count {  get; set; }
    }
}
