using ApiShared.Core.Data.BaseInterface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using ApiShared.Core.Middlewares;

namespace ApiShared.Core.Data.BaseClass
{
    public class Repository<TContext> : IRepository<TContext> where TContext : DbContext
    {
        public TContext CurrentContext { private set; get; }
        public ICurrentUser CurrentUser { private set; get; }
        protected readonly IRemoteIPResolver IPResolver;
        private bool disposedValue;

        protected DbContext Db
        {
            get
            {
                return (DbContext)CurrentContext;
            }
        }
        protected void RejectChanges()
        {
            foreach (var entry in Db.ChangeTracker.Entries())
                switch (entry.State)
                {
                    case EntityState.Added: entry.State = EntityState.Detached; break;
                    case EntityState.Modified:
                    case EntityState.Deleted: entry.State = EntityState.Unchanged; break;
                }
        }

        public Repository(TContext context, IRemoteIPResolver remoteIPResolver, ICurrentUser currentUser)
        {
            CurrentContext = context;
            IPResolver = remoteIPResolver;
            CurrentUser = currentUser;
        }

        public T? Single<T>(Expression<Func<T, bool>> whereCondition) where T : EntityModel
        {
            return Query<T>().FirstOrDefault(whereCondition);
        }

        public void Add<T>(T entity) where T : EntityModel
        {
            Db.Set<T>().Add(entity);
        }

        public void Delete<T>(T entity) where T : EntityModel
        {
            Db.Set<T>().Remove(entity);
        }

        public void DeleteRange<T>(IQueryable<T> entity) where T : EntityModel
        {
            Db.Set<T>().RemoveRange(entity);
        }

        public void Attach<T>(T entity) where T : EntityModel
        {
            Db.Set<T>().Attach(entity);
        }

        public IQueryable<T> Query<T>() where T : EntityModel
        {
            var result = from instance in Db.Set<T>() select instance;
            var pe = Expression.Parameter(typeof(T), "instance");
            bool hasFiscalYear = typeof(T).GetInterfaces().Any(t => t == typeof(IEntityHasFiscalYear));
            if (hasFiscalYear)
            {
                var fiscalYearExp = Expression.Equal(Expression.PropertyOrField(pe, "FiscalYearId"), Expression.Constant(CurrentUser.FiscalYearId.GetValueOrDefault(), typeof(int)));
                var whereCallExpression = Expression.Call(typeof(Queryable),
                                  "Where",
                                  new[] { result.ElementType },
                                  result.Expression,
                                  Expression.Lambda<Func<T, bool>>(fiscalYearExp, new[] { pe }));
                result = result.Provider.CreateQuery<T>(whereCallExpression);
            }

            bool hasPath = typeof(T).GetInterfaces().Any(t => t == typeof(IEntityHasPath));
            if (hasPath)
            {
                PropertyInfo? propertyInfo = typeof(T).GetProperty("Path");
                if (propertyInfo != null)
                {
                    MemberExpression m = Expression.MakeMemberAccess(pe, propertyInfo);
                    ConstantExpression c = Expression.Constant(CurrentUser.Path, typeof(string));
                    MethodInfo? mi = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                    if (mi != null)
                    {
                        Expression call = Expression.Call(m, mi, c);
                        var whereCallExpression = Expression.Call(typeof(Queryable),
                                  "Where",
                                  new[] { result.ElementType },
                                  result.Expression,
                                  Expression.Lambda<Func<T, bool>>(call, new[] { pe }));
                        result = result.Provider.CreateQuery<T>(whereCallExpression);
                    }
                }
            }

            bool hasCompany = typeof(T).GetInterfaces().Any(t => t == typeof(IEntityHasCompany));
            bool hasBranch = typeof(T).GetInterfaces().Any(t => t == typeof(IEntityHasBranch));
            bool isFixed = typeof(T).GetInterfaces().Any(t => t == typeof(IEntityFixed));
            bool isShared = typeof(T).GetInterfaces().Any(t => t == typeof(IEntityShared));

            if (hasCompany)
            {
                var companyIdExp = Expression.Equal(Expression.PropertyOrField(pe, "CompanyId"), Expression.Constant(CurrentUser.CompanyId.GetValueOrDefault(), typeof(int)));
                BinaryExpression? exp = null;

                if (isFixed)
                {
                    var isFixedExp = Expression.Equal(Expression.PropertyOrField(pe, "IsFixed"), Expression.Constant(true, typeof(bool)));
                    exp = Expression.Or(companyIdExp, isFixedExp);
                }

                if (isShared)
                {
                    var isSharedExp = Expression.Equal(Expression.PropertyOrField(pe, "IsShared"), Expression.Constant(true, typeof(bool)));
                    if (exp == null)
                        exp = Expression.Or(companyIdExp, isSharedExp);
                    else
                        exp = Expression.Or(exp, isSharedExp);
                }

                if (exp != null)
                {
                    var whereCallExpression = Expression.Call(typeof(Queryable),
                                  "Where",
                                  new[] { result.ElementType },
                                  result.Expression,
                                  Expression.Lambda<Func<T, bool>>(exp, new[] { pe }));
                    return result.Provider.CreateQuery<T>(whereCallExpression);
                }
                else
                {
                    var whereCallExpression = Expression.Call(typeof(Queryable),
                                   "Where",
                                   new[] { result.ElementType },
                                   result.Expression,
                                   Expression.Lambda<Func<T, bool>>(companyIdExp, new[] { pe }));
                    return result.Provider.CreateQuery<T>(whereCallExpression);
                }

            }
            else if (hasBranch)
            {
                var branchIdExp = Expression.Equal(Expression.PropertyOrField(pe, "BranchId"), Expression.Constant(CurrentUser.BranchId.GetValueOrDefault(), typeof(int)));
                BinaryExpression? exp = null;

                if (isFixed)
                {
                    var isFixedExp = Expression.Equal(Expression.PropertyOrField(pe, "IsFixed"), Expression.Constant(true, typeof(bool)));
                    exp = Expression.Or(branchIdExp, isFixedExp);
                }

                if (exp != null)
                {
                    var whereCallExpression = Expression.Call(typeof(Queryable),
                                 "Where",
                                 new[] { result.ElementType },
                                 result.Expression,
                                 Expression.Lambda<Func<T, bool>>(exp, new[] { pe }));
                    return result.Provider.CreateQuery<T>(whereCallExpression);
                }
                else
                {
                    var whereCallExpression = Expression.Call(typeof(Queryable),
                                   "Where",
                                   new[] { result.ElementType },
                                   result.Expression,
                                   Expression.Lambda<Func<T, bool>>(branchIdExp, new[] { pe }));
                    return result.Provider.CreateQuery<T>(whereCallExpression);
                }
            }

            return result;
        }

