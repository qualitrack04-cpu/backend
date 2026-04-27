# QualiTrack API 🎯

Backend API untuk sistem manajemen audit dan CAPA (Corrective & Preventive Action).

Dibangun dengan **ASP.NET Core 10** + **Entity Framework Core** + **PostgreSQL**.

---

## Tech Stack

| Layer | Teknologi |
|---|---|
| Framework | ASP.NET Core 10 |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL (lokal / Supabase) |
| Auth | JWT Bearer Token |
| Password | BCrypt.Net |
| Docs | Swagger / OpenAPI |

---

## Struktur Project

```
QualiTrack/
├── Controllers/        # Endpoint API (kasir - terima & balas request)
│   ├── AuthController.cs
│   ├── AuditPlanController.cs
│   ├── FindingController.cs
│   ├── CapaController.cs
│   ├── AuditSessionController.cs
│   ├── ChecklistController.cs
│   └── UploadController.cs
├── Models/             # Entity class (struktur tabel database)
│   ├── User.cs
│   ├── AuditPlan.cs
│   ├── AuditSchedule.cs
│   ├── AuditSession.cs
│   ├── AuditResponse.cs
│   ├── Checklist.cs
│   ├── ChecklistItem.cs
│   ├── Finding.cs
│   ├── CAPA.cs
│   ├── CAPAAction.cs
│   ├── CloseOutVerification.cs
│   └── EvidenceFile.cs
├── DTOs/               # Request/Response object
│   └── AuthDtos.cs
├── Data/
│   └── AppDbContext.cs # Pintu masuk ke database
├── Migrations/         # Catatan perubahan schema database
├── uploads/            # File evidence yang diupload
└── Program.cs          # Setup & konfigurasi aplikasi
```

---

## Cara Menjalankan

### Prasyarat
- .NET 10 SDK
- PostgreSQL

### Setup

**1. Clone repo dan masuk folder**
```bash
git clone <repo-url>
cd QualiTrack
```

**2. Sesuaikan koneksi database di `appsettings.json`**
```json
{
  "ConnectionStrings": {
    "Supabase": "Host=localhost;Database=qualitrack;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "qualitrack-secret-key-minimal-32-characters-panjang",
    "Issuer": "QualiTrack",
    "Audience": "QualiTrack"
  }
}
```

**3. Jalankan migration**
```bash
dotnet ef database update
```

**4. Jalankan aplikasi**
```bash
dotnet run
```

**5. Buka Swagger**
```
http://localhost:5146/swagger
```

---

## API Endpoints

### 🔐 Authentication

| Method | Endpoint | Deskripsi |
|---|---|---|
| POST | `/api/Auth/register` | Daftar user baru |
| POST | `/api/Auth/login` | Login, dapat JWT token |
| POST | `/api/Auth/logout` | Logout (hapus token di client) |

**Register:**
```json
POST /api/Auth/register
{
  "fullName": "Dika Admin",
  "email": "dika@qualitrack.com",
  "password": "123456",
  "role": "Admin"
}
```
Role yang valid: `Admin`, `QualityManager`, `Auditor`, `Auditee`

**Login:**
```json
POST /api/Auth/login
{
  "email": "dika@qualitrack.com",
  "password": "123456"
}
```
Response:
```json
{
  "token": "eyJhbGci...",
  "role": "Admin",
  "fullName": "Dika Admin"
}
```
> Token dipakai di header `Authorization: Bearer <token>` untuk endpoint yang butuh auth.

---

### 📋 Audit Plan

| Method | Endpoint | Deskripsi |
|---|---|---|
| GET | `/api/AuditPlan` | List semua audit plan |
| GET | `/api/AuditPlan/{id}` | Detail audit plan |
| POST | `/api/AuditPlan` | Buat audit plan baru |
| PUT | `/api/AuditPlan/{id}` | Update audit plan |
| DELETE | `/api/AuditPlan/{id}` | Hapus audit plan |

**Buat Audit Plan:**
```json
POST /api/AuditPlan
{
  "title": "Audit ISO 9001 Q1 2026",
  "year": 2026,
  "standard": "ISO9001",
  "schedules": []
}
```

---

### 🔍 Finding

| Method | Endpoint | Deskripsi |
|---|---|---|
| GET | `/api/Finding` | List semua finding (bisa filter) |
| GET | `/api/Finding/{id}` | Detail finding |
| POST | `/api/Finding` | Catat finding baru |
| PATCH | `/api/Finding/{id}/status` | Update status finding |
| DELETE | `/api/Finding/{id}` | Hapus finding |

