using MusicApp.Domain.Common;
using MusicApp.Domain.Enums;
using MusicApp.Domain.Exceptions;

namespace MusicApp.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public string? AvatarUrl { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsEmailConfirmed { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private readonly List<Playlist> _playlists = new();
    public IReadOnlyList<Playlist> Playlists => _playlists.AsReadOnly();

    private readonly List<Track> _likedTracks = new();
    public IReadOnlyList<Track> LikedTracks => _likedTracks.AsReadOnly();

    private readonly List<User> _following = new();
    public IReadOnlyList<User> Following => _following.AsReadOnly();

    private readonly List<User> _followers = new();
    public IReadOnlyList<User> Followers => _followers.AsReadOnly();

    private readonly List<Playlist> _followedPlaylists = new();
    public IReadOnlyList<Playlist> FollowedPlaylists => _followedPlaylists.AsReadOnly();

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

    public void UpdateProfile(string? displayName, string? avatarUrl)
    {
        if (displayName is not null) DisplayName = displayName;
        if (avatarUrl is not null) AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
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

    public void LikeTrack(Track track)
    {
        if (!_likedTracks.Contains(track))
            _likedTracks.Add(track);
    }

    public void UnlikeTrack(Track track) => _likedTracks.Remove(track);

    public void Follow(User user)
    {
        if (!_following.Contains(user))
            _following.Add(user);
    }

    public void Unfollow(User user) => _following.Remove(user);
}
