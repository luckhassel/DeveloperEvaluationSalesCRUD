namespace Ambev.DeveloperEvaluation.Application.Dtos;

public class CustomerInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}