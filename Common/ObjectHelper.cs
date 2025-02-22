using Newtonsoft.Json;

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
        /// 并不追求性能
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