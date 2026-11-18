using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public IReadOnlyCollection<Item> Items { get; set; } = new List<Item>();
    }
}