using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Media;

namespace ECommerceBackend.Application.Medias.Queries.GetMediaByFileName;


public record GetMediaByFileNameQuery(string FileName) : IQuery<MediaDto>;
