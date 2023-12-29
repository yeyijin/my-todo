using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Common
{
    public static class ObjectHelper
    {
        public static bool IsNullOrEmpty(this string val)
        {
            return string.IsNullOrWhiteSpace(val);
        }
        public static bool IsNotNullOrEmpty(this string val)
        {
            return !string.IsNullOrWhiteSpace(val);
        }

        /// <summary>
        /// 这边暴力地使用json序列处理，不使用 automapper 获取其他emit表达式处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T CopyTo<T>(this T obj) where T : class, new()
        {
            if (obj == null) return null!;
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj))!;
        }
    }
}
