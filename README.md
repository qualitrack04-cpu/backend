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
| Password Hashing | BCrypt.Net |
| API Docs | Swagger / OpenAPI |
| Environment | DotNetEnv |

---

## Struktur Project

```
QualiTrack/
├── Controllers/
│   ├── AuthController.cs           # Register, Login, Logout, Forgot Password
│   ├── AuditPlanController.cs      # CRUD Audit Plan + Role-based access
│   ├── FindingController.cs        # CRUD Finding + filter status/kategori/tanggal
│   ├── CapaController.cs           # CRUD CAPA + actions + closeout + overdue
│   ├── AuditSessionController.cs   # CRUD Audit Session
│   ├── ChecklistController.cs      # CRUD Checklist + filter + items endpoint
│   └── UploadController.cs         # Upload foto/dokumen evidence
├── Models/
│   ├── User.cs
│   ├── AuditPlan.cs                # + field Priority (Low/Medium/High/Critical)
│   ├── AuditSchedule.cs
│   ├── AuditSession.cs
│   ├── AuditResponse.cs
│   ├── Checklist.cs                # + field Department
│   ├── ChecklistItem.cs
│   ├── Finding.cs
│   ├── CAPA.cs
│   ├── CAPAAction.cs
│   ├── CloseOutVerification.cs
│   └── EvidenceFile.cs
├── DTOs/
│   ├── AuthDtos.cs                 # RegisterRequest, LoginRequest, AuthResponse, ForgotPasswordRequest
│   └── UpdateCapaRequest.cs
├── Data/
│   ├── AppDbContext.cs
│   └── DbSeeder.cs                 # Seed data checklist template per standar & departemen
├── Filters/
│   └── ValidateModelAttribute.cs
├── Middlewares/
│   └── GlobalExceptionMiddleware.cs
├── Migrations/                     # EF Core migrations
├── uploads/                        # File evidence yang diupload
└── Program.cs
```

---

## Cara Menjalankan

### Prasyarat
- .NET 10 SDK
- PostgreSQL

### Setup

**1. Clone repo**
```bash
git clone <repo-url>
cd QualiTrack
```

**2. Sesuaikan `appsettings.json`**
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

> Saat pertama jalan, aplikasi otomatis seed 4 template checklist ke database.

**5. Buka Swagger**
```
http://localhost:5146/swagger
```

---

## API Endpoints

### 🔐 Authentication

| Method | Endpoint | Deskripsi | Auth |
|---|---|---|---|
| POST | `/api/Auth/register` | Daftar user baru | ❌ |
| POST | `/api/Auth/login` | Login, dapat JWT token | ❌ |
| POST | `/api/Auth/logout` | Logout | ❌ |
| POST | `/api/Auth/forgot-password` | Reset password | ❌ |
| GET | `/api/Auth/whoami` | Cek info user dari token | ✅ |

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

**Forgot Password:**
```json
POST /api/Auth/forgot-password
{
  "email": "dika@qualitrack.com",
  "newPassword": "newpass123",
  "confirmPassword": "newpass123"
}
```

> Token JWT dipakai di header `Authorization: Bearer <token>` untuk semua endpoint yang butuh auth.

---

### 📋 Audit Plan

| Method | Endpoint | Deskripsi | Role |
|---|---|---|---|
| GET | `/api/AuditPlan` | List semua audit plan | Admin, QualityManager, Auditor |
| GET | `/api/AuditPlan/{id}` | Detail audit plan | Admin, QualityManager, Auditor |
| POST | `/api/AuditPlan` | Buat audit plan baru | Admin, QualityManager |
| PUT | `/api/AuditPlan/{id}` | Update audit plan | Admin, QualityManager |
| DELETE | `/api/AuditPlan/{id}` | Hapus audit plan | Admin |

**Buat Audit Plan:**
```json
POST /api/AuditPlan
Authorization: Bearer <token>

{
  "title": "Audit ISO 9001 Q1 2026",
  "year": 2026,
  "standard": "ISO9001",
  "schedules": []
}
```

**Priority:** `Low`, `Medium`, `High`, `Critical` (default: `Medium`)

Response:
```json
{
  "message": "Audit plan berhasil dibuat",
  "data": { ... }
}
```

---

### 🔍 Finding

| Method | Endpoint | Deskripsi | Auth |
|---|---|---|---|
| GET | `/api/Finding` | List semua finding | ✅ |
| GET | `/api/Finding/{id}` | Detail finding | ✅ |
| POST | `/api/Finding` | Catat finding baru | ✅ |
| PATCH | `/api/Finding/{id}/status` | Update status finding | ✅ |
| DELETE | `/api/Finding/{id}` | Hapus finding | ✅ |

**Filter Finding:**
```
GET /api/Finding?status=Open
GET /api/Finding?category=MajorNC
GET /api/Finding?from=2026-01-01&to=2026-12-31
GET /api/Finding?status=Open&category=MajorNC&from=2026-01-01
```

**Kategori:** `MajorNC`, `MinorNC`, `Observation`, `OFI`

**Status:** `Open` → `InProgress` → `Closed`

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
```
PATCH /api/Finding/{id}/status
"InProgress"
```

---

### ⚙️ CAPA

