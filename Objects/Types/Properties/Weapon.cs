using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Objects.Types.Properties
{
    public interface Weapon
    {
        Vector3 BulletHole {get; set;}
        Vector3 BulletVector { get; set; }
        
    }
}
