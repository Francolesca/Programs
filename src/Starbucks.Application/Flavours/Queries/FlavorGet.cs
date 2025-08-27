using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Core.mediatOR.Contracts;
using Starbucks.Application.Abstractions;
using Starbucks.Application.Flavours.DTOs;
using Starbucks.Domain.Abstractions;
using System.Reflection.Metadata;

namespace Starbucks.Application.Flavours.Queries
{
    public class FlavorGet
    {
        public class Query : IRequest<Result<List<FlavorDto>>>
        {
        }
        public class Handler(
                TableServiceClient tableServiceClient,
                BlobServiceClient flavorBlobServiceClient
            ) : IRequestHandler<Query, Result<List<FlavorDto>>>
        {
            private readonly TableServiceClient _tableServiceClient = tableServiceClient;
            private readonly BlobServiceClient _flavorBlobServiceClient = flavorBlobServiceClient;
            public Task<Result<List<FlavorDto>>> Handle(
                Query request, 
                CancellationToken cancellationToken
                )
            {
                var flavorsCategoriesTableClient = _tableServiceClient
                                                        .GetTableClient("CoffeCategorie");
                //var filter = TableClient.CreateQueryFilter($"Id eq 1");
                var filter = "";
                var quantity = 100;

                var flavors = flavorsCategoriesTableClient.Query<TableEntity>(
                    filter,
                    quantity,
                    ["Id", "Name", "Description"],
                    cancellationToken
                    );

                var res = flavors.AsPages()
                                    .SelectMany(page => page.Values)
                                    .Select(entity => new FlavorDto
                                    {
                                        Id = entity.GetInt64("Id") ?? -1,
                                        Name = entity.GetString("Name"),
                                        Description = entity.GetString("Description")
                                    })
                                    .ToList();
                if (res.Count == 0) {
                    return Task.FromResult(Result<List<FlavorDto>>
                                        .Failure(
                                            new Error("NO_ELEMENTS", "No category flavors")
                                            )
                                        );
                }

                var flavorImageContainerClient = _flavorBlobServiceClient
                                                    .GetBlobContainerClient("flavor-images");
                foreach (var flavor in res)
                {
                    var blobs = flavorImageContainerClient.FindBlobsByTags(
                        $"flavorId='{flavor.Id}'"
                        );
                    foreach (var blob in blobs) {
                        var blobClient = flavorImageContainerClient
                                            .GetBlobClient(blob.BlobName);
                        flavor.Images.Add(blobClient.Uri.AbsoluteUri);
                    }
                }

                return Task.FromResult(Result<List<FlavorDto>>.Success(res));
            }
        }
    }
}
