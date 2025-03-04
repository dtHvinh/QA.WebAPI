using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using WebAPI.ElasticSearch;
using WebAPI.Model;
using WebAPI.Response;

namespace WebAPI.Utilities.Services;

public sealed class QuestionSearchService(ElasticsearchClientSettings clientSettings)
    : ElasticServiceBase(clientSettings)

{
    public const string QuestionIndexName = "user_questions_2";

    public async Task<bool> IndexOrUpdateAsync(Question question, CancellationToken cancellationToken)
    {
        var response = await ElasticSearchClient.IndexAsync(
            document: question,
            idx => idx.Index(QuestionIndexName)
                .Id(question.Id)
                .OpType(OpType.Index),
            cancellationToken);

        return response.IsValidResponse;
    }

    public async Task<bool> IndexOrUpdateManyAsync(List<Question> questions, CancellationToken cancellationToken)
    {
        var response = await ElasticSearchClient.BulkAsync(
            idx => idx.Index(QuestionIndexName)
                .UpdateMany(questions, (e, u) => e.Doc(u).Id(u.Id).DocAsUpsert(true)),
            cancellationToken);

        return response.IsValidResponse;
    }

    public async Task<SearchResult<Question>> SearchSimilarQuestionAsync(int questionId, int skip, int take,
        CancellationToken cancellationToken)
    {
        var response = await ElasticSearchClient.SearchAsync<Question>(s => s
            .Index(QuestionIndexName)
            .From(skip)
            .Size(take)
            .Query(q => q
                .MoreLikeThis(mlt => mlt
                    .Fields(Field.FromExpression((Question q) => q.Title)!.And((Question q) => q.Content))
                    .Like([
                        new Like(new LikeDocument()
                        {
                            Id = questionId,
                            Index = QuestionIndexName,
                        })
                    ])
                )
            ), cancellationToken);

        return response.IsValidResponse
            ? new SearchResult<Question>([.. response.Documents], -1)
            : new SearchResult<Question>([], -1);
    }

    public async Task<SearchResult<Question>> SearchQuestionYouMayLikeAsync(int skip, int take,
        CancellationToken cancellationToken)
    {
        var response = await ElasticSearchClient.SearchAsync<Question>(s => s
                .Index(QuestionIndexName)
                .From(skip)
                .Size(take)
                .Query(q => q
                    .FunctionScore(fs => { fs.Functions(f => f.RandomScore(new RandomScoreFunction())); })),
            cancellationToken);

        return response.IsValidResponse
            ? new SearchResult<Question>([.. response.Documents], -1)
            : new SearchResult<Question>([], -1);
    }

    public async Task<SearchResult<Question>> SearchQuestionAsync(
        string keyword, int tagId, int skip, int take, CancellationToken cancellationToken)
    {
        var response = await ElasticSearchClient.SearchAsync<Question>(s => s
                .Index(QuestionIndexName)
                .TrackTotalHits(new TrackHits(true))
                .From(skip)
                .Size(take)
                .Query(q => q
                    .Bool(b =>
                        b.Must(m => m.Term(t => t.Field(f => f.IsDeleted).Value(false)),
                                m => m.MultiMatch(new MultiMatchQuery()
                                {
                                    Fields = Field.FromExpression((Question fq) => fq.Title)!
                                        .And((Question aq) => aq.Content),
                                    Query = keyword,
                                    Analyzer = "standard",
                                    Boost = 1.5F,
                                    Slop = 2,
                                    Fuzziness = new Fuzziness("auto"),
                                    PrefixLength = 2,
                                    MaxExpansions = 2,
                                    Operator = Operator.Or,
                                    MinimumShouldMatch = 2,
                                    Lenient = true,
                                    ZeroTermsQuery = ZeroTermsQuery.All,
                                    QueryName = "search_question",
                                    AutoGenerateSynonymsPhraseQuery = false
                                }))
                            .Filter(f => f.Nested(n => n
                                .Path(p => p.Tags)
                                .Query(nq => nq
                                    .Term(t => t.Field(tf => tf.Tags.First().Id).Value(tagId))
                                )
                                .IgnoreUnmapped()
                            )))),
            cancellationToken);

        return response.IsValidResponse
            ? new SearchResult<Question>([.. response.Documents], response.Total)
            : new SearchResult<Question>([], -1);
    }

    public async Task<SearchResult<Question>> SearchQuestionNoTagAsync(
        string keyword, int skip, int take, CancellationToken cancellationToken)
    {
        var response = await ElasticSearchClient.SearchAsync<Question>(s => s
                .Index(QuestionIndexName)
                .TrackTotalHits(new TrackHits(true))
                .From(skip)
                .Size(take)
                .Query(q => q
                    .Bool(b =>
                        b.Must(
                            m => m.Term(t => t.Field(f => f.IsDeleted).Value(false)),
                            m => m.MultiMatch(new MultiMatchQuery()
                            {
                                Fields = Field.FromExpression((Question fq) => fq.Title)!
                                    .And((Question aq) => aq.Content),
                                Query = keyword,
                                Analyzer = "standard",
                                Boost = 1.5F,
                                Slop = 2,
                                Fuzziness = new Fuzziness("auto"),
                                PrefixLength = 2,
                                MaxExpansions = 2,
                                Operator = Operator.Or,
                                MinimumShouldMatch = 2,
                                Lenient = true,
                                ZeroTermsQuery = ZeroTermsQuery.All,
                                QueryName = "search_question",
                                AutoGenerateSynonymsPhraseQuery = false
                            })))),
            cancellationToken);

        return response.IsValidResponse
            ? new SearchResult<Question>([.. response.Documents], response.Total)
            : new SearchResult<Question>([], -1);
    }
}