using DxChinook.Data.Models;
using FluentValidation;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxChinook.Data
{
    public enum DataMode
    {
        Create,
        Update,
        Delete
    }

    public interface IDataResult
    {
        bool Success { get; set; }
        DataMode Mode { get; set; }
        ValidationException Exception { get; set; }
    }

    public interface IDataStore<TKey, TModel>
        where TKey : IEquatable<TKey>
        where TModel : class, new()
    {
        string KeyField { get; }        
        TModel GetByKey(TKey key);
        TKey ModelKey(TModel model);
        void SetModelKey(TModel model, TKey key);        
        Task<IDataResult> CreateAsync(params TModel[] items);
        Task<IDataResult> UpdateAsync(params TModel[] items);
        Task<IDataResult> DeleteAsync(params TKey[] ids);
    }

    public interface IQueryableDataStore<TKey, TModel> : IDataStore<TKey, TModel>
        where TKey : IEquatable<TKey>
        where TModel : class, new()
    {
        bool PaginateViaPrimaryKey { get; }
        IQueryable<T> Query<T>() where T : class, new();
        IQueryable<TModel> Query();

    }


    public interface IInvoiceLineStore : IDataStore<int, InvoiceLineModel>
    {
        Task<List<InvoiceLineModel>> GetByInvoiceIdAsync(int invoiceId);
        Task Store(int invoiceId, params InvoiceLineModel[] items);
    }

}
