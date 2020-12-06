﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using SE_Praktikum.Components;
using SE_Praktikum.Models;
using SE_Praktikum.Services.Factories;
using SE_Praktikum.Services.StateMachines;

namespace SE_Praktikum.Core.GameStates
{
    public class SaveSelect : GameState
    {
        private readonly IScreen _screen;
        private readonly ControlElementFactory _factory;
        private ComponentGrid _components;
        private Logger _logger;

        public SaveSelect(IScreen screen, ControlElementFactory factory)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _screen = screen;
            _factory = factory;
        }
        
        public override void LoadContent()
        {
            if (!(_components is null))
                return;
            var center = new Vector2(0, 0);

            var width = _screen.Camera.GetPerspectiveScreenWidth();
            var height = _screen.Camera.GetPerspectiveScreenHeight();
            
            _components = new ComponentGrid(center, width, height, 1);
            var bWidth = width / 3;
            var bHeight = height / 3;


            var slots1 = _factory.GetButton(bWidth, bHeight, Vector2.Zero, "Slot 1", _screen.Camera);
            slots1.Click += (sender, args) => { _logger.Trace("slot one selected"); _subject.OnNext(GameStateMachine.GameStateMachineTrigger.SaveSlotSelected); };
            var slots2 = _factory.GetButton(bWidth, bHeight, Vector2.Zero, "Slot 2", _screen.Camera);
            slots2.Click += (sender, args) => { _logger.Trace("slot one selected"); _subject.OnNext(GameStateMachine.GameStateMachineTrigger.SaveSlotSelected); };
            var slots3 = _factory.GetButton(bWidth, bHeight, Vector2.Zero, "Slot 3", _screen.Camera);
            slots3.Click += (sender, args) => { _logger.Trace("slot one selected"); _subject.OnNext(GameStateMachine.GameStateMachineTrigger.SaveSlotSelected); };

            _components.Add(slots1);
            _components.Add(slots2);
            _components.Add(slots3);

        }

        public override void UnloadContent()
        {
            _components = null;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
            {
                component.Update(gameTime);
            }
        }

        public override void PostUpdate(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack,
                null,
                SamplerState.PointClamp, // Sharp Pixel rendering
                null,
                RasterizerState.CullCounterClockwise, // Render only the texture side that faces the camara to boost performance 
                _screen.Camera.GetCameraEffect());
            foreach (var component in _components)
            {
                component.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
        }
    }
}