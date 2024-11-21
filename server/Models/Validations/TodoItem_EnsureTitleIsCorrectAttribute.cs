using System.ComponentModel.DataAnnotations;

namespace ToDoEasyApp.Models.Validations
{
    public class TodoItem_EnsureTitleIsCorrectAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var todoItem = validationContext.ObjectInstance as TodoItem;

            if (todoItem != null && !string.IsNullOrWhiteSpace(todoItem.Title))
            {
                if (todoItem.Title.Length > 20 )
                {
                    return new ValidationResult("The title has to be less then 20 symbols");
                }
            }
            return ValidationResult.Success;
        }
    }
}
