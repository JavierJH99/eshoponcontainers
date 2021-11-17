using Catalog.API.Data;
using Catalog.API.Model;
using Catalog.API.Model.CatalogDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        CatalogContext _context;

        public ItemsController(CatalogContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CatalogItemDTO>> PostCatalogItem(CatalogItemDTO catalogItemDTO)
        {
            CatalogItem catalogItem = await _context.CatalogItems.FirstOrDefaultAsync(ci => ci.Name == catalogItemDTO.Name);
            if (catalogItem != null) return BadRequest("Product Name must be unique");
            catalogItem = new CatalogItem()
            {
                Name = catalogItemDTO.Name,
                Description = catalogItemDTO.Description,
                Price = catalogItemDTO.Price,
                AvailableStock = catalogItemDTO.AvailableStock,
                RestockThreshold = catalogItemDTO.RestockThreshold,
                MaxStockThreshold = catalogItemDTO.MaxStockThreshold,
                OnReorder = catalogItemDTO.OnReorder
            };
            CatalogType catalogtype = await _context.CatalogTypes.FirstOrDefaultAsync(ct => ct.Type == catalogItemDTO.CatalogType);
            if (catalogtype == null) catalogtype = new CatalogType() { Type = catalogItemDTO.CatalogType };
            CatalogBrand catalogBrand = await _context.CatalogBrands.FirstOrDefaultAsync(cb => cb.Brand == catalogItemDTO.CatalogBrand);
            if (catalogBrand == null) catalogBrand = new CatalogBrand() { Brand = catalogItemDTO.CatalogBrand };
            catalogItem.CatalogBrand = catalogBrand;
            catalogItem.CatalogType = catalogtype;
            _context.CatalogItems.Add(catalogItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction("PostCatalogItem", new { id = catalogItem.Id }, catalogItemDTO);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsDTO<CatalogItemDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ItemsAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var itemsOnPage = await _context.CatalogItems
            //join table CatalogBrand and CatalogType
            .Include(item => item.CatalogBrand).Include(item => item.CatalogType)
            .Select(item => new CatalogItemDTO()
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                CatalogType = item.CatalogType.Type,
                CatalogBrand = item.CatalogBrand.Brand,
                AvailableStock = item.AvailableStock,
                RestockThreshold = item.RestockThreshold,
                MaxStockThreshold = item.MaxStockThreshold,
                OnReorder = item.OnReorder
            }).OrderBy(c => c.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();
            var totalItems = await _context.CatalogItems.LongCountAsync();
            var model = new PaginatedItemsDTO<CatalogItemDTO>(pageIndex, pageSize, totalItems, itemsOnPage);
            return Ok(model);
        }
    }
}
