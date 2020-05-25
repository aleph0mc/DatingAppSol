using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse Response, string Message)
        {
            Response.Headers.Add("Application-Error", Message);
            Response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static int CalculateAge(this DateTime Dob)
        {
            var age = DateTime.Today.Year - Dob.Year;
            if (Dob.AddYears(age) > DateTime.Today)
                --age;

            return age;
        }

        /// <summary>
        /// Header for pagination: data must be returned in camelCase for Angular works in camelCase
        /// </summary>
        /// <param name="Response"></param>
        /// <param name="CurrentPage"></param>
        /// <param name="ItemsPerPage"></param>
        /// <param name="TotalItems"></param>
        /// <param name="TotalPages"></param>
        public static void AddPagination(this HttpResponse Response, int CurrentPage, int ItemsPerPage, int TotalItems, int TotalPages)
        {
            var paginationHeader = new PaginationHeader(CurrentPage, ItemsPerPage, TotalItems, TotalPages);

            //USED TO RETURN DATA IN camelCase
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();

            Response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            Response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
