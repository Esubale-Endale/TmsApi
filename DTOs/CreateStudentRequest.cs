public class CreateStudentRequest
{
    public string RegistrationNumber { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal GPA { get; set; }

    public bool IsActive { get; set; }
}