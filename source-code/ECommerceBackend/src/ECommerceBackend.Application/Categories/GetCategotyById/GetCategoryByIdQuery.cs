using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Categories;
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Categories.GetCategotyByID;
public sealed record GetCategoryByIdQuery(Guid CategoryId, bool IncludeChildren): IQuery<object>;
