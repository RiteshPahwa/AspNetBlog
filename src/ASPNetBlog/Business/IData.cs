using System.Collections.Generic;
using System.Threading.Tasks;
using ASPNetBlog.Models;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/22/15*/

namespace ASPNetBlog.Business
{
    public interface IData<T,TKey>
    {
        ApplicationDbContext Db { get; set; }

        Task<T> GetByIdAsync(TKey id);
        Task<IEnumerable<T>> SearchAsync(string searchText, int pageNo);
    }
}