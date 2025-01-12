using System;
using System.ComponentModel.DataAnnotations;

public class RequiredIfAttribute : ValidationAttribute
{
	private readonly string _propertyName;
	private readonly object _expectedValue;

	public RequiredIfAttribute(string propertyName, object expectedValue)
	{
		_propertyName = propertyName;
		_expectedValue = expectedValue;
	}

	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		// Lấy giá trị của thuộc tính cần kiểm tra
		var property = validationContext.ObjectType.GetProperty(_propertyName);
		if (property == null)
		{
			return new ValidationResult($"Property {_propertyName} not found.");
		}

		var propertyValue = property.GetValue(validationContext.ObjectInstance);

		// Nếu giá trị của thuộc tính khớp với expectedValue, yêu cầu giá trị
		if (propertyValue != null && propertyValue.Equals(_expectedValue))
		{
			if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
			{
				return new ValidationResult(ErrorMessage);
			}
		}

		return ValidationResult.Success;
	}
}
