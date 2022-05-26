using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    private Rigidbody rigidbodyComponent;
    private Vector3 startPosition;
    PhotonView view;

    //Move
    private float horizontalInput, lastHorizontalInput, verticalInput, lastVerticalInput, speed = 1;
    private bool moving;
    //

    //Jump
    private bool spaceWasPressed, canJump, wallJumping, canWallJump;
    private int jumpDirection, wallJumpCounter;
    //

    //Dash
    private bool mouseButtonPressed, canDash, isDashing;
    private float xDash, yDash;
    private int dashCounter;
    //

    //Material
    private int myColorNum;
    public int colorNum;
    public int colorNumDash;
    public Material[] material;
    private Renderer rend;
    //

    //Particles
    public ParticleSystem effect;
    private bool isEffectPlaying = false;
    //

    //Collisions
    private bool isOnGround;
    private bool isOnWall;
    //

    [SerializeField] private Transform wallCheckTransformLeftTop = null;
    [SerializeField] private Transform wallCheckTransformRightTop = null;
    [SerializeField] private Transform wallCheckTransformLeftBottom = null;
    [SerializeField] private Transform wallCheckTransformRightBottom = null;
    [SerializeField] private Transform myCamera = null;
    [SerializeField] private Text UserNameDisplay;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform GroundLeft = null;
    [SerializeField] private Transform GroundRight = null;

    //Woring on
    private string userName;
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    //


    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        startPosition = rigidbodyComponent.position;

        rend = GetComponent<Renderer>();
        rend.enabled = true;

        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            userName = PlayerPrefs.GetString("UserName");
            view.Owner.NickName = userName;
            int i = Random.Range(1, material.Length);
            int j = Random.Range(1, material.Length);
            colorNum = i;
            colorNumDash = j;
            view.Owner.CustomProperties.Add("colorNum", colorNum);
            
        }
        else
        {
            Destroy(myCamera.gameObject);
            userName = view.Owner.NickName;
            colorNum = (int)GetComponent<PhotonView>().Owner.CustomProperties["color"]; 
        }
        UserNameDisplay.text = userName;
        rend.sharedMaterial = material[colorNum];

    }

    void Update()
    {
        if (view.IsMine)
        {
            //collisions
            isOnWall = !isOnGround && isOnWall;
            canJump = isOnGround;
            canWallJump = isOnWall;
            //

            //effect
            if (isOnGround && !isEffectPlaying && horizontalInput != 0)
            {
                playEffect();
                isEffectPlaying = true;
            }
            if (horizontalInput == 0 || !isOnGround)
            {
                stopEffect();
                isEffectPlaying = false;
            }
            //

            //color
            if (canDash)
            {
               myColorNum = colorNum;
            }
            else
            {
                myColorNum = colorNumDash;
            }
            rend.sharedMaterial = material[myColorNum];
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props.Add("color", myColorNum);
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            //

            //check for death
            if (rigidbodyComponent.position.y < -25)
            {
                rigidbodyComponent.position = startPosition;
            }
            //

            //get button presses
            spaceWasPressed = Input.GetKeyDown(KeyCode.Space) && (canJump || canWallJump);
            mouseButtonPressed = Input.GetMouseButtonDown(0);
            //
            horizontalInput = 0;
            verticalInput = 0;
            //get directional input

            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
            {
                horizontalInput = -lastHorizontalInput;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                horizontalInput = -1;
                lastHorizontalInput = horizontalInput;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontalInput = 1;
                lastHorizontalInput = horizontalInput;
            }

            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W))
            {
                verticalInput = -lastVerticalInput;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                verticalInput = -1;
                lastVerticalInput = verticalInput;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                verticalInput = 1;
                lastVerticalInput = verticalInput;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                horizontalInput = horizontalInput / 8;
            }
        }
        else
        {
            rend.sharedMaterial = material[(int)GetComponent<PhotonView>().Owner.CustomProperties["color"]];
        }
        //
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            //dash
            if (canDash)
            {
                if (mouseButtonPressed)
                {
                    rigidbodyComponent.velocity = new Vector3(horizontalInput * 16, verticalInput * 16, 0);
                    xDash = horizontalInput * 16;
                    yDash = verticalInput * 16;
                    dashCounter = 0;
                    mouseButtonPressed = false;
                    isDashing = true;
                    canDash = false;
                }

            }
            if (isDashing)
            {
                rigidbodyComponent.velocity = new Vector3(xDash, yDash, 0);
                dashCounter += 1;
                if (dashCounter > 8)
                {
                    isDashing = false;
                    rigidbodyComponent.velocity = new Vector3(0, 0, 0);
                }
                return;
            }
            mouseButtonPressed = false;

            //

            //jump

            if (canWallJump && !canJump)
            {
                if (spaceWasPressed)
                {
                    WallJump();
                    return;

                }
            }

            if (wallJumping)
            {
                wallJumpCounter++;
                if (wallJumpCounter > 18)
                {
                    wallJumping = false;
                }
                return;
            }

            if (canJump)
            {
                if (spaceWasPressed)
                {
                    Jump();
                }
            }



            //

            //move
            rigidbodyComponent.velocity = new Vector3(horizontalInput * 7, rigidbodyComponent.velocity.y, 0);
            if (isOnWall && horizontalInput < 0 && getJumpDirection() == 1 && rigidbodyComponent.velocity.y < 0)
            {
            rigidbodyComponent.velocity = new Vector3(rigidbodyComponent.velocity.x, -4.00f, 0);
            }
            if (isOnWall && horizontalInput > 0 && getJumpDirection() == -1 && rigidbodyComponent.velocity.y < 0)
            {
                rigidbodyComponent.velocity = new Vector3(rigidbodyComponent.velocity.x, -4.00f, 0);
            }
            //
        }
        else
        {
            rigidbodyComponent.position = Vector3.MoveTowards(rigidbodyComponent.position, networkPosition, Time.fixedDeltaTime);
            rigidbodyComponent.rotation = Quaternion.RotateTowards(rigidbodyComponent.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        wallJumping = false;
        if (collision.gameObject.tag == "Phatom")
        {
            canDash = true;
            canJump = true;
        }

    }

    private void OnCollisionStay(Collision collision)
    {

        if (collision.gameObject.tag == "Floor")
        {
            isOnGround = true;
        }

        if (collision.gameObject.tag == "Wall")
        {
            isOnWall = true;
        }
        if (collision.gameObject.tag == "Floor" && (Physics.OverlapSphere(GroundLeft.position, 0.1f).Length > 1 || Physics.OverlapSphere(GroundRight.position, 0.1f).Length > 1))
        {
            canDash = true;

        }
        if (collision.gameObject.name == "Player")
        {
            Debug.Log("here");
            //PhotonView pv = collision.gameObject.GetPhotonView();
            //pv.RPC("changeVelocity");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isOnGround = false;
        isOnWall = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CheckPoint")
        {
            startPosition = other.transform.position;
            other.gameObject.GetComponentInChildren<ParticleSystem>().Play();
        }
        if (other.gameObject.tag == "Lava")
        {
            rigidbodyComponent.position = startPosition;
        }
        if (other.gameObject.tag == "Phatom")
        {
            canDash = true;
            canJump = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Phatom")
        {
            canJump = false;
        }
    }

    private void Jump()
    {
        rigidbodyComponent.velocity = new Vector3(0, 0, 0);
        rigidbodyComponent.AddForce(Vector3.up * 9, ForceMode.VelocityChange);
        spaceWasPressed = false;
    }

    private void WallJump()
    {
        Jump();
        jumpDirection = getJumpDirection();
        if (jumpDirection == -1)
        {
            rigidbodyComponent.velocity = new Vector3(jumpDirection * 7, rigidbodyComponent.velocity.y, 0);
        }
        if (jumpDirection == 1)
        {
            rigidbodyComponent.velocity = new Vector3(jumpDirection * 7, rigidbodyComponent.velocity.y, 0);
        }
        wallJumping = true;
        wallJumpCounter = 0;

    }

    private int getJumpDirection()
    {
        bool right = Physics.OverlapSphere(wallCheckTransformRightTop.position, 0.01f, playerMask).Length > 1 || Physics.OverlapSphere(wallCheckTransformRightBottom.position, 0.01f, playerMask).Length > 1;
        bool left = Physics.OverlapSphere(wallCheckTransformLeftTop.position, 0.01f, playerMask).Length > 1 || Physics.OverlapSphere(wallCheckTransformLeftBottom.position, 0.01f, playerMask).Length > 1;
        if (right)
        {
            return -1;
        }
        if (left)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void playEffect()
    {
        effect.Play();
    }

    private void stopEffect()
    {
        effect.Stop();
    }
}
