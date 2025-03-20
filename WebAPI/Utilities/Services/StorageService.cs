using Supabase.Storage;
using Supabase.Storage.Interfaces;
using WebAPI.Storage;

namespace WebAPI.Utilities.Services;

public class StorageService(string url, string key) : FileStorage(url, key)
{
    private IStorageFileApi<FileObject> CommunityBucket => Client.Storage.From("qa");

    public async Task<string> UploadCommunityIcon(string communityName, IFormFile file, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);

        var extension = Path.GetExtension(file.FileName);

        var supabasePath = await CommunityBucket.Upload(
            stream.ToArray(),
            "icons/" + communityName + extension);

        return supabasePath;
    }

    public async Task<string> UploadUserPfp(int userId, IFormFile file, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);

        var extension = Path.GetExtension(file.FileName);

        var supabasePath = await CommunityBucket.Upload(
            stream.ToArray(),
            "pfp/" + userId + extension);

        return supabasePath;
    }
}
