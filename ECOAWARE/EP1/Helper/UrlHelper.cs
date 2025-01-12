

using System.Text.RegularExpressions;
using System.Text;

namespace EP1.Helper
{
	public static class UrlHelper
	{
		//saoweb(KhOA học--asp) => khoa-hoc-asp
		public static string seoweb(string input)
		{

			if (string.IsNullOrEmpty(input)) return string.Empty;

			// Bước 1: Chuẩn hóa chuỗi, chuyển về chữ thường
			string normalizedString = input.Normalize(NormalizationForm.FormD);

			// Bước 2: Loại bỏ dấu tiếng Việt
			var stringBuilder = new StringBuilder();
			foreach (var c in normalizedString)
			{
				var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}
			string noDiacritics = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

			// Bước 3: Thay thế khoảng trắng và ký tự đặc biệt bằng dấu '-'
			string slug = Regex.Replace(noDiacritics, @"[^a-zA-Z0-9\s-]", "").Trim(); // Xóa ký tự không hợp lệ
			slug = Regex.Replace(slug, @"\s+", "-"); // Thay khoảng trắng bằng dấu '-'
			slug = Regex.Replace(slug, @"-+", "-"); // Xóa dấu '-' thừa

			return slug.ToLower();
		}
	}
}
