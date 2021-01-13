﻿using Microsoft.Xna.Framework;
using NLog;
using SE_Praktikum.Components.Controls;
using SE_Praktikum.Models;
using System.IO;
using System.Linq;
using SE_Praktikum.Components;
using SE_Praktikum.Services.Factories;
using SE_Praktikum.Services.StateMachines;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace SE_Praktikum.Core.GameStates
{
    public class LevelSelect : GameState, ILevelContainer
    {
        private readonly IGameEngine _engine;
        private readonly IScreen _screen;
        private readonly ControlElementFactory _factory;
        private readonly ISaveGameHandler _saveGameHandler;
        private readonly LevelFactory _levelFactory;
        private readonly ContentManager contentManager;
        private ComponentGrid _buttons;
        private Logger _logger;
        private const string _levelPath = @".\Content\MetaData\Level\";
        private Dictionary<int,Song> _songSelection;

        public LevelSelect(IGameEngine engine, IScreen screen, ControlElementFactory factory, ISaveGameHandler saveGameHandler, LevelFactory levelFactory, ContentManager contentManager)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _engine = engine;
            _screen = screen;
            _factory = factory;
            _saveGameHandler = saveGameHandler;
            _levelFactory = levelFactory;
            this.contentManager = contentManager;
        }

        public override void Draw()
        {
            _engine.Render(_buttons);
        }

        public override void LoadContent()
        {
            if (_buttons != null) return;

            _songSelection = new Dictionary<int, Song>();
            _songSelection.Add(0, contentManager.Load<Song>("Audio/Music/Song3_remaster2_mp3"));
            _songSelection.Add(1, contentManager.Load<Song>("Audio/Music/Song2_remaster2_mp3"));
            _songSelection.Add(2, contentManager.Load<Song>("Audio/Music/Song4_remaster_mp3"));
                
            _screen.Camera.Position = new Vector3(0, 0,150);
            
            _logger.Debug("LoadingContent");
            _buttons = new ComponentGrid(new Vector2(0,0), 
                _screen.Camera.GetPerspectiveScreenWidth(),
                _screen.Camera.GetPerspectiveScreenHeight(),
                1);
            var level = Directory.GetFiles(_levelPath, "*.json");
            var buttons = level.Length + 1;
            uint width = (uint) (_screen.Camera.GetPerspectiveScreenWidth()/3);
            uint height = (uint) (_screen.Camera.GetPerspectiveScreenHeight() / buttons);
            int c = 0;
            foreach (var path in level)
            {
                var n = path.Split(".json")[0].Split("\\").Last();
                
                var button = _factory.GetButton(
                    width,
                    height,
                    new Vector2(0, 0),
                    n,
                    _screen.Camera);
                var l = _levelFactory.GetInstance(path, c, _songSelection.ContainsKey(c)? _songSelection[c] : null);
                //button.Enabled = _saveGameHandler.SaveGame.clearedStage >= c;
                button.Click += (sender, args) => 
                {
                    _logger.Debug($"starting {n}");
                    SelectedLevel = l;
                    _subject.OnNext(GameStateMachine.GameStateMachineTrigger.StartGame);
                };
                _buttons.Add(button);
                c++;
            }
            
            MenuButton b = _factory.GetButton(
                width,
                height,
                new Vector2(0, 0),
                "back to mainmenu",
                _screen.Camera);

            b.Click += (sender, args) => 
            {
                _logger.Debug("back to mainmenu");
                _subject.OnNext(GameStateMachine.GameStateMachineTrigger.Back);
            };
            _buttons.Add(b);
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void UnloadContent()
        {
            _buttons = null;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var button in _buttons)
            {
                button.Update(gameTime);
            }
        }

        public Level SelectedLevel { get; private set; }
    }
}
