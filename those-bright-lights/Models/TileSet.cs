﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using System;

namespace SE_Praktikum.Models
{
    public class TileSet
    {
        public int TileDimX;
        public int TileDimY;
        public int Columns;
        public int Rows;
        public Texture2D Texture;
        public int Tiles => Columns * Rows;
        public int StartEntry;
        private ILogger _logger;
        public int FrameCount => Columns * Rows;
        public int FrameWidth => Rows * TileDimX;
        public int FrameHeight => Columns * TileDimY;

        
        public TileSet(Texture2D texture, int tileDimX, int tileDimY, int startEntry)
        {
            _logger = LogManager.GetCurrentClassLogger();
            Texture = texture;
            TileDimX = tileDimX;
            TileDimY = tileDimY;
            Columns = Texture.Width / TileDimX;
            Rows = Texture.Height / TileDimY;
            StartEntry = startEntry;
        }

        internal Rectangle GetFrame(uint index)
        {
            

            var c = 0;
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (index == StartEntry + c)
                        return new Rectangle(TileDimX * column, TileDimY * row, TileDimX, TileDimY);
                    c++;
                }
            }
            _logger.Warn($"index{index} not found");
            return Rectangle.Empty;
        }
    }
}