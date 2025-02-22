using SqlSugar;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ToDoList.Models;

namespace ToDoList.Resposity
{
    /// <summary>
    /// 仓储管理类
    /// todo: 抽象
    /// </summary>
    public class ResposityManager
    {
        #region Public Constructors

        /// <summary>
        /// 创建dbcontent
        /// </summary>
        public ResposityManager(string connectionString)
        {
            DbContent = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.Sqlite,
                ConnectionString = connectionString,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    //注意:  这儿AOP设置不能少
                    EntityService = (c, p) =>
                    {
                        /***高版C#写法***/
                        //支持string?和string
                        if (p.IsPrimarykey == false && new NullabilityInfoContext()
                         .Create(c).WriteState is NullabilityState.Nullable)
                        {
                            //设置自动可空
                            p.IsNullable = true;
                        }
                    },
                    EntityNameService = (type, entity) =>
                    {
                        entity.IsDisabledDelete = true;  //禁止删除列
                        entity.IsDisabledUpdateAll = true;//禁止codefirst 创建更新表列
                    }
                }
            });
            InitDb();
        }

        #endregion Public Constructors

        #region Properties

        public SqlSugarScope DbContent { get; private set; }
        private SqlSugarScope db => DbContent;

        #endregion Properties

        #region Public Methods

        public async Task<int> AddAsync<T>(T obj) where T : class, new()
        {
            var insert = DbContent.Insertable<T>(obj);
            return await insert.ExecuteReturnIdentityAsync();
        }

        public async Task<int> AddAsync<T>(List<T> obj) where T : class, new()
        {
            var insert = DbContent.Insertable<T>(obj);
            return await insert.ExecuteReturnIdentityAsync();
        }

        public ISugarQueryable<T> AsQueryable<T>() where T : class, new()
        {
            return DbContent.Queryable<T>();
        }

        public async Task<bool> DeleteAsync<T>(T obj) where T : class, new()
        {
            var insert = DbContent.Deleteable<T>(obj);
            return await insert.ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> UpdateAsync<T>(T obj) where T : class, new()
        {
            var insert = DbContent.Updateable<T>(obj);
            return await insert.ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> UpdateAsync<T>(List<T> obj) where T : class, new()
        {
            var insert = DbContent.Updateable<T>(obj);
            return await insert.ExecuteCommandHasChangeAsync();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 初始化db
        /// </summary>
        private void InitDb()
        {
            //创建DB,

            List<Type> types = new List<Type>() {
               typeof(MenuInfo)
               , typeof(StatisticsInfo)
               , typeof(ToDoInfo)
           };
            db.Open();
            //跟进model创建表
            /***手动建多个表***/
            db.CodeFirst.SetStringDefaultLength(200)
            .InitTables(types.ToArray()
            );

            //比较数据库差异
            var diffString = db.CodeFirst.GetDifferenceTables(types.ToArray()).ToDiffString();
            Console.WriteLine($"数据库差异：" + diffString);
        }

        #endregion Private Methods
    }
}