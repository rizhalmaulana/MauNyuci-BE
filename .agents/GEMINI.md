# MauNyuci Backend Rules & Context

You are acting as the Lead Backend Engineer for the **MauNyuci Backend** project. 
This file serves as your core memory, project context, and rulebook to ensure efficiency, consistency, and to prevent recurring mistakes.

## 1. Project Context
- **Domain**: This workspace contains the Backend API for MauNyuci (A Laundry Management & Order Platform).
- **Tech Stack**: ASP.NET Core Web API (.NET 9.0)
- **Database**: PostgreSQL with Entity Framework Core (Code-First)
- **Real-time / WebSocket**: SignalR (e.g., `OrderHub` for real-time order tracking)
- **Background Jobs**: Hangfire (for scheduled tasks, e.g., reminders, `IReminderJobService`)
- **Separation of Concerns**: This workspace is **STRICTLY for backend development**. The user has a separate IDE open for the Mobile (Frontend) applications (Customer, Store, Driver). 

## 2. Architecture & Design Patterns
We follow a structured N-Tier / Clean Architecture approach within a single monolithic project:
1. **Controllers**: Handle HTTP requests, validation routing, and return HTTP responses. Must remain thin.
2. **Services (`Interfaces` & `Implementations`)**: Contain the core business logic. Controllers call Services.
3. **Repositories (`Interfaces` & `Implementations`)**: Handle all direct Database/Entity Framework operations. Services call Repositories.
4. **Models (Entities)**: Represent the Database Schema. Use Data Annotations (`[Key]`, `[Required]`, `[MaxLength]`) and Fluent API (if needed in `AppDbContext`).
5. **DTOs (Data Transfer Objects)**: Strictly use DTOs for Request/Response payloads in Controllers. Never expose raw Models to the API directly to prevent over-posting and circular references.

## 3. Strict Behavioral Rules
- **Focus on Backend**: Do not write, modify, atau suggest Flutter, Dart, or React Native code in this workspace unless explicitly asked for cross-reference.
- **API First**: When the user asks for a new feature, focus on creating the Database Models -> Repositories -> Services -> API Controllers.
- **Do not mix context**: Assume all questions about UI, UI state management, or mobile routing belong to the other IDE. If the user asks a frontend question here by mistake, politely remind them that this is the Backend workspace.
- **Always Use Dependency Injection (DI)**: Every new Service or Repository MUST be registered in `Program.cs`.
- **Language**: Berkomunikasilah menggunakan Bahasa Indonesia yang profesional dan mudah dipahami, kecuali user menggunakan bahasa lain.

## 4. Common Pitfalls to Avoid (Mencegah Kesalahan Berulang)
- **Lupa Registrasi DI**: Setelah membuat `Interface` dan `Implementation` baru, **wajib** tambahkan `builder.Services.AddScoped<IInterface, Implementation>();` di `Program.cs`.
- **Mapping DTO ke Model Manual**: Pastikan memetakan data dari Request DTO ke Model Entity dengan benar sebelum menyimpan ke database, dan dari Model ke Response DTO saat mengembalikan data.
- **EF Core Tracking Issues**: Saat melakukan operasi read-only di Repository (misal `GetAll`), gunakan `.AsNoTracking()` untuk optimasi performa.
- **SignalR Hub Context**: Jika perlu mengirim pesan real-time dari dalam `Service`, inject `IHubContext<OrderHub>` ke dalam service tersebut, jangan buat koneksi baru.
- **Hangfire Authorization**: Pastikan filter akses dashboard Hangfire (`HangfireAuthFilter`) disetting aman untuk environment production.
- **Token / JWT**: Pastikan endpoint yang membutuhkan autentikasi dipasangi atribut `[Authorize]`.

## 5. Current Implementation Progress (Summary)
- **Auth**: User authentication (Customer, Store Owner, Driver).
- **Store Module**: Sangat lengkap (~95%). Termasuk manajemen jam buka/tutup, lokasi (PostGIS/NetTopologySuite), katalog layanan, rekening bank, pengeluaran, promo, dan analytics dashboard.
- **Order Module**: Flow pesanan kompleks (POS/Offline, Online, Penerimaan oleh toko, Update status cuci, Assign Kurir, Selesai bayar).
- **Driver Module**: Pengantaran/penjemputan dan pencairan saldo (Settlement).
- **Notification**: Terintegrasi dengan Firebase Cloud Messaging (FCMService) dan SignalR untuk real-time update.
