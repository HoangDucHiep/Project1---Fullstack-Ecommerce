using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceBackend.Application.Abstracts.Messaging;


namespace ECommerceBackend.Application.Categories.DeleteCategory;
public sealed record DeleteCategoryCommand(
    Guid CategoryId,
    Guid? ReplacementCategoryId,
    bool Cascade
) : ICommand;
