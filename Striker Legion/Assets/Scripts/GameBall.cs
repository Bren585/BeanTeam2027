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
            end = to.transform.Find("Feet").position;
            maxHeight = height;
            float distance = (start - end).magnitude;
            duration = distance / speed;
            t = 0;
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
            transform.position = ownerFeet.position;
        }
    }

    public void Kick(flight path)
    {
        inFlight = true;
        currentFlight = path;
    }
}
