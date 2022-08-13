using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPoolable : MonoBehaviour
{
	public int TypeID = 0;
	
	public abstract void Init(Vector3 hitPos,Vector3 startPos,float damage);
	
	public void Dispose()
	{
		LineCreator.instance.DisposeProjectile( this );
	}
}
