using UnityEngine;

// This is a ScriptableObject that holds a list of waypoints for a ghost to patrol
// It is used by the GhostScatter behavior
// It is created by right clicking in the project view and selecting Create -> Patrol Path (Once this script is in the project folder)
// It is then assigned to the GhostScatter component in the inspector

[CreateAssetMenu(fileName = "NewPatrolPath", menuName = "Patrol Path")]
public class PatrolPath : ScriptableObject
{
    public Vector2[] waypoints;
}