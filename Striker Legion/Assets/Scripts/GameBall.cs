using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameBall : MonoBehaviour
{
    public struct flight 
    {
        Vector3 start;
        Vector3 end;
        public float maxHeight;
        public float duration;
        public float t; // タイム

        public flight(CharacterBase from, CharacterBase to, float height, float speed)
        {
            start = from.transform.Find("Feet").position;
            Vector3 target = to.transform.Find("Feet").position;
            Vector3 velocity = new Vector3(to.velocity.x, 0, to.velocity.z);
            Vector3 offset = target - start;

            duration = Mathf.Infinity;
            do
            {
                float a = Vector3.Dot(velocity, velocity) - (speed * speed);
                float b = 2f * Vector3.Dot(offset, velocity);
                float c = Vector3.Dot(offset, offset);

                if (Mathf.Approximately(a, 0))
                {
                    if (!Mathf.Approximately(b, 0))
                    {
                        float t = -c / b;
                        if (t > 0)
                        {
                            duration = t;
                        }
                    }
                }
                else
                {
                    float discriminant = b * b - 4f * a * c;

                    if (discriminant >= 0)
                    {
                        float sqrt = Mathf.Sqrt(discriminant);
                        float t1 = (-b - sqrt) / (2f * a);
                        float t2 = (-b + sqrt) / (2f * a);

                        if (t1 > 0f) { duration = t1; }
                        if (t2 > 0f && t2 < duration) { duration = t2; }
                    }
                }
                if (duration == Mathf.Infinity)
                {
                    // No valid intercept.
                    // Adjust speed to make it possible.
                    float offsetSqr = Vector3.Dot(offset, offset);
                    float velocitySqr = Vector3.Dot(velocity, velocity);
                    float dot = Vector3.Dot(offset, velocity);
                    float minSpeed;
                    if (dot < 0.0f)
                        minSpeed = Mathf.Sqrt(velocitySqr - (dot * dot / offsetSqr));
                    else
                        minSpeed = Mathf.Sqrt(velocitySqr);
                    speed = minSpeed + 1f;

                }
            }
            while (duration == Mathf.Infinity);

            end = target + velocity * duration;
            maxHeight = height;
            t = 0;

            float expectedDistance = speed * duration;
            float actualDistance = Vector3.Distance(start, end);
        }

        public bool update(out Vector3 position)
        {
            t += Time.deltaTime;
            if (t > duration)
            {
                position = end;
                return false;
            }
            float p = t / duration; // パーセント
            float h = 1 - Mathf.Pow(2 * p - 1, 2);

            position.x = Mathf.Lerp(start.x, end.x, p);
            position.y = Mathf.Lerp(start.y, end.y, p) + h * maxHeight;
            position.z = Mathf.Lerp(start.z, end.z, p);
            return true;
        }
    }

    bool inFlight = false;
    flight currentFlight;

    [SerializeField] float maxCatchupSpeed = 40;

    [SerializeField] CharacterBase owner = null;
    Transform ownerFeet;

    public CharacterBase GetOwner() { return owner; }
    public void SetOwner(CharacterBase character) { owner = character; ownerFeet = owner.transform.Find("Feet"); }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (owner)
        {
            ownerFeet = owner.transform.Find("Feet");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inFlight)
        {
            inFlight = currentFlight.update(out Vector3 position);
            transform.position = position;
            if (!inFlight)
            {
                owner.GetBall();
            }
        } 
        else
        {
            Vector3 toTarget = ownerFeet.position - transform.position;
            float distance = toTarget.magnitude;
            float speedTimme = maxCatchupSpeed * Time.deltaTime;
            if (distance > speedTimme)
            {
                toTarget *= (speedTimme / distance);
            }
            toTarget.y = 0;
            transform.position += toTarget;

        }
    }

    public void Kick(flight path)
    {
        inFlight = true;
        currentFlight = path;
    }
}
