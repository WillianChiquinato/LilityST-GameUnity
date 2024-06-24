using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeRemoveBehavior : StateMachineBehaviour
{
    public float TimerFade = 0.5f;
    private float timerElapsed = 0f;
    SpriteRenderer spriteRenderer;
    GameObject removeObj;
    Color startCor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       timerElapsed = 0f;
       spriteRenderer = animator.GetComponent<SpriteRenderer>();
       startCor = spriteRenderer.color;
       removeObj = animator.gameObject;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timerElapsed += Time.deltaTime;

        float newAlpha = startCor.a * (1 - (timerElapsed / TimerFade));
        spriteRenderer.color = new Color(startCor.r, startCor.g, startCor.b, newAlpha);

        if(timerElapsed > TimerFade) 
        {
            Destroy(removeObj);
        }
    }
}
