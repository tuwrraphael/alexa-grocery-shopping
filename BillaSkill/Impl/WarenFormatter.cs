using BillaSkill.Models;
using BillaSkill.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BillaSkill.Impl
{
    public class WarenFormatter : IWarenFormatter
    {
        public string Format(Ware ware)
        {
            return $"{ware.Name}, {ware.Menge.Replace(".",",")}, um {ware.Preis.ToString(CultureInfo.InvariantCulture)}€";
        }
    }
}
