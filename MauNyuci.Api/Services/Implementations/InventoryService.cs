using MauNyuci.Api.Data;
using MauNyuci.Api.DTOs.Inventory;
using MauNyuci.Api.Models;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        private async Task<bool> IsUserAuthorizedForStoreAsync(Guid storeId, Guid userId)
        {
            // Owner check
            var isOwner = await _context.Store.AnyAsync(s => s.Id == storeId && s.OwnerId == userId);
            if (isOwner) return true;

            // Staff check
            var isStaff = await _context.StoreStaffs.AnyAsync(s => s.StoreId == storeId && s.UserId == userId && s.IsActive);
            return isStaff;
        }

        public async Task<InventoryItemResponseDto> CreateInventoryItemAsync(Guid storeId, Guid userId, InventoryItemCreateDto request)
        {
            if (!await IsUserAuthorizedForStoreAsync(storeId, userId))
            {
                throw new Exception("Anda tidak memiliki akses ke toko ini.");
            }

            var item = new InventoryItem
            {
                StoreId = storeId,
                ItemName = request.ItemName,
                StockQuantity = request.StockQuantity,
                Unit = request.Unit,
                MinimumStockAlert = request.MinimumStockAlert
            };

            _context.InventoryItems.Add(item);

            // Add initial transaction if quantity > 0
            if (request.StockQuantity > 0)
            {
                var transaction = new InventoryTransaction
                {
                    InventoryItem = item,
                    TransactionType = "In",
                    Quantity = request.StockQuantity,
                    Notes = "Initial Stock",
                    PerformedById = userId
                };
                _context.InventoryTransactions.Add(transaction);
            }

            await _context.SaveChangesAsync();

            return MapToItemDto(item);
        }

        public async Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsAsync(Guid storeId, Guid userId)
        {
            if (!await IsUserAuthorizedForStoreAsync(storeId, userId))
            {
                throw new Exception("Anda tidak memiliki akses ke toko ini.");
            }

            var items = await _context.InventoryItems
                .Where(i => i.StoreId == storeId)
                .ToListAsync();

            return items.Select(MapToItemDto);
        }

        public async Task<InventoryItemResponseDto> UpdateInventoryItemAsync(Guid storeId, Guid userId, Guid itemId, InventoryItemUpdateDto request)
        {
            if (!await IsUserAuthorizedForStoreAsync(storeId, userId))
            {
                throw new Exception("Anda tidak memiliki akses ke toko ini.");
            }

            var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == itemId && i.StoreId == storeId);
            if (item == null) throw new Exception("Barang tidak ditemukan.");

            if (request.ItemName != null) item.ItemName = request.ItemName;
            if (request.Unit != null) item.Unit = request.Unit;
            if (request.MinimumStockAlert.HasValue) item.MinimumStockAlert = request.MinimumStockAlert.Value;

            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToItemDto(item);
        }

        public async Task RemoveInventoryItemAsync(Guid storeId, Guid userId, Guid itemId)
        {
            if (!await IsUserAuthorizedForStoreAsync(storeId, userId))
            {
                throw new Exception("Anda tidak memiliki akses ke toko ini.");
            }

            var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == itemId && i.StoreId == storeId);
            if (item == null) throw new Exception("Barang tidak ditemukan.");

            // Remove related transactions
            var transactions = await _context.InventoryTransactions.Where(t => t.InventoryItemId == itemId).ToListAsync();
            _context.InventoryTransactions.RemoveRange(transactions);

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<InventoryTransactionResponseDto> AddTransactionAsync(Guid storeId, Guid userId, Guid itemId, InventoryTransactionCreateDto request)
        {
            if (!await IsUserAuthorizedForStoreAsync(storeId, userId))
            {
                throw new Exception("Anda tidak memiliki akses ke toko ini.");
            }

            var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == itemId && i.StoreId == storeId);
            if (item == null) throw new Exception("Barang tidak ditemukan.");

            if (request.Quantity <= 0) throw new Exception("Quantity harus lebih dari 0.");

            if (request.TransactionType == "Out" && item.StockQuantity < request.Quantity)
            {
                throw new Exception($"Stok tidak mencukupi. Stok saat ini: {item.StockQuantity} {item.Unit}");
            }

            var transaction = new InventoryTransaction
            {
                InventoryItemId = itemId,
                TransactionType = request.TransactionType,
                Quantity = request.Quantity,
                Notes = request.Notes,
                PerformedById = userId
            };

            // Update item stock
            if (request.TransactionType == "In")
            {
                item.StockQuantity += request.Quantity;
            }
            else if (request.TransactionType == "Out")
            {
                item.StockQuantity -= request.Quantity;
            }
            else if (request.TransactionType == "Adjustment")
            {
                // Adjustment means replacing the current stock or setting it
                // For simplicity, let's treat it as a direct set if needed, or difference
                // Let's assume adjustment sets the absolute value
                item.StockQuantity = request.Quantity;
            }

            item.UpdatedAt = DateTime.UtcNow;
            
            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return MapToTransactionDto(transaction);
        }

        public async Task<IEnumerable<InventoryTransactionResponseDto>> GetItemTransactionsAsync(Guid storeId, Guid userId, Guid itemId)
        {
            if (!await IsUserAuthorizedForStoreAsync(storeId, userId))
            {
                throw new Exception("Anda tidak memiliki akses ke toko ini.");
            }

            var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == itemId && i.StoreId == storeId);
            if (item == null) throw new Exception("Barang tidak ditemukan.");

            var transactions = await _context.InventoryTransactions
                .Where(t => t.InventoryItemId == itemId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return transactions.Select(MapToTransactionDto);
        }

        private InventoryItemResponseDto MapToItemDto(InventoryItem item)
        {
            return new InventoryItemResponseDto
            {
                Id = item.Id,
                StoreId = item.StoreId,
                ItemName = item.ItemName,
                StockQuantity = item.StockQuantity,
                Unit = item.Unit,
                MinimumStockAlert = item.MinimumStockAlert,
                UpdatedAt = item.UpdatedAt
            };
        }

        private InventoryTransactionResponseDto MapToTransactionDto(InventoryTransaction t)
        {
            return new InventoryTransactionResponseDto
            {
                Id = t.Id,
                InventoryItemId = t.InventoryItemId,
                TransactionType = t.TransactionType,
                Quantity = t.Quantity,
                Notes = t.Notes,
                PerformedById = t.PerformedById,
                CreatedAt = t.CreatedAt
            };
        }
    }
}
