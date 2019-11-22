using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Support.Domain.Models;

namespace Support.Utility
{
    public static class SupportExtensions
    {
        public static string GetDescription(this Enum enymType) //Hint: Change the method signature and input paramter to use the type parameter T
        {
            Type genericEnumType = enymType.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(enymType.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return enymType.ToString();
        }

    }
}