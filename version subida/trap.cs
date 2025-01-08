using System;
using System.Collections.Generic;
using System.Timers;
using Spectre.Console;
using Timer = System.Timers.Timer;



class Trap 
{
 public (int x, int y) Position { get; set; }
 public int TurnsInactive { get; set; } 
 public Trap(int x, int y)
    { 
    Position = (x, y);
    TurnsInactive = 0; 
      } 
  }


