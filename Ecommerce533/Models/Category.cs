using Ecommerce533.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce533.Models
{
    public class Category
    {
        public int Id { get; set; }
        //[MaxLength(30)]
        //[MinLength(2)]
        [MinMaxLengthCustom(2 , 30)]
        public string Name { get; set; }
        public string? Description  { get; set; }
        public bool Status { get; set; }
    }
}
