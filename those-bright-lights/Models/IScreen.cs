﻿using SE_Praktikum.Core;

namespace SE_Praktikum.Models
{
    public interface IScreen
    {
        int ScreenHeight { get; }
        int ScreenWidth { get; }
        
        Camera Camera { get; }
    }
}