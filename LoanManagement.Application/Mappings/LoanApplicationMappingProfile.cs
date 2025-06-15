using AutoMapper;
using LoanManagement.Application.DTOs.Loan;
using LoanManagement.Domain.Entities;

namespace LoanManagement.Application.Mappings;

public class LoanApplicationMappingProfile : Profile
{
    public LoanApplicationMappingProfile()
    {
        CreateMap<LoanApplication, LoanApplicationDto>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.GetFullName()))
            .ForMember(dest => dest.ApproverFullName, opt => opt.MapFrom(src => src.Approver != null ? src.Approver.GetFullName() : null))
            .ForMember(dest => dest.CanEdit, opt => opt.MapFrom(src => src.CanBeEdited()))
            .ForMember(dest => dest.CanSubmit, opt => opt.MapFrom(src => src.CanBeSubmitted()))
            .ForMember(dest => dest.CanApprove, opt => opt.MapFrom(src => src.CanBeApproved()))
            .ForMember(dest => dest.CanReject, opt => opt.MapFrom(src => src.CanBeRejected()))
            .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => src.CanBeEdited()));
    }
}