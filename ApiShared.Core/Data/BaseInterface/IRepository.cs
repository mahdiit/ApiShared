using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Data.BaseInterface
{
    public interface IRepository<TContext> : IDisposable where TContext : DbContext
    {
        TContext CurrentContext { get; }
        T? Single<T>(Expression<Func<T, bool>> whereCondition) where T : EntityModel;
        void Add<T>(T entity) where T : EntityModel;
        void Delete<T>(T entity) where T : EntityModel;
        void DeleteRange<T>(IQueryable<T> entity) where T : EntityModel;
        void Attach<T>(T entity) where T : EntityModel;
        IQueryable<T> Query<T>() where T : EntityModel;
        IQueryable<T> FromSql<T>(FormattableString sql) where T : EntityModel;
        int ExecuteCommand(string sqlQuery, SqlParameter[] parameters);
        int Save();
    }
}
