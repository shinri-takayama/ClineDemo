using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ECShop.API.Data;
using ECShop.API.Models;
using ECShop.API.DTOs;

namespace ECShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly ECShopContext _context;
        private readonly ILogger<AnnouncementsController> _logger;

        public AnnouncementsController(ECShopContext context, ILogger<AnnouncementsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// お知らせ一覧取得（クエリパラメータ対応）
        /// GET /api/announcements?category=重要&keyword=メンテナンス&published=true
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnouncementSummaryResponse>>> GetAnnouncements(
            [FromQuery] string? category = null,
            [FromQuery] string? keyword = null,
            [FromQuery] bool? published = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                _logger.LogInformation("お知らせ一覧取得開始 - Category: {Category}, Keyword: {Keyword}, Published: {Published}", 
                    category, keyword, published);

                // 1. 基本クエリを作成（ECShopContextからAnnouncementsテーブルを取得）
                var query = _context.Announcements.AsQueryable();

                // 2. カテゴリフィルタ適用
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(a => a.Category.ToLower() == category.ToLower());
                }

                // 3. キーワード検索適用（タイトルまたは内容に部分一致）
                if (!string.IsNullOrEmpty(keyword))
                {
                    var lowerKeyword = keyword.ToLower();
                    query = query.Where(a => a.Title.ToLower().Contains(lowerKeyword) || 
                                           a.Content.ToLower().Contains(lowerKeyword));
                }

                // 4. 公開状態フィルタ適用
                if (published.HasValue)
                {
                    query = query.Where(a => a.IsPublished == published.Value);
                }

                // 5. 日付範囲フィルタ適用
                if (fromDate.HasValue)
                {
                    query = query.Where(a => a.PublishDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(a => a.PublishDate <= toDate.Value);
                }

                // 6. 公開日降順でソート
                query = query.OrderByDescending(a => a.PublishDate);

                // 7. データベースから取得
                var announcements = await query.ToListAsync();

                // 8. AnnouncementSummaryResponse形式に変換
                var response = announcements.Select(a => new AnnouncementSummaryResponse
                {
                    Id = a.Id,
                    Title = a.Title,
                    Category = a.Category,
                    IsPublished = a.IsPublished,
                    PublishDate = a.PublishDate,
                    CreatedAt = a.CreatedAt
                }).ToList();

                _logger.LogInformation("お知らせ一覧取得完了 - 件数: {Count}", response.Count);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "お知らせ一覧取得中にエラーが発生しました");
                return StatusCode(500, "お知らせの取得に失敗しました");
            }
        }

        /// <summary>
        /// お知らせ詳細取得
        /// GET /api/announcements/1
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AnnouncementResponse>> GetAnnouncement(int id)
        {
            try
            {
                _logger.LogInformation("お知らせ詳細取得開始 - ID: {Id}", id);

                // 1. URLパラメータからIDを取得（既に引数で取得済み）
                if (id <= 0)
                {
                    return BadRequest("無効なお知らせIDです");
                }

                // 2. ECShopContextを使用してAnnouncementsテーブルから指定IDのお知らせを検索
                var announcement = await _context.Announcements
                    .FirstOrDefaultAsync(a => a.Id == id);

                // 3. 存在チェック
                if (announcement == null)
                {
                    _logger.LogWarning("お知らせが見つかりません - ID: {Id}", id);
                    return NotFound("お知らせが見つかりません");
                }

                // 4. AnnouncementResponse形式に変換
                var response = new AnnouncementResponse
                {
                    Id = announcement.Id,
                    Title = announcement.Title,
                    Content = announcement.Content,
                    Category = announcement.Category,
                    IsPublished = announcement.IsPublished,
                    PublishDate = announcement.PublishDate,
                    CreatedAt = announcement.CreatedAt,
                    UpdatedAt = announcement.UpdatedAt
                };

                _logger.LogInformation("お知らせ詳細取得完了 - ID: {Id}, Title: {Title}", id, announcement.Title);

                // 5. JSON形式で返却
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "お知らせ詳細取得中にエラーが発生しました - ID: {Id}", id);
                return StatusCode(500, "お知らせの取得に失敗しました");
            }
        }

        /// <summary>
        /// お知らせ作成（管理者のみ）
        /// POST /api/announcements
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AnnouncementResponse>> CreateAnnouncement(CreateAnnouncementRequest request)
        {
            try
            {
                _logger.LogInformation("お知らせ作成開始 - Title: {Title}", request.Title);

                // 1. JWT解析とクレーム取得
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized("認証情報が無効です");
                }

                // 2. 管理者権限チェック
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null || !user.IsAdmin)
                {
                    _logger.LogWarning("管理者権限なしでお知らせ作成を試行 - UserId: {UserId}", userId);
                    return Forbid("管理者権限が必要です");
                }

                // 3. リクエストボディのバリデーション
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return BadRequest("タイトルは必須です");
                }

                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return BadRequest("内容は必須です");
                }

                if (string.IsNullOrWhiteSpace(request.Category))
                {
                    return BadRequest("カテゴリは必須です");
                }

                // 4. お知らせエンティティ作成
                var announcement = new Announcement
                {
                    Title = request.Title.Trim(),
                    Content = request.Content.Trim(),
                    Category = request.Category.Trim(),
                    IsPublished = request.IsPublished,
                    PublishDate = request.PublishDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // 5. データベース保存
                _context.Announcements.Add(announcement);
                await _context.SaveChangesAsync();

                // 6. AnnouncementResponse形式に変換
                var response = new AnnouncementResponse
                {
                    Id = announcement.Id,
                    Title = announcement.Title,
                    Content = announcement.Content,
                    Category = announcement.Category,
                    IsPublished = announcement.IsPublished,
                    PublishDate = announcement.PublishDate,
                    CreatedAt = announcement.CreatedAt,
                    UpdatedAt = announcement.UpdatedAt
                };

                _logger.LogInformation("お知らせ作成完了 - ID: {Id}, Title: {Title}", announcement.Id, announcement.Title);

                // 7. 201 Createdで返却
                return CreatedAtAction(nameof(GetAnnouncement), new { id = announcement.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "お知らせ作成中にエラーが発生しました - Title: {Title}", request.Title);
                return StatusCode(500, "お知らせの作成に失敗しました");
            }
        }

        /// <summary>
        /// お知らせ更新（管理者のみ）
        /// PUT /api/announcements/1
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAnnouncement(int id, UpdateAnnouncementRequest request)
        {
            try
            {
                _logger.LogInformation("お知らせ更新開始 - ID: {Id}, Title: {Title}", id, request.Title);

                // 1. JWT解析とクレーム取得
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized("認証情報が無効です");
                }

                // 2. 管理者権限チェック
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null || !user.IsAdmin)
                {
                    _logger.LogWarning("管理者権限なしでお知らせ更新を試行 - UserId: {UserId}", userId);
                    return Forbid("管理者権限が必要です");
                }

                // 3. URLパラメータ取得とバリデーション
                if (id <= 0)
                {
                    return BadRequest("無効なお知らせIDです");
                }

                // 4. お知らせ検索
                var announcement = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id);
                if (announcement == null)
                {
                    _logger.LogWarning("更新対象のお知らせが見つかりません - ID: {Id}", id);
                    return NotFound("お知らせが見つかりません");
                }

                // 5. リクエストボディのバリデーション
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return BadRequest("タイトルは必須です");
                }

                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return BadRequest("内容は必須です");
                }

                if (string.IsNullOrWhiteSpace(request.Category))
                {
                    return BadRequest("カテゴリは必須です");
                }

                // 6. お知らせ情報更新
                announcement.Title = request.Title.Trim();
                announcement.Content = request.Content.Trim();
                announcement.Category = request.Category.Trim();
                announcement.IsPublished = request.IsPublished;
                announcement.PublishDate = request.PublishDate;
                announcement.UpdatedAt = DateTime.UtcNow;

                // 7. データベース保存
                await _context.SaveChangesAsync();

                _logger.LogInformation("お知らせ更新完了 - ID: {Id}, Title: {Title}", id, announcement.Title);

                // 8. 204 No Contentで返却
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "お知らせ更新中にエラーが発生しました - ID: {Id}", id);
                return StatusCode(500, "お知らせの更新に失敗しました");
            }
        }

        /// <summary>
        /// お知らせ削除（管理者のみ）
        /// DELETE /api/announcements/1
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            try
            {
                _logger.LogInformation("お知らせ削除開始 - ID: {Id}", id);

                // 1. JWT解析とクレーム取得
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized("認証情報が無効です");
                }

                // 2. 管理者権限チェック
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null || !user.IsAdmin)
                {
                    _logger.LogWarning("管理者権限なしでお知らせ削除を試行 - UserId: {UserId}", userId);
                    return Forbid("管理者権限が必要です");
                }

                // 3. URLパラメータ取得とバリデーション
                if (id <= 0)
                {
                    return BadRequest("無効なお知らせIDです");
                }

                // 4. お知らせ検索
                var announcement = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id);
                if (announcement == null)
                {
                    _logger.LogWarning("削除対象のお知らせが見つかりません - ID: {Id}", id);
                    return NotFound("お知らせが見つかりません");
                }

                // 5. お知らせ削除
                _context.Announcements.Remove(announcement);

                // 6. データベース保存
                await _context.SaveChangesAsync();

                _logger.LogInformation("お知らせ削除完了 - ID: {Id}, Title: {Title}", id, announcement.Title);

                // 7. 204 No Contentで返却
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "お知らせ削除中にエラーが発生しました - ID: {Id}", id);
                return StatusCode(500, "お知らせの削除に失敗しました");
            }
        }
    }
}
