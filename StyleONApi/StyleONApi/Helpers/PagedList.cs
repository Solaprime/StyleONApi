using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.Helpers
{
    // Making those Property Private Means i dont wont people outside this class to assign 
    // Property to these values
    public class PagedList<T> :  List<T>
    {
        public int CurrentPage { get;  private set; }
        public int TotalPages { get;  private set; }
        public int PageSize { get;  private set; }
        public int TotalCount{ get; private set; }
        public bool HasPrevious => (CurrentPage > 1);
        public bool HasNext=> (CurrentPage < TotalPages);

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
                 // ceiling returns the smallest integral number that is greater than or eqault to
                 // a specifed decimal Number
        }

        // We are not Calling the constructor directly, we will use a static metthod
        //to call the constructor

        public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
        // Take method returns a specified number of contigious element from the start of 
        // a sequence

        //public async  static  Task<PagedList<T>> Create(IQueryable<T> source, int pageNumber, int pageSize)
        //{
        //    var count = source.Count();
        //    var items = await  source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        //    return new PagedList<T>(items, count, pageNumber, pageSize);
        //}
    }
}
