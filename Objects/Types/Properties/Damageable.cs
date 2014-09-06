using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Objects.Types.Properties
{
    public interface Damageable
    {
        int Health { get; set; }
        int Damage { get; set; }
        bool IsDead { get; set; }
    }
}
