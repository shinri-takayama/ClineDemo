using System.ComponentModel.DataAnnotations;

namespace ECShop.API.DTOs
{
    // お知らせ作成リクエスト
    public class CreateAnnouncementRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }              // タイトル
        
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }            // 内容
        
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }           // カテゴリ
        
        public bool IsPublished { get; set; }          // 公開フラグ
        
        [Required]
        public DateTime PublishDate { get; set; }      // 公開日
    }

    // お知らせ更新リクエスト
    public class UpdateAnnouncementRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }              // タイトル
        
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }            // 内容
        
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }           // カテゴリ
        
        public bool IsPublished { get; set; }          // 公開フラグ
        
        [Required]
        public DateTime PublishDate { get; set; }      // 公開日
    }

    // お知らせレスポンス
    public class AnnouncementResponse
    {
        public int Id { get; set; }                    // お知らせID
        public string Title { get; set; }              // タイトル
        public string Content { get; set; }            // 内容
        public string Category { get; set; }           // カテゴリ
        public bool IsPublished { get; set; }          // 公開フラグ
        public DateTime PublishDate { get; set; }      // 公開日
        public DateTime CreatedAt { get; set; }        // 作成日時
        public DateTime UpdatedAt { get; set; }        // 更新日時
    }

    // お知らせサマリーレスポンス（一覧用）
    public class AnnouncementSummaryResponse
    {
        public int Id { get; set; }                    // お知らせID
        public string Title { get; set; }              // タイトル
        public string Category { get; set; }           // カテゴリ
        public bool IsPublished { get; set; }          // 公開フラグ
        public DateTime PublishDate { get; set; }      // 公開日
        public DateTime CreatedAt { get; set; }        // 作成日時
    }
}
