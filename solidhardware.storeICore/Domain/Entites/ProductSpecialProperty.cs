using System;

namespace solidhardware.storeCore.Domain.Entites
{
    public class ProductSpecialProperty
    {
        public Guid Id { get; set; } 
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

        public int? Size { get; set; }

        public string? Unit { get; set; }

        public int DisplayOrder { get; set; }

    }
}
