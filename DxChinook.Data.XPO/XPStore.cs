using AutoMapper;
using AutoMapper.QueryableExtensions;
using DevExpress.Xpo;
using FluentValidation;
using FluentValidation.Results;

namespace DxChinook.Data.XPO
{
	public class XPDataResult<TKey> : DataResult
		where TKey: IEquatable<TKey>
	{
		public TKey Key { get; set; } = default!;
	}
	public abstract class XPDataStore<TKey, TModel, TDBModel> : IQueryableDataStore<TKey, TModel>, IDisposable
		where TKey : IEquatable<TKey>
		where TModel : class, new()
		where TDBModel : class, IXPSimpleObject
	{
		public const string CtxMode = "datamode";
		public const string CtxStore = "datastore";

		public XPDataStore(IDataLayer dataLayer, IMapper mapper, IValidator<TDBModel> validator)
		{
			Mapper = mapper;
			DataLayer = dataLayer;
			Validator = validator;
		}

		private UnitOfWork _uow = default!;
		private bool disposedValue;

		protected UnitOfWork UnitOfWork { 
			get
			{
				if (_uow == default)
					_uow = new UnitOfWork(DataLayer);
				return _uow;
			}
		}

		protected IMapper Mapper { get; }
		public IDataLayer DataLayer { get; }
		public IValidator<TDBModel> Validator { get; }
		public virtual bool PaginateViaPrimaryKey { get => false; }
		
		public TModel CreateModel() => new TModel();
		protected virtual TDBModel? XPOGetByKey(TKey key, Session session)
		{
			return session.GetObjectByKey<TDBModel>(key);
		}

		protected virtual IQueryable<TDBModel> DBQuery(UnitOfWork uow) => uow.Query<TDBModel>();

		public virtual IQueryable<TModel> Query() => DBQuery(UnitOfWork).ProjectTo<TModel>(Mapper.ConfigurationProvider);
		public virtual IQueryable<T> Query<T>() where T : class, new()
		{
			return DBQuery(UnitOfWork).ProjectTo<T>(Mapper.ConfigurationProvider);
		}

		public virtual TModel GetByKey(TKey key)
		{
			using (var wrk = new Session(DataLayer))
			{
				TModel result = CreateModel();
				return Mapper.Map(XPOGetByKey(key, wrk), result);
			}
		}

		protected async virtual Task<T> TransactionalExecAsync<T>(
			Func<XPDataStore<TKey, TModel, TDBModel>, Session, Task<T>> work,
			bool transactional = true, bool autoCommit = true)
		{
			T result = default!;
			using (var wrk = transactional ? new UnitOfWork(DataLayer) : new Session(DataLayer))
			{
				if (transactional)
					wrk.BeginTransaction();

				result = await work(this, wrk);

				if (autoCommit && transactional)
					await wrk.CommitTransactionAsync();
			}
			return result;
		}
		protected async virtual Task TransactionalExecAsync<T>(
			Func<XPDataStore<TKey, TModel, TDBModel>, Session, Task> work,
			bool transactional = true, bool autoCommit = true)
		{
			using(var wrk = transactional ? new UnitOfWork(DataLayer) : new Session(DataLayer))
			{
				if (transactional)
					wrk.BeginTransaction();

				await work(this, wrk);

				if (autoCommit && transactional)
					await wrk.CommitTransactionAsync();
			}
		}
		public abstract string KeyField { get; }

		public abstract void SetModelKey(TModel model, TKey key);
		public abstract TKey ModelKey(TModel model);
		protected abstract TKey DBModelKey(TDBModel model);

		protected async Task<ValidationResult> ValidateDBModelAsync(TDBModel item,
			DataMode mode,
			XPDataStore<TKey, TModel, TDBModel> store)
		{
			var validationContext = new ValidationContext<TDBModel>(item);
			validationContext.RootContextData[CtxMode] = mode;
			validationContext.RootContextData[CtxStore] = store;

			var result = await Validator.ValidateAsync(validationContext);
			return result;
		}

        class InsertHelper
        {
            public InsertHelper(TModel model, ValidationResult insertingResult, XPDataResult<TKey> insertedResult)
            {
                Model = model;
                InsertingResult = insertingResult;
                InsertedResult = insertedResult;
            }
            public TModel Model { get; private set; }
            public ValidationResult InsertingResult { get; private set; }
            public XPDataResult<TKey> InsertedResult { get; private set; }
        }

        protected enum StoreMode { Create, Update, Store }
        protected TKey EmptyKeyValue => default!;

        protected async virtual Task<IDataResult> StoreAsync(StoreMode mode, params TModel[] items)
		{
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			var result = await TransactionalExecAsync(
				async (s, wrk) =>
				{
                    // need to keep the xpo entities together with the model items so we can update 
                    // the id's of the models afterwards.
                    Dictionary<TDBModel, InsertHelper> batchPairs = new Dictionary<TDBModel, InsertHelper>();
                    try
                    {
                        ValidationResult validationResult = null!;
						DataMode dataMode = DataMode.Create;
                        foreach (var item in items)
						{
							var modelKey = ModelKey(item);
							TDBModel dbItem = null!;
	                        if (modelKey == null || modelKey.Equals(EmptyKeyValue) || mode == StoreMode.Create)
							{
								dataMode = DataMode.Create;
                                dbItem = (Activator.CreateInstance(typeof(TDBModel), wrk) as TDBModel)!;
                                batchPairs.Add(dbItem, new InsertHelper(item, validationResult, new XPDataResult<TKey>()));
                            }
                            else if (!modelKey.Equals(EmptyKeyValue) && (mode != StoreMode.Create))
							{
								dataMode = DataMode.Update;
                                dbItem = XPOGetByKey(modelKey, wrk)!;
                                if (dbItem == null)
                                    throw new ValidationException($"Unable to locate {typeof(TDBModel).Name}({modelKey}) in datastore");
                            }
                            Mapper.Map(item, dbItem);
                            validationResult = await ValidateDBModelAsync(dbItem, dataMode, s);
                            if (!validationResult.IsValid)
                                throw new ValidationException(validationResult.Errors);
                            await wrk.SaveAsync(dbItem);
                        }


                        wrk.ObjectSaved += (s, e) =>
                        {
                            // sync the model ids with the newly generated xpo id's
                            if (e.Object is TDBModel xpoItem && batchPairs.ContainsKey(xpoItem))
                            {
                                TKey k = (TKey)xpoItem.Session.GetKeyValue(xpoItem);
                                SetModelKey(batchPairs[xpoItem].Model, k);
                                if (batchPairs[xpoItem].InsertedResult != null)
                                    batchPairs[xpoItem].InsertedResult.Key = k;
								// INFO: We need to map back the XPO Item into the model.							
								Mapper.Map(xpoItem, batchPairs[xpoItem].Model);
                            }
                        };
						ValidationException commitFailure = null!;
                        wrk.FailedCommitTransaction += (s, e) =>
                        {
							commitFailure = new ValidationException(e.Exception.InnerException != null ? e.Exception.InnerException.Message : e.Exception.Message);
                            e.Handled = true;
                        };
                        await wrk.CommitTransactionAsync();
						if (commitFailure != null)
							return new DataResult { Success = false, Mode = dataMode, Exception = commitFailure };
						else
							return new DataResult { Success = true, Mode = dataMode };
					}
					catch (Exception err)
					{
						wrk.RollbackTransaction();
						return new DataResult(DataMode.Create, nameof(TDBModel), err);
					}
				},
				true, false);
			return result;
		}

        public async virtual Task<IDataResult> StoreAsync(params TModel[] items)
		{
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            return await StoreAsync(StoreMode.Store, items);

        }

        public async virtual Task<IDataResult> CreateAsync(params TModel[] items)
		{
            if (items == null)
                throw new ArgumentNullException(nameof(items));
			return await StoreAsync(StoreMode.Create, items);
        }

        public async virtual Task<IDataResult> UpdateAsync(params TModel[] items)
		{
			if (items == null)
				throw new ArgumentNullException(nameof(items));
			return await StoreAsync(StoreMode.Update, items);
		}

		public async virtual Task<IDataResult> DeleteAsync(params TKey[] ids)
		{
			if (ids == null)
				throw new ArgumentNullException(nameof(ids));

			var result = await TransactionalExecAsync(async (s, wrk) =>
			{
				try
				{
					foreach (var id in ids)
					{
						var dbModel = XPOGetByKey(id, wrk);
						if (dbModel != null)
						{
							var validationResult = await ValidateDBModelAsync(dbModel, DataMode.Delete, s);
							if (!validationResult.IsValid)
								throw new ValidationException(validationResult.Errors);

							await wrk.DeleteAsync(dbModel);
						}
					}
                    ValidationException commitFailure = null!;
                    wrk.FailedCommitTransaction += (s, e) =>
                    {
                        commitFailure = new ValidationException(e.Exception.InnerException != null ? e.Exception.InnerException.Message : e.Exception.Message);
                        e.Handled = true;
                    };

                    await wrk.CommitTransactionAsync();
                    if (commitFailure != null)
                        return new DataResult { Success = false, Mode = DataMode.Delete, Exception = commitFailure };
                    else
                        return new DataResult { Success = true, Mode = DataMode.Delete };
				}
				catch (ValidationException err)
				{
					wrk.RollbackTransaction();
					return new DataResult(DataMode.Delete, nameof(TDBModel), err);
				}
			}, true, false);
			return result;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					if (_uow != default)
						_uow.Dispose();
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}