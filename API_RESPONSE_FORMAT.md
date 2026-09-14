# Format Response API - MauNyuci

Semua response API menggunakan format JSON dengan struktur yang konsisten.

## Struktur Response Umum

### Success Response
```json
{
  "success": true,
  "data": { ... }
}
```

### Error Response - Validasi
```json
{
  "success": false,
  "message": "Validasi gagal",
  "errors": ["Pesan error 1", "Pesan error 2"]
}
```

### Error Response - Exception
```json
{
  "success": false,
  "message": "Pesan error dalam bahasa Indonesia",
  "errors": {
    "detail": "Detail error (opsional)"
  }
}
```

---

## Auth API

### POST /api/Auth/register

**Request:**
```json
{
  "fullName": "string (wajib)",
  "phoneNumber": "string (wajib, 10-13 digit)",
  "password": "string (wajib)",
  "email": "string (opsional)"
}
```

**Success Response (200):**
```json
{
  "token": "jwt_token",
  "fullName": "Kustoyo Jaya",
  "role": "Customer",
  "isProfileComplete": true
}
```

**Error Response (400) - Validation:**
```json
{
  "success": false,
  "message": "Validasi gagal",
  "errors": [
    "Nama lengkap wajib diisi",
    "Nomor telepon harus 10-13 digit"
  ]
}
```

**Error Response (400) - Phone Already Exists:**
```json
"Nomor Telepon sudah terdaftar!"
```

---

### POST /api/Auth/login

**Request:**
```json
{
  "phoneNumber": "string (wajib)",
  "password": "string (wajib)"
}
```

**Success Response (200):**
```json
{
  "token": "jwt_token",
  "fullName": "Kustoyo Jaya",
  "role": "Customer",
  "isProfileComplete": true
}
```

**Error Response (401):**
```json
"Nomor Telepon atau Password salah!"
```

---

### GET /api/Auth/profile

**Headers:** `Authorization: Bearer {token}`

**Success Response (200):**
```json
{
  "id": "guid",
  "fullName": "string",
  "phoneNumber": "string",
  "email": "string | null",
  "profilePictureUrl": "string | null",
  "defaultAddress": "string | null",
  "defaultLatitude": number | null,
  "defaultLongitude": number | null,
  "role": "string",
  "authProvider": "string"
}
```

**Error Response (401):**
```json
{ "message": "Token tidak valid." }
```

**Error Response (404):**
```json
{ "message": "Pengguna tidak ditemukan." }
```

---

### PUT /api/Auth/profile

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "fullName": "string (wajib)",
  "phoneNumber": "string (opsional)",
  "email": "string (opsional)",
  "profilePictureUrl": "string (opsional)",
  "defaultAddress": "string (opsional)",
  "defaultLatitude": number (opsional),
  "defaultLongitude": number (opsional)
}
```

**Success Response (200):**
```json
{
  "id": "guid",
  "fullName": "string",
  "phoneNumber": "string",
  "email": "string | null",
  "profilePictureUrl": "string | null",
  "defaultAddress": "string | null",
  "defaultLatitude": number | null,
  "defaultLongitude": number | null,
  "role": "string",
  "authProvider": "string"
}
```

**Error Response (400):**
```json
{ "message": "Nomor telepon sudah digunakan oleh user lain." }
```

---

### PUT /api/Auth/change-password

**Headers:** `Authorization: Bearer {token}`

**Request:**
```json
{
  "oldPassword": "string (wajib)",
  "newPassword": "string (wajib)"
}
```

**Success Response (200):**
```json
{
  "message": "Password berhasil diubah."
}
```

**Error Response (400):**
```json
{ "message": "Password lama tidak sesuai." }
```

---

### POST /api/Auth/check-exists

**Request:**
```json
{
  "phoneNumber": "string (opsional)",
  "email": "string (opsional)"
}
```

**Success Response (200):**
```json
{
  "exists": true,
  "userId": "guid (jika exists=true)",
  "fullName": "string",
  "email": "string",
  "phoneNumber": "string",
  "message": "User sudah terdaftar"
}
```

**Error Response (400):**
```json
{ "message": "Nomor telepon atau email wajib diisi." }
```

---

### POST /api/Auth/firebase-auth

**Request:**
```json
{
  "idToken": "string (wajib)",
  "fullName": "string (opsional)",
  "phoneNumber": "string (opsional)"
}
```

**Success Response (200):**
```json
{
  "token": "jwt_token",
  "fullName": "string",
  "role": "Customer",
  "isProfileComplete": boolean
}
```

**Error Response (400):**
```json
{ "message": "Autentikasi Firebase gagal. Token tidak valid." }
```

---

## Kode Status HTTP

| Kode | Arti |
|------|-----|
| 200 | OK |
| 201 | Created |
| 400 | Bad Request (Validasi gagal/Error) |
| 401 | Unauthorized (Token tidak valid/tidak ada) |
| 404 | Not Found (Data tidak ditemukan) |
| 500 | Internal Server Error |

---

## Daftar Pesan Error

### Validasi
- `Nama lengkap wajib diisi`
- `Nomor telepon wajib diisi`
- `Nomor telepon harus 10-13 digit`
- `Password wajib diisi`
- `Token Firebase wajib diisi`
- `Email tidak valid`

### Database/Constraint
- `Nomor Telepon sudah terdaftar!`
- `Data sudah ada`
- `Data tidak valid`
- `Gagal menyimpan data`

### Authentication
- `Nomor Telepon atau Password salah!`
- `Token tidak valid.`
- `Pengguna tidak ditemukan.`
- `Autentikasi Firebase gagal. Token tidak valid.`

### General
- `Koneksi timeout, coba lagi`
- `Layanan sedang tidak tersedia`
- `Terjadi kesalahan, coba lagi nanti`