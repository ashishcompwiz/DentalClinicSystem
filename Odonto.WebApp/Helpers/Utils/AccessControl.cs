﻿using Microsoft.AspNetCore.Http;


namespace Odonto.WebApp.Helpers.Utils
{
    public static class AccessControl
    {
        private static readonly HttpContext context;

        public static bool CanRender(string[] userTypes)
        {
            var userType = context.Session.GetString("userType");
            if (userType != "DEV" && userTypes.Length > 0)
            {
                if (!userTypes.ToString().Contains(userType))
                    return false;
            }
            return true;
        }
    }
}
