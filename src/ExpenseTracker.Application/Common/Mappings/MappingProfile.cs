using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<Category, CategoryDto>();

        CreateMap<Expense, ExpenseDto>()
            .ForMember(d => d.CategoryName,
                opt => opt.MapFrom(s => s.Category.Name));

        CreateMap<Receipt, ReceiptDto>();

        CreateMap<User,UserProfileDto >();
    }
}
