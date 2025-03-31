﻿namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public const string TokenCookie = "MangoJwtToken";
        public enum ApiType
        {
            DELETE,
            GET,
            POST,
            PUT,
        }
    }
}
