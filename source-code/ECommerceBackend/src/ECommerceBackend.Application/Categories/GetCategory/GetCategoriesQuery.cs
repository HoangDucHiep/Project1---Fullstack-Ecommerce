
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Application.Categories.GetCategories;
public sealed record GetCategoriesQuery : IQuery<List<CategoriesDTO>>;
