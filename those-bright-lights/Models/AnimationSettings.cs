﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SE_Praktikum.Models
{
    public class AnimationSettings
    {
        public bool IsLooping;
        
        /// <summary>
        /// Determines how long it takes to update the frame. [ms] 
        /// </summary>
        public List<(int,float)> UpdateList;

        public float Layer;

        public SpriteEffects SpriteEffects;

        public float Rotation;

        public float Scale;

        public Color Color;

        public float Opacity;

        public bool IsPlaying;

        
        public AnimationSettings(List<(int, float)> updateList = null, float layer = 1, bool isLooping = false,  Color? color = null, 
                                 float rotation = 0f, float scale = 1f, float opacity = 1, bool isPlaying = true)
        {
            Layer = layer;
            IsLooping = isLooping;
            UpdateList = updateList ?? new List<(int,float)> {(0,100)};
            SpriteEffects = SpriteEffects.None;
            Color = color??Color.White;
            Rotation = rotation;
            Scale = scale;
            Opacity = opacity;
            IsPlaying = isPlaying;
        }
        
        public AnimationSettings(int frames = 1, float duration = 100f, float layer = 1, bool isLooping = false,  Color? color = null, 
                                 float rotation = 0f, float scale = 1f, float opacity = 1, bool isPlaying = true)
        {
            Layer = layer;
            IsLooping = isLooping;
            UpdateList = new List<(int, float)>();
            for (var i = 0; i < frames; i++)
            {
                UpdateList.Add((i,duration));
            }     
            SpriteEffects = SpriteEffects.None;
            Color = color??Color.White;
            Rotation = rotation;
            Scale = scale;
            Opacity = opacity;
            IsPlaying = isPlaying;
        }
    }
}