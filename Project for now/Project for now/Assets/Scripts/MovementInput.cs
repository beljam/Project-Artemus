using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    public float inputX;
    public float inputZ;
    public Vector3 desiredMoveDir;
    public bool blockPlayerRotation;
    public float desiredRotationSpeed;
    public Animator anim;
    public float speed;
    public float currantSpeed;
    public float allowPlayerRotation;
    public Camera cam;
    public CharacterController controller;
    public AudioSource source;
    public AudioClip[] footsteps;
    public bool isGrounded;
    public bool sound;
    private float verticalVel;
    private Vector3 moveVector;

    void Start()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
        controller = GetComponent<CharacterController>();
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        InputMagnitude();

        //If you don't need the character grounded then delete to...
        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            verticalVel -= 0;
        }
        else
        {
            verticalVel -= 2;
        }

        moveVector = new Vector3(0, verticalVel, 0);
        controller.Move(moveVector);
        //here
    }

    void Footstep (int num)
    {
        if (sound)
        {
            source.clip = footsteps[num];
            source.Play();
        }
    }

    void PlayerMoveAndRotation()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0.0f;
        right.y = 0.0f;

        forward.Normalize();
        right.Normalize();

        desiredMoveDir = forward * inputZ + right * inputX;
        var inputs = new Vector2(inputX, inputZ).sqrMagnitude;
        inputs = Mathf.Clamp(inputs, 0.0f, 1.0f);
        var move = inputs * transform.forward * speed;
        controller.Move(move * Time.deltaTime);

        if (!blockPlayerRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDir), desiredRotationSpeed);
        }
    }

    void InputMagnitude()
    {
        //Calculate input vectors
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        anim.SetFloat("InputZ", inputZ, 0.0f, Time.deltaTime * 2);
        anim.SetFloat("InputX", inputX, 0.0f, Time.deltaTime * 2);

        //Calculate Input Magnitude
        currantSpeed = new Vector2(inputX, inputZ).sqrMagnitude;

        //Physicaly move the player
        if(currantSpeed > allowPlayerRotation)
        {
            anim.SetFloat("InputMagnitude", currantSpeed, 0.0f, Time.deltaTime);

            PlayerMoveAndRotation();
        }
        else if (currantSpeed < allowPlayerRotation)
        {
            anim.SetFloat("InputMagnitude", currantSpeed, 0.0f, Time.deltaTime);
        }
    }
}