        /// <summary>
        /// کوئری مستقیم روی sql 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IQueryable<T> FromSql<T>(FormattableString sql) where T : EntityModel
        {
            return Db.Set<T>().FromSqlInterpolated(sql);
        }

        public int Save()
        {
            try
            {
                #region کار با داده ها قبل از ثبت در دیتابیس
                Db.ChangeTracker.Entries().Where(t => t.State != EntityState.Unchanged).ToList().ForEach(entry =>
                {
                    #region ثبت log
                    if (entry.Entity is IEntityLog)
                    {
                        switch (entry.State)
                        {
                            case EntityState.Added:
                                ((IEntityLog)entry.Entity).Creator = CurrentUser.UserName;
                                ((IEntityLog)entry.Entity).CreationTime = DateTime.Now;
                                ((IEntityLog)entry.Entity).HostName = IPResolver.CallerIP;
                                break;
                            case EntityState.Modified:
                                ((IEntityLog)entry.Entity).Modifier = CurrentUser.UserName;
                                ((IEntityLog)entry.Entity).ModificationTime = DateTime.Now;
                                ((IEntityLog)entry.Entity).HostName = IPResolver.CallerIP;
                                break;
                        }
                    }
                    #endregion

                    #region جلوگیری از ویرایش یا حذف داده های ثابت
                    if (entry.Entity is IEntityFixed)
                    {
                        var isFixed = ((IEntityFixed)entry.Entity).IsFixed;
                        if ((isFixed && entry.State == EntityState.Modified) || (isFixed && entry.State == EntityState.Deleted))
                        {
                            if (!CurrentUser.IsSuperAdmin)
                            {
                                RejectChanges();
                                throw new AppException("اطلاعات ثابت قابل تغییر نیستند");
                            }
                        }
                    }
                    #endregion

                    #region محلی سازی
                    if (entry.State == EntityState.Added)
                    {
                        if (entry.Entity.GetType().GetInterfaces().Any(t => t == typeof(IEntityHasCompany)))
                            ((IEntityHasCompany)entry.Entity).CompanyId = CurrentUser.CompanyId.GetValueOrDefault();

                        if (entry.Entity.GetType().GetInterfaces().Any(t => t == typeof(IEntityHasBranch)))
                            ((IEntityHasBranch)entry.Entity).BranchId = CurrentUser.BranchId.GetValueOrDefault();
                    }
                    #endregion

                    #region جلوگیری از ویرایش یا حذف داده های غیر محلی
                    if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                    {
                        if (entry.Entity.GetType().GetInterfaces().Any(t => t == typeof(IEntityHasCompany)))
                        {
                            if (((IEntityHasCompany)entry.Entity).CompanyId != CurrentUser.CompanyId.GetValueOrDefault())
                            {
                                RejectChanges();
                                throw new AppException("اطلاعات ویرایش شده مربوط به شرکت دیگری است");
                            }
                        }

                        if (entry.Entity.GetType().GetInterfaces().Any(t => t == typeof(IEntityHasBranch)))
                        {
                            if (((IEntityHasBranch)entry.Entity).BranchId != CurrentUser.BranchId.GetValueOrDefault())
                            {
                                RejectChanges();
                                throw new AppException("اطلاعات ویرایش شده مربوط به شعبه دیگری است");
                            }
                        }
                    }
                    #endregion

                    #region سال مالی
                    if (entry.State == EntityState.Added)
                    {
                        if (entry.Entity.GetType().GetInterfaces().Any(t => t == typeof(IEntityHasFiscalYear)))
                            ((IEntityHasFiscalYear)entry.Entity).FiscalYearId = CurrentUser.FiscalYearId.GetValueOrDefault();
                    }
                    #endregion

                    var validationContext = new ValidationContext(entry);
                    Validator.ValidateObject(entry, validationContext);
                });
                #endregion

                return Db.SaveChanges();
            }
            catch (ValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(string.Format("- Entity of type \"{0}\" : \"{1}\"",
                   ex.ValidationResult.MemberNames.First(), ex.ValidationResult.ErrorMessage));
                sb.AppendLine();

                throw new AppException(sb.ToString());
            }
        }

        /// <summary>
        /// example $"SELECT * FROM [Blogs] WHERE {columnName} = @columnValue"
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteCommand(string sqlQuery, SqlParameter[] parameters)
        {
            return Db.Database.ExecuteSqlRaw(sqlQuery, parameters);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Db.Dispose();
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
