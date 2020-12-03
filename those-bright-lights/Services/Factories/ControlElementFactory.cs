﻿using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using SE_Praktikum.Components.Controls;
using SE_Praktikum.Core;
using SE_Praktikum.Models;

namespace SE_Praktikum.Services.Factories
{
    public class ControlElementFactory
    {
        private readonly AnimationHandlerFactory _animationHandlerFactory;
        private readonly ContentManager _contentManager;
        private TileSet _buttonsAndSwitches;
        private SpriteFont _font;
        private const int _tileWidth = 32;
        private const int _tileHeight = 32;
        private Logger _logger;

        public ControlElementFactory(AnimationHandlerFactory animationHandlerFactory, ContentManager contentManager)
        {
            _animationHandlerFactory = animationHandlerFactory;
            _contentManager = contentManager;
            _logger = LogManager.GetCurrentClassLogger();
        }

        private void loadAssetsIfNecessary()
        {
            _buttonsAndSwitches ??= new TileSet(_contentManager.Load<Texture2D>("Artwork/Tilemaps/ButtonsAndSwitches"), _tileWidth,
                _tileHeight);
            _font ??= _contentManager.Load<SpriteFont>("Font/Font2");
        }

        public MenuButton GetButton(uint width, uint height, Vector2 position,
            string text = "", Camera camera = null)
        {
            loadAssetsIfNecessary();
            uint tilesX = (uint) (width / _buttonsAndSwitches.TileDimX);
            uint tilesY = (uint) (height / _buttonsAndSwitches.TileDimY);
            return GetButtonByTiles(tilesX, tilesY, position, text, camera);
        }
        public MenuButton GetButtonByTiles(uint tilesX, uint tilesY, Vector2 position, string text = "",  Camera camera = null)
        {
            List<AnimationHandler> handlers = new List<AnimationHandler>();
            loadAssetsIfNecessary();

            for (var y = 0; y < tilesY; y++)
            {
                for (var x = 0; x < tilesX; x++)
                {
                    AnimationSettings animationSettings;
                    // Top left
                    if (x == 0 && y == 0)
                    {
                        if (y == tilesY - 1 && y == tilesY - 1)
                        {
                            _logger.Trace($"Single tile");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (6, 1f), // normal
                                (7, 1f), // clicked
                            });
                        }
                        // only one line
                        else if (y == tilesY - 1)
                        {
                            _logger.Trace($"column {x} top left, only one line");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (0, 1f), // normal
                                (3, 1f), // clicked
                            });
                        }
                        // only one column
                        else if (x == tilesX - 1)
                        {
                            _logger.Trace($"row {y} top left, only one column");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (14, 1f), // normal
                                (15, 1f), // clicked
                            });
                        }
                        else
                        {
                            _logger.Trace($"pos {x} {y} top left");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (8, 1f),
                                (11, 1f)
                            });
                        }
                    }
                    // Top right
                    else if (x == tilesX - 1 && y == 0)
                    {
                        // only one line
                        if (y == tilesY - 1)
                        {
                            _logger.Trace($"column {x} top right, only one line");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (2, 1f),
                                (5, 1f),
                            });
                        }
                        else
                        {
                            _logger.Trace($"pos {x} {y} top right");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (10, 1f),
                                (13, 1f)
                            });
                        }
                    }
                    // Bottom left
                    else if (x == 0 && y == tilesY - 1)
                    {
                        // only one column
                        if (x == tilesX - 1)
                        {
                            _logger.Trace($"row {y} bottom left, only one column");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (30, 1f), // normal
                                (23, 1f), // clicked
                            });
                        }
                        else
                        {
                            _logger.Trace($"pos {x} {y} bottom left");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (24, 1f),
                                (19, 1f)
                            });
                        }
                    }
                    // Bottom right
                    else if (x == tilesX - 1 && y == tilesY - 1)
                    {
                        _logger.Trace($"pos {x} {y} bottom right");
                        animationSettings = new AnimationSettings(new List<(int, float)>
                        {
                            (26, 1f),
                            (21, 1f)
                        });
                    }
                    // Top
                    else if(y == 0)
                    {
                        // only one line
                        if (y == tilesY - 1)
                        {
                            _logger.Trace($"column {x} , only one line");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (1, 1f),
                                (4, 1f),
                            });
                        }
                        else
                        {
                            _logger.Trace($"pos {x} {y} top");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (9, 1f),
                                (12, 1f)
                            });
                        }
                    }
                    // Bottom
                    else if(y == tilesY - 1)
                    {
                        _logger.Trace($"pos {x} {y} bottom");
                        animationSettings = new AnimationSettings(new List<(int, float)>
                        {
                            (25, 1f),
                            (20, 1f)
                        });
                    }
                    // Left
                    else if (x == 0)
                    {
                        // only one column
                        if (x == tilesX - 1)
                        {
                            _logger.Trace($"row {y} bottom left, only one column");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (22, 1f), // normal
                            });
                        }
                        else
                        {
                            _logger.Trace($"pos {x} {y} left");
                            animationSettings = new AnimationSettings(new List<(int, float)>
                            {
                                (16, 1f),
                            });
                        }
                        
                    }
                    // Right
                    else if (x == tilesX - 1)
                    {
                        _logger.Trace($"pos {x} {y} right");
                        animationSettings = new AnimationSettings(new List<(int, float)>
                        {
                            (18, 1f),
                        });
                    }
                    // Middle tile
                    else
                    {
                        _logger.Trace($"pos {x} {y}");
                        animationSettings = new AnimationSettings(new List<(int, float)>
                        {
                            (17, 1f)
                        });
                    }
                    
                    animationSettings.IsPlaying = false;
                    // should be < 0 to avoid problem with text rendering
                    animationSettings.Layer = -.10f;
                    var handler = _animationHandlerFactory.GetAnimationHandler(
                        _buttonsAndSwitches, 
                        animationSettings,
                        new Vector2(x * _tileWidth, y * _tileWidth),
                        Vector2.Zero
                    );
                    handlers.Add(handler);
                }
            }
            return new MenuButton(handlers, _font, text: text, position: position, camera: camera, textOffSetWhenPressed: 3);;
        }

        public Slider GetSlider(ContentManager contentManager,
            float initialValue,
            float min,
            float max,
            Vector2 position,
            uint width,
            Camera camera = null, float layer = 0)
        {
            
            loadAssetsIfNecessary();
            uint tilesX = (uint) (width / _buttonsAndSwitches.TileDimX);
            return GetSliderByTiles(contentManager,initialValue, min, max, position, tilesX, camera);
        }
        public Slider GetSliderByTiles(ContentManager contentManager,
            float initialValue,
            float min,
            float max,
            Vector2 position,
            uint tilesX,
            Camera camera = null, 
            float layer = 0)
        {
            List<AnimationHandler> handlers = new List<AnimationHandler>();
            loadAssetsIfNecessary();

            for (var x = 0; x < tilesX; x++)
            {
                AnimationSettings animationSettings;
                if (x == 0)
                {
                    animationSettings = new AnimationSettings(new List<(int, float)>
                    {
                        (27,1f)
                    });
                }
                else if (x == tilesX - 1)
                {
                    animationSettings = new AnimationSettings(new List<(int, float)>
                    {
                        (29,1f)
                    });
                }
                else
                {
                    animationSettings = new AnimationSettings(new List<(int, float)>
                    {
                        (28,1f)
                    });
                }

                animationSettings.IsPlaying = false;
                animationSettings.Layer = layer;
                var handler = _animationHandlerFactory.GetAnimationHandler(
                    _buttonsAndSwitches, 
                    animationSettings,
                    new Vector2(x * _tileWidth, 0),
                    Vector2.Zero
                );
                handlers.Add(handler);
            }

            var sliderSettings = new AnimationSettings(new List<(int, float)>
            {
                (31, 1f), // when still
                (39, 1f) // when dragged
            }) 
            {
                IsPlaying = false,
                Layer =  layer + float.Epsilon // forcing render order
            };
            var _sliderBladeHandler = new List<AnimationHandler>
            {
                _animationHandlerFactory.GetAnimationHandler(
                    _buttonsAndSwitches, 
                    sliderSettings,
                    Vector2.Zero,
                    Vector2.Zero
                )
            };
            return new Slider(initialValue, min, max,  position, new Slider.SliderBlade(_sliderBladeHandler, camera), handlers, camera);
        }
    }
    
    
    
}