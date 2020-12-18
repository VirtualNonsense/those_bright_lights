﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using NLog;
using SE_Praktikum.Components.Sprites.Weapons;
using SE_Praktikum.Models;
using SE_Praktikum.Services;

namespace SE_Praktikum.Components.Sprites
{
    public class Enemy : Spaceship
    {
        private Logger _logger;
        private bool _shot = false;
        public Polygon ViewBox;
        private float _shootIntervall;
        private float _timeSinceLastShot = 0;
        private bool _canShoot;
        

        public Enemy(AnimationHandler animationHandler, float speed = 3, float health = 50, SoundEffect impactSound = null) : base(animationHandler, speed, health, impactSound)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _shootIntervall = 2000;
        }
        
        
        public override void Update(GameTime gameTime)
        {
            _timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_timeSinceLastShot >= _shootIntervall)
            {
                _canShoot = true;
                _timeSinceLastShot = 0;
            }
            Vector2 velocity = Vector2.Zero;
            
            base.Update(gameTime);
            ViewBox.Position = Position;
            ViewBox.Rotation = Rotation;
            ViewBox.Layer = Layer;
        }

        

        protected override void InvokeOnTakeDamage(float damage)
        {
            _logger.Info(Health);
            Health -= damage;
            base.InvokeOnTakeDamage(damage);
        }
        

        protected override bool InteractAble(Actor other)
        {
            switch (other)
            {
                case Player p:
                    foreach (var polygon in p.HitBox)
                    {
                        if (ViewBox.Overlap(polygon))
                        {
                            InvokeOnShootPlayer(Velocity,p);
                            return true;
                        }
                    }

                    break;
            }
            return base.InteractAble(other);
        }
        

        protected override void InvokeOnShootPlayer(Vector2 velocity, Actor player)
        {
            if (!_canShoot)
                return;
            _canShoot = false;
            base.InvokeOnShootPlayer(velocity, player);
        }
    }
}