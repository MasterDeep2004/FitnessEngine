using Elastic.Clients.Elasticsearch;
using FitnessEngine.Api.Models;

namespace FitnessEngine.Api.Services;

public class ElasticsearchService
{
    private readonly ElasticsearchClient _client;

    private const string IndexName = "fitness-workouts";

    public ElasticsearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task IndexWorkoutAsync(Workout workout)
    {
        var response = await _client.IndexAsync(
            workout,
            index => index
                .Index(IndexName)
                .Id(workout.Id.ToString())
        );

        if (!response.IsValidResponse)
        {
            throw new Exception(
                $"Failed to index workout: {response.ElasticsearchServerError}"
            );
        }
    }

    public async Task<List<Workout>> SearchWorkoutsAsync(
        string? query,
        string? goal,
        string? fitnessLevel)
    {
        var response = await _client.SearchAsync<Workout>(s =>
            s.Index(IndexName)
             .Query(q =>
                 q.Bool(b =>
                 {
                     var must = new List<Action<QueryDescriptor<Workout>>>();

                     if (!string.IsNullOrWhiteSpace(query))
                     {
                         must.Add(m =>
                             m.MultiMatch(mm => mm
                                 .Fields(
                                     "name",
                                     "description",
                                     "goal",
                                     "fitnessLevel",
                                     "type"
                                 )
                                 .Query(query)
                             )
                         );
                     }

                     if (!string.IsNullOrWhiteSpace(goal))
                     {
                         must.Add(m =>
                             m.Match(mt => mt
                                 .Field("goal")
                                 .Query(goal)
                             )
                         );
                     }

                     if (!string.IsNullOrWhiteSpace(fitnessLevel))
                     {
                         must.Add(m =>
                             m.Match(mt => mt
                                 .Field("fitnessLevel")
                                 .Query(fitnessLevel)
                             )
                         );
                     }

                     b.Must(must.ToArray());
                 })
             )
        );

        if (!response.IsValidResponse)
        {
            return new List<Workout>();
        }

        return response.Documents.ToList();
    }
}
