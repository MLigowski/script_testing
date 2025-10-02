using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    public static PlayerMining Instance;

    public float miningRange = 1.5f; // zasi�g kopania
    public KeyCode mineKey = KeyCode.Tab; // klawisz do kopania (mo�esz zmieni� na inny)
    public float miningCooldown = 1.0f; // cooldown mi�dzy kolejnymi kopaniami (w sekundach)

    private float lastMineTime = -999f;
    private int mineralsCollected = 0;

    void Awake()
    {
        Instance = this;
        Debug.Log("PlayerMining Instance set");
    }

    void Update()
    {
        if (Input.GetKeyDown(mineKey) && Time.time - lastMineTime >= miningCooldown)
        {
            TryMine();
            lastMineTime = Time.time;
        }
    }

    void TryMine()
    {
        // Rzucamy raycast w kierunku, w kt�rym patrzy posta� (w prawo lub w lewo)
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Dodajemy debug, �eby zobaczy� raycast w scenie (czerwony kolor na 1 sekund�)
        Debug.DrawRay(transform.position, direction * miningRange, Color.red, 1f);

        // Raycast z filtrem na warstw� Mineral (upewnij si�, �e bloki maj� t� warstw�)
        int mineralLayerMask = LayerMask.GetMask("Mineral");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, miningRange, mineralLayerMask);

        if (hit.collider != null)
        {
            MineralBlock block = hit.collider.GetComponent<MineralBlock>();
            if (block != null)
            {
                block.BreakBlock();
            }
        }
    }

    public void AddMinerals(int amount)
    {
        mineralsCollected += amount;
        MineralUIManager.Instance.UpdateMineralCount(mineralsCollected);
    }
}

