using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredSurfaceParam : MonoBehaviour
{
	public Color SurfaceColor;
	public Collider SurfaceCollider;

	private void Start()
	{
		ColoredSurfaceManager.instance.RegisterSurface( SurfaceCollider,this );
	}
}
