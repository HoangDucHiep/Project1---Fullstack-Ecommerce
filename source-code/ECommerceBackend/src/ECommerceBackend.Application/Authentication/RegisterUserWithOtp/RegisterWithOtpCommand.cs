using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Authentication.Register;
public record RegisterWithOtpCommand(string PhoneNumber, string Password) : ICommand;