**Filter Finding:**
```
GET /api/Finding?status=Open&category=MajorNC
```

**Kategori:** `MajorNC`, `MinorNC`, `Observation`, `OFI`

**Status:** `Open`, `InProgress`, `Closed`

**Buat Finding:**
```json
POST /api/Finding
{
  "category": "MajorNC",
  "description": "Prosedur tidak terdokumentasi di bagian produksi",
  "clauseRef": "ISO9001 8.1"
}
```

**Update Status:**
```json
PATCH /api/Finding/{id}/status
"InProgress"
```

---

### ⚙️ CAPA

| Method | Endpoint | Deskripsi |
|---|---|---|
| GET | `/api/Capa` | List semua CAPA (bisa filter) |
| GET | `/api/Capa/{id}` | Detail CAPA |
| GET | `/api/Capa/overdue` | CAPA yang sudah melewati deadline |
| POST | `/api/Capa/finding/{findingId}` | Buat CAPA dari finding |
| PUT | `/api/Capa/{id}` | Update CAPA |
| PATCH | `/api/Capa/{id}/status` | Update status CAPA |
| POST | `/api/Capa/{id}/actions` | Tambah progress action |
| POST | `/api/Capa/{id}/closeout` | Verifikasi close out |
| DELETE | `/api/Capa/{id}` | Hapus CAPA |

**Buat CAPA:**
```json
POST /api/Capa/finding/{findingId}
{
  "rootCause": "Tidak ada SOP tertulis",
  "correctiveAction": "Buat SOP dokumen produksi",
  "preventiveAction": "Training rutin tiap bulan",
  "deadline": "2026-06-30"
}
```

**Status CAPA:** `Open` → `InProgress` → `PendingVerification` → `Closed`

**Tambah Action:**
```json
POST /api/Capa/{id}/actions
{
  "description": "Draft SOP sudah selesai dibuat",
  "doneById": "user-id"
}
```

**Close Out:**
```json
POST /api/Capa/{id}/closeout
{
  "isEffective": true,
  "verificationNotes": "SOP sudah diimplementasikan dan efektif",
  "verifiedById": "user-id"
}
```
> Close out otomatis mengubah status Finding terkait menjadi `Closed`

---

### 📁 Upload File Evidence

| Method | Endpoint | Deskripsi |
|---|---|---|
| POST | `/api/Upload/finding/{findingId}` | Upload foto/dokumen untuk finding |
| POST | `/api/Upload/capa-action/{actionId}` | Upload foto/dokumen untuk CAPA action |

**Upload:**
```bash
curl -X POST /api/Upload/finding/{findingId} \
  -F "file=@/path/to/foto.jpg"
```

Format yang didukung: `JPG`, `PNG`, `PDF`

Ukuran maksimal: **10 MB**

Response:
```json
{
  "fileId": "uuid",
  "fileName": "foto.jpg",
  "url": "/uploads/uuid_foto.jpg"
}
```

---

## Alur Lengkap Audit

```
1. Buat Audit Plan
        ↓
2. Buat AuditSession (field audit)
        ↓
3. Isi Checklist + foto evidence
        ↓
4. Catat Finding (MajorNC / MinorNC / Obs / OFI)
        ↓
5. Buat CAPA dari Finding
        ↓
6. Tambah CAPA Actions (progress update)
        ↓
7. Close Out + verifikasi efektivitas
        ↓
8. Finding & CAPA otomatis Closed ✅
```

---

## Database Schema

```
User
 └── CAPA (sebagai PIC)

AuditPlan
 └── AuditSchedule (auditor per klausul)
      └── AuditSession
           ├── AuditResponse (jawaban checklist)
           │    └── EvidenceFile (foto)
           └── Finding
                └── CAPA
                     ├── CAPAAction
                     │    └── EvidenceFile
                     └── CloseOutVerification

Checklist
 └── ChecklistItem
```

---

## Catatan Pengembangan

- **Local DB:** Saat ini menggunakan PostgreSQL lokal. Untuk production ganti connection string ke Supabase di `appsettings.json`
- **Auth:** Endpoint belum semuanya di-protect dengan `[Authorize]` — perlu ditambahkan di sprint berikutnya
- **File Storage:** File tersimpan di folder `uploads/` lokal. Untuk production perlu migrasi ke Supabase Storage
- **Repository Pattern:** Belum diimplementasi — controller langsung akses DbContext. Bisa direfactor di sprint berikutnya

---
