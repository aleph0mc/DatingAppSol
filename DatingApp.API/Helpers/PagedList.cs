using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    /// <summary>
    /// Generic class helper used to manage paging
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedList(List<T> Items, int Count, int PageNumber, int PageSize)
        {
            TotalCount = Count;
            CurrentPage = PageNumber;
            this.PageSize = PageSize;
            TotalPages = (int)Math.Ceiling(Count / (double)PageSize);
            base.AddRange(Items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> Source, int PageNumber, int PageSize)
        {
            var count = await Source.CountAsync();
            var items = await Source.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToListAsync();

            return new PagedList<T>(items, count, PageNumber, PageSize);
        }
    }
}
