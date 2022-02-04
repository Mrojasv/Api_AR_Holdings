using System;
using System.Collections.Generic;

namespace AR_Holdings.Models
{
    public class Shopify
    {
        public class ShopifyOrder
        {
            public long id { get; set; }
            public string email { get; set; }
            public DateTime? closed_at { get; set; }
            public DateTime? created_at { get; set; }
            public DateTime? updated_at { get; set; }
            public int? order_number { get; set; }
            public decimal? subtotal_price { get; set; }
            public decimal? total_tax { get; set; }
            public decimal? total_price { get; set; }

            public List<Item> line_items { get; set; }
            public Customer customer { get; set; }
        }

        public class Item
        {
            public string sku { get; set; }
            public decimal? price { get; set; }
            public int? quantity { get; set; }
            public string name { get; set; }
            public decimal? subtotal_price { get; set; }
            public decimal? total_tax { get; set; }
            public decimal? total_price { get; set; }
        }

        public class Customer
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public Address default_address { get; set; }
        }

        public class Address
        {
            public string address1 { get; set; }
        }
    }
}
