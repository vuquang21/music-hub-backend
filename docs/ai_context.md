# MUSIC APP — Backend API Specification

> **ASP.NET Core 8 · Clean Architecture · REST API**

| | |
|---|---|
| Phiên bản | v1.0.0 |
| Trạng thái | Draft |
| Ngày tạo | 2026-03-27 |
| Framework | ASP.NET Core 8 |
| Kiến trúc | Clean Architecture |
| Database | PostgreSQL + Redis |

---

## Mục lục

1. [Tổng quan hệ thống](#1-tổng-quan-hệ-thống)
2. [Clean Architecture — Cấu trúc Layer](#2-clean-architecture--cấu-trúc-layer)
3. [Domain Layer](#3-domain-layer)
4. [Application Layer — CQRS với MediatR](#4-application-layer--cqrs-với-mediatr)
5. [API Conventions & Quy ước chung](#5-api-conventions--quy-ước-chung)
6. [API Endpoints](#6-api-endpoints)
7. [Infrastructure Layer](#7-infrastructure-layer)
8. [Presentation Layer — Controllers](#8-presentation-layer--controllers)
9. [Program.cs & Dependency Injection](#9-programcs--dependency-injection)
10. [Authentication — Implementation chi tiết](#10-authentication--implementation-chi-tiết)
11. [Testing Strategy](#11-testing-strategy)
12. [Implementation Checklist](#12-implementation-checklist)

---

## 1. Tổng quan hệ thống

Tài liệu này mô tả chi tiết kiến trúc backend, cấu trúc project, các API endpoint và quy ước lập trình cho Music App — một nền tảng streaming âm nhạc xây dựng trên ASP.NET Core 8 theo Clean Architecture.

### 1.1 Mục tiêu thiết kế

- Separation of Concerns rõ ràng theo từng layer
- Testability cao — mỗi layer có thể unit test độc lập
- Extensibility — dễ thêm tính năng mà không phá vỡ code hiện có
- Domain-driven: business logic tập trung ở Domain layer
- RESTful API chuẩn với versioning, pagination, filtering

### 1.2 Tech Stack

| Thành phần | Công nghệ | Ghi chú |
|---|---|---|
| Runtime | ASP.NET Core 8 | LTS, cross-platform |
| ORM | Entity Framework Core 8 | Code-first migrations |
| Database | PostgreSQL 16 | Primary data store |
| Cache | Redis 7 | Session, rate limit, hot data |
| Messaging | RabbitMQ / MassTransit | Async events, notifications |
| Auth | JWT + Refresh Token | Bearer scheme, OAuth2 ready |
| Docs | Swagger / Scalar | OpenAPI 3.0 |
| Storage | AWS S3 / Azure Blob | Audio files, artwork |
| CDN | CloudFront / Azure CDN | Audio delivery |
| Logging | Serilog + Seq | Structured logging |
| Testing | xUnit + Moq + FluentAssertions | Unit & integration tests |

---

## 2. Clean Architecture — Cấu trúc Layer

Hệ thống chia thành 4 layer chính, phụ thuộc hướng vào trong (Dependency Rule): Infrastructure và Presentation phụ thuộc vào Application; Application phụ thuộc vào Domain; Domain không phụ thuộc bất kỳ layer nào.

### 2.1 Sơ đồ phụ thuộc

```
┌────────────────────────────────────────────────┐
│           Presentation Layer                   │
│   (MusicApp.API — Controllers, Middlewares)    │
└────────────────────┬───────────────────────────┘
                     │ depends on
┌────────────────────▼───────────────────────────┐
│           Application Layer                    │
│  (MusicApp.Application — UseCases, DTOs,       │
│   CQRS Commands/Queries, Interfaces)           │
└────────────────────┬───────────────────────────┘
        ┌────────────┤ depends on
┌───────▼──────┐    │    ┌───────────────────────┐
│   Domain     │    │    │  Infrastructure Layer  │
│   Layer      │    └───►│  (MusicApp.Infra —    │
│ (Entities,   │         │   EF Core, Repos,      │
│  Domain Svc, │         │   External Services)   │
│  Value Obj)  │         └───────────────────────┘
└──────────────┘
```

### 2.2 Cấu trúc Solution

```
MusicApp.sln
├── src/
│   ├── MusicApp.Domain/                  # Entities, Value Objects, Domain Events
│   │   ├── Entities/
│   │   │   ├── Track.cs
│   │   │   ├── Album.cs
│   │   │   ├── Artist.cs
│   │   │   ├── Playlist.cs
│   │   │   └── User.cs
│   │   ├── ValueObjects/
│   │   │   ├── ISRC.cs
│   │   │   ├── Duration.cs
│   │   │   └── AudioQuality.cs
│   │   ├── Events/                       # Domain Events
│   │   ├── Exceptions/                   # Domain Exceptions
│   │   └── Interfaces/                   # Repository contracts
│   │
│   ├── MusicApp.Application/             # Use Cases, CQRS, Mappings
│   │   ├── Auth/
│   │   │   ├── Commands/
│   │   │   │   ├── Register/
│   │   │   │   ├── Login/
│   │   │   │   ├── RefreshToken/
│   │   │   │   └── Logout/
│   │   │   └── Queries/
│   │   ├── Tracks/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateTrack/
│   │   │   │   └── UpdateTrack/
│   │   │   └── Queries/
│   │   │       ├── GetTrackById/
│   │   │       └── GetTracksPaged/
│   │   ├── Common/
│   │   │   ├── Interfaces/               # ICurrentUser, ITokenService...
│   │   │   ├── Mappings/                 # AutoMapper profiles
│   │   │   ├── Behaviors/                # MediatR pipeline behaviors
│   │   │   │   ├── ValidationBehavior.cs
│   │   │   │   ├── LoggingBehavior.cs
│   │   │   │   └── CachingBehavior.cs
│   │   │   └── DTOs/
│   │   └── DependencyInjection.cs
│   │
│   ├── MusicApp.Infrastructure/          # EF Core, External services
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/
│   │   │   ├── Migrations/
│   │   │   └── Repositories/
│   │   ├── Auth/
│   │   │   ├── TokenService.cs           # JWT generation & validation
│   │   │   ├── PasswordHasher.cs
│   │   │   └── RefreshTokenRepository.cs
│   │   ├── Services/
│   │   │   ├── StorageService.cs
│   │   │   ├── CdnService.cs
│   │   │   └── EmailService.cs
│   │   ├── Caching/                      # Redis
│   │   └── DependencyInjection.cs
│   │
│   └── MusicApp.API/                     # Presentation Layer
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── TracksController.cs
│       │   ├── AlbumsController.cs
│       │   ├── PlaylistsController.cs
│       │   └── UsersController.cs
│       ├── Middlewares/
│       │   ├── ExceptionMiddleware.cs
│       │   └── RequestLoggingMiddleware.cs
│       ├── Filters/
│       ├── Program.cs
│       └── appsettings.json
└── tests/
    ├── MusicApp.Domain.Tests/
    ├── MusicApp.Application.Tests/
    └── MusicApp.API.IntegrationTests/
```

---

## 3. Domain Layer

Domain layer chứa toàn bộ business logic thuần túy. Không có dependency với EF Core, ASP.NET hay bất kỳ thư viện infrastructure nào.

### 3.1 Base Entity

```csharp
// Domain/Common/BaseEntity.cs
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

### 3.2 Core Entities

#### Track

```csharp
// Domain/Entities/Track.cs
public class Track : BaseEntity
{
    public string Title { get; private set; }
    public Guid ArtistId { get; private set; }
    public Guid? AlbumId { get; private set; }
    public ISRC Isrc { get; private set; }          // Value Object
    public Duration Duration { get; private set; }  // Value Object
    public AudioQuality Quality { get; private set; }
    public string StorageKey { get; private set; }  // S3/Blob key
    public string? CdnUrl { get; private set; }
    public int PlayCount { get; private set; }
    public TrackStatus Status { get; private set; } // Draft | Published | Removed

    public Artist Artist { get; private set; }
    public Album? Album { get; private set; }
    public IReadOnlyList<Genre> Genres { get; private set; }

    public static Track Create(string title, Guid artistId, ISRC isrc, Duration duration)
    {
        Guard.Against.NullOrWhiteSpace(title, nameof(title));
        var track = new Track
        {
            Title = title, ArtistId = artistId,
            Isrc = isrc, Duration = duration,
            Status = TrackStatus.Draft
        };
        track.AddDomainEvent(new TrackCreatedEvent(track.Id));
        return track;
    }

    public void IncrementPlayCount()
    {
        PlayCount++;
        AddDomainEvent(new TrackPlayedEvent(Id, ArtistId));
    }
}
```

#### Value Objects

```csharp
// Domain/ValueObjects/ISRC.cs
public record ISRC
{
    public string Value { get; }
    private static readonly Regex Pattern =
        new(@"^[A-Z]{2}-[A-Z0-9]{3}-\d{2}-\d{5}$", RegexOptions.Compiled);

    public ISRC(string value)
    {
        if (!Pattern.IsMatch(value))
            throw new DomainException($"Invalid ISRC: {value}");
        Value = value.ToUpper();
    }

    public static implicit operator string(ISRC isrc) => isrc.Value;
}

// Domain/ValueObjects/Duration.cs
public record Duration
{
    public int Seconds { get; }
    public Duration(int seconds)
    {
        if (seconds <= 0) throw new DomainException("Duration must be positive.");
        Seconds = seconds;
    }
    public string Formatted => TimeSpan.FromSeconds(Seconds).ToString(@"m\:ss");
}
```

---

## 4. Application Layer — CQRS với MediatR

Application layer điều phối use cases thông qua CQRS pattern (MediatR). Mỗi use case là một Command hoặc Query riêng biệt, có Handler xử lý và Validator kiểm tra đầu vào.

### 4.1 CQRS Pattern

```csharp
// Commands thay đổi state
public record CreateTrackCommand(
    string Title, Guid ArtistId,
    string Isrc, int DurationSeconds,
    IFormFile AudioFile) : IRequest<Guid>;

public class CreateTrackCommandHandler : IRequestHandler<CreateTrackCommand, Guid>
{
    private readonly ITrackRepository _trackRepo;
    private readonly IStorageService _storage;
    private readonly IUnitOfWork _uow;

    public async Task<Guid> Handle(CreateTrackCommand cmd, CancellationToken ct)
    {
        var isrc = new ISRC(cmd.Isrc);
        var duration = new Duration(cmd.DurationSeconds);
        var track = Track.Create(cmd.Title, cmd.ArtistId, isrc, duration);

        var storageKey = await _storage.UploadAudioAsync(cmd.AudioFile, ct);
        track.SetStorageKey(storageKey);

        await _trackRepo.AddAsync(track, ct);
        await _uow.SaveChangesAsync(ct);
        return track.Id;
    }
}

// Queries chỉ đọc dữ liệu
public record GetTrackByIdQuery(Guid Id) : IRequest<TrackDto>;

public class GetTrackByIdQueryHandler : IRequestHandler<GetTrackByIdQuery, TrackDto>
{
    private readonly ITrackReadRepository _readRepo;
    private readonly IMapper _mapper;

    public async Task<TrackDto> Handle(GetTrackByIdQuery q, CancellationToken ct)
    {
        var track = await _readRepo.GetByIdAsync(q.Id, ct)
            ?? throw new NotFoundException(nameof(Track), q.Id);
        return _mapper.Map<TrackDto>(track);
    }
}
```

### 4.2 Pipeline Behaviors

```csharp
// ValidationBehavior.cs — chạy FluentValidation trước mọi handler
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(r => r.Errors)
            .Where(e => e != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}

// CachingBehavior.cs — cache IQuery results trong Redis
public class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var key = request.CacheKey;
        var cached = await _cache.GetAsync<TResponse>(key, ct);
        if (cached is not null) return cached;

        var response = await next();
        await _cache.SetAsync(key, response, request.Expiry, ct);
        return response;
    }
}
```

---

## 5. API Conventions & Quy ước chung

### 5.1 Base URL & Versioning

```
Base URL:    https://api.musicapp.io
Versioning:  /api/v{version}/...

Ví dụ:
  https://api.musicapp.io/api/v1/tracks
  https://api.musicapp.io/api/v1/playlists/{id}
```

### 5.2 Authentication

Mọi endpoint (trừ auth và public endpoints) yêu cầu JWT Bearer token.

```
Authorization: Bearer <access_token>

// Token payload
{
  "sub":   "user-uuid",
  "email": "user@example.com",
  "roles": ["user", "artist"],
  "exp":   1711500000,
  "iss":   "musicapp-auth"
}

// Access token:  15 phút
// Refresh token: 30 ngày (lưu trong HttpOnly Cookie)
```

### 5.3 Request / Response Format

```json
// Standard success response
{
  "success": true,
  "data": { },
  "message": null,
  "meta": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 543,
    "totalPages": 28
  }
}

// Standard error response
{
  "success": false,
  "data": null,
  "message": "Validation failed.",
  "errors": {
    "title": ["Title is required.", "Max length is 200."],
    "isrc":  ["Invalid ISRC format."]
  },
  "traceId": "00-abc123-def456-00"
}
```

### 5.4 HTTP Status Codes

| Status | Tên | Khi nào dùng |
|---|---|---|
| 200 | OK | GET, PUT, PATCH thành công |
| 201 | Created | POST tạo resource thành công — kèm Location header |
| 204 | No Content | DELETE thành công, không có body |
| 400 | Bad Request | Validation error, malformed request body |
| 401 | Unauthorized | Thiếu hoặc token không hợp lệ |
| 403 | Forbidden | Không đủ quyền truy cập resource |
| 404 | Not Found | Resource không tồn tại |
| 409 | Conflict | Trùng lặp (ISRC đã tồn tại, email đã dùng...) |
| 422 | Unprocessable | Business rule violation |
| 429 | Too Many Requests | Rate limit exceeded — kèm Retry-After header |
| 500 | Server Error | Lỗi không xử lý được — log + traceId |

### 5.5 Pagination & Filtering

```
GET /api/v1/tracks
  ?page=1           // trang hiện tại (default: 1)
  &pageSize=20      // số item/trang (default: 20, max: 100)
  &search=bohemian  // full-text search
  &genre=rock       // filter theo genre
  &artistId=uuid    // filter theo artist
  &sortBy=playCount // field sắp xếp
  &sortDir=desc     // asc | desc
  &from=2024-01-01  // filter theo ngày tạo
  &to=2024-12-31
```

---

## 6. API Endpoints

### 6.1 Tracks

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `GET` | `/api/v1/tracks` | Public | Danh sách track (phân trang) |
| `GET` | `/api/v1/tracks/{id}` | Public | Chi tiết track + CDN URL |
| `POST` | `/api/v1/tracks` | Artist, Admin | Upload track mới |
| `PUT` | `/api/v1/tracks/{id}` | Artist (owner), Admin | Cập nhật metadata |
| `PATCH` | `/api/v1/tracks/{id}/publish` | Artist (owner), Admin | Publish track |
| `DELETE` | `/api/v1/tracks/{id}` | Artist (owner), Admin | Xóa mềm |
| `POST` | `/api/v1/tracks/{id}/play` | User | Ghi nhận lượt phát, trả streaming URL |

```json
// POST /api/v1/tracks — multipart/form-data
{
  "title":           "Bohemian Rhapsody",
  "isrc":            "GB-EMI-75-00234",
  "durationSeconds": 354,
  "albumId":         "uuid | null",
  "genres":          ["rock", "classic-rock"],
  "audioFile":       "<binary, max 100MB>",
  "artworkFile":     "<binary | null, max 5MB>"
}

// Response 201 Created
// Location: /api/v1/tracks/{newId}
{
  "success": true,
  "data": { "id": "uuid", "status": "Draft" }
}
```

> **Lưu ý:** `POST /play` không stream trực tiếp — trả về pre-signed CDN URL. Client dùng URL này để stream audio từ CDN.

### 6.2 Playlists

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `GET` | `/api/v1/playlists` | Public/User | Danh sách playlist |
| `GET` | `/api/v1/playlists/{id}` | Public/User | Chi tiết + tracklist |
| `POST` | `/api/v1/playlists` | User | Tạo playlist |
| `PUT` | `/api/v1/playlists/{id}` | Owner | Cập nhật thông tin |
| `DELETE` | `/api/v1/playlists/{id}` | Owner | Xóa playlist |
| `POST` | `/api/v1/playlists/{id}/tracks` | Owner, Collaborator | Thêm track |
| `DELETE` | `/api/v1/playlists/{id}/tracks/{trackId}` | Owner, Collaborator | Xóa track |
| `PUT` | `/api/v1/playlists/{id}/tracks/reorder` | Owner | Sắp xếp lại thứ tự |
| `POST` | `/api/v1/playlists/{id}/follow` | User | Follow / unfollow |

### 6.3 Albums

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `GET` | `/api/v1/albums` | Public | Danh sách album |
| `GET` | `/api/v1/albums/{id}` | Public | Chi tiết + tracklist |
| `POST` | `/api/v1/albums` | Artist, Admin | Tạo album |
| `PUT` | `/api/v1/albums/{id}` | Artist (owner), Admin | Cập nhật |
| `PATCH` | `/api/v1/albums/{id}/publish` | Artist (owner), Admin | Publish album |
| `DELETE` | `/api/v1/albums/{id}` | Artist (owner), Admin | Xóa album |

### 6.4 Authentication

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `POST` | `/api/v1/auth/register` | Public | Đăng ký tài khoản |
| `POST` | `/api/v1/auth/login` | Public | Đăng nhập |
| `POST` | `/api/v1/auth/refresh` | Public | Làm mới access token |
| `POST` | `/api/v1/auth/logout` | User | Invalidate refresh token |
| `POST` | `/api/v1/auth/forgot-password` | Public | Gửi email reset |
| `POST` | `/api/v1/auth/reset-password` | Public | Đặt lại mật khẩu |

### 6.5 Users & Library

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `GET` | `/api/v1/users/me` | User | Thông tin user hiện tại |
| `PUT` | `/api/v1/users/me` | User | Cập nhật profile |
| `GET` | `/api/v1/users/me/library` | User | Thư viện cá nhân |
| `GET` | `/api/v1/users/me/history` | User | Lịch sử nghe |
| `POST` | `/api/v1/users/me/tracks/{id}/like` | User | Like / unlike track |
| `GET` | `/api/v1/users/{id}/profile` | Public | Public profile |
| `POST` | `/api/v1/users/{id}/follow` | User | Follow / unfollow user |

---

## 7. Infrastructure Layer

### 7.1 Repository Pattern

```csharp
// Domain/Interfaces/ITrackRepository.cs (contract)
public interface ITrackRepository
{
    Task<Track?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Track?> GetByIsrcAsync(string isrc, CancellationToken ct = default);
    Task<PagedResult<Track>> GetPagedAsync(TrackFilter filter, CancellationToken ct = default);
    Task AddAsync(Track track, CancellationToken ct = default);
    void Update(Track track);
    void Remove(Track track);
}

// Infrastructure/Persistence/Repositories/TrackRepository.cs
public class TrackRepository : ITrackRepository
{
    private readonly AppDbContext _context;

    public async Task<Track?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.Tracks
            .Include(t => t.Artist)
            .Include(t => t.Genres)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<PagedResult<Track>> GetPagedAsync(TrackFilter filter, CancellationToken ct)
    {
        var query = _context.Tracks.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(t => t.Title.Contains(filter.Search));
        if (filter.Genre is not null)
            query = query.Where(t => t.Genres.Any(g => g.Slug == filter.Genre));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.PlayCount)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Track>(items, total, filter.Page, filter.PageSize);
    }
}
```

### 7.2 Unit of Work

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IPublisher _publisher;

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        var entities = _context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var result = await _context.SaveChangesAsync(ct);

        foreach (var entity in entities)
        {
            foreach (var domainEvent in entity.DomainEvents)
                await _publisher.Publish(domainEvent, ct);
            entity.ClearDomainEvents();
        }
        return result;
    }
}
```

### 7.3 AppDbContext & Entity Configuration

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}

// Infrastructure/Persistence/Configurations/TrackConfiguration.cs
public class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        builder.OwnsOne(t => t.Isrc, isrc =>
            isrc.Property(i => i.Value).HasColumnName("Isrc").HasMaxLength(15));
        builder.OwnsOne(t => t.Duration, d =>
            d.Property(x => x.Seconds).HasColumnName("DurationSeconds"));
        builder.HasOne(t => t.Artist).WithMany(a => a.Tracks)
               .HasForeignKey(t => t.ArtistId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(t => t.Isrc).IsUnique();
        builder.HasQueryFilter(t => t.Status != TrackStatus.Removed);
    }
}
```

---

## 8. Presentation Layer — Controllers

### 8.1 Base Controller

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender _sender;
    protected ApiController(ISender sender) => _sender = sender;

    protected IActionResult Ok<T>(T data, string? message = null)
        => base.Ok(ApiResponse.Success(data, message));

    protected IActionResult Created<T>(T data, string location)
        => base.Created(location, ApiResponse.Success(data));

    protected Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
```

### 8.2 TracksController

```csharp
[Authorize]
public class TracksController : ApiController
{
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetTracksPagedQuery query, CancellationToken ct)
        => Ok(await _sender.Send(query, ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetTrackByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Roles = "Artist,Admin")]
    [RequestSizeLimit(105_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(
        [FromForm] CreateTrackCommand cmd, CancellationToken ct)
    {
        var id = await _sender.Send(cmd, ct);
        return Created(new { id }, $"/api/v1/tracks/{id}");
    }

    [HttpPost("{id:guid}/play")]
    public async Task<IActionResult> Play(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new RecordPlayCommand(id, CurrentUserId), ct));
}
```

### 8.3 Exception Middleware

```csharp
public class ExceptionMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try { await next(context); }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            await WriteError(context, "Validation failed.", ex.Errors);
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await WriteError(context, ex.Message);
        }
        catch (ForbiddenException)
        {
            context.Response.StatusCode = 403;
            await WriteError(context, "Access denied.");
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = 422;
            await WriteError(context, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");
            context.Response.StatusCode = 500;
            await WriteError(context, "An unexpected error occurred.");
        }
    }
}
```

---

## 9. Program.cs & Dependency Injection

```csharp
// MusicApp.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration);

// ── AddApplication() ─────────────────────────────────────
services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssembly).Assembly);
    cfg.AddBehavior(typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(CachingBehavior<,>));
});
services.AddValidatorsFromAssembly(typeof(ApplicationAssembly).Assembly);
services.AddAutoMapper(typeof(ApplicationAssembly).Assembly);

// ── AddInfrastructure() ──────────────────────────────────
services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(config.GetConnectionString("Postgres")));
services.AddScoped<ITrackRepository, TrackRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddStackExchangeRedisCache(opt =>
    opt.Configuration = config.GetConnectionString("Redis"));
services.AddSingleton<IStorageService, S3StorageService>();

// ── Build & middleware pipeline ───────────────────────────
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

if (app.Environment.IsDevelopment())
    app.MapScalarApiReference();

app.Run();
```

---

## 10. Authentication — Implementation chi tiết

### 10.1 Tổng quan luồng xác thực

```
[Client]                          [API]                        [Redis/DB]
   │                                │                               │
   │── POST /auth/register ────────►│                               │
   │                                │── Hash password               │
   │                                │── Save User ─────────────────►│
   │◄─ 201 Created ─────────────────│                               │
   │                                │                               │
   │── POST /auth/login ───────────►│                               │
   │                                │── Verify password             │
   │                                │── Generate AccessToken (JWT)  │
   │                                │── Generate RefreshToken       │
   │                                │── Save RefreshToken ─────────►│
   │◄─ 200 { accessToken } ─────────│                               │
   │   Set-Cookie: refreshToken     │                               │
   │                                │                               │
   │── GET /tracks (Bearer token) ──►│                               │
   │                                │── Validate JWT                │
   │◄─ 200 { data } ────────────────│                               │
   │                                │                               │
   │── POST /auth/refresh ──────────►│                               │
   │   Cookie: refreshToken         │── Validate RefreshToken ─────►│
   │                                │── Issue new AccessToken       │
   │                                │── Rotate RefreshToken ────────►│
   │◄─ 200 { accessToken } ─────────│                               │
```

### 10.2 NuGet Packages

```xml
<!-- MusicApp.Infrastructure/MusicApp.Infrastructure.csproj -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.*" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.*" />
<PackageReference Include="BCrypt.Net-Next" Version="4.*" />
```

### 10.3 Domain Entity — User & RefreshToken

```csharp
// Domain/Entities/User.cs
public class User : BaseEntity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string DisplayName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public UserRole Role { get; private set; }   // User | Artist | Admin
    public bool IsEmailConfirmed { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public static User Create(string email, string passwordHash, string displayName)
    {
        Guard.Against.InvalidEmail(email, nameof(email));
        return new User
        {
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            DisplayName = displayName,
            Role = UserRole.User,
            IsActive = true
        };
    }

    public void AddRefreshToken(RefreshToken token) => _refreshTokens.Add(token);

    public void RevokeRefreshToken(string token, string reason)
    {
        var rt = _refreshTokens.SingleOrDefault(t => t.Token == token)
            ?? throw new DomainException("Refresh token not found.");
        rt.Revoke(reason);
    }

    public RefreshToken? GetActiveRefreshToken(string token)
        => _refreshTokens.SingleOrDefault(t => t.Token == token && t.IsActive);
}

// Domain/Entities/RefreshToken.cs
public class RefreshToken : BaseEntity
{
    public string Token { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedReason { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt is not null;
    public bool IsActive => !IsExpired && !IsRevoked;

    public static RefreshToken Create(Guid userId, int expiryDays = 30)
        => new()
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
        };

    public void Revoke(string reason, string? replacedBy = null)
    {
        RevokedAt = DateTime.UtcNow;
        RevokedReason = reason;
        ReplacedByToken = replacedBy;
    }
}

// Domain/Enums/UserRole.cs
public enum UserRole { User, Artist, Admin }
```

### 10.4 Application — Commands & Interfaces

#### Interfaces

```csharp
// Application/Common/Interfaces/ITokenService.cs
public interface ITokenService
{
    string GenerateAccessToken(User user);
    ClaimsPrincipal? ValidateAccessToken(string token);
}

// Application/Common/Interfaces/IPasswordHasher.cs
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

// Application/Common/Interfaces/ICurrentUser.cs
public interface ICurrentUser
{
    Guid Id { get; }
    string Email { get; }
    UserRole Role { get; }
    bool IsAuthenticated { get; }
}
```

#### DTOs

```csharp
// Application/Auth/DTOs/AuthResponseDto.cs
public record AuthResponseDto(
    string AccessToken,
    int ExpiresIn,       // seconds
    string TokenType,    // "Bearer"
    UserDto User);

// Application/Auth/DTOs/UserDto.cs
public record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    string? AvatarUrl,
    string Role);
```

#### Register Command

```csharp
// Application/Auth/Commands/Register/RegisterCommand.cs
public record RegisterCommand(
    string Email,
    string Password,
    string DisplayName) : IRequest<AuthResponseDto>;

// Application/Auth/Commands/Register/RegisterCommandValidator.cs
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(IUserRepository userRepo)
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress()
            .MustAsync(async (email, ct) =>
                !await userRepo.ExistsByEmailAsync(email, ct))
            .WithMessage("Email already in use.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Must contain uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Must contain lowercase letter.")
            .Matches(@"\d").WithMessage("Must contain a digit.")
            .Matches(@"[\W_]").WithMessage("Must contain a special character.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().MaximumLength(100);
    }
}

// Application/Auth/Commands/Register/RegisterCommandHandler.cs
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public async Task<AuthResponseDto> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        var hash = _hasher.Hash(cmd.Password);
        var user = User.Create(cmd.Email, hash, cmd.DisplayName);

        var refreshToken = RefreshToken.Create(user.Id);
        user.AddRefreshToken(refreshToken);

        await _userRepo.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var accessToken = _tokenService.GenerateAccessToken(user);
        return new AuthResponseDto(
            AccessToken: accessToken,
            ExpiresIn: 900,
            TokenType: "Bearer",
            User: _mapper.Map<UserDto>(user));
    }
}
```

#### Login Command

```csharp
// Application/Auth/Commands/Login/LoginCommand.cs
public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;

// Application/Auth/Commands/Login/LoginCommandHandler.cs
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public async Task<AuthResponseDto> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByEmailAsync(cmd.Email.ToLowerInvariant(), ct)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!_hasher.Verify(cmd.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new ForbiddenException("Account is deactivated.");

        // Revoke old refresh tokens if any active (optional: allow multi-device)
        var refreshToken = RefreshToken.Create(user.Id);
        user.AddRefreshToken(refreshToken);
        await _uow.SaveChangesAsync(ct);

        var accessToken = _tokenService.GenerateAccessToken(user);
        return new AuthResponseDto(
            AccessToken: accessToken,
            ExpiresIn: 900,
            TokenType: "Bearer",
            User: _mapper.Map<UserDto>(user));
    }
}
```

#### RefreshToken Command

```csharp
// Application/Auth/Commands/RefreshToken/RefreshTokenCommand.cs
public record RefreshTokenCommand(string Token) : IRequest<AuthResponseDto>;

// Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandler.cs
public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByRefreshTokenAsync(cmd.Token, ct)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        var existingToken = user.GetActiveRefreshToken(cmd.Token)
            ?? throw new UnauthorizedException("Refresh token expired or revoked.");

        // Rotate: revoke old, issue new
        var newRefreshToken = RefreshToken.Create(user.Id);
        existingToken.Revoke("Replaced", newRefreshToken.Token);
        user.AddRefreshToken(newRefreshToken);
        await _uow.SaveChangesAsync(ct);

        var accessToken = _tokenService.GenerateAccessToken(user);
        return new AuthResponseDto(
            AccessToken: accessToken,
            ExpiresIn: 900,
            TokenType: "Bearer",
            User: _mapper.Map<UserDto>(user));
    }
}
```

#### Logout Command

```csharp
// Application/Auth/Commands/Logout/LogoutCommand.cs
public record LogoutCommand(string RefreshToken) : IRequest;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByRefreshTokenAsync(cmd.RefreshToken, ct);
        if (user is null) return; // Idempotent

        user.RevokeRefreshToken(cmd.RefreshToken, "Logged out");
        await _uow.SaveChangesAsync(ct);
    }
}
```

### 10.5 Infrastructure — TokenService

```csharp
// Infrastructure/Auth/TokenService.cs
public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(IOptions<JwtSettings> settings)
        => _settings = settings.Value;

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.SecretKey));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("displayName", user.DisplayName),
        };

        var token = new JwtSecurityToken(
            issuer:             _settings.Issuer,
            audience:           _settings.Audience,
            claims:             claims,
            notBefore:          DateTime.UtcNow,
            expires:            DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            return handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer           = true,
                ValidIssuer              = _settings.Issuer,
                ValidateAudience         = true,
                ValidAudience            = _settings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = key,
                ValidateLifetime         = true,
                ClockSkew                = TimeSpan.Zero,
            }, out _);
        }
        catch { return null; }
    }
}
```

### 10.6 Infrastructure — PasswordHasher

```csharp
// Infrastructure/Auth/PasswordHasher.cs
public class PasswordHasher : IPasswordHasher
{
    // BCrypt work factor 12 — cân bằng bảo mật và hiệu năng
    private const int WorkFactor = 12;

    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
```

### 10.7 Infrastructure — Cấu hình JWT

```csharp
// Infrastructure/Auth/JwtSettings.cs
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;        // >= 32 chars
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; set; } = 15;
    public int RefreshTokenExpiryDays { get; set; } = 30;
}

// Infrastructure/DependencyInjection.cs (Auth section)
services.Configure<JwtSettings>(config.GetSection("Jwt"));
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<IPasswordHasher, PasswordHasher>();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>()!;
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = jwtSettings.Issuer,
            ValidateAudience         = true,
            ValidAudience            = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = key,
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.Zero,
        };

        // Trả lỗi dạng JSON thay vì HTML
        opt.Events = new JwtBearerEvents
        {
            OnChallenge = async ctx =>
            {
                ctx.HandleResponse();
                ctx.Response.StatusCode = 401;
                ctx.Response.ContentType = "application/json";
                await ctx.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Authentication required.",
                });
            },
            OnForbidden = async ctx =>
            {
                ctx.Response.StatusCode = 403;
                ctx.Response.ContentType = "application/json";
                await ctx.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Access denied.",
                });
            },
        };
    });

services.AddAuthorization(opt =>
{
    opt.AddPolicy("ArtistOrAdmin", p =>
        p.RequireRole("Artist", "Admin"));
    opt.AddPolicy("AdminOnly", p =>
        p.RequireRole("Admin"));
});
```

### 10.8 Infrastructure — CurrentUser Service

```csharp
// Infrastructure/Auth/CurrentUserService.cs
public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
        => _accessor = accessor;

    public Guid Id =>
        Guid.Parse(_accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedException("User not authenticated."));

    public string Email =>
        _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
            ?? throw new UnauthorizedException("User not authenticated.");

    public UserRole Role =>
        Enum.Parse<UserRole>(
            _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? "User");

    public bool IsAuthenticated =>
        _accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}

// Register in DI
services.AddHttpContextAccessor();
services.AddScoped<ICurrentUser, CurrentUserService>();
```

### 10.9 Presentation — AuthController

```csharp
// API/Controllers/AuthController.cs
[AllowAnonymous]
public class AuthController : ApiController
{
    public AuthController(ISender sender) : base(sender) { }

    /// <summary>Đăng ký tài khoản mới</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand cmd, CancellationToken ct)
    {
        var result = await _sender.Send(cmd, ct);
        SetRefreshTokenCookie(result.RefreshToken);
        return Created(result with { RefreshToken = null }, "/api/v1/users/me");
    }

    /// <summary>Đăng nhập</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand cmd, CancellationToken ct)
    {
        var result = await _sender.Send(cmd, ct);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(result with { RefreshToken = null });
    }

    /// <summary>Làm mới access token</summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var token = Request.Cookies["refreshToken"]
            ?? throw new UnauthorizedException("Refresh token not found.");

        var result = await _sender.Send(new RefreshTokenCommand(token), ct);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(result with { RefreshToken = null });
    }

    /// <summary>Đăng xuất</summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var token = Request.Cookies["refreshToken"] ?? string.Empty;
        await _sender.Send(new LogoutCommand(token), ct);
        DeleteRefreshTokenCookie();
        return NoContent();
    }

    /// <summary>Quên mật khẩu — gửi email reset</summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand cmd, CancellationToken ct)
    {
        await _sender.Send(cmd, ct);
        return NoContent(); // Luôn 204 dù email có tồn tại hay không (tránh user enumeration)
    }

    /// <summary>Đặt lại mật khẩu với token từ email</summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand cmd, CancellationToken ct)
    {
        await _sender.Send(cmd, ct);
        return NoContent();
    }

    // ── Helpers ──────────────────────────────────────────────────
    private void SetRefreshTokenCookie(string token)
    {
        Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure   = true,
            SameSite = SameSiteMode.Strict,
            Expires  = DateTimeOffset.UtcNow.AddDays(30),
            Path     = "/api/v1/auth",       // Chỉ gửi cookie tới auth endpoints
        });
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure   = true,
            SameSite = SameSiteMode.Strict,
            Path     = "/api/v1/auth",
        });
    }
}
```

### 10.10 appsettings.json — JWT Config

```json
{
  "Jwt": {
    "SecretKey": "YOUR-SUPER-SECRET-KEY-MIN-32-CHARS-CHANGE-IN-PROD",
    "Issuer": "musicapp-auth",
    "Audience": "musicapp-client",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 30
  },
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Database=musicapp;Username=postgres;Password=secret",
    "Redis": "localhost:6379"
  }
}
```

> **Bảo mật:** Không commit `SecretKey` vào source control. Dùng `dotnet user-secrets` cho local dev và Azure Key Vault / AWS Secrets Manager cho production.

### 10.11 EF Core — RefreshToken Configuration

```csharp
// Infrastructure/Persistence/Configurations/UserConfiguration.cs
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.DisplayName).HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasMany(u => u.RefreshTokens)
               .WithOne()
               .HasForeignKey(rt => rt.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(u => u.IsActive);
    }
}

// Infrastructure/Persistence/Configurations/RefreshTokenConfiguration.cs
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Token).HasMaxLength(128).IsRequired();
        builder.HasIndex(rt => rt.Token).IsUnique();
        builder.HasIndex(rt => new { rt.UserId, rt.IsActive: false });
    }
}
```

### 10.12 Rate Limiting cho Auth endpoints

```csharp
// Program.cs — Rate limiting
builder.Services.AddRateLimiter(opt =>
{
    // Strict limit cho login/register để chặn brute force
    opt.AddFixedWindowLimiter("auth", limiter =>
    {
        limiter.Window            = TimeSpan.FromMinutes(15);
        limiter.PermitLimit       = 10;
        limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiter.QueueLimit        = 0;
    });

    // General API limit
    opt.AddTokenBucketLimiter("api", limiter =>
    {
        limiter.TokenLimit            = 100;
        limiter.ReplenishmentPeriod   = TimeSpan.FromMinutes(1);
        limiter.TokensPerPeriod       = 100;
        limiter.AutoReplenishment     = true;
    });

    opt.OnRejected = async (ctx, ct) =>
    {
        ctx.HttpContext.Response.StatusCode = 429;
        ctx.HttpContext.Response.Headers.RetryAfter = "60";
        await ctx.HttpContext.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = "Too many requests. Please try again later.",
        }, ct);
    };
});

// Áp dụng rate limit per-controller
[EnableRateLimiting("auth")]
public class AuthController : ApiController { ... }
```

### 10.13 Security Checklist — Authentication

| # | Kiểm tra | Trạng thái |
|---|---|---|
| 1 | SecretKey >= 32 ký tự, không commit vào git | ☐ |
| 2 | Access token TTL <= 15 phút | ☐ |
| 3 | Refresh token lưu HttpOnly + Secure cookie | ☐ |
| 4 | Refresh token rotation mỗi lần dùng | ☐ |
| 5 | BCrypt work factor >= 12 | ☐ |
| 6 | Rate limiting trên login/register | ☐ |
| 7 | ForgotPassword luôn trả 204 (tránh user enumeration) | ☐ |
| 8 | HTTPS enforced (UseHttpsRedirection) | ☐ |
| 9 | ClockSkew = TimeSpan.Zero khi validate JWT | ☐ |
| 10 | Revoke tất cả refresh token khi đổi mật khẩu | ☐ |
| 11 | Log failed login attempts (Serilog) | ☐ |
| 12 | Xóa expired refresh tokens định kỳ (background job) | ☐ |

---

## 11. Testing Strategy

### 11.1 Pyramid Testing

| Layer | Loại test | Tool | Phạm vi |
|---|---|---|---|
| Domain | Unit | xUnit | Entities, Value Objects, Domain Services |
| Application | Unit | xUnit + Moq | Command/Query Handlers (mock repos) |
| Infrastructure | Integration | xUnit + Testcontainers | Repositories với real PostgreSQL container |
| API | Integration | WebApplicationFactory | Full HTTP request/response cycle |

### 11.2 Unit Test — Domain

```csharp
public class TrackTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var isrc = new ISRC("GB-EMI-75-00234");
        var duration = new Duration(354);
        var artistId = Guid.NewGuid();

        var track = Track.Create("Bohemian Rhapsody", artistId, isrc, duration);

        track.Title.Should().Be("Bohemian Rhapsody");
        track.Status.Should().Be(TrackStatus.Draft);
        track.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TrackCreatedEvent>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyTitle_ShouldThrow(string title)
    {
        var act = () => Track.Create(title, Guid.NewGuid(),
            new ISRC("GB-EMI-75-00234"), new Duration(354));

        act.Should().Throw<ArgumentException>();
    }
}
```

### 11.3 Unit Test — Authentication Handler

```csharp
public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var user = User.Create("user@test.com", "hashed", "Test User");
        _userRepo.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("password123", "hashed")).Returns(true);
        _tokenService.Setup(t => t.GenerateAccessToken(user)).Returns("jwt-token");

        var handler = new LoginCommandHandler(
            _userRepo.Object, _hasher.Object,
            _tokenService.Object, _uow.Object, mapper: null!);

        // Act
        var result = await handler.Handle(
            new LoginCommand("user@test.com", "password123"), CancellationToken.None);

        // Assert
        result.AccessToken.Should().Be("jwt-token");
        result.ExpiresIn.Should().Be(900);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ShouldThrowUnauthorized()
    {
        var user = User.Create("user@test.com", "hashed", "Test User");
        _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var handler = new LoginCommandHandler(
            _userRepo.Object, _hasher.Object,
            _tokenService.Object, _uow.Object, mapper: null!);

        var act = () => handler.Handle(
            new LoginCommand("user@test.com", "wrong"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
```

### 11.4 Integration Test — Auth Endpoint

```csharp
public class AuthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Register_WithValidData_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email = "newuser@test.com",
            password = "SecurePass1!",
            displayName = "New User"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Should().ContainKey("Set-Cookie");

        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        body!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "user@test.com",
            password = "WrongPassword"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/v1/users/me");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_Returns200()
    {
        var token = await GetValidTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/v1/users/me");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

---

## 12. Implementation Checklist

### Phase 1 — Setup & Foundation

1. Tạo Solution với 4 project: Domain, Application, Infrastructure, API
2. Cài packages: MediatR, FluentValidation, AutoMapper, EF Core, Npgsql, StackExchange.Redis, BCrypt.Net-Next
3. Tạo `BaseEntity`, `IDomainEvent` interface
4. Setup `AppDbContext`, cấu hình Postgres connection string
5. Tạo `ExceptionMiddleware` và custom exceptions (`NotFoundException`, `DomainException`, `UnauthorizedException`...)
6. Cấu hình JWT Authentication & Authorization trong `AddInfrastructure()`

### Phase 2 — Domain & Application

7. Implement Entities: `User`, `RefreshToken`, `Track`, `Album`, `Artist`, `Playlist`
8. Implement Value Objects: `ISRC`, `Duration`, `AudioQuality`
9. Implement Repository interfaces (contracts) trong Domain
10. Implement Auth Commands: `Register`, `Login`, `RefreshToken`, `Logout`, `ForgotPassword`, `ResetPassword`
11. Implement `ValidationBehavior` với FluentValidation validators
12. Implement `CachingBehavior` với Redis

### Phase 3 — Infrastructure & API

13. Implement `TokenService` (JWT generation + validation)
14. Implement `PasswordHasher` (BCrypt)
15. Implement `CurrentUserService`
16. Implement Repositories với EF Core + Entity Configurations
17. Implement `UnitOfWork` với Domain Event dispatch
18. Implement `AuthController` với cookie-based refresh token
19. Implement `StorageService` (S3 / Azure Blob)
20. Cấu hình Rate Limiting — strict cho auth endpoints
21. Cấu hình Swagger/Scalar với JWT bearer scheme

### Phase 4 — Testing & Deployment

22. Unit tests cho Domain entities (>90% coverage)
23. Unit tests cho Auth handlers (mock repositories)
24. Integration tests với Testcontainers (PostgreSQL)
25. API integration tests với `WebApplicationFactory`
26. Setup CI/CD pipeline (GitHub Actions / Azure DevOps)
27. Cấu hình health checks: `/health/live`, `/health/ready`
28. Background job dọn dẹp expired refresh tokens

---

> *Tài liệu này là living document — cập nhật theo tiến độ phát triển. Mọi thay đổi API cần cập nhật cả spec này và OpenAPI schema tương ứng.*

*— End of Document —*