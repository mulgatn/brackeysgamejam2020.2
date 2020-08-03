using UnityEngine;

public class RewindKeyFrames
{
    public Vector2 position_;
    public Quaternion rotation_;
    public float time_stamp_;

    public RewindKeyFrames(Vector2 position, Quaternion rotation, float time_stamp)
    {
        position_ = position;
        rotation_ = rotation;
        time_stamp_ = time_stamp;
    }

    public bool IsFrameRelevant(float current_time, float relevant_time)
    {
        return current_time - time_stamp_ < relevant_time ? true : false;
    }
}