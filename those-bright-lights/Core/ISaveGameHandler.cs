﻿using SE_Praktikum.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SE_Praktikum.Core
{
    // Enum for determining which slot to use
    public enum SaveSlot
    {
        Slot1,
        Slot2,
        Slot3
    }

    // Interface with all the desired components
    public interface ISaveGameHandler
    {
        void Save();
        void Load();

        SaveSlot SaveSlot { get; set; }

        bool SaveExists(SaveSlot saveSlot);

        void CreateSave();

        SaveGame SaveGame { get; set; }
    }
}
