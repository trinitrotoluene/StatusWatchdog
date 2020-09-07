using System;
using System.ComponentModel.DataAnnotations;

namespace StatusWatchdog.Controllers.RequestModels
{
    public class RequiredEnumAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            var type = value.GetType();
            return type.IsEnum && Enum.IsDefined(type, value);
        }
    }
}
