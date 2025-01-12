using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EP1.Models
{
	public class ShopContext : IdentityDbContext<Account, IdentityRole, string>
	{
		public ShopContext(DbContextOptions<ShopContext> options) : base(options)
		{
		}

		public DbSet<UserQuestion> UserQuestions { get; set; }
		public DbSet<FAQ> FAQs { get; set; }

		public DbSet<Survey> Surveys { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<Choice> Choices { get; set; }
		public DbSet<Answer> Answers { get; set; }
		public DbSet<SurveySubmission> SurveySubmissions { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder); // Gọi phương thức từ IdentityDbContext

			// Survey
			modelBuilder.Entity<Survey>(entity =>
			{
				entity.HasKey(s => s.SurveyId); // Thiết lập khóa chính
				entity.Property(s => s.Title).IsRequired().HasMaxLength(255); // Tiêu đề bắt buộc
			});

			// Question
			modelBuilder.Entity<Question>(entity =>
			{
				entity.HasKey(q => q.QuestionId);
				entity.Property(q => q.Content).IsRequired(); // Nội dung câu hỏi bắt buộc
				entity.HasOne(q => q.Survey) // Ràng buộc quan hệ với Survey
					  .WithMany(s => s.Questions)
					  .HasForeignKey(q => q.SurveyId)
					  .OnDelete(DeleteBehavior.Cascade); // Xóa Survey sẽ xóa tất cả câu hỏi liên quan
			});

			// Choice
			modelBuilder.Entity<Choice>(entity =>
			{
				entity.HasKey(c => c.ChoiceId);
				entity.Property(c => c.Content).IsRequired(); // Nội dung của lựa chọn bắt buộc
				entity.HasOne(c => c.Question) // Ràng buộc quan hệ với Question
					  .WithMany(q => q.Choices)
					  .HasForeignKey(c => c.QuestionId)
					  .OnDelete(DeleteBehavior.Cascade); // Cascade cho Choices
			});

			// Answer
			modelBuilder.Entity<Answer>(entity =>
			{
				entity.HasKey(a => a.AnswerId);
				entity.Property(a => a.AnswerText).IsRequired(); // Câu trả lời bắt buộc

				entity.HasOne(a => a.Question) // Ràng buộc quan hệ với Question
					  .WithMany()
					  .HasForeignKey(a => a.QuestionId)
					  .OnDelete(DeleteBehavior.Cascade); // Cascade cho Question

				entity.HasOne(a => a.SurveySubmission) // Ràng buộc với SurveySubmission
					  .WithMany(s => s.Answers)
					  .HasForeignKey(a => a.SurveySubmissionId)
					  .OnDelete(DeleteBehavior.Restrict); // Tránh multiple cascade paths
			});

			// SurveySubmission
			modelBuilder.Entity<SurveySubmission>(entity =>
			{
				entity.HasKey(s => s.SurveySubmissionId);
				entity.Property(s => s.UserName).IsRequired(); // Người nộp bài bắt buộc
				entity.HasOne(s => s.Survey) // Ràng buộc quan hệ với Survey
					  .WithMany(s => s.SurveySubmissions)
					  .HasForeignKey(s => s.SurveyId)
					  .OnDelete(DeleteBehavior.Cascade); // Cascade cho Survey
			});

			// UserQuestion
			modelBuilder.Entity<UserQuestion>(entity =>
			{
				entity.HasKey(uq => uq.Id); // Thiết lập khóa chính
				entity.Property(uq => uq.Question).IsRequired().HasMaxLength(1000); // Nội dung bắt buộc, tối đa 1000 ký tự
				entity.Property(uq => uq.UserId).IsRequired(); // Người dùng bắt buộc
				entity.HasOne(uq => uq.FAQ) // Ràng buộc quan hệ với FAQ
					  .WithMany()
					  .HasForeignKey(uq => uq.FAQId)
					  .OnDelete(DeleteBehavior.Cascade); // Cascade khi FAQ bị xóa
			});

			// FAQ
			modelBuilder.Entity<FAQ>(entity =>
			{
				entity.HasKey(f => f.Id); // Thiết lập khóa chính
				entity.Property(f => f.Title).IsRequired().HasMaxLength(200); // Tiêu đề bắt buộc, tối đa 200 ký tự
				entity.Property(f => f.Description).IsRequired(); // Mô tả bắt buộc
			});


			// Seed vai trò Admin
			modelBuilder.Entity<IdentityRole>().HasData(
				new IdentityRole
				{
					Id = "1", // Id tĩnh để tránh trùng lặp
					Name = "Admin",
					NormalizedName = "ADMIN"
				}
			);
		}
		public static async Task SeedAdminUser(IServiceProvider serviceProvider)
		{
			var userManager = serviceProvider.GetRequiredService<UserManager<Account>>();
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			// Đảm bảo vai trò Admin tồn tại
			if (!await roleManager.RoleExistsAsync("Admin"))
			{
				await roleManager.CreateAsync(new IdentityRole("Admin"));
			}

			// Kiểm tra xem tài khoản Admin đã tồn tại chưa
			var adminUser = await userManager.FindByEmailAsync("Admin@gmail.com");
			if (adminUser == null)
			{
				// Tạo tài khoản Admin
				var newAdmin = new Account
				{
					UserName = "Admin",
					Email = "Admin@gmail.com",
					FullName = "Administrator",
					Class = "Management",
					Specification = "System",
					Section = "Administration",
					IsActive = true,
					
					EmailConfirmed = true // Xác nhận email để bỏ qua bước xác thực
				};

				// Thêm tài khoản với mật khẩu
				var result = await userManager.CreateAsync(newAdmin, "Admin123456");
				if (result.Succeeded)
				{
					// Gán vai trò Admin
					await userManager.AddToRoleAsync(newAdmin, "Admin");
				}
			}




		}
	}
}	

