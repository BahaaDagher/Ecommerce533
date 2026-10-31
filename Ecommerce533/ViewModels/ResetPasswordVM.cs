using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization.Metadata;

namespace Ecommerce533.ViewModels
{
    public class ResetPasswordVM
    {
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password) ,Compare(nameof (NewPassword))]
        public string ConfirmPassword { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
