
using Elastic.Clients.Elasticsearch;

namespace ElasticSearch;

public interface IElasticSearchService
{
    Task<bool> IndexOrUpdateAsync<T>(T document, IndexName indexName, CancellationToken cancellationToken = default) where T : class;
    Task<bool> IndexOrUpdateManyAsync<T>(IEnumerable<T> documents, IndexName indexName, CancellationToken cancellationToken = default) where T : class;
    Task<bool> DeleteAsync<T>(string id, IndexName indexName, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<T>> SearchAsync<T>(ElasticStringSearchParams<T> searchParam, IndexName indexName, string sortField, bool isDesc, CancellationToken cancellationToken = default) where T : class;
    Task<bool> CreateIndexIfNotExist(IndexName indexName);
}
