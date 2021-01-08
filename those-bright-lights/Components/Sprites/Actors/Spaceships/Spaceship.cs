using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using SE_Praktikum.Components.Sprites.Actors.Bullets;
using SE_Praktikum.Components.Sprites.Actors.PowerUps;
using SE_Praktikum.Components.Sprites.Actors.Weapons;
using SE_Praktikum.Models;
using SE_Praktikum.Services;

namespace SE_Praktikum.Components.Sprites.Actors.Spaceships
{
    public abstract class Spaceship : Actor
    {
        protected List<Weapon> Weapons;
        protected int _currentWeapon;
        protected float Speed;
        private Logger _logger;
        protected Polygon _impactPolygon;
        protected AnimationHandler Propulsion;
        // #############################################################################################################
        // Constructor
        // #############################################################################################################
        public Spaceship(AnimationHandler animationHandler, float speed = 3, float health = 100, SoundEffect impactSound = null) : base(
            animationHandler, impactSound)
        {
            Speed = speed;
            Health = health;
            Weapons = new List<Weapon>();
            _logger = LogManager.GetCurrentClassLogger();
        }
        
        // #############################################################################################################
        // Events
        // #############################################################################################################
        #region Events
        public event EventHandler OnShoot;
        public event EventHandler OnTakeDamage;
        public event EventHandler OnPositionChanged;
        public event EventHandler OnWeaponChanged;

        #endregion
        
        // #############################################################################################################
        // Properties
        // #############################################################################################################
        protected bool FlippedHorizontal => _animationHandler.SpriteEffects == SpriteEffects.FlipVertically;

        public int CurrentWeapon 
        {
            get => _currentWeapon;
            set
            {
                if (value < 0)
                {
                    _currentWeapon = 0;
                    InvokeOnWeaponChanged();
                    return;
                }

                if (value >= Weapons.Count)
                {
                    _currentWeapon = Weapons.Count - 1;
                    InvokeOnWeaponChanged();
                    return;
                }
                _currentWeapon = value;
                InvokeOnWeaponChanged();
            }
        }
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

        public override Vector2 Position 
        { 
            get => base.Position;
            set
            {
                base.Position = value;
                InvokeOnPositionChanged();
            } 
        }

        // #############################################################################################################
        // public Methods
        // #############################################################################################################
        public override void Update(GameTime gameTime)
        {
            foreach (var weapon in Weapons)
            {
                weapon.Update(gameTime);
            }

            for (var i = 0; i < Weapons.Count;)
            {
                var w = Weapons[i];
                if (!w.IsRemoveAble)
                {
                    i++;
                    continue;
                }
                Weapons.RemoveAt(i);
                CurrentWeapon = Weapons.Count;
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

        public virtual void AddWeapon(Weapon weapon)
        {
            weapon.Parent = this;
            Weapons.Add(weapon);
            CurrentWeapon = Weapons.Count;
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
        }

        public virtual void RemoveWeapon(Weapon weapon)
        {
            if (!Weapons.Contains(weapon)) return;
            weapon.OnEmitBullet -= EmitBulletToOnShot;
            // Weapons.Remove(weapon);
            weapon.IsRemoveAble = true;
        }

        // #############################################################################################################
        // protected / private Methods
        // #############################################################################################################
        protected virtual void ShootCurrentWeapon()
        {
            if (Weapons.Count == 0) return;
            Weapons[CurrentWeapon].Fire();
        }

        protected virtual void InvokeOnTakeDamage(float damage)
        {
            OnTakeDamage?.Invoke(this,EventArgs.Empty);
        }

        protected virtual void InvokeOnWeaponChanged()
        {
            OnWeaponChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void InvokeOnPositionChanged()
        {
            OnPositionChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void InvokeOnShoot(Bullet b)
        {
            if (b is null)
                return;
            var e = new LevelEvent.ShootBullet {Bullet = b};
            b.Layer = Layer;
            OnShoot?.Invoke(this,e);
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