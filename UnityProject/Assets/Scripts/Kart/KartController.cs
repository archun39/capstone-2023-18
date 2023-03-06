using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KartController : MonoBehaviour
{
    [Space, Header("Suspension")]
    public float SuspensionDistance = 0.2f;
    public float suspensionForce = 30000f;
    public float suspensionDamper = 200f;
    public Transform groundCheck;
    public Transform fricAt;
    public Transform CentreOfMass;
    public Transform turningAt;
    public Transform EngineAt;

    private Rigidbody rb;

    [Header("Kart Stats")]
    public float speed = 200f;
    public float DownValue = 100f;
    public float turn = 100f;
    public float friction = 70f;
    public float dragAmount = 4f;
    public float TurnAngle = 30f;

    public float maxRayLength = 0.8f, slerpTime = 0.2f;
    [HideInInspector]
    public bool grounded;

    [Header("Visuals")]
    public Transform[] TireMeshes;
    public Transform[] TurnTires;

    [Header("Curves")]
    public AnimationCurve AngularDragCurve;
    public AnimationCurve frictionCurve;
    public AnimationCurve speedCurve;
    public bool seperateReverseCurve = false;
    public AnimationCurve ReverseCurve;
    public AnimationCurve turnCurve;
    public AnimationCurve driftCurve;
    public AnimationCurve engineCurve;




    private float speedValue, fricValue, turnValue, curveVelocity, brakeInput;
    [HideInInspector]
    public Vector3 carVelocity;
    [HideInInspector]
    public RaycastHit hit;
    //public bool drftSndMachVel;

    [Header("Other Settings")]
    public AudioSource[] engineSounds;
    public bool airDrag;
    public float UpForce;
    public float SkidEnable = 20f;
    public float skidWidth = 0.12f;
    private float frictionAngle;


    [HideInInspector]
    public Vector3 normalDir;

    private KartInput input;

    private void Awake()
    {
        input = GetComponent<KartInput>();

        rb = GetComponent<Rigidbody>();
        grounded = false;
        engineSounds[1].mute = true;
        rb.centerOfMass = CentreOfMass.localPosition;
    }

    void FixedUpdate()
    {
        carVelocity = transform.InverseTransformDirection(rb.velocity); //local velocity of car

        curveVelocity = Mathf.Abs(carVelocity.magnitude) / 100;

        //inputs
        float turnInput = turn * input.Hmove * Time.fixedDeltaTime * 1000;
        float speedInput = speed *  input.Vmove * Time.fixedDeltaTime * 1000;

        //helping veriables

        speedValue = speedInput * speedCurve.Evaluate(Mathf.Abs(carVelocity.z) / 100);
        if (seperateReverseCurve && carVelocity.z < 0 && speedInput < 0)
        {
            speedValue = speedInput * ReverseCurve.Evaluate(Mathf.Abs(carVelocity.z) / 100);
        }
        fricValue = friction * frictionCurve.Evaluate(carVelocity.magnitude / 100);
        turnValue = turnInput * turnCurve.Evaluate(carVelocity.magnitude / 100);

        //grounded check
        if (Physics.Raycast(groundCheck.position, -transform.up, out hit, maxRayLength))
        {
            accelarationLogic();
            turningLogic();
            frictionLogic();
            brakeLogic();
            AddAngularDrag();
            OnDrift();
            //for drift behaviour
            //rb.angularDrag = dragAmount * driftCurve.Evaluate(Mathf.Abs(carVelocity.x) / 70);

            //draws green ground checking ray ....ingnore
            Debug.DrawLine(groundCheck.position, hit.point, Color.green);
            grounded = true;

            rb.centerOfMass = Vector3.zero;

            normalDir = hit.normal;
        }
        else if (!Physics.Raycast(groundCheck.position, -transform.up, out hit, maxRayLength))
        {
            Debug.Log("air");
            grounded = false;
            rb.drag = 1f;
            rb.centerOfMass = CentreOfMass.localPosition;
            if (!airDrag)
            {
                rb.angularDrag = 0.1f;
            }
        }



    }

    void Update()
    {
        tireVisuals();
        audioControl();
    }


    public void audioControl()
    {
        //audios
        if (grounded)
        {
            if (Mathf.Abs(carVelocity.x) > SkidEnable - 0.1f)
            {
                engineSounds[1].mute = false;
            }
            else { engineSounds[1].mute = true; }
        }
        else
        {
            engineSounds[1].mute = true;
        }

        /*if (drftSndMachVel) 
        { 
            engineSounds[1].pitch = (0.7f * (Mathf.Abs(carVelocity.x) + 10f) / 40);
        }
        else { engineSounds[1].pitch = 1f; }*/

        engineSounds[1].pitch = 1f;

        engineSounds[0].pitch = 2 * engineCurve.Evaluate(curveVelocity);
        if (engineSounds.Length == 2)
        {
            return;
        }
        else { engineSounds[2].pitch = 2 * engineCurve.Evaluate(curveVelocity); }



    }

    public void tireVisuals()
    {
        //Tire mesh rotate
        foreach (Transform mesh in TireMeshes)
        {
            mesh.transform.RotateAround(mesh.transform.position, mesh.transform.right, carVelocity.z / 3);
            mesh.transform.localPosition = Vector3.zero;
        }

        //TireTurn
        foreach (Transform FM in TurnTires)
        {
            FM.localRotation = Quaternion.Slerp(FM.localRotation, Quaternion.Euler(FM.localRotation.eulerAngles.x,
                               TurnAngle * input.Hmove, FM.localRotation.eulerAngles.z), slerpTime);
        }
    }

    public void accelarationLogic()
    {
        //speed control
        if (input.Vmove > 0.1f)
        {
            rb.AddForceAtPosition(transform.forward * speedValue, EngineAt.position);
        }
        if (input.Vmove < -0.1f)
        {
            rb.AddForceAtPosition(transform.forward * speedValue, EngineAt.position);
        }
    }

    public void turningLogic()
    {
        //turning
        if (carVelocity.z > 0.001f)
        {
            rb.AddTorque(turningAt.transform.up * turnValue);
        }
        else if(carVelocity.z < 0.001f)
        {
            rb.AddTorque(turningAt.transform.up * -turnValue);
        }
    }

    public void OnDrift()
    {
        if(input.Drift)
        {
            rb.angularDrag = 3f * driftCurve.Evaluate(Mathf.Abs(carVelocity.x) / 70);
        }
        else
        {
            rb.angularDrag = dragAmount * AngularDragCurve.Evaluate(Mathf.Abs(carVelocity.magnitude) / 100);
        }
    }

    public void AddAngularDrag()
    {
            //rb.angularDrag = dragAmount * AngularDragCurve.Evaluate(Mathf.Abs(carVelocity.magnitude) / 100);
    }

    public void frictionLogic()
    {
        //Friction
        if (carVelocity.magnitude > 0)
        {
            //Debug.Log(-carVelocity.normalized.x);
            //frictionAngle = (-Vector3.Angle(transform.up, Vector3.up) / 90f) + 1;
            for (int i=0; i<TireMeshes.Length; i++)
            {
                frictionAngle = (-Vector3.Angle(TireMeshes[i].transform.up, Vector3.up) / 90f) + 1;
                if (!input.Drift && i > 2)
                {
                    rb.AddForceAtPosition(transform.right * fricValue * frictionAngle * 100 * -carVelocity.normalized.x, TireMeshes[i].position);
                }
                else
                {
                    rb.AddForceAtPosition(transform.right * fricValue * frictionAngle * 100 * -carVelocity.normalized.x, TireMeshes[i].position);
                }
            }
            //Origin code
            /*
            frictionAngle = (-Vector3.Angle(transform.up, Vector3.up) / 90f) + 1;
            rb.AddForceAtPosition(transform.right * fricValue * frictionAngle * 100 * -carVelocity.normalized.x, fricAt.position);
        */

        }
    }

    public void brakeLogic()
    {
        //brake
        if (carVelocity.z > 1f)
        {
            rb.AddForceAtPosition(transform.forward * -brakeInput, groundCheck.position);
        }
        if (carVelocity.z < -1f)
        {
            rb.AddForceAtPosition(transform.forward * brakeInput, groundCheck.position);
        }
        /*
        if (carVelocity.z < 1f)
        {
            rb.drag = 5f;
        }
        else
        {
            rb.drag = 0.1f;
        }
        */
    }


    private void OnDrawGizmos()
    {

        if (!Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position - maxRayLength * groundCheck.up);
            Gizmos.DrawWireCube(groundCheck.position - maxRayLength * (groundCheck.up.normalized), new Vector3(5, 0.02f, 10));
            Gizmos.color = Color.magenta;
            if (GetComponent<BoxCollider>())
            {
                Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
            }
            if (GetComponent<CapsuleCollider>())
            {
                Gizmos.DrawWireCube(transform.position, GetComponent<CapsuleCollider>().bounds.size);
            }



            Gizmos.color = Color.red;
            foreach (Transform mesh in TireMeshes)
            {
                var ydrive = mesh.parent.parent.GetComponent<ConfigurableJoint>().yDrive;
                ydrive.positionDamper = suspensionDamper;
                ydrive.positionSpring = suspensionForce;


                mesh.parent.parent.GetComponent<ConfigurableJoint>().yDrive = ydrive;

                var jointLimit = mesh.parent.parent.GetComponent<ConfigurableJoint>().linearLimit;
                jointLimit.limit = SuspensionDistance;
                mesh.parent.parent.GetComponent<ConfigurableJoint>().linearLimit = jointLimit;

                Handles.color = Color.red;
                //Handles.DrawWireCube(mesh.position, new Vector3(0.02f, 2 * jointLimit.limit, 0.02f));
                Handles.ArrowHandleCap(0, mesh.position, mesh.rotation * Quaternion.LookRotation(Vector3.up), jointLimit.limit, EventType.Repaint);
                Handles.ArrowHandleCap(0, mesh.position, mesh.rotation * Quaternion.LookRotation(Vector3.down), jointLimit.limit, EventType.Repaint);

            }
            float wheelRadius = TurnTires[0].parent.GetComponent<SphereCollider>().radius;
            float wheelYPosition = TurnTires[0].parent.parent.localPosition.y + TurnTires[0].parent.localPosition.y;
            maxRayLength = (groundCheck.localPosition.y - wheelYPosition + (0.05f + wheelRadius));

        }

    }



}
