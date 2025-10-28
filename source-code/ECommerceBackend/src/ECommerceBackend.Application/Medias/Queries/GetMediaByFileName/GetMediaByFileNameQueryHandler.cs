using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Media;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Medias;

namespace ECommerceBackend.Application.Medias.Queries.GetMediaByFileName;



public class GetMediaByFileNameQueryHandler : IQueryHandler<GetMediaByFileNameQuery, MediaDto>
{
    private readonly IMediaRepository _mediaRepository;

    public GetMediaByFileNameQueryHandler(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    public async Task<Result<MediaDto>> Handle(GetMediaByFileNameQuery request, CancellationToken cancellationToken)
    {
        Domain.Medias.Media? media = await _mediaRepository.GetByFileNameAsync(request.FileName, cancellationToken);

        if (media == null)
        {
            return Result.Failure<MediaDto>(MediaErrors.NotFound);
        }

        return MediaDto.FromEntity(media);
    }
}
