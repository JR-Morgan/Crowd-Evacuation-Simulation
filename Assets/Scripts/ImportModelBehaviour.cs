using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Obsolete]
public class ImportModelBehaviour : MonoBehaviour
{
    private const int BUILDING_LAYER = 6;


    //public void ImportUpdate(SpeckleUnityUpdate update)
    //{
    //    if(update.updateProgress >= 1f)
    //    {
    //        //Model finished importing
    //        OnImport(update.streamRoot.gameObject);
    //    }
    //}
    
    private void OnImport(GameObject model)
    {

        //model.layer = BUILDING_LAYER;
        firstUpdate = true;



    }

    private bool firstUpdate = false; 
    public void Update()
    {
        if(firstUpdate)
        {
            var navMeshSurface = GameObject.FindGameObjectWithTag("NavMeshManager").GetComponent<NavMeshSurface>();
            firstUpdate = false;
            //navMeshSurface.BuildNavMesh();

            //AgentFactory.Instance.SpawnAllAgents();
        }
    }
}
