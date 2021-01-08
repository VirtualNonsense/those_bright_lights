﻿using SE_Praktikum.Components.HUD;
using SE_Praktikum.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SE_Praktikum.Services.Factories
{
    public class HUDItemFactory
    {
        private readonly TileSetFactory tileSetFactory;
        private readonly AnimationHandlerFactory animationHandlerFactory;

        public HUDItemFactory(TileSetFactory tileSetFactory, AnimationHandlerFactory animationHandlerFactory)
        {
            this.tileSetFactory = tileSetFactory;
            this.animationHandlerFactory = animationHandlerFactory;
        }

        public HUDItem GetLifeBar(HUD hUD)
        {
            var tileSet = tileSetFactory.GetInstance(@".\Content\MetaData\TileSets\heart_11_17.json",0);
            var animationSettingsLeftHeart = new AnimationSettings(new List<(int, float)> { 
                (0,0),
                (2,0)
            }, isPlaying : false);

            var animationSettingsRightHeart = new AnimationSettings(new List<(int, float)> {
                (1,0),
                (3,0)
            }, isPlaying: false);
            return new LifeBar(hUD,animationHandlerFactory,tileSet,animationSettingsLeftHeart,animationSettingsRightHeart);
        }

        public HUDItem GetScoreBar(HUD hUD)
        {
            var tileSet = tileSetFactory.GetInstance(@".\Content\MetaData\TileSets\numbers_8_12.json", 0);
            var animationSettings = new AnimationSettings(new List<(int, float)> {
                (0,0),
                (1,0),
                (2,0),
                (3,0),
                (4,0),
                (5,0),
                (6,0),
                (7,0),
                (8,0),
                (9,0)
            }, isPlaying: false);

            return new ScoreBar(hUD, animationHandlerFactory, tileSet, animationSettings);
        }
    }
}
