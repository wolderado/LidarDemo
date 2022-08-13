using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LidarGun : MonoBehaviour
{
    [Header("General Parameters")]
    public LayerMask RaycastMask;
    public float MaxRayDistance = 10F;


    
    [Header("Stream Parameters")]
    public float StreamCreateNewPointTime = 2F;
    public int PointCountPerStream = 5;
    public float RaycastViewportRangeMin = 0.1F;
    public float RaycastViewportRangeMax = 0.3F;
    
    [Header("Scan Parameters")]
    public float ScanTimePerLine = 0.05F;
    public float ScanDensity = 0.01F;
    public float ScanRaycastViewportMinX = 0.3F;
    public float ScanRaycastViewportMaxX = 0.7F;
    public float ScanRaycastViewportMinY = 0.3F;
    public float ScanRaycastViewportMaxY = 0.7F;

    [Header( "Audio" )]
    public AudioSource StreamSource;
    public AudioSource ScanSource;
    public AudioClip ScanCompleteClip;

    
    [Header( "Runtime" )]
    public GunMode CurrentGunMode;

    private float streamTimer = 0;
    private PointManager pointManager;
    private Camera cam;
    private bool scanProbing = false;
    private float RaycastViewportRange = 0.3F;
    
    [HideInInspector]
    public bool scanFinished = false;


    public delegate void probePointDel( Vector3 pos, Quaternion rot );
    public probePointDel OnProbePointCreated;
    
    void Start()
    {
        pointManager = PointManager.instance;

        cam = Camera.main;
        RaycastViewportRange = RaycastViewportRangeMax;
    }
    
    void Update()
    {
        StreamMode();
        ScanMode();
    }

    void StreamMode()
    {
        if( CurrentGunMode != GunMode.Idle && CurrentGunMode != GunMode.Streaming)
            return;

        if( Input.mouseScrollDelta.y > 0.01F )
        {
            RaycastViewportRange += Time.deltaTime * 4F;
            RaycastViewportRange = RaycastViewportRange > RaycastViewportRangeMax ? RaycastViewportRangeMax : RaycastViewportRange;
        }
        
        if( Input.mouseScrollDelta.y < -0.01F )
        {
            RaycastViewportRange -= Time.deltaTime * 4F;
            RaycastViewportRange = RaycastViewportRange < RaycastViewportRangeMin ? RaycastViewportRangeMin : RaycastViewportRange;
        }
        
        if( Input.GetMouseButton( 0 ) )
        {
            CurrentGunMode = GunMode.Streaming;
            streamTimer += Time.deltaTime;

            if( streamTimer > StreamCreateNewPointTime )
            {
                streamTimer = 0;

                for( int i = 0; i < PointCountPerStream; i++ )
                {
                    Vector2 screenPos = new Vector2( 0.5F + Random.Range( -RaycastViewportRange, RaycastViewportRange ), 0.5F + Random.Range( -RaycastViewportRange, RaycastViewportRange ) );
                    ProbePoint( screenPos);
                }
            }
            
            if(StreamSource.isPlaying == false)
                StreamSource.Play();
        }
        else
        {
            streamTimer = 0;
            
            if(StreamSource.isPlaying )
                StreamSource.Stop();
        }
        
        
        if( Input.GetMouseButtonUp( 0 ) )
            CurrentGunMode = GunMode.Idle;
    }


    void ScanMode()
    {
        if( CurrentGunMode != GunMode.Idle && CurrentGunMode != GunMode.Scanning)
            return;

        if( Input.GetMouseButton( 1 ) && !scanProbing)
        {
            StartCoroutine( StartScanProbing() );
        }
    }

    IEnumerator StartScanProbing()
    {
        CurrentGunMode = GunMode.Scanning;
        
        scanProbing = true;
        scanFinished = false;

        ScanSource.time = 0;
        ScanSource.Play();
        
        bool stopScanning = false;
        for( float y = ScanRaycastViewportMinY; y < ScanRaycastViewportMaxY; y += ScanDensity )
        {
            for( float x = ScanRaycastViewportMinX; x < ScanRaycastViewportMaxX; x += ScanDensity )
            {
                ProbePoint( new Vector2( x + Random.Range(-0.005F,0.05F), y+ Random.Range(-0.005F,0.05F) ) );
                if( Input.GetMouseButton( 1 ) == false )
                {
                    stopScanning = true;
                    break;
                }
            }

            if( stopScanning )
                break;
            else
                yield return new WaitForSeconds( ScanTimePerLine );
        }
        
        if(ScanSource.isPlaying )
            ScanSource.Stop();
        
        ScanSource.PlayOneShot( ScanCompleteClip );

        scanFinished = true;
        
        if(stopScanning == false)
            yield return new WaitUntil( () => Input.GetMouseButtonUp( 1 ) );
        
        scanProbing = false;
        CurrentGunMode = GunMode.Idle;
    }


    void ProbePoint(Vector2 viewportPosition)
    {
        Vector3 camPosition = cam.ViewportToWorldPoint( new Vector3( viewportPosition.x, viewportPosition.y, cam.nearClipPlane ) );
        //Debug.Log( $"Raycast! " + camPosition );

        RaycastHit hit;

        Vector3 rayDir = camPosition - cam.transform.position;
                        
        Physics.Raycast( camPosition, rayDir, out hit, MaxRayDistance, RaycastMask );
        if( hit.collider != null )
        {
            //Debug.Log( $"Creating new point at " + hit.point  );
            Vector3 hitPos = hit.point + ( hit.normal * 0.01F );

            Quaternion pointRot = Quaternion.LookRotation( -hit.normal, Vector3.up );

            ColoredSurfaceParam surfaceParam = null;
            if( hit.collider.gameObject.CompareTag( "ColoredSurface"))
            {
                surfaceParam = ColoredSurfaceManager.instance.GetColor( hit.collider );
            }
            
            pointManager.AddNewPoint( hitPos, pointRot, surfaceParam);
            
            if(OnProbePointCreated != null)
                OnProbePointCreated.Invoke( hitPos,pointRot );
        }
    }

    public enum GunMode
    {
        Idle,
        Streaming,
        Scanning
    }
}
