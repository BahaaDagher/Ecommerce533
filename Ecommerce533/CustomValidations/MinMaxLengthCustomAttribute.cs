using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce533.CustomValidations
{
    public class MinMaxLengthCustomAttribute :ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;
        public MinMaxLengthCustomAttribute (int mnValue  , int mxValue)
        {
            _minLength = mnValue; 
            _maxLength = mxValue; 
        }
        public override string FormatErrorMessage(string name)
        {
            return $"the Length of {name} must be between [{_minLength} , {_maxLength}]"; 
        }
        public override bool IsValid(object? value)
        {
            if(value is string name)
            {
                if (name.Length >= _minLength && name.Length <= _maxLength)
                    return true; 
            }
            return false; 
            
        }
    }
}
