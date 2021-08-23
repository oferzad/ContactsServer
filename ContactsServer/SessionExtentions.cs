using Microsoft.AspNetCore.Http;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

public static class SessionExtensions
{
    //The first parameter (this) in a static function define an extention method to the class type mentioned after the "this" param!
    //It is a cosmetic method that allow developers to wrte the GetObject method as if it was originally written as part of the class.
    //It does not provide access to any private members in the class! Only replace the alternative of having a static function that gets the object as first param.
    public static T GetObject<T>(this ISession session, string key)
    {
        var data = session.GetString(key);
        if (data == null)
        {
            return default(T);
        }
        return JsonSerializer.Deserialize<T>(data);
    }

    public static void SetObject(this ISession session, string key, object value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    // Convert an object to a byte array
    //public static void SetObject(this ISession session, string key, object value)
    //{
    //    BinaryFormatter bf = new BinaryFormatter();
    //    using (var ms = new MemoryStream())
    //    {
    //        bf.Serialize(ms, value);
    //        session.Set(key, ms.ToArray());
    //    }
    //}

    //public static T GetObject<T>(this ISession session, string key)
    //{
    //    using (var memStream = new MemoryStream())
    //    {
    //        byte[] arrBytes = session.Get(key);
    //        var binForm = new BinaryFormatter();
    //        memStream.Write(arrBytes, 0, arrBytes.Length);
    //        memStream.Seek(0, SeekOrigin.Begin);
    //        var obj = binForm.Deserialize(memStream);
    //        if (obj is T)
    //            return (T) obj;
    //        return default(T);
    //    }
    //}

}