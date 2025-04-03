using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Models.helper
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum @enum)
        {
            Type genericEnumType = @enum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(@enum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return @enum.ToString();
        }
    }
}
