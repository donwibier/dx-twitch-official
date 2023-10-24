using AutoMapper.QueryableExtensions;
using AutoMapper;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxChinook.Data.EF
{
    public class EFResult : IDataResult
    {
        public EFResult()
        {

        }
        public EFResult(DataMode mode, string propertyName, Exception err)
        {
            Mode = mode;
            Success = (err == null);
            if (!Success)
            {
                Exception = (err as ValidationException)!;
                if (Exception == null)
                    Exception = new ValidationException(new[] {
                        new ValidationFailure(propertyName, err!.InnerException == null ? err.Message : err.InnerException.Message)
                    });
            }
        }
        public bool Success { get; set; }
        public DataMode Mode { get; set; }
        public ValidationException Exception { get; set; } = default!;
    }

    public abstract class EFDataStore<TEFContext, TKey, TModel, TDBModel> : IDataStore<TKey, TModel>            
        where TEFContext : DbContext, new()
        where TKey : IEquatable<TKey>
        where TModel : class, new()
        where TDBModel : class, new()
    {
        public const string CtxMode = "datamode";
        public const string CtxStore = "datastore";

        public EFDataStore(TEFContext context, IMapper mapper, IValidator<TDBModel> validator)
        {
            Mapper = mapper;
            DbContext = context;
            Validator = validator;
        }

        protected IMapper Mapper { get; }
        public TEFContext DbContext { get; }
        public IValidator<TDBModel> Validator { get; }

        public TModel CreateModel()
        {
            return new TModel();
        }
        protected virtual TDBModel? EFGetByKey(TKey key)
        {
            return DbContext.Find<TDBModel>(key);
        }

        protected virtual IQueryable<TDBModel> EFQuery()
        {
            return DbContext.Set<TDBModel>();
        }

        public virtual IQueryable<TModel> Query()
        {
            return EFQuery().ProjectTo<TModel>(Mapper.ConfigurationProvider);
        }
        public virtual IQueryable<T> Query<T>() where T : class, new()
        {
            return EFQuery().ProjectTo<T>(Mapper.ConfigurationProvider);
        }

        public virtual TModel GetByKey(TKey key)
        {
            TModel result = CreateModel();
            return Mapper.Map(EFGetByKey(key), result);
        }
        
        protected async virtual Task<T> TransactionalExecAsync<T>(
            Func<EFDataStore<TEFContext, TKey, TModel, TDBModel>,
            IDbContextTransaction, Task<T>> work,
            bool autoCommit = true)
        {
            T result = default!;
            using (var dbTrans = await DbContext.Database.BeginTransactionAsync())
            {
                result = await work(this, dbTrans);
                if (autoCommit && DbContext.ChangeTracker.HasChanges())
                {
                    await DbContext.SaveChangesAsync();
                    await dbTrans.CommitAsync();
                }
            }
            return result;
        }
        protected async virtual Task TransactionalExecAsync<T>(
            Func<EFDataStore<TEFContext, TKey, TModel, TDBModel>,
            IDbContextTransaction, Task> work, bool autoCommit = true)
        {
            using (var dbTrans = await DbContext.Database.BeginTransactionAsync())
            {
                await work(this, dbTrans);
                if (autoCommit && DbContext.ChangeTracker.HasChanges())
                {
                    await DbContext.SaveChangesAsync();
                    await dbTrans.CommitAsync();
                }
            }
        }        
        public abstract string KeyField { get; }
        public abstract void SetModelKey(TModel model, TKey key);
        

        public abstract TKey ModelKey(TModel model);

        protected abstract TKey DBModelKey(TDBModel model);

        protected async Task<ValidationResult> ValidateDBModelAsync(TDBModel item,
            DataMode mode,
            EFDataStore<TEFContext, TKey, TModel, TDBModel> store)
        {
            var validationContext = new ValidationContext<TDBModel>(item);
            validationContext.RootContextData[CtxMode] = mode;
            validationContext.RootContextData[CtxStore] = store;

            var result = await Validator.ValidateAsync(validationContext);
            return result;
        }

        public async virtual Task<IDataResult> CreateAsync(params TModel[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var result = await TransactionalExecAsync(
                async (s, t) =>
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            var newItem = new TDBModel();
                            Mapper.Map(item, newItem);

                            var validationResult = await ValidateDBModelAsync(newItem, DataMode.Create, s);
                            if (!validationResult.IsValid)
                                throw new ValidationException(validationResult.Errors);

                            var r = await DbContext.Set<TDBModel>().AddAsync(newItem);
                            await DbContext.SaveChangesAsync();
                            Mapper.Map(r.Entity, item); //reload changes in item
                                                        //SetModelKey(item, DBModelKey(r.Entity));
                        }
                        await s.DbContext.SaveChangesAsync();
                        await t.CommitAsync();
                        return new EFResult { Success = true, Mode = DataMode.Create };
                    }
                    catch (Exception err)
                    {
                        return new EFResult(DataMode.Create, nameof(TDBModel), err);
                    }
                },
                false);
            return result;
        }
        public async virtual Task<IDataResult> UpdateAsync(params TModel[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var result = await TransactionalExecAsync(async (s, t) =>
            {
                try
                {
                    foreach (var item in items)
                    {
                        var key = ModelKey(item);
                        var dbModel = EFGetByKey(key);
                        if (dbModel == null)
                            throw new Exception($"Unable to locate {typeof(TDBModel).Name}({key}) in datastore");

                        Mapper.Map(item, dbModel);

                        var validationResult = await ValidateDBModelAsync(dbModel, DataMode.Update, s);
                        if (!validationResult.IsValid)
                            throw new ValidationException(validationResult.Errors);

                        DbContext.Entry(dbModel).State = EntityState.Modified;
                        await DbContext.SaveChangesAsync();
                    }
                    await s.DbContext.SaveChangesAsync();
                    await t.CommitAsync();
                    return new EFResult { Success = true, Mode = DataMode.Update };
                }
                catch (Exception err)
                {
                    return new EFResult(DataMode.Update, nameof(TDBModel), err);
                }
            }, false);
            return result;
        }

        public async virtual Task<IDataResult> DeleteAsync(params TKey[] ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await TransactionalExecAsync(async (s, t) =>
            {
                try
                {
                    foreach (var id in ids)
                    {
                        var dbModel = EFGetByKey(id);
                        if (dbModel != null)
                        {
                            var validationResult = await ValidateDBModelAsync(dbModel, DataMode.Delete, s);
                            if (!validationResult.IsValid)
                                throw new ValidationException(validationResult.Errors);

                            DbContext.Entry(dbModel).State = EntityState.Deleted;
                            await DbContext.SaveChangesAsync();
                        }
                    }

                    await s.DbContext.SaveChangesAsync();
                    await t.CommitAsync();
                    return new EFResult { Success = true, Mode = DataMode.Delete };
                }
                catch (ValidationException err)
                {
                    return new EFResult(DataMode.Delete, nameof(TDBModel), err);
                }
            }, false);
            return result;
        }
    }
}
