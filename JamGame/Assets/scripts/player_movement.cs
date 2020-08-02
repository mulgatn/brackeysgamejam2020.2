using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class player_movement : MonoBehaviour
{
    private Rigidbody2D body_;
    private BoxCollider2D collider_;
    private Transform player_ghost_;

    [SerializeField] private float speed_;
    [SerializeField] private float jump_force_;
    [SerializeField] private float gravity_;
    [SerializeField] private float fall_gravity_multiplier_;
    [SerializeField] private float jump_gravity_multiplier_;
    [SerializeField] private LayerMask ground_layer_;

    [SerializeField] private float jump_cache_time_;
    private float jump_cache_timer_;

    [SerializeField] private float cayote_time_;
    private float grounded_timer_ = 0.0f;

    private bool is_grounded_ { get { return grounded_timer_ > .0f ? true : false; } }

    #region REWIND_VARIABLES
    private List<RewindKeyFrames> rewind_key_frames_;
    [SerializeField] private float rewind_time_;
    private bool rewinding_ = false;
    [SerializeField] private float rewind_cooldown_;
    private float rewind_timer_;
    #endregion


    private void Start()
    {
        body_ = GetComponent<Rigidbody2D>();
        collider_ = GetComponent<BoxCollider2D>();

        player_ghost_ = transform.GetChild(0);

        rewind_key_frames_ = new List<RewindKeyFrames>();
    }

    private void Update()
    {
        if (!rewinding_)
        {
            CheckAndSetGrounded();
            HandleHorizontalMove();
            HandleJump();
            StoreRewindKeyFrames();
        }
        else
        {
            Rewind();
        }
        CheckRewind();
        SetPlayerGhost();
    }

    private void HandleHorizontalMove()
    {
        float horizontal_input = Input.GetAxisRaw("Horizontal");
        if (horizontal_input > 0) transform.eulerAngles = new Vector3(0, 0, 0);
        else if (horizontal_input < 0) transform.eulerAngles = new Vector3(0, 180, 0);

        body_.velocity = new Vector2(speed_ * horizontal_input, body_.velocity.y);
    }

    private void HandleJump()
    {
        //This is to cache the pressed jump button so the player can press jump before landing and still jump
        if (Input.GetButtonDown("Jump")) jump_cache_timer_ = jump_cache_time_;
        jump_cache_timer_ -= Time.deltaTime;

        if (is_grounded_ && jump_cache_timer_ > .0f)
        {
            body_.velocity = Vector2.up * jump_force_;
            jump_cache_timer_ = .0f;
        }

        ApplyAirGravity();
    }

    private void ApplyAirGravity()
    {
        if (body_.velocity.y < 0.0f)
        {
            body_.velocity += Vector2.up * Physics2D.gravity.y * (fall_gravity_multiplier_ - 1) * Time.deltaTime;
        }
        else if (body_.velocity.y > 0.0f && !Input.GetButton("Jump")) //This part is to jump longer when the jump button is pressed longer.
        {
            body_.velocity += Vector2.up * Physics2D.gravity.y * (jump_gravity_multiplier_ - 1) * Time.deltaTime;
        }
    }

    private void CheckAndSetGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(collider_.bounds.center, collider_.size, .0f, Vector2.down, 0.2f, ground_layer_);

        if (hit.collider != null) grounded_timer_ = cayote_time_;

        SetGravity();

        grounded_timer_ -= Time.deltaTime;
    }

    private void SetGravity()
    {
        //Set gravity to 0 whilst the grounded_timer_ is greater than 0 to allow the cayote jump.
        if (grounded_timer_ > .0f) body_.gravityScale = 0.0f;
        else body_.gravityScale = gravity_;
    }

    private void CheckRewind()
    {
        if (Input.GetKeyDown(KeyCode.L) && rewind_timer_ > rewind_cooldown_)
        {
            rewinding_ = true;
            body_.isKinematic = true;
            collider_.enabled = false;
        }

        rewind_timer_ += Time.deltaTime;
    }

    private void StoreRewindKeyFrames()
    {
        rewind_key_frames_.Insert(0, new RewindKeyFrames(transform.position, transform.rotation, Time.time));
        if (!rewind_key_frames_.ElementAt(rewind_key_frames_.Count - 1).IsFrameRelevant(Time.time, rewind_time_)) rewind_key_frames_.RemoveAt(rewind_key_frames_.Count - 1);
    }

    private void Rewind()
    {
        transform.position = rewind_key_frames_.ElementAt(0).position_;
        rewind_key_frames_.RemoveAt(0);

        if (rewind_key_frames_.Count < 1)
        {
            rewinding_ = false;
            rewind_timer_ = 0;
            body_.isKinematic = false;
            collider_.enabled = true;
        }
    }

    private void SetPlayerGhost()
    {
        if (rewind_key_frames_.Any())
        {
            player_ghost_.position = rewind_key_frames_.ElementAt(rewind_key_frames_.Count - 1).position_;
            player_ghost_.rotation = rewind_key_frames_.ElementAt(rewind_key_frames_.Count - 1).rotation_;
        }

    }
}