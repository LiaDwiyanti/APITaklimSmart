# API TaklimSmart
API Taklim Smart adalah proyek Web API berbasis ASP.NET Core untuk mengelola jadwal kegiatan pengajian tingkat RT. API ini memiliki sistem multi-user (admin dan user) serta menyediakan fitur CRUD untuk jadwal pengajian, dan dokumentasi kegiatan serta fitur READ pada lokasi pengajian terdekat. Pada API ini juga didukung dengan geocoding untuk menambah dan mencari lokasi pengguna ketika melakukan registrasi serta memungkinkan untuk upload gambar.

## Endpoint
### Autentikasi dan Akun
- `POST` /register
- `POST` /login
- `POST` /logout
- `GET` /akun

### Penjadwalan
- `GET` /penjadwalan/read
- `GET` /penjadwalan/read/{id}
- `POST` /penjadwalan/create
- `DELETE` /penjadwalan/delete/{id}
- `GET`	/penjadwalan/{id}/lokasi

### Lokasi
- `GET` /lokasi
- `GET` /lokasi-terdekat

### Dokumentasi
- `POST` /dokumentasi/create
- `GET` /dokumentasi/read
- `POST` /dokumentasi/uploadfile
- `PUT` /dokumentasi/edit
- `DELETE` /dokumentasi/delete/{id}

### Riwayat
- `GET` /riwayat/read
- `PATCH` update-status/{id}
