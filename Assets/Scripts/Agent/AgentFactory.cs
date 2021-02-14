using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject AgentPrefab;


    public void SpawnAllAgents(GameObject environmentModel, int numberOfAgents) => SpawnAllAgents(numberOfAgents, environmentModel);


    public void SpawnAllAgents(int numberOfAgents, GameObject environmentModel, int tries = 50, float distance = 1f)
    {
        Bounds bounds = CalculateLocalBounds(environmentModel);

        for (int i = 0; i< numberOfAgents; i++)
        {
            Vector3 position = RandomPoint(bounds, environmentModel.transform, tries, distance);
            if (position != Vector3.positiveInfinity)
            {
                InstantateAgent(position);
            }
            else
            {
                Debug.LogError($"Could not spawn agent after {tries} tries");
            }
            
        }
        
    }

    private static Bounds CalculateLocalBounds(GameObject model)
    {
        Quaternion currentRotation = model.transform.rotation;
        model.transform.rotation = Quaternion.Euler(Vector3.zero);

        Bounds bounds = new Bounds(model.transform.position, Vector3.one);

        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 localCenter = bounds.center - model.transform.position;
        bounds.center = localCenter;

        model.transform.rotation = currentRotation;


        return bounds;
    }

    private GameObject InstantateAgent(Vector3 position)
    {
        GameObject agent = Instantiate(AgentPrefab, transform);
        agent.transform.GetComponent<NavMeshAgent>().Warp(position);
        return agent;
    }


    private static Vector3 RandomPoint(in Bounds bounds, Transform transform, int tries, float distance)
    {
        for (int i = 0; i < tries; i++)
        {
            Vector3 randomPoint =  new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            randomPoint = transform.TransformPoint(randomPoint);

            //randomPoint += Random.insideUnitSphere * distance;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, distance, NavMesh.AllAreas))
            {
                return hit.position + Vector3.up ;
            }

        }
        return Vector3.positiveInfinity;
    }

}
