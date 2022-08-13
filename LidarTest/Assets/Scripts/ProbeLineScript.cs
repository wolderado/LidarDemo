using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProbeLineScript : IPoolable
{
	public Color BrightLineColor;
	public Color DarkLineColor;
	public float Duration = 0.1F;
	public LineRenderer line;

	private float timer = 0;
	private float lineOpacity = 1;
	private float totalLineWidthMultiplier = 1F;
	
	public override void Init( Vector3 hitPos, Vector3 startPos, float damage )
	{
		
		line.SetPosition( 0,transform.InverseTransformPoint(startPos) );
		line.SetPosition( 1,transform.InverseTransformPoint(hitPos) );

		timer = 0;

		/*lineOpacity = Random.Range( 0.1F,1F );
		var color = line.startColor;
		color.a = lineOpacity;
		line.startColor = color;
		line.endColor = color;*/
		Color color = Color.Lerp( DarkLineColor, BrightLineColor, Random.value );
		color.a = Random.value * 0.5F;
		line.startColor = line.endColor = color;
		totalLineWidthMultiplier = Random.Range( 0.2F, 1F );
		
		//Debug.Log( $"C={color}" );
	}


	private void Update()
	{
		timer += Time.deltaTime;
		line.widthMultiplier = (1F - (timer / Duration)) * totalLineWidthMultiplier;
		
		if( timer > Duration )
		{
			LineCreator.instance.DisposeProjectile( this );
		}
	}
}
