using Supabase.Storage;
using Supabase.Storage.Interfaces;
using WebAPI.Storage;

namespace WebAPI.Utilities.Services;

public class StorageService(string url, string key) : FileStorage(url, key)
{
    private IStorageFileApi<FileObject> CommunityBucket => Client.Storage.From("qa");
    private readonly Supabase.Storage.FileOptions _options = new() { Upsert = true };

    public async Task<string> UploadCommunityIcon(string communityName, IFormFile file, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);

        var extension = Path.GetExtension(file.FileName);

        var supabasePath = await CommunityBucket.Upload(
            stream.ToArray(),
            "icons/" + communityName + "-" + Random.Shared.NextInt64(1, 500) + extension,
            _options);

        return supabasePath;
    }

    public async Task<string> UploadUserPfp(int userId, IFormFile file, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);

        var extension = Path.GetExtension(file.FileName);

        var supabasePath = await CommunityBucket.Upload(
            stream.ToArray(),
            "pfps/" + userId + extension,
            _options);

        return supabasePath;
    }

    public async Task Delete(string path)
    {
        await CommunityBucket.Remove(path);
    }
}
