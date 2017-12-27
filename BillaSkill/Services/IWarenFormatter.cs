using BillaSkill.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillaSkill.Services
{
    public interface IWarenFormatter
    {
        string Format(Ware ware);
        string Format(WarenkorbEintrag ware);
        string Format(Warenkorb ware);
        string Marken(Ware[] ware);
    }
}
