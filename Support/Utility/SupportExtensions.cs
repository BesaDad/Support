using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Support.Domain.Models;

namespace Support.Utility
{
    public static class SupportExtensions
    {
        public static string GetDescription(this Enum enymType) //Hint: Change the method signature and input paramter to use the type parameter T
        {
            Type genericEnumType = enymType.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(enymType.ToString());
            if ((memberInfo.Length > 0))
            {
                var attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((attribs.Any()))
                {
                    return ((System.ComponentModel.DescriptionAttribute)attribs.ElementAt(0)).Description;
                }
            }
            return enymType.ToString();
        }

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                select new { Id = e, Name = e.ToString() };
            return new SelectList(values, "Id", "Name", enumObj);
        }

    }
}