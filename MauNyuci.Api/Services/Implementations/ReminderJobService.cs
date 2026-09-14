using MauNyuci.Api.Data;
using MauNyuci.Api.Models;
using MauNyuci.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MauNyuci.Api.Services.Implementations
{
    public class ReminderJobService : IReminderJobService
    {
        private readonly AppDbContext _dbContext;
        private readonly INotificationDispatcher _dispatcher;

        public ReminderJobService(AppDbContext dbContext, INotificationDispatcher dispatcher)
        {
            _dbContext = dbContext;
            _dispatcher = dispatcher;
        }

        // Job: 30 Menit setelah order jika belum dibayar
        public async Task SendPaymentReminderAsync(Guid orderId)
        {
            var order = await _dbContext.Orders.Include(o => o.Store).FirstOrDefaultAsync(o => o.Id == orderId);
            // Batalkan pengingat jika status order sudah tidak Pending atau sudah Paid
            if (order == null || order.Status != OrderStatus.Pending || order.PaymentStatus == PaymentStatus.Paid)
                return; 

            if (order.CustomerId.HasValue)
            {
                await _dispatcher.DispatchNotificationAsync(
                    order.CustomerId.Value,
                    "Segera Selesaikan Pembayaran!",
                    $"Pesanan Anda di {order.Store?.Name ?? "Toko"} menunggu pembayaran. Segera bayar sebelum pesanan dibatalkan."
                );
            }
        }

        // Job: 15 Menit setelah order diselesaikan
        public async Task SendReviewReminderAsync(Guid orderId)
        {
            var order = await _dbContext.Orders.Include(o => o.Store).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null || order.Status != OrderStatus.Completed)
                return;

            if (order.CustomerId.HasValue)
            {
                await _dispatcher.DispatchNotificationAsync(
                    order.CustomerId.Value,
                    "Bagaimana Hasil Cucian Kami?",
                    $"Pesanan Anda dari {order.Store?.Name ?? "Toko"} telah selesai. Yuk, berikan ulasan Anda sekarang!"
                );
            }
        }

        // Job: 1 Hari sebelum estimasi penyelesaian
        public async Task SendStoreSLAReminderAsync(Guid orderId)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null || order.Status == OrderStatus.Completed || order.Status == OrderStatus.ReadyForPickup || order.Status == OrderStatus.Cancelled)
                return; // Jika sudah selesai/siap diambil, abaikan pengingat

            string shortId = order.Id.ToString().Substring(0, 6).ToUpper();

            await _dispatcher.DispatchNotificationAsync(
                order.StoreId, // Kirim ke Toko
                "Peringatan Estimasi Waktu (SLA)",
                $"Pesanan #{shortId} mendekati batas waktu estimasi selesai besok. Harap segera diselesaikan!"
            );
        }
    }
}
