using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Categories;
using ECommerceBackend.Application.Users.GetUserById;

namespace ECommerceBackend.Application.Categories.SearchCategory;

/// PBNMinh- 08/09/2025
public sealed record SearchCategoryQuery(string QueryText) : IQuery<List<CategoryDto>>;

