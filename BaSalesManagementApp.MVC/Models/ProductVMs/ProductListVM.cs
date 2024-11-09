﻿namespace BaSalesManagementApp.MVC.Models.ProductVMs
{
    public class ProductListVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public byte[]? QRCode { get; set; }

        public string CategoryName { get; set; }

        public string? CompanyName { get; set; }

        public byte[]? CompanyPhoto { get; set; } = null!;
    }
}