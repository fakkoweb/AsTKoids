// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// Last Edited: 08-09-14

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsTKoids.Objects.Types.Behaviours
{
    public interface Damageable
    {
        int Health { get; set; }
        int Damage { get; set; }
        bool IsDead { get; set; }
    }
}
