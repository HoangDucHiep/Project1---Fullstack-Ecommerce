using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Users.GetUserById;

namespace ECommerceBackend.Application.Categories.SearchCategory;
public sealed record SearchCategoryQuery(string QueryText) : IQuery<List<CategoryResponse>>;

