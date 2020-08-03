using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Timeline;
using UnityEngine;

public class player_ghost_animation : MonoBehaviour
{
    private SpriteRenderer renderer_;

    [SerializeField] private Sprite[] keyframes_;
    private int current_index_ = 0;
    [SerializeField] private float animation_timer_;
    private float timer_;

    private void Start()
    {
        renderer_ = GetComponent<SpriteRenderer>();
        renderer_.sprite = keyframes_[current_index_];
    }

    private void Update()
    {
        timer_ += Time.deltaTime;
        
        if(timer_ > animation_timer_)
        {
            current_index_++;
            if(current_index_ >= keyframes_.Length)
            {
                current_index_ = 0;
            }
            renderer_.sprite = keyframes_[current_index_];
            timer_ = .0f;
        }
    }
}
