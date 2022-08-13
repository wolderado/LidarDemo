using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    public CanvasGroup TutorialGroup;
    public float TutorialActiveDuration = 5F;

    private float tutorialActiveTimer = 0;
    private bool IsTutorialActive = true;
    private float exitTimer = 0;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if( Input.GetKeyDown( KeyCode.Escape ) )
        {
            IsTutorialActive = false;
        }
        
        
        //Exit game
        if( Input.GetKey( KeyCode.Escape ) )
        {
            exitTimer += Time.deltaTime;
            if( exitTimer > 1.5F )
            {
                exitTimer = -999;
                Application.Quit();
            }
        }
        else
            exitTimer = 0;
        
        if( Input.GetKeyDown( KeyCode.F1 ) )
        {
            IsTutorialActive = true;
            tutorialActiveTimer = 0;
        }

        if( IsTutorialActive )
        {
            tutorialActiveTimer += Time.deltaTime;
            if( tutorialActiveTimer > TutorialActiveDuration)
            {
                tutorialActiveTimer = 0;
                IsTutorialActive = false;
            }
        }

        TutorialGroup.alpha = IsTutorialActive ? 1F : 0F;
    }
}
