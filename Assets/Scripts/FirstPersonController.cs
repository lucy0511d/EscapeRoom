using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float gravityScale;

    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float originalScale;

    [SerializeField]
    private float scaleWhenCrouched;

    [SerializeField]
    private float crouchedSpeed;

    [Header("Ground Detection")]
    [SerializeField]
    private Transform feet;

    [SerializeField]
    private float detectionRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    private Vector3 verticalMovement;
    private CharacterController controller;
    private Camera cam;
    private PlayerInput playerInput;
    private Vector2 input;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Start is called before the first frame update
    private void OnEnable()
    {
        playerInput.actions["Jump"].started += Saltar;
        playerInput.actions["Move"].performed += Mover;
        playerInput.actions["Move"].canceled += CancelarMover;
        // playerInput.deviceLostEvent.AddListener((X) => Debug.Log("reconecta el mando, por favor"));//para cuando se te desconecta un dispo
        // playerInput.SwitchCurrentActionMap("Coche"); // solo te deja utilizar los controles del mapa "coche"
    }
    private void Mover(InputAction.CallbackContext ctx) //CTX: CONTEXTO
    {
        input = ctx.ReadValue<Vector2>();
    }
    private void CancelarMover(InputAction.CallbackContext obj)
    {
        input =new Vector2 (0,0); // se cancela el input
    }

    private void Saltar(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
        {
            verticalMovement.y = 0;  
            verticalMovement.y = Mathf.Sqrt(-2 * gravityScale * jumpHeight);
          
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveAndRotate();
        ApplyGravity();
        Crouch();
        

    }

    private void Crouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            transform.localScale /= 2; //A la mitad TODO para que no deforme objetos.
        }
        else if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            transform.localScale *= 2;
        }
    }

    private void MoveAndRotate()
    {
        //Se aplica al cuerpo la rotaci�n que tenga la c�mara.
        transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

        ////Si hay input...
        if (input.sqrMagnitude > 0)
        {
            //Se calcula el �ngulo en base a los inputs
            float angleToRotate = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;

            //Se rota el vector (0, 0, 1) a dicho �ngulo
            Vector3 movementInput = Quaternion.Euler(0, angleToRotate, 0) * Vector3.forward;

            //Se aplica movimiento en dicha direcci�n.
            controller.Move(movementInput * movementSpeed * Time.deltaTime);
        }
    }


    private void ApplyGravity()
    {
        verticalMovement.y += gravityScale * Time.deltaTime;
        controller.Move(verticalMovement * Time.deltaTime);
    }
    private bool IsGrounded()
    {
        return Physics.CheckSphere(feet.position, detectionRadius, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(feet.position, detectionRadius);
    }
}
