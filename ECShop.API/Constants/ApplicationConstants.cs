namespace ECShop.API.Constants
{
    public static class ApplicationConstants
    {
        public static class Validation
        {
            public const int MAX_USERNAME_LENGTH = 50;
            public const int MIN_USERNAME_LENGTH = 3;
            public const int MAX_EMAIL_LENGTH = 100;
            public const int MIN_PASSWORD_LENGTH = 6;
            public const int MAX_PASSWORD_LENGTH = 100;
            public const int MAX_FIRSTNAME_LENGTH = 50;
            public const int MAX_LASTNAME_LENGTH = 50;
            public const int MAX_SHIPPING_NAME_LENGTH = 100;
            public const int MAX_POSTAL_CODE_LENGTH = 10;
            public const int MAX_PREFECTURE_LENGTH = 50;
            public const int MAX_CITY_LENGTH = 100;
            public const int MAX_ADDRESS_LINE_LENGTH = 200;
            public const int MAX_PHONE_LENGTH = 20;
            public const int MAX_NOTES_LENGTH = 500;
        }

        public static class Pagination
        {
            public const int DEFAULT_PAGE_SIZE = 20;
            public const int MAX_PAGE_SIZE = 100;
            public const int MIN_PAGE_NUMBER = 1;
        }

        public static class Stock
        {
            public const int LOW_STOCK_THRESHOLD = 5;
            public const int MIN_QUANTITY = 1;
        }

        public static class Dashboard
        {
            public const int RECENT_ORDERS_COUNT = 5;
            public const int LOW_STOCK_PRODUCTS_COUNT = 5;
        }
    }
}
