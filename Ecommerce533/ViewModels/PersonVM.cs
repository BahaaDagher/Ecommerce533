using Ecommerce533.Models;

namespace Ecommerce533.ViewModels
{
    public class PersonVM
    {
        public IEnumerable<Person> Persons { get; set; }
        public int Count { get; set; }
    }
}
