﻿using Elastic.Clients.Elasticsearch;

namespace WebAPI.ElasticSearch;

public class ElasticServiceBase
    : IElasticSearchService
{
    private readonly ElasticsearchClientSettings _clientSettings;

    public ElasticsearchClientSettings ClientSettings => _clientSettings;
    protected ElasticsearchClient ElasticSearchClient { get; }

    protected ElasticServiceBase(ElasticsearchClientSettings clientSettings)
    {
        _clientSettings = clientSettings;

        ElasticSearchClient = new ElasticsearchClient(_clientSettings);
    }

    public async Task<bool> CreateIndexIfNotExist(IndexName indexName)
    {
        if ((await ElasticSearchClient.Indices.ExistsAsync(indexName)).IsValidResponse)
        {
            return false;
        }

        var res = await ElasticSearchClient.Indices.CreateAsync(indexName);

        return res.IsValidResponse;
    }

    public async Task<bool> IndexOrUpdateAsync<T>(T document, IndexName indexName,
        CancellationToken cancellationToken = default) where T : class
    {
        var response = await ElasticSearchClient.IndexAsync(
            document: document,
            idx => idx.Index(indexName)
                .OpType(OpType.Index),
            cancellationToken);

        return response.IsValidResponse;
    }

    public async Task<bool> IndexOrUpdateManyAsync<T>(IEnumerable<T> documents, IndexName indexName,
        CancellationToken cancellationToken = default) where T : class
    {
        var response = await ElasticSearchClient.BulkAsync(
            idx => idx.Index(indexName)
                .UpdateMany(documents, (e, u) => e.Doc(u).DocAsUpsert(true)),
            cancellationToken);

        return response.IsValidResponse;
    }

    public async Task<bool> DeleteAsync<T>(
        string id,
        IndexName indexName,
        CancellationToken cancellationToken = default) where T : class
    {
        var response = await ElasticSearchClient.DeleteAsync<T>(
            id: id,
            index: indexName,
            cancellationToken: cancellationToken);

        return response.IsValidResponse;
    }

    public async Task<IEnumerable<T>> SearchAsync<T>(ElasticStringSearchParams<T> searchParam, IndexName indexName,
        CancellationToken cancellationToken = default) where T : class
    {
        var response = await ElasticSearchClient.SearchAsync<T>(s => s
                .Index(indexName)
                .From(0)
                .Size(10)
                .Query(q => q
                    .Term(t => t.Field(searchParam.Selector).Value(searchParam.Value))),
            cancellationToken);

        if (response.IsValidResponse)
        {
            return response.Documents;
        }

        return [];
    }

    public Task<IEnumerable<T>> SearchAsync<T>(ElasticStringSearchParams<T> searchParam, IndexName indexName,
        string sortField, bool isDesc, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }
}