﻿using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using SE_Praktikum.Components.Sprites.Actors.Bullets;
using SE_Praktikum.Components.Sprites.Actors.Spaceships;
using SE_Praktikum.Models;
using SE_Praktikum.Services;

namespace SE_Praktikum.Components.Sprites.Actors
{
    /// <summary>
    /// Whenever a power up is needed
    /// </summary>
    public abstract class PowerUp : Actor 
    {
        //fields
        private List<PowerUp> _powerUps;
        
        
        //Constructor
        protected PowerUp(AnimationHandler animationHandler, float health = 0.01f, SoundEffect soundEffect = null) : base(
            animationHandler, soundEffect)
        {
            Health = health;
            _powerUps = new List<PowerUp>();
            
        }
       
        // Bullets and the player can interact with powerups
        protected override void ExecuteInteraction(Actor other)
        {
            switch(other)
            {
                case Bullet b:
                    Tool = b;
                    LastAggressor = b.Parent;
                    Health -= b.Damage;
                    break;
                
                case Spaceship s:
                    IsRemoveAble = true;
                    ImpactSound?.Play();
                    break;
            }
        }

        protected override LevelEventArgs.ActorDiedEventArgs GetOnDeadEventArgs()
        {
            return new LevelEventArgs.PowerUpDiedEventArgs();
        }
    }




        
        
        

}

