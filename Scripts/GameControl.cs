using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IGameState 
{
  bool isPlay();
}
public class WaitingState:IGameState
{
    public bool isPlay()
    {
        return true;
    }
}
public class MovingState:IGameState
{
    public bool isPlay()
    {
        return false;
    }
}
