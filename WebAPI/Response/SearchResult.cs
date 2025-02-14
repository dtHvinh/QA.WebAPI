namespace WebAPI.Response;

public record SearchResult<TDocument>(List<TDocument> Documents, long Total);
