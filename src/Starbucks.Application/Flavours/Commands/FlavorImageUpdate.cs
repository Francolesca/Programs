using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Core.mediatOR.Contracts;
using Starbucks.Application.Abstractions;
using Starbucks.Application.Flavours.DTOs;

namespace Starbucks.Application.Flavours.Commands
{
    public class FlavorImageUpdate
    {
        public class Command(int flavorId, FlavorUpdateRequest flavorUpdateRequest)
            : IRequest<Result<int>>
        {
            public FlavorUpdateRequest FlavorUpdateRequest { get;} = flavorUpdateRequest;
            public int FlavorId { get;} = flavorId;
        }

        public class Handler(
            BlobServiceClient flavorBlobServiceClient
            ) : IRequestHandler<Command, Result<int>>
        {
            private readonly BlobServiceClient _flavorBlobServiceClient = flavorBlobServiceClient;
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                var flavorImagesContainerClient = _flavorBlobServiceClient
                                                    .GetBlobContainerClient("flavor-images");
                var blobClient = flavorImagesContainerClient.GetBlobClient(
                    request.FlavorUpdateRequest.Name
                    );
                if (!blobClient.Exists(cancellationToken).Value)
                {
                    await blobClient.UploadAsync(
                        request.FlavorUpdateRequest.Stream,
                        new BlobUploadOptions
                        {
                            Tags = new Dictionary<string, string>
                            { { "FlavorId", request.FlavorId.ToString() } }
                        },
                        cancellationToken
                        );
                } else
                {
                    var tags = blobClient.GetTags(cancellationToken: cancellationToken);
                    if(tags.Value.Tags.TryGetValue("FlavorId", out var flavorId) && flavorId == request.FlavorId.ToString())
                    {
                        await blobClient.UploadAsync(
                                request.FlavorUpdateRequest.Stream,
                                new BlobUploadOptions
                                {
                                    Tags = new Dictionary<string, string>
                                    { { "FlavorId", request.FlavorId.ToString()} }
                                }
                            );
                    }
                }

                return Result<int>.Success(request.FlavorId);
            }
        }
    }
}
