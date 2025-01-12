using Newtonsoft.Json;

namespace EP1.Extensions
{
	public static class SessionExtensions
	{
		// Phương thức mở rộng để lưu đối tượng dưới dạng JSON vào Session
		public static void SetObjectAsJson(this ISession session, string key, object value)
		{
			var json = JsonConvert.SerializeObject(value); // Chuyển đối tượng thành chuỗi JSON
			session.SetString(key, json); // Lưu chuỗi JSON vào Session
		}

		// Phương thức mở rộng để lấy đối tượng từ JSON trong Session
		public static T? GetObjectFromJson<T>(this ISession session, string key)
		{
			var value = session.GetString(key); // Lấy chuỗi JSON từ Session
			return value == null ? default : JsonConvert.DeserializeObject<T>(value); // Chuyển chuỗi JSON thành đối tượng
		}
	}
}
