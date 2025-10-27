using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Media;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Medias;

namespace ECommerceBackend.Application.Medias.Queries.GetMedia;

public class GetMediaQueryHandler : IQueryHandler<GetMediaQuery, MediaDto>
{
    private readonly IMediaRepository _mediaRepository;

    public GetMediaQueryHandler(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    public async Task<Result<MediaDto>> Handle(GetMediaQuery request, CancellationToken cancellationToken)
    {
        Domain.Medias.Media? media = await _mediaRepository.GetByIdAsync(request.MediaId, cancellationToken);

        if (media == null)
        {
            return Result.Failure<MediaDto>(MediaErrors.NotFound);
        }

        return MediaDto.FromEntity(media);
    }
}
