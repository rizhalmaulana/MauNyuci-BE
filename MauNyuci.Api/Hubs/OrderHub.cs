using Microsoft.AspNetCore.SignalR;

namespace MauNyuci.Api.Hubs
{
    // Hub ini adalah terminal komunikasi real-time. 
    // Flutter akan melakukan koneksi (listen) ke kelas ini.
    public class OrderHub : Hub
    {
        // Fungsi ini bisa dipanggil oleh Flutter jika driver ingin 
        // memancarkan lokasi live-nya ke aplikasi customer tertentu
        public async Task SendLocationUpdate(string orderId, double lat, double lng)
        {
            // Meneruskan pesan ke semua client yang mendengarkan event "ReceiveLocationUpdate" untuk orderId ini
            await Clients.Group(orderId).SendAsync("ReceiveLocationUpdate", lat, lng);
        }

        // Saat Flutter membuka halaman "Tracking Order", aplikasi akan join ke grup Order ID tersebut
        public async Task JoinOrderGroup(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
        }

        // Saat keluar dari halaman, tinggalkan grup agar hemat memori
        public async Task LeaveOrderGroup(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
        }

        // --- Fitur Dashboard Toko ---
        // Saat aplikasi Toko dibuka, bergabung ke grup toko miliknya sendiri
        public async Task JoinStoreGroup(string storeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Store-{storeId}");
        }

        public async Task LeaveStoreGroup(string storeId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Store-{storeId}");
        }
    }
}