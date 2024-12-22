using Ardalis.Result;
using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain
{
    public class Product : AggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }

        private Product(string name, string description, decimal price) : base(Guid.CreateVersion7())
        {
            Name = name;
            Description = description;
            Price = price;
        }

        public static Result<Product> Create(string name, string description, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Errors.Product.Create.InvalidName();
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return Errors.Product.Create.InvalidDescription();
            }

            if (price < 0)
            {
                return Errors.Product.Create.InvalidPrice();
            }

            return new Product(name, description, price);
        }
    }
}
