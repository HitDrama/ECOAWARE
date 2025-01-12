using System;
using System.ComponentModel.DataAnnotations;

public class CompareDateAttribute : ValidationAttribute
{
	private readonly string _comparisonProperty;

	public CompareDateAttribute(string comparisonProperty)
	{
		_comparisonProperty = comparisonProperty;
	}

	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		var currentValue = (DateTime?)value;

		// Lấy giá trị của thuộc tính cần so sánh
		var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

		if (property == null)
		{
			return new ValidationResult($"Property {_comparisonProperty} not found.");
		}

		var comparisonValue = (DateTime?)property.GetValue(validationContext.ObjectInstance);

		// Kiểm tra logic (EndDate > StartDate)
		if (currentValue.HasValue && comparisonValue.HasValue && currentValue <= comparisonValue)
		{
			return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be later than {_comparisonProperty}.");
		}

		return ValidationResult.Success;
	}
}
