using System;
using System.ComponentModel.DataAnnotations;

namespace MiniTool.Attributes
{
    /// <summary>
    /// 检查字符串长度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class StringLengthAttribute : ValidationAttribute
    {
        private Int32 MaxLength { get; set; }

        /// <summary>
        /// 不设定长度则为100
        /// </summary>
        /// <param name="maxLength"></param>
        public StringLengthAttribute(Int32 maxLength=100)
        {
            MaxLength = maxLength;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            if (value.ToString().Length > MaxLength)
            {
                ErrorMessage = "字符串长度超过设定的长度!";
                return false;
            }
            else return true;
  
        }
    }
}
