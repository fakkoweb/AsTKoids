// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// Last Edited: 08-09-14


using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsTKoids.Objects.Types.Behaviours
{
    public interface Weapon
    {
        Vector3 BulletHole {get; set;}
        Vector3 BulletVector { get; set; }
        
    }
}
