using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauNyuci.Api.Models
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // RELASI: Rincian ini milik struk pesanan yang mana?
        [Required]
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        // RELASI: Layanan apa yang dipilih dari katalog?
        [Required]
        public Guid CatalogItemId { get; set; }
        [ForeignKey("CatalogItemId")]
        public StoreCatalogItem? CatalogItem { get; set; }

        // Kita wajib meng-copy Nama, Harga, dan Satuan ke sini. 
        // Kenapa? Jika bulan depan toko menaikkan harga dari Rp8000 jadi Rp10000,
        // transaksi lama di masa lalu harganya tidak ikut berubah/rusak.
        [Required]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Harga asli saat tombol "Pesan" ditekan

        [Required]
        public string Unit { get; set; } = string.Empty; // Contoh: "Kg" atau "Pcs"


        // Saat Customer pesan (terutama Kiloan), Quantity akan bernilai 0.
        // Nanti, Toko yang akan meng-update kolom ini setelah baju ditimbang.
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; } = 0;

        // Hasil dari (UnitPrice * Quantity). 
        // Nilainya akan 0 di awal, dan baru terisi ketika Toko sudah input berat/jumlah.
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; } = 0;
    }
}