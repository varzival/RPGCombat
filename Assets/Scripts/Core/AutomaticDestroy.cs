using UnityEngine;

public class AutomaticDestroy : MonoBehaviour
{
    [SerializeField]
    float destroyAfter = 5f;
    float aliveTime = 0f;

    // Update is called once per frame
    void Update()
    {
        aliveTime += Time.deltaTime;
        if (aliveTime > destroyAfter)
            Destroy(gameObject);
    }
}
