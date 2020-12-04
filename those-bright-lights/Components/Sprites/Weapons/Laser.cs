﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SE_Praktikum.Services;

namespace SE_Praktikum.Components.Sprites.Weapons
{
    public class Laser : Bullet
    {
        private readonly Vector2 _spaceShipVelocity;
        private float _elapsedTime = 0;

        public Laser(AnimationHandler animationHandler,Vector2 positionSpaceship,float rotation, Particle explosion, Sprite parent) : base(animationHandler, explosion)
        {
            Rotation = rotation;
            Parent = parent;
            Position = positionSpaceship;
            Velocity = 5;
            Acceleration = 0;
            _spaceShipVelocity = Vector2.Zero;
            MaxTime = 5;
        }
        
        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime.Milliseconds / 1000f;
            Position = Movement(_spaceShipVelocity,_elapsedTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        protected override void InvokeOnCollide()
        {
            base.InvokeOnCollide();
        }
    }
}