| Method | Endpoint | Deskripsi | Auth |
|---|---|---|---|
| GET | `/api/Capa` | List semua CAPA | ✅ |
| GET | `/api/Capa/{id}` | Detail CAPA | ✅ |
| GET | `/api/Capa/overdue` | CAPA yang melewati deadline | ✅ |
| POST | `/api/Capa/finding/{findingId}` | Buat CAPA dari finding | ✅ |
| PUT | `/api/Capa/{id}` | Update CAPA | ✅ |
| PATCH | `/api/Capa/{id}/status` | Update status CAPA | ✅ |
| POST | `/api/Capa/{id}/actions` | Tambah progress action | ✅ |
| POST | `/api/Capa/{id}/closeout` | Verifikasi close out | ✅ |
| DELETE | `/api/Capa/{id}` | Hapus CAPA | ✅ |

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

### ✅ Checklist

| Method | Endpoint | Deskripsi | Auth |
|---|---|---|---|
| GET | `/api/Checklist` | List semua checklist | ❌ |
| GET | `/api/Checklist/{id}` | Detail checklist | ❌ |
| GET | `/api/Checklist/{id}/items` | Items checklist urut by orderIndex | ❌ |
| POST | `/api/Checklist` | Buat checklist baru | ❌ |
| DELETE | `/api/Checklist/{id}` | Hapus checklist | ❌ |

**Filter Checklist:**
```
GET /api/Checklist?standard=ISO9001
GET /api/Checklist?department=Warehouse
GET /api/Checklist?standard=ISO9001&department=Warehouse
```

**Template yang sudah tersedia (seed data):**

| Template | Standard | Departemen | Jumlah Item |
|---|---|---|---|
| ISO 9001 - Warehouse | ISO9001 | Warehouse | 5 |
| ISO 14001 - Warehouse | ISO14001 | Warehouse | 5 |
| ISO 9001 - Produksi | ISO9001 | Produksi | 5 |
| ISO 14001 - Produksi | ISO14001 | Produksi | 5 |

---

### 📁 Upload File Evidence

| Method | Endpoint | Deskripsi 
|---|---|---|
| POST | `/api/Upload/finding/{findingId}` | Upload foto/dokumen untuk finding 
| POST | `/api/Upload/capa-action/{actionId}` | Upload foto/dokumen untuk CAPA action 
| GET | `/api/Upload/finding/{findingId}` | List file evidence finding 

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
1. Register & Login → dapat JWT token
        ↓
2. Buat Audit Plan (dengan Priority)
        ↓
3. Pilih Checklist template (filter by standard & department)
        ↓
4. Jalankan AuditSession
        ↓
5. Isi AuditResponse per ChecklistItem + upload foto evidence
        ↓
6. Catat Finding (MajorNC / MinorNC / Observation / OFI)
        ↓
7. Buat CAPA dari Finding
        ↓
8. Tambah CAPA Actions (progress update)
        ↓
9. Close Out + verifikasi efektivitas
        ↓
10. Finding & CAPA otomatis Closed ✅
```

---

## Database Schema

```
User
 └── CAPA (sebagai PIC)

AuditPlan (+ Priority)
 └── AuditSchedule (auditor per klausul)
      └── AuditSession
           ├── AuditResponse (jawaban checklist)
           │    └── EvidenceFile (foto)
           └── Finding
                └── CAPA
                     ├── CAPAAction
                     │    └── EvidenceFile
                     └── CloseOutVerification

Checklist (+ Department)
 └── ChecklistItem
```

---

## Migrations

| Migration | Deskripsi |
|---|---|
| `InitialCreate` | Schema awal semua tabel |
| `AddUserTable` | Tabel User untuk auth |
| `MakeSessionIdNullable` | SessionId di Finding jadi nullable |
| `AddAuditPriority` | Field Priority di AuditPlan |
| `AddDepartmentToChecklist` | Field Department di Checklist |

---


## Catatan Pengembangan

- **Local DB:** Saat ini pakai PostgreSQL lokal. Untuk production ganti connection string ke Supabase di `appsettings.json`
- **File Storage:** File tersimpan di folder `uploads/` lokal. Sprint 2 akan migrasi ke Supabase Storage
- **Forgot Password:** Saat ini reset langsung via API tanpa email. Sprint 3 akan tambah email SMTP
- **User Management:** CRUD user belum ada, direncanakan sprint berikutnya sesuai feedback stakeholder
- **Repository Pattern:** Belum diimplementasi, bisa direfactor sprint berikutnya

---

## Sprint Progress

| Fitur | Status | Sprint |
|---|---|---|
| Authentication (register, login, logout, forgot password) | ✅ Done | Sprint 1 |
| Audit Plan CRUD + Priority | ✅ Done | Sprint 1 |
| Finding CRUD + filter + logic status | ✅ Done | Sprint 1 |
| CAPA CRUD + actions + closeout + overdue | ✅ Done | Sprint 1 |
| Checklist template seed data | ✅ Done | Sprint 1 |
| Upload file evidence | ✅ Done | Sprint 1 |
| Role-based access control | ✅ Done | Sprint 1 |
| Documentation upload ke cloud storage | 🔜 Sprint 2 |
| Dashboard KPI API | 🔜 Sprint 2/3 |
| User Management CRUD | 🔜 Sprint 2 |
| Forgot password via email | 🔜 Sprint 3 |
| Reporting PDF | 🔜 Sprint 4 |

---

