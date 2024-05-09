using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    public bool isExistance { get; private set; }

    public SaveLoadManager() { }
    ~SaveLoadManager() { }
}
