using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Model.CatalogDTOs
{
    public class CatalogItemDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string CatalogType { get; set; }

        public string CatalogBrand { get; set; }

        public int AvailableStock { get; set; }

        public int RestockThreshold { get; set; }

        public int MaxStockThreshold { get; set; }

        public bool OnReorder { get; set; }
    }
}
