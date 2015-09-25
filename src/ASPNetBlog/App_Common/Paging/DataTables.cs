using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace ASPNetBlog.App_Common.Paging
{
    // Ritesh Pahwa @ Ritesh.Pahwa.com
    // Due to errors, copied here from Github until it starts working again with ASP.Net 5
    // Read more @ https://github.com/mcintyre321/mvc.jquery.datatables

    public class DataTableValueProvider : IValueProvider
    {
        public Task<bool> ContainsPrefixAsync(string prefix)
        {
            throw new NotImplementedException();
        }

        public Task<ValueProviderResult> GetValueAsync(string key)
        {
            throw new NotImplementedException();
        }
    }

    public class DataTablesParam
    {
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }
        public int iColumns { get; set; }
        public string sSearch { get; set; }
        public bool bEscapeRegex { get; set; }
        public int iSortingCols { get; set; }
        public int sEcho { get; set; }
        public List<string> sColumnNames { get; set; }
        public List<bool> bSortable { get; set; }
        public List<bool> bSearchable { get; set; }
        public List<string> sSearchValues { get; set; }
        public List<int> iSortCol { get; set; }
        public List<string> sSortDir { get; set; }
        public List<bool> bEscapeRegexColumns { get; set; }

        public DataTablesParam()
        {
            sColumnNames = new List<string>();
            bSortable = new List<bool>();
            bSearchable = new List<bool>();
            sSearchValues = new List<string>();
            iSortCol = new List<int>();
            sSortDir = new List<string>();
            bEscapeRegexColumns = new List<bool>();
        }

        public DataTablesParam(int iColumns)
        {
            this.iColumns = iColumns;
            sColumnNames = new List<string>(iColumns);
            bSortable = new List<bool>(iColumns);
            bSearchable = new List<bool>(iColumns);
            sSearchValues = new List<string>(iColumns);
            iSortCol = new List<int>(iColumns);
            sSortDir = new List<string>(iColumns);
            bEscapeRegexColumns = new List<bool>(iColumns);
        }

        //public DataTablesResponseData GetDataTablesResponse<TSource>(IQueryable<TSource> data)
        //Removed

    }


    public class DataTables
    {
        public static async Task<DataTablesParam> BindModelAsync(IReadableStringCollection valueProvider)
        {
            DataTablesParam obj = new DataTablesParam();
            obj.iDisplayStart = await GetValueAsync<int>(valueProvider, "start");
            obj.iDisplayLength = await GetValueAsync<int>(valueProvider, "length");
            obj.sSearch = await GetValueAsync<string>(valueProvider, "search[value]");
            obj.bEscapeRegex = await GetValueAsync<bool>(valueProvider, "search[regex]");
            obj.sEcho = await GetValueAsync<int>(valueProvider, "draw");

            int colIdx = 0;
            while (true)
            {
                string colPrefix = String.Format("columns[{0}]", colIdx);
                string colName = await GetValueAsync<string>(valueProvider, colPrefix + "[data]");
                if (String.IsNullOrWhiteSpace(colName))
                {
                    break;
                }
                obj.sColumnNames.Add(colName);
                obj.bSortable.Add(await GetValueAsync<bool>(valueProvider, colPrefix + "[orderable]"));
                obj.bSearchable.Add(await GetValueAsync<bool>(valueProvider, colPrefix + "[searchable]"));
                obj.sSearchValues.Add(await GetValueAsync<string>(valueProvider, colPrefix + "[search][value]"));
                obj.bEscapeRegexColumns.Add(await GetValueAsync<bool>(valueProvider, colPrefix + "[searchable][regex]"));
                colIdx++;
            }
            obj.iColumns = colIdx;
            colIdx = 0;
            while (true)
            {
                string colPrefix = $"order[{colIdx}]";
                int? orderColumn = await GetValueAsync<int?>(valueProvider, colPrefix + "[column]");
                if (orderColumn.HasValue)
                {
                    obj.iSortCol.Add(orderColumn.Value);
                    obj.sSortDir.Add(await GetValueAsync<string>(valueProvider, colPrefix + "[dir]"));
                    colIdx++;
                }
                else
                {
                    break;
                }
            }
            obj.iSortingCols = colIdx;
            return obj;
        }

        private static async Task<T> GetValueAsync<T>(IReadableStringCollection valueProvider, string key)
        {
            try
            {
                var valueResult = valueProvider.Get(key);
                var ret = (valueResult == null) ? default(T) : (T)Convert.ChangeType(valueResult, typeof(T));
                return ret;
            }
            catch (Exception e) { }

            return default(T);            
        }
    }


}
