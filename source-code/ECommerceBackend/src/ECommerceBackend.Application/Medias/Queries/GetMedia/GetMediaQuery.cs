using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Media;

namespace ECommerceBackend.Application.Medias.Queries.GetMedia;
public record GetMediaQuery(Guid MediaId) : IQuery<MediaDto>;
