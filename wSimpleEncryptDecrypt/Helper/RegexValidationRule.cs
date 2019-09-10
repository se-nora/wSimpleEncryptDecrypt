using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace wSimpleEncryptDecrypt.Helper
{
    public class RegexValidationRule : ValidationRule
    {
        private string _pattern;
        private Regex _regex;

        public string Pattern
        {
            get => _pattern;
            set
            {
                _pattern = value;
                _regex = new Regex(_pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }

        public string Error { get; set; }

        public RegexValidationRule()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo ultureInfo)
        {
            if (value == null || !_regex.Match(value.ToString()).Success)
            {
                return new ValidationResult(false, Error);
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
