using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public struct BootOptions
{
    public Color color;

    public float bonusSpeed;
    public bool hasTeleport;
    public float teleportDistance;
    public float teleportCooldown;
}

public class Boots : MonoBehaviour
{
    public BootOptions options;

    [SerializeField]
    SpriteRenderer sprite;

    public float cooldown = 0.0f;

    GameObject teleportee;

    [SerializeField]
    ParticleSystem teleportEffect;

    [SerializeField]
    ParticleSystem trail;

    [SerializeField]
    BootsInfo bootsInfo;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetOptions(BootOptions options)
    {
        this.options = options;
        UpdateComponents();
    }

    void UpdateComponents()
    {
        sprite.color = options.color;
        trail.startColor = options.color;
        bootsInfo.UpdateUI(this);
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown < 0.01f)
        {
            cooldown = 0.0f;
        }
    }

    public void Equip(GameObject obj)
    {
        sprite.gameObject.SetActive(false);
        teleportee = obj;
        trail.Play();
    }

    public void Drop()
    {
        sprite.gameObject.SetActive(true);
        teleportee = null;
        trail.Stop();
    }

    public void Teleport(Vector2 position)
    {
        if (options.hasTeleport && cooldown < 0.01f && teleportee != null)
        {
            var diff = (Vector3)position - teleportee.transform.position;
            var dir = diff.normalized;
            var target = diff.magnitude <= options.teleportDistance ? (Vector3)position : teleportee.transform.position + dir * options.teleportDistance;
            var dist = (target - teleportee.transform.position).magnitude;
            NavMeshPath path = new NavMeshPath();
            NavMeshHit hit;
            while (dist > 0.1f)
            {
                var destinationCandidate = teleportee.transform.position + dir * dist;
                if (NavMesh.SamplePosition(destinationCandidate, out hit, 0.25f, NavMesh.AllAreas))
                {
                    teleportDestination = destinationCandidate;
                    Invoke("DoTeleport", 0.1f);
                    break;
                }
                dist -= 0.1f;
            }
            teleportEffect.Play();
            cooldown = options.teleportCooldown;
        }
    }

    private void DrawPath(NavMeshPath path)
    {
        for (int i = 1; i < path.corners.Length; i++)
        {
            Debug.DrawLine(path.corners[i - 1], path.corners[i], new Color(0, i / path.corners.Length, 0), 3);
        }
    }

    Vector3 teleportDestination;

    public void DoTeleport()
    {
        teleportee.transform.position = teleportDestination;
    }
    
    public static BootOptions GetOptions(float powerLevel)
    {
        powerLevel = Mathf.Clamp(powerLevel, 0.0f, 1.0f);
        powerLevel = powerLevel * 0.9f + (1.0f - 0.9f);
        var weights = LootUtil.getRandomWeights(4, 3f * Random.Range(powerLevel / 2.0f, powerLevel));
        var options = new BootOptions()
        {
            color = getRandomColor(),

            bonusSpeed = GetRandomMS(weights[0]),
            hasTeleport = GetRandomTeleport(weights[1]),
            teleportDistance = GetRandomTeleportDistance(weights[2]),
            teleportCooldown = GetTeleportCooldown(weights[3])
        };

        if (options.bonusSpeed < 0)
        {
            options.hasTeleport = true;
        }

        return options;
    }

    private static Color getRandomColor()
    {
        return Random.ColorHSV(
            0.0f, 1.0f, // hue
            0.0f, 1.0f, // saturation
            0.8f, 1.0f, // value
            1.0f, 1.0f); //alpha
    }

    private static float GetRandomMS(float powerLevel)
    {
        //return Random.Range(-1.0f, powerLevel * 3.0f);
        return -0.3f + powerLevel * 2.5f;
    }

    private static bool GetRandomTeleport(float powerLevel)
    {
        return powerLevel > 0.3f;
    }

    private static float GetRandomTeleportDistance(float powerLevel)
    {
        //return Random.Range(1.0f + 2 * powerLevel, 2.0f + 8.0f * powerLevel);
        return 1.0f + 8.0f * powerLevel;
    }

    private static float GetTeleportCooldown(float powerLevel)
    {
        //return Random.Range(0.1f, 30.0f - 25.0f * powerLevel);
        return 15.0f - 14.9f * powerLevel;
    }
}
