namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CuponAPIBase { get; set; }
        public static string productAPIBase { get; set; }
        public static string ShoppingCartAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "JwtToken";

        public static string AuthAPIBase { get; set; }
        public enum ApiType
        {
            Get,
            Post,
            Put,
            Delete
        }

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyForPickup = "ReadForPickup";
        public const string Status_Completed = "Completed";
        public const string Status_Refunded = "Refunded";
        public const string Status_Cancelled = "Cancelled";

    }
}
