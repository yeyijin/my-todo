using MaterialDesignThemes.Wpf;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ToDoList.Models;

namespace ToDoList.Resposity
{
    public class ResposityManager
    {
        public SqlSugarScope DbContent { get; private set; }
        private SqlSugarScope db => DbContent;

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


        public async Task<int> AddAsync<T>(T obj) where T: class, new()
        {
           
            var insert = DbContent.Insertable<T>(obj);
            return await insert.ExecuteReturnIdentityAsync();
        }
        public async Task<int> AddAsync<T>(List<T> obj) where T : class, new()
        {
            var insert = DbContent.Insertable<T>(obj);
            return await insert.ExecuteReturnIdentityAsync();
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




        public  ISugarQueryable<T> AsQueryable<T>()where T: class, new()
        {
            return DbContent.Queryable<T>();
        }

        public async Task<bool> DeleteAsync<T>(T obj) where T : class, new()
        {
            var insert = DbContent.Deleteable<T>(obj);
            return await insert.ExecuteCommandHasChangeAsync();
        }

    }
}
