namespace ECShop.API.Constants
{
    public static class ErrorMessages
    {
        public static class Authentication
        {
            public const string USERNAME_ALREADY_EXISTS = "Username already exists";
            public const string EMAIL_ALREADY_EXISTS = "Email already exists";
            public const string INVALID_CREDENTIALS = "Invalid username or password";
            public const string UNAUTHORIZED = "Unauthorized access";
            public const string ADMIN_REQUIRED = "管理者権限が必要です。";
        }

        public static class User
        {
            public const string USER_NOT_FOUND = "ユーザーが見つかりません。";
            public const string CANNOT_REMOVE_LAST_ADMIN = "最後の管理者の権限を削除することはできません。";
        }

        public static class Product
        {
            public const string PRODUCT_NOT_FOUND = "商品が見つかりません。";
            public const string INSUFFICIENT_STOCK = "在庫が不足しています。";
        }

        public static class Order
        {
            public const string ORDER_NOT_FOUND = "注文が見つかりません。";
            public const string EMPTY_CART = "カートが空です。";
            public const string INVALID_ORDER_STATUS = "無効な注文ステータスです。";
        }

        public static class Validation
        {
            public const string ID_MISMATCH = "IDが一致しません。";
            public const string INVALID_RANGE = "無効な範囲です。";
            public const string REQUIRED_FIELD = "必須項目です。";
        }

        public static class General
        {
            public const string INTERNAL_ERROR = "内部エラーが発生しました。";
            public const string NOT_FOUND = "リソースが見つかりません。";
            public const string BAD_REQUEST = "不正なリクエストです。";
            public const string INVALID_REQUEST = "無効なリクエストです。";
            public const string INVALID_OPERATION = "無効な操作です。";
            public const string REQUEST_TIMEOUT = "リクエストがタイムアウトしました。";
        }

        // Global exception handling constants
        public const string InvalidRequest = General.BAD_REQUEST;
        public const string Unauthorized = Authentication.UNAUTHORIZED;
        public const string InvalidOperation = General.INVALID_OPERATION;
        public const string NotFound = General.NOT_FOUND;
        public const string RequestTimeout = General.REQUEST_TIMEOUT;
        public const string InternalServerError = General.INTERNAL_ERROR;
    }
}
