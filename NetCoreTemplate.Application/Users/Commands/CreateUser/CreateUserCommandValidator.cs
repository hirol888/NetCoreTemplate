using FluentValidation;

namespace NetCoreTemplate.Application.Users.Commands.CreateUser {
  public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand> {
    public CreateUserCommandValidator() {
      RuleFor(x => x.Id).NotEmpty();
      RuleFor(x => x.Password).NotEmpty();
    }
  }
}
