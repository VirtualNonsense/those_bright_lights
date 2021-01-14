﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SE_Praktikum.Extensions;
using SE_Praktikum.Models;
using SE_Praktikum.Services;

namespace SE_Praktikum.Components.Sprites.Actors.Spaceships
{
    public abstract class EnemyWithViewbox : Enemy
    {
        public Polygon ViewBox;
        public override float Rotation
        {
            get => base.Rotation;
            set
            {
                base.Rotation = value;
                if (Rotation < 3 * Math.PI/2 && Rotation > Math.PI/2)
                {
                    if (HitBoxFlipped) return;
                    ViewBox = ViewBox?.MirrorSingleVertical(Position);
                }
                else if (HitBoxFlipped)
                {
                    ViewBox = ViewBox?.MirrorSingleVertical(Position);
                }
            }
        }


        protected EnemyWithViewbox(AnimationHandler animationHandler,
                                   Polygon viewBox,
                                   float maxSpeed = 3,
                                   float acceleration =5,
                                   float rotationAcceleration = .1f,
                                   float maxRotationSpeed = 10,
                                   float health = 50,
                                   float? maxHealth = null,
                                   float impactDamage = 5,
                                   SoundEffect impactSound = null) : base(animationHandler, maxSpeed, acceleration,rotationAcceleration, maxRotationSpeed, health, maxHealth, impactDamage, impactSound: impactSound)
        {
            ViewBox = viewBox;
            RotateAndShoot = true;
        }


        public override void Update(GameTime gameTime)
        {
            ViewBox.Position = Position;
            ViewBox.Rotation = Rotation;
            ViewBox.Layer = Layer;
            Shoot.Update(gameTime);
            if (I == InterAction.InView && Target != null)
                Shoot.Fire();

            base.Update(gameTime);
        }

        
        

        protected override bool InteractAble(Actor other)
        {
            switch (other)
            {
                case Player p:
                    if (p.HitBox.Any(polygon => ViewBox.Overlap(polygon)))
                    {
                        I = InterAction.InView;
                        return true;
                    }

                    break;
            }

            return base.InteractAble(other);
        }

    }

   
}