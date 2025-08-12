using System.ComponentModel.DataAnnotations;

namespace ECShop.API.Models
{
    public class Announcement
    {
        public int Id { get; set; }                    // お知らせID（主キー）
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }              // タイトル（必須、最大200文字）
        
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }            // 内容（必須、最大2000文字）
        
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }           // カテゴリ（必須、最大50文字）
        
        public bool IsPublished { get; set; }          // 公開フラグ（デフォルト: false）
        
        [Required]
        public DateTime PublishDate { get; set; }      // 公開日（必須）
        
        public DateTime CreatedAt { get; set; }        // 作成日時
        public DateTime UpdatedAt { get; set; }        // 更新日時
    }
}
