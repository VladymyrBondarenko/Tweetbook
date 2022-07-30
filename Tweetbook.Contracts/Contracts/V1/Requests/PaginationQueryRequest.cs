using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweetbook.Contracts.Contracts.V1.Requests
{
    public class PaginationQueryRequest
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
