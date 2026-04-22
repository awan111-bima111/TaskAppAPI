# Task Management App

## Deskripsi

Project ini adalah aplikasi sederhana untuk mengelola project dan task.
Aplikasi ini terdiri dari backend API, web frontend, dan mobile app yang semuanya saling terhubung.

Tujuan dari project ini adalah untuk mencoba membuat sistem fullstack dari awal sampai bisa dipakai, termasuk autentikasi dan integrasi antar platform.

---

## Teknologi yang Dipakai

**Backend**

* ASP.NET Core (.NET 8)
* PostgreSQL
* Entity Framework Core
* JWT Authentication

**Frontend Web**

* React (Vite)
* Fetch API

**Mobile**

* Flutter
* HTTP package

---

## Fitur

* Register & Login (JWT)
* CRUD Project
* CRUD Task
* Relasi Project dan Task
* API sudah dilindungi dengan token
* Bisa diakses dari web dan mobile

---

## Cara Menjalankan

### Backend

Masuk ke folder backend:

```bash
cd TaskAppAPI
```

Jalankan:

```bash
dotnet run
```

Akses Swagger:

```
http://localhost:5000/swagger
```

---

### Frontend (React)

Masuk ke folder frontend:

```bash
cd frontend
```

Jalankan:

```bash
npm install
npm run dev
```

---

### Mobile (Flutter)

Masuk ke folder:

```bash
cd taskapp_mobile
```

Jalankan:

```bash
flutter pub get
flutter run
```

---

## Cara Pakai API

1. Login lewat endpoint `/api/Auth/login`
2. Copy token yang didapat
3. Klik tombol Authorize di Swagger
4. Masukkan:

```
Bearer {token}
```

---

## Catatan

Untuk JWT, pastikan secret key di `appsettings.json` sama dengan yang digunakan di backend, kalau tidak akan muncul error unauthorized.

---

## Penutup

Project ini saya buat untuk latihan dan juga sebagai bagian dari proses belajar fullstack development.
Masih banyak yang bisa dikembangkan, tapi untuk sekarang fokusnya adalah memastikan semua bagian bisa terhubung dan berjalan dengan baik.
