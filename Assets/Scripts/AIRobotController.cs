using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIRobotController : AIBirdController
{
    [HideInInspector] public bool isSpying = false;
    [HideInInspector] public bool hasBeenCaught = false;

    private Coroutine slidingCoroutine;
    private Coroutine hoveringCoroutine;

    private void Update()
    {
        CheckIfSpying();

        CheckAudio();

        var action = currentState.stateAction;
        if (action != null) action();
        PerformActionsSequence();
        if (shallWeLog)
        {
            shallWeLog = false;
            Debug.Log("State: " + currentState.name);
        }
        if (currentState.name != "sitting")
        {
            StopSliding();
        }
    }

    protected void CheckIfSpying()
    {
        try {
            string anim = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (anim.StartsWith('R')) isSpying = true;
            else isSpying = false;
        }
        catch { 
            isSpying = false;
        }
    }

    protected void CheckAudio()
    {
        try {
            string anim = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (anim.Contains("Antenna")) {
                var audio = GetComponent<AudioSource>();
                if (audio != null && !audio.isPlaying) audio.Play();
            }
        }
        catch { 

        }
    }

    protected override void InitializeStates()
    {

        // states
        states["standing"] = new State("standing", new (float, string)[]{
            (1.0f, "01_Standing_Idle"),
            (1.0f, "01_Standing_Cleaning"),
            (1.0f, "R01_Standing_Idle_Antenna"),
            (1.0f, "R01_Standing_Idle_Head"),
            (1.0f, "R01_Standing_Idle_Shutter"),
            (1.0f, "R01_Standing_Idle_Tweak"),
        });

        states["sitting"] = new State("sitting", new (float, string)[]{
            (1.0f, "02_Sitting_Idle"),
            (1.0f, "R02_Sitting_Idle_Head"),
            (1.0f, "R02_Sitting_Idle_Tweak"),
            (1.0f, "R02_Sitting_Idle_Shutter"),
        }, SittingBehavior);
        states["sleeping"] = new State("sleeping", new (float, string)[]{
            (1.0f, "02_Sitting_Sleeping_Idle"),
            (1.0f, "R02_Sitting_Sleeping_Antenna"),
            (1.0f, "R02_Sitting_Sleeping_Idle_Tweak"),
        });

        states["walking"] = new State("walking", new (float, string)[]{
            (1.0f, "03_Walking_Idle"),
            (1.0f, "R03_Walking_Idle_Tweak"),
            (1.0f, "R03_Walking_Picking_Shutter"),
            (1.0f, "R03_Walking_Picking_Stiff"),
            (1.0f, "R03_Walking_Idle_Shutter"),
        }, WalkAround);

        states["flying"] = new State("flying", new (float, string)[]{
            (1.0f, "03_Walking_Idle"),
        }, Fly);

        // state transitions
        states["standing"].transitions = new (float, (string[], State))[]
        {
            (1.0f, (new string[]{ "02_Sitting_Down" }, states["sitting"])),
            (1.0f, (new string[]{  }, states["walking"])),
            (1.0f, (new string[]{  }, states["flying"])),
        };

        states["sitting"].transitions = new (float, (string[], State))[]
        {
            (1.0f, (new string[]{ "02_Sitting_Standing_up" }, states["standing"])),
            (1.0f, (new string[]{ "02_Sitting_Falling_Asleep" }, states["sleeping"])),
        };
        states["sleeping"].transitions = new (float, (string[], State))[]
        {
            (1.0f, (new string[]{ "02_Sitting_Waking_up" }, states["sitting"])),
        };

        states["walking"].transitions = new (float, (string[], State))[]
        {
            (1.0f, (new string[]{  }, states["standing"])),
        };

        states["flying"].transitions = new (float, (string[], State))[]
        {
            (1.0f, (new string[]{  }, states["standing"])),
        };

        // starting state
        currentState = states["standing"];
    }


    protected override bool RandomPoint(Vector3 center, Vector3 rectangleSize, float rotationAngle, out Vector3 result)
    {
        // Use the parent logic but customize rectangle dimensions if needed
        return base.RandomPoint(center, rectangleSize, rotationAngle, out result);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.magenta;

        Quaternion floorRotation = Quaternion.Euler(0, walkRotationAngle, 0);

        Gizmos.matrix = Matrix4x4.TRS(centrePoint, floorRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, rectangleSize); 
    }


    private IEnumerator SlidingMovement()
    {
        float slideSpeed = 1.0f;
        float slideTime = 1.0f;
        float pauseTime = 1.0f;
        Vector3 slideDirection = transform.forward;

        while (true)
        {
            float elapsedTime = 0f;

            while (elapsedTime < slideTime)
            {
                Vector3 newPosition = transform.position + slideDirection * slideSpeed * Time.deltaTime;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(newPosition, out hit, 1.0f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                    elapsedTime += Time.deltaTime;
                }
                else
                {
                    StopSliding();
                    yield break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(pauseTime);
        }
    }

    private void StopSliding()
    {
        if (slidingCoroutine != null)
        {
            StopCoroutine(slidingCoroutine);
            slidingCoroutine = null;
        }
    }




    public void SittingBehavior()
    {
        if (animator == null) return;

        var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            string currentClipName = clipInfo[0].clip.name;

            if (currentClipName == "02_Sitting_Idle")
            {
                if (slidingCoroutine == null)
                {
                    StopHovering();
                    slidingCoroutine = StartCoroutine(SlidingMovement());
                }

                if (hoveringCoroutine == null)
                {
                    hoveringCoroutine = StartCoroutine(MovingUp(0.5f, transform.position, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z)));
                }
            }
            else if (currentClipName == "02_Sitting_Sleeping_Idle")
            {
                StopSliding(); 
                if (hoveringCoroutine == null)
                {
                    hoveringCoroutine = StartCoroutine(MovingUp(0.5f, transform.position, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z)));
                }
            }
            else
            {
                StopSliding();
                StopHovering();
            }
        }
        else
        {
            StopSliding();
            StopHovering();
        }
    }

    IEnumerator MovingUp(float time, Vector3 startpos, Vector3 endpos)
    {

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startpos, endpos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(endpos, startpos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }



    private void StopHovering()
    {
        if (hoveringCoroutine != null)
        {
            StopCoroutine(hoveringCoroutine);
            hoveringCoroutine = null;
        }
    }

}
