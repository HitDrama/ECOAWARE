using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
    public class ProductViewModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
        public string? Name { get; set; }
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [MaxLength(5000, ErrorMessage = "Mô tả không được vượt quá 5000 ký tự.")]
        public string? Description { get; set; }

        // Chọn CategoryId cho sản phẩm
        [Required(ErrorMessage = "Danh mục sản phẩm không được để trống.")]
        public int CategoryId { get; set; }
        public DateTime CreatedDate { get; set; }

        // Đường dẫn ảnh của sản phẩm
        [Display(Name = "Ảnh sản phẩm")]
        public IFormFile? Image { get; set; }

        public string? ImagePath { get; set; }  // Lưu đường dẫn ảnh
    }
}
