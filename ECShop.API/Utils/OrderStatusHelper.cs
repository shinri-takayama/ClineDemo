using ECShop.API.Models;

namespace ECShop.API.Utils
{
    public static class OrderStatusHelper
    {
        private static readonly Dictionary<OrderStatus, string> StatusTexts = new()
        {
            { OrderStatus.Pending, "注文確認中" },
            { OrderStatus.Confirmed, "注文確定" },
            { OrderStatus.Processing, "処理中" },
            { OrderStatus.Shipped, "発送済み" },
            { OrderStatus.Delivered, "配送完了" },
            { OrderStatus.Cancelled, "キャンセル" }
        };

        public static string GetStatusText(OrderStatus status)
        {
            return StatusTexts.TryGetValue(status, out var text) ? text : "不明";
        }

        public static string GetStatusText(int statusValue)
        {
            if (Enum.IsDefined(typeof(OrderStatus), statusValue))
            {
                return GetStatusText((OrderStatus)statusValue);
            }
            return "不明";
        }

        public static bool IsValidStatus(int statusValue)
        {
            return Enum.IsDefined(typeof(OrderStatus), statusValue);
        }

        public static IEnumerable<(OrderStatus Status, string Text)> GetAllStatuses()
        {
            return StatusTexts.Select(kvp => (kvp.Key, kvp.Value));
        }
    }
}
