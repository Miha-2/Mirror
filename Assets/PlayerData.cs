using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class PlayerData
{
        //Player Data on server only
        public string pName;
        public float pHue;
        public bool team;
        public PlayerStats pStats = new PlayerStats();
}
