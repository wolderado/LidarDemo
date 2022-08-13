using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PointManager : MonoBehaviour
{
    [Header( "Mesh Data" )]
    public float MinPointScale = 0.025F;
    public float MaxPointScale = 0.025F;
    public int BatchSize = 1000;
    public Mesh PointMesh;
    public Material PointMaterial;
    public bool CullBatchPerm = true;
    public float CullDotThreshold = 0.3F;
    public float CullDistanceThreshold = 10F;

    [Header( "Runtime" )]
    public int TotalPointCount;

    
    public List<List<Matrix4x4>> batches;
    private List<MaterialPropertyBlock> pointMaterialProps;
    private List<Vector4> pointColors;
    private int currentBatchIndex = 0;
    private int batchCount = 0;
    private Camera cam;
    private int batchColorTimer = 0;
    
    public static PointManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        batches = new List<List<Matrix4x4>>();
        batches.Add( new List<Matrix4x4>() );
        pointMaterialProps = new List<MaterialPropertyBlock>();
        CreateNewMaterialBlock();
        
        
        cam = Camera.main;
    }

    void Update()
    {
        //Render points every visual frame
        //If we don't render it every frame, then it will flicker
        RenderBatches();
    }

    void RenderBatches()
    {
        //Simple frustum culling to increase performance
        List<int> culledBatchIndexes = CullBatches();

        List<Matrix4x4> batch;
        for( int i = 0; i < batches.Count; i++ )
        {
            if( culledBatchIndexes.Contains( i ) == false )
            {
                if( batches[ i ] == null || batches[ i ].Count == 0 )
                    break;
                
                batch = batches[ i ];

                //Draw the batch
                Graphics.DrawMeshInstanced( PointMesh, 0, PointMaterial, batch, pointMaterialProps[i] );
            }
        }

        batchColorTimer += 1;
        if( batchColorTimer > 999 )
            batchColorTimer = 0;
    }

    List<int> CullBatches()
    {
        if( CullBatchPerm == false )
            return new List<int>();
        
        //This cull method is super simple but not very accurate
        //But it beats having to go through thousands of points every frame
        
        List<int> culledList = new List<int>();
        for( int i = 0; i < batches.Count; i++ )
        {
            bool isCulled = false;

            if( batches[ i ] != null && batches[ i ].Count > 0 )
            {
                isCulled = true;

                //Don't cull the current unfilled batch
                if( i == currentBatchIndex )
                {
                    isCulled = false;
                    break;
                }
                
                //Sample 50 points from the batch
                //Higher this number means higher accuracy (less flickering) at the cost of performance
                for( int j = 0; j < 50; j++ )
                {
                    int samplePointIndex = j * 20;

                    //Don't cull the current batch
                    Matrix4x4 point = batches[ i ][ samplePointIndex ];
                    Vector3 pointPosition = new Vector3( point.m03, point.m13, point.m23 );

                    //If point is roughly inside the camera 
                    Vector3 dirToCam = cam.transform.position - pointPosition;
                    if( Vector3.Dot( dirToCam, cam.transform.forward ) < CullDotThreshold && dirToCam.sqrMagnitude < CullDistanceThreshold )
                    {
                        //If any of the points are inside the screen and if its close to camera,
                        //then don't cull this batch
                        isCulled = false;
                        break;
                    }
                }
            }

            if( isCulled )
            {
                //if its culled, add the index to culled batches list
                culledList.Add( i );
            }
        }

        return culledList;
    }
    
    

    public void AddNewPoint( Vector3 pos,Quaternion rot,ColoredSurfaceParam customSurfaceHit = null,float customScale = -1F )
    {
        if( batchCount == BatchSize )
        {
            //Create a new batch if the current batch is filled
            //DrawMeshInstanced only supports up to 1024 mesh per batch 
            batches.Add( new List<Matrix4x4>() );
            CreateNewMaterialBlock();
            
            currentBatchIndex++;
            batchCount = 0;
        }

        //Randomize scale if a custom scale is not defined 
        float scale = customScale;
        if(customScale < 0)
            scale = Random.Range( MinPointScale, MaxPointScale );

        //Create a new matrix and add it to the current batch
        batches[currentBatchIndex].Add( Matrix4x4.TRS( pos, rot, Vector3.one * scale ) );
        
        
        //If its a colored surface, get the color
        if( customSurfaceHit != null )
        {
            pointColors[batchCount] = customSurfaceHit.SurfaceColor;
        }
        
        //Updating material property array
        pointMaterialProps[currentBatchIndex].SetVectorArray( "_Color", pointColors );
        
        
        TotalPointCount++;
        batchCount++;
    }


    void CreateNewMaterialBlock()
    {
        pointMaterialProps.Add( new MaterialPropertyBlock() );
        
        //Fill it with dump data cause material property vector array size can't be changed one it has been set
        //This variable is a temp color storage that resets after each batch
        pointColors = new List<Vector4>();
        for( int i = 0; i < BatchSize; i++ )
            pointColors.Add( Vector4.one );
    }
}
