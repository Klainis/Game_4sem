//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class WaveManager : MonoBehaviour
//{
//    NavMeshAgent agent;

//    [SerializeField] private Transform enemyies;
//    [SerializeField] private GameObject units;
//    void Start()
//    {
//        //agent = GetComponent<NavMeshAgent>();
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        CreateWave();
        
//    }

//    private void CreateWave()
//    {
//        foreach (var enemy in enemyies)
//        {
//            Instantiate(enemy, enemy.position);
            
//            agent = enemy.GetComponent<NavMeshAgent>();

//            agent.SetDestination(units.transform.position);

//        }
//    }
//}
