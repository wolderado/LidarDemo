using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreator : MonoBehaviour
{
    public Dictionary<int, Queue<IPoolable>> AllPools;

    public static LineCreator instance;
    
    public IPoolable ScanLinePrefab;
    public LidarGun Gun;
    
    
    private void Awake()
    {
        instance = this;
        AllPools = new Dictionary<int, Queue<IPoolable>>();
    }

    void Start()
    {
        Gun.OnProbePointCreated += CreateLineForProbe;
    }

    private void OnDisable()
    {
        Gun.OnProbePointCreated -= CreateLineForProbe;
    }
    
    void Update()
    {
        
    }

    public void CreateLineForProbe( Vector3 pos, Quaternion rot )
    {
        CreateProjectile( ScanLinePrefab, transform.position, pos, 0 );
    }

    public void CreateProjectile(IPoolable projectilePrefab,Vector3 startPos,Vector3 endPos,float damage)
    {
        IPoolable newProjectile;
        if( AllPools.ContainsKey(projectilePrefab.TypeID) && AllPools[ projectilePrefab.TypeID ] != null && AllPools[ projectilePrefab.TypeID ].Count > 0) 
        {
            newProjectile = AllPools[ projectilePrefab.TypeID ].Dequeue();
        }
        else
        {
            newProjectile = Instantiate( projectilePrefab,transform );
        }
        
        newProjectile.gameObject.SetActive( true );
        newProjectile.Init(endPos,startPos,damage);
    }


    public void DisposeProjectile( IPoolable projectile )
    {
        if(AllPools.ContainsKey(projectile.TypeID) == false || AllPools[projectile.TypeID] == null)
            AllPools[projectile.TypeID] = new Queue<IPoolable>();
        
        
        projectile.gameObject.SetActive( false );
        AllPools[ projectile.TypeID ].Enqueue( projectile );
    }
}
