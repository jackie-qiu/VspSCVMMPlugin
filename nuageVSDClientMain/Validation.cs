using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Nuage.VSDClient.Main
{
    public class RequiredValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Must not be empty");
            }

            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "Must not be empty");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class URLValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Must not be empty");
            }

            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "Must not be empty");
            }

            if (!(Uri.IsWellFormedUriString(value.ToString(), UriKind.Absolute)))
            {
                return new ValidationResult(false, "Invalid Url address");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class IpAddressValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Must not be empty");
            }


            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "Must not be empty");
            }

            string ipaddr = value.ToString().Replace(" ", "");

            IPAddress ipAddress;
            if (!IPAddress.TryParse(ipaddr, out ipAddress))
            {
                return new ValidationResult(false, "Invalid ip address");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class IpNetworkValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Must not be empty");
            }


            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "Must not be empty");
            }

            string cidr = value.ToString().Replace(" ", "");
            string[] split1 = cidr.Split('/');
            if(split1.Length != 2)
            {
                return new ValidationResult(false, "Invalid network address");
            }

            string[] split2 = split1[0].Split('.');
            if(split2.Length != 4)
            {
                return new ValidationResult(false, "Invalid network address");
            }

            IPNetwork ipnetwork;
            if (!IPNetwork.TryParse(cidr, out ipnetwork))
            {
                return new ValidationResult(false, "Invalid network address");
            }

            return ValidationResult.ValidResult;
        }
    }
}
