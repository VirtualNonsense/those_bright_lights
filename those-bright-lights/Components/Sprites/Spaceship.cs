using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SE_Praktikum.Services;
using SE_Praktikum.Components.Sprites.Weapons;
using System;
using Microsoft.Xna.Framework.Input;
using NLog;
using SE_Praktikum.Models;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SE_Praktikum.Components.Sprites.Bullets;
using SE_Praktikum.Extensions;
using SE_Praktikum.Components.Sprites.PowerUps;

namespace SE_Praktikum.Components.Sprites
{
    public abstract class Spaceship : Actor
    {
        protected List<Weapon> Weapons;
        protected int CurrentWeapon;
        protected float Speed;
        private Logger _logger;
        protected KeyboardState CurrentKey;
        protected KeyboardState PreviousKey;
        protected Polygon _impactPolygon;
        protected AnimationHandler Propulsion;
        protected bool FlippedHorizontal => _animationHandler.SpriteEffects == SpriteEffects.FlipVertically;
        


        #region Events
        public event EventHandler OnShoot;
        public event EventHandler OnDie;
        public event EventHandler OnPickUpWeapon;
        public event EventHandler OnTakeDamage;

        #endregion

        public override float Rotation
        {
            get => base.Rotation;
            set
            {
                base.Rotation = value;
                if (Rotation < 3 * Math.PI/2 && Rotation > Math.PI/2)
                {
                    if (FlippedHorizontal) return;
                    _animationHandler.SpriteEffects = SpriteEffects.FlipVertically;
                    if (Propulsion == null) return;
                    Propulsion.SpriteEffects = SpriteEffects.FlipVertically;
                }
                else if (FlippedHorizontal)
                {
                    _animationHandler.SpriteEffects = SpriteEffects.None;
                    if (Propulsion == null) return;
                    Propulsion.SpriteEffects = SpriteEffects.None;
                }

            }
        }


        public Spaceship(AnimationHandler animationHandler, float speed = 3, float health = 100, SoundEffect impactSound = null) : base(
            animationHandler, impactSound)
        {
            Speed = speed;
            Health = health;
            Weapons = new List<Weapon>();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var weapon in Weapons)
            {
                weapon.Update(gameTime);
            }

            if (Propulsion != null)
            {
                Propulsion.Position = Position;
                Propulsion.Rotation = Rotation;
                Propulsion.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Propulsion?.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }


        protected virtual void InvokeOnTakeDamage(float damage)
        {
            OnTakeDamage?.Invoke(this,EventArgs.Empty);
        }

        protected virtual void InvokeOnDie()
        {
            OnDie?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void InvokeOnShoot(Bullet b)
        {
            if (b is null)
                return;
            var e = new LevelEvent.ShootBullet {Bullet = b};
            b.Layer = Layer;
            OnShoot?.Invoke(this,e);
        }

        public virtual void AddWeapon(Weapon weapon)
        {
            weapon.Parent = this;
            Weapons.Add(weapon);
            CurrentWeapon = Weapons.Count - 1;
            weapon.OnEmitBullet += EmitBulletToOnShot;
            switch (weapon)
            {
                case SingleShotWeapon ssw:
                    ssw.OnClipEmpty += (sender, args) => ReloadWeapon((ClipWeapon)sender);
                    ssw.OnWeaponEmpty += (sender, args) => RemoveWeapon((Weapon)sender);
                    ssw.OnReloadProgressUpdate += (sender, args) => _logger.Debug(args.Progress * 100);
                    ssw.OnWeaponEmpty += (sender, args) => _logger.Debug("weapon empty!");
                    break;
            }
            
            OnPickUpWeapon?.Invoke(this, EventArgs.Empty);
        }

        public virtual void RemoveWeapon(Weapon weapon)
        {
            if (!Weapons.Contains(weapon)) return;
            weapon.OnEmitBullet -= EmitBulletToOnShot;
            Weapons.Remove(weapon);
            if (CurrentWeapon >= Weapons.Count)
                CurrentWeapon = Weapons.Count - 1;
        }

        private void EmitBulletToOnShot(object sender, Weapon.EmitBulletEventArgs args)
        {
            InvokeOnShoot(args.Bullet);
        }

        private void ReloadWeapon(ClipWeapon w)
        {
            _logger.Debug(w);
            w.Reload();
        }

        protected override bool Collide(Actor other)
        {
            if ( this == other || 
                 Math.Abs(Layer - other.Layer) > float.Epsilon || 
                 !(CollisionEnabled && IsCollideAble && other.CollisionEnabled && other.IsCollideAble)) 
                return false;
            foreach (var polygon in HitBox)
            {
                _impactPolygon = polygon;
                foreach (var polygon1 in other.HitBox)
                {
                    if(polygon.Overlap(polygon1)) return true;
                }
            }
            _impactPolygon = null;
            return false;
        }

        protected override void ExecuteInteraction(Actor other)
        {
            switch (other)
            {
                case Bullet b:
                    // bullet shouldn't damage it's parent
                    if (this == b.Parent) return;
                    Health -= b.Damage;
                    _impactSound?.Play();
                    break;
                case Tile t :
                    var v = _impactPolygon.Position - t.Position;
                    v /= v.Length();
                    Position += v;
                    break;
                case PowerUp p:
                    ProcessPowerUp(p);
                    break;

            }

            _impactPolygon = null;
        }
        private void ProcessPowerUp(PowerUp powerup)
        {
            switch(powerup)
            {
                case HealthPowerUp h:
                    Health += h.HealthBonus;
                    break;
                case InstaDeathPowerUp i:
                    Health -= Health;
                    break;
                
                case AmmoPowerUp ra:
                    //Weapons[CurrentWeapon].Ammo += ra.AmmoBonus;
                    break;
                
                case WeaponPowerUp r:
                    AddWeapon(r.Weapon);
                    break;
                
                case ScoreBonusPowerUp sb:
                    //score+= sb.bonusScore;
                    break;
                
                    
            }
        }
    }
}