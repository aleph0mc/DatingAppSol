using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public class MessageParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _PageSize = 10;
        public int PageSize
        {
            get { return _PageSize; }
            set
            {
                _PageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }
        public int UserId { get; set; }
        public string MessageContainer { get; set; } = "Unread";
    }
}
