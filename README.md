# MauNyuci Backend API

MauNyuci Backend API adalah layanan inti (core service) untuk platform manajemen dan pemesanan laundry terpadu (MauNyuci). Proyek ini dibangun menggunakan **ASP.NET Core 9.0**, mengimplementasikan arsitektur *Clean Architecture* / N-Tier dalam struktur monolitik, dan menyediakan antarmuka API RESTful untuk aplikasi *Customer*, *Store Owner*, dan *Driver*.

## 🚀 Fitur Utama (Modules)

1.  **Authentication & Authorization**
    *   Registrasi dan login multi-role: *Customer*, *Store*, dan *Driver*.
    *   Pengamanan endpoint menggunakan JWT Authentication.

2.  **Store Module (Manajemen Toko)**
    *   Manajemen profil dan informasi toko (Jam Buka/Tutup).
    *   Sistem geolokasi dengan tipe data spasial (PostGIS/NetTopologySuite).
    *   Katalog layanan laundry (Harga, durasi).
    *   Manajemen rekening bank pencairan dan pengeluaran operasional.
    *   Manajemen Promo.
    *   Analytics Dashboard (Rekap pendapatan, metrik pesanan).

3.  **Order Module (Manajemen Pesanan)**
    *   Dukungan alur pemesanan yang komprehensif:
        *   **Online**: Pelanggan memesan via aplikasi.
        *   **Offline/POS**: Kasir toko menginput pesanan secara langsung.
    *   Alur kerja status cucian terperinci (Menunggu diterima, proses cuci, selesai, dll).
    *   Integrasi pembayaran dan penugasan penjemputan/pengantaran kurir.

4.  **Driver Module (Kurir)**
    *   Penerimaan tugas penjemputan dan pengantaran cucian.
    *   Manajemen saldo dompet dan penarikan saldo (Settlement).

5.  **Real-Time & Notifications**
    *   **SignalR**: Pembaruan status pesanan secara *real-time* kepada pelanggan, toko, dan driver (`OrderHub`).
    *   **FCM (Firebase Cloud Messaging)**: Terintegrasi untuk push notification.
    *   **Hangfire**: Eksekusi background jobs (tugas latar belakang terdistribusi) untuk pengingat dan tugas terjadwal lainnya (`IReminderJobService`).

## 🛠️ Tech Stack & Tools

*   **Framework**: ASP.NET Core Web API (.NET 9.0)
*   **Database**: PostgreSQL dengan PostGIS Extension
*   **ORM**: Entity Framework Core 9.0 (Code-First)
*   **Real-time Communication**: ASP.NET Core SignalR
*   **Background Tasks**: Hangfire
*   **Cloud Storage**: Cloudflare R2 (S3 Compatible API)
*   **Notifications**: Firebase Admin SDK (FCM)

## 🏗️ Struktur Arsitektur (N-Tier)

Proyek ini sangat mengedepankan prinsip pemisahan tanggung jawab (*Separation of Concerns*):
`Controllers` -> `Services` -> `Repositories` -> `Database`.

*   **Controllers**: Menangani proses routing *HTTP Request*, validasi parameter, dan mengembalikan *HTTP Response*. Dibuat agar tetap ramping (*thin controllers*).
*   **Services (`Interfaces` & `Implementations`)**: Tempat semua **Business Logic** berjalan. Semua logika inti aplikasi harus ada di layer ini.
*   **Repositories (`Interfaces` & `Implementations`)**: Menangani akses dan manipulasi data langsung ke database melalui *Entity Framework Core*.
*   **Data Transfer Objects (DTOs)**: Digunakan secara eksklusif untuk struktur payload pada Request dan Response API guna mencegah kebocoran struktur basis data (*over-posting*). 
*   **Models (Entities)**: Mewakili skema tabel di *Database* menggunakan *Data Annotations* dan *Fluent API*.

## ⚙️ Persyaratan Sistem (Prerequisites)

Sebelum menjalankan proyek ini, pastikan *environment* Anda telah memiliki:
*   [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
*   [PostgreSQL](https://www.postgresql.org/download/) dengan ekstensi [PostGIS](https://postgis.net/install/) terinstal.
*   IDE direkomendasikan: Visual Studio 2022, JetBrains Rider, atau VS Code (dengan C# Dev Kit).

## 🚀 Panduan Menjalankan Secara Lokal (Local Development)

1.  **Kloning Repositori**
    ```bash
    git clone <url-repository-anda>
    cd MauNyuci
    ```

2.  **Konfigurasi Parameter Aplikasi**
    Periksa file `MauNyuci.Api/appsettings.json` (atau buat file `appsettings.Development.json` jika diabaikan oleh git) dan pastikan parameter berikut terisi sesuai kredensial lokal Anda:
    *   `ConnectionStrings:DefaultConnection`: Sesuaikan kredensial akses PostgreSQL.
    *   `Jwt`: Konfigurasi *Key* rahasia, *Issuer*, dan *Audience*.
    *   `CloudflareR2`: Kredensial keranjang penyimpanan cloud.

3.  **Terapkan Migrasi Database (EF Core)**
    Buka terminal yang mengarah ke dalam proyek (direktori tempat file `.csproj` berada, misalnya `MauNyuci.Api`) lalu jalankan update database:
    ```bash
    cd MauNyuci.Api
    dotnet ef database update
    ```
    *Catatan: Perintah ini akan secara otomatis membuat skema dan tabel database `mau_nyuci_db` sesuai model yang didefinisikan.*

4.  **Jalankan API (Build & Run)**
    ```bash
    dotnet run
    ```
    Setelah layanan berjalan, buka browser dan akses antarmuka **Swagger UI** untuk mencoba API (secara default biasanya di alamat `https://localhost:<port>/swagger` atau `http://localhost:<port>/swagger`).

## 📝 Panduan Bagi Pengembang (Developer Rules)

Mengingat proyek ini menerapkan standar khusus, perhatikan beberapa aturan wajib berikut saat mengembangkan fitur baru:
1.  **Injeksi Dependensi (DI)**: Setiap kelas `Service` dan `Repository` baru wajib didaftarkan pada container DI di `Program.cs` (`builder.Services.AddScoped<IInterface, Implementation>();`).
2.  **Optimasi *Query***: Saat merancang fungsi di *Repository* yang bersifat *Read-Only* (seperti `GetAll`), Anda wajib menambahkan metode ekstensi `.AsNoTracking()` untuk meningkatkan performa EF Core.
3.  **Jangan Campur Konteks**: Repositori ini murni diperuntukkan bagi **Backend / API**. Semua kode terkait tampilan UI (React Native, Flutter) berada di repositori terpisah.
4.  **Manajemen Real-Time**: Jangan pernah membuat koneksi SignalR baru di dalam service. Jika ingin memancarkan acara real-time, injeksikan `IHubContext<OrderHub>` melalui konstruktor.
