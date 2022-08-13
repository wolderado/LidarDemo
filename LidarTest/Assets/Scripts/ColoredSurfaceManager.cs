using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredSurfaceManager : MonoBehaviour
{
    public Dictionary<Collider, ColoredSurfaceParam> AllSurfaces;


    public static ColoredSurfaceManager instance;

    void Awake()
    {
        AllSurfaces = new Dictionary<Collider, ColoredSurfaceParam>();
        instance = this;
    }
    

    public void RegisterSurface( Collider targetCollider, ColoredSurfaceParam surfaceParam )
    {
        AllSurfaces.Add( targetCollider,surfaceParam );
    }
    
    public ColoredSurfaceParam GetColor( Collider hitCollider)
    {
        if( AllSurfaces.ContainsKey( hitCollider ) == false )
        {
            Debug.LogError( $"Surface {hitCollider.name} is not registered!" );
        }
        
        return AllSurfaces[hitCollider];
    }
}
