using UnityEngine;

public interface IAgent
{
    bool SetGoal(Vector3 terminalGoal);
    void Initialise(int id);
}
