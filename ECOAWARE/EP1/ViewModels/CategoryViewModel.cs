using EP1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")] 

        public string? Name { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(250, ErrorMessage = "Description cannot be longer than 250 characters.")] 

        public string? Description { get; set; }

        public bool IsActive { get; set; }
        [DataType(DataType.Date)]

        public DateTime CreatedDate { get; set; }

        public int? ParentId { get; set; }  // Thêm thuộc tính ParentId để chọn danh mục cha

        public IEnumerable<SelectListItem>? ParentCategories { get; set; }  // Danh sách các danh mục cha cho dropdown
    }
}
