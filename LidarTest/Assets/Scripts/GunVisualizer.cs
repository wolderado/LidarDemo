using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GunVisualizer : MonoBehaviour
{
    [Header("Parameters")]
    public float GunLightShootIntensityMin = 0.5F;
    public float GunLightShootIntensityMax = 1F;
    public float GunKickMin = 1F;
    public float GunKickMax = 1F;
    
    
    [Header("References")]
    public Text GunModeText;
    public Text TotalPointsText;
    public LidarGun Gun;
    public ParticleSystem GunShootParticle;
    public Light GunLight;
    public Animator GunAnim;
    public Animator CamAnim;


    private bool IsShooting = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Gun.OnProbePointCreated += UpdateVisuals;
    }

    private void OnDisable()
    {
        Gun.OnProbePointCreated -= UpdateVisuals;
    }

    // Update is called once per frame
    void Update()
    {

        IsShooting = false;
        if( Gun.CurrentGunMode == LidarGun.GunMode.Idle )
        {
            GunModeText.text = "";
        }
        else if( Gun.CurrentGunMode == LidarGun.GunMode.Streaming )
        {
            GunModeText.text = "Streaming";
            IsShooting = true;

        }
        else if( Gun.CurrentGunMode == LidarGun.GunMode.Scanning )
        {
            if( Gun.scanFinished == false )
            {
                GunModeText.text = "Scanning...";
                IsShooting = true;
            }
            else
            {
                GunModeText.text = "Scan Complete";
            }
        }


        if( IsShooting  )
        {
            if(GunShootParticle.isPlaying == false)
                GunShootParticle.Play();

            GunLight.intensity = Random.Range( GunLightShootIntensityMin, GunLightShootIntensityMax );

            Vector3 gunTarget = -Vector3.forward * Random.Range( GunKickMin ,GunKickMax );
            transform.localPosition = Vector3.Lerp( transform.localPosition, gunTarget, Time.deltaTime * 16F );
        }
        else
        {
            if( GunShootParticle.isPlaying )
                GunShootParticle.Stop();

            GunLight.intensity = Mathf.Lerp( GunLight.intensity, 0F, Time.deltaTime * 8F );
            
            transform.localPosition = Vector3.Lerp( transform.localPosition, Vector3.zero, Time.deltaTime * 4F );
        }
        
        if(GunAnim != null && CamAnim != null)
            GunAnim.SetFloat( "Movement",CamAnim.GetFloat( "Movement" ) );
    }


    public void UpdateVisuals( Vector3 pos, Quaternion rot )
    {
        TotalPointsText.text = PointManager.instance.TotalPointCount.ToString();
        
    }
}
