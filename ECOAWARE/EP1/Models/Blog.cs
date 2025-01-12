namespace EP1.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; } // Tiêu đề bài viết
        public string Description { get; set; } // Chi tiết bài viết
        public string Image { get; set; } // Đường dẫn hình ảnh
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày tạo
    }
}
