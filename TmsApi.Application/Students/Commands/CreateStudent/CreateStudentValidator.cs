using FluentValidation;

namespace TmsApi.Application.Students.Commands.CreateStudent;

public class CreateStudentValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.RegistrationNumber)
            .NotEmpty();
        RuleFor(x => x.GPA)
            .InclusiveBetween(0, 4);
    }
}