using System;
using SevenEngine;
using Seven.Structures;
using SevenEngine.StaticModels;
using Game.States;

namespace Game.Units
{
    class Spaceship : Unit
    {
        public Spaceship(string id) : base(id, StaticModelManager.GetModel("Spaceship"))
        {

        }
    }
}
