using Application.Interfaces;
using Application.Models.Item;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<List<ItemDTO>> GetAllAsync()
        {
            var items = await _itemRepository.GetAllAsync();
            return items.Select(MapDTO).ToList();
        }

        public async Task<ItemDTO?> GetByIdAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);

            if (item == null) return null;

            return MapDTO(item);
        }

        public async Task<List<ItemDTO>> GetAvailableItemsAsync()
        {
            var availableItems = await _itemRepository.GetAvailableItemsAsync();
            return availableItems.Select(MapDTO).ToList();
        }

        public async Task<ItemDTO?> AddAsync(CreateItemDTO item)
        {
            if (item == null) return null;
            if(item.Stock < 0 || item.Price < 0)
            {
                throw new BadRequestException("El stock y/o precio no puede ser negativo");
            }

            var newItem = new Item
            {
                Name = item.Name,
                Price = item.Price,
                Stock = item.Stock,
                Available = item.Stock > 0,
                Description = item.Description,
            };

            var addedItem = await _itemRepository.AddAsync(newItem);
            await _itemRepository.SaveChangesAsync();
            if (addedItem == null) return null;

            return MapDTO(addedItem);
        }

        public async Task<ItemDTO?> UpdateAsync(ItemDTO item)
        {
            if (item == null) return null;
            if (item.Stock < 0 || item.Price < 0)
            {
                throw new BadRequestException("El stock y/o precio no puede ser negativo");
            }

            var updateItem = await _itemRepository.GetByIdAsync(item.Id);

            if (updateItem == null) return null;

            updateItem.Name = item.Name;
            updateItem.Price = item.Price;
            updateItem.Stock = item.Stock;
            updateItem.Available = item.Available;
            updateItem.Description = item.Description;

            await _itemRepository.UpdateAsync(updateItem);
            await _itemRepository.SaveChangesAsync();

            return MapDTO(updateItem);
        }

        public async Task<ItemDTO?> DeleteAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);

            if (item == null) return null;

            await _itemRepository.DeleteAsync(id);
            await _itemRepository.SaveChangesAsync();

            return MapDTO(item);
        }

        private static ItemDTO MapDTO(Item i)
        {
            return new ItemDTO
            {
                Id = i.Id,
                Name = i.Name,
                Price = i.Price,
                Stock = i.Stock,
                Available = i.Available,
                Description = i.Description,
            };
        }
    }
}