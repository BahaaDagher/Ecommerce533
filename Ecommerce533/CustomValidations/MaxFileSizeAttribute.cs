using System.ComponentModel.DataAnnotations;

namespace Ecommerce533.CustomValidations
{
    public class MaxFileSizeAttribute :  ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxSize) {
            _maxFileSize = maxSize * 1024 * 1024;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"the max size to upload a file is {_maxFileSize / 1024 / 1024} mb";
        }
        public override bool IsValid(object? value)
        {
            if(value is IFormFile file)
            {
                if(file.Length>0 && file.Length <=_maxFileSize)
                    return true; 
            }


            return false; 
        }
    }
}
