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
            return $"{ware.Name}, {ware.Menge.Replace(".", ",")}, um {ware.Preis.ToString(CultureInfo.InvariantCulture)}€";
        }

        public string Format(WarenkorbEintrag ware)
        {
            if (ware.Ammount == 1)
            {
                return $"{ware.Ware.Name}";
            }
            else
            {
                return $"{ware.Ammount} Mal {ware.Ware.Name}";
            }
        }

        public string Format(Warenkorb ware)
        {
            return Und(ware.Waren, Format);
        }

        private string Und<T>(T[] arr, Func<T,string> func, string und = "und")
        {
            if (arr.Length == 1)
            {
                return func(arr[0]);
            }
            else
            {
                return $"{string.Join(", ", arr.Take(arr.Length - 1).Select(func))} {und} {func(arr[arr.Length - 1])}";
            }
        }

        public string Marken(Ware[] ware)
        {
            return Und(ware, v => v.Marke, "und von");
        }
    }
}
