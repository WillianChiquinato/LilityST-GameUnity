#if UNITY_EDITOR 

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DevMode : MonoBehaviour
{
    [Header("Configurações")]
    public KeyCode toggleKey = KeyCode.F1;
    public bool showMenu = false;

    [Header("Player Config")]
    public GameObject player;
    public float rewindSeconds = 5f;
    public float rewindStep = 1f;

    [Header("Spawn Config")]
    public List<GameObject> spawnPrefabs;
    private int selectedPrefabIndex = 0;

    private Vector3 lastPosition;
    private List<Vector3> positionHistory = new List<Vector3>();

    private bool noclip = false;
    private Rigidbody2D rb;

    private Rect menuRect;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindFirstObjectByType<PlayerMoviment>().gameObject;
        }

        if (player != null)
        {
            rb = player.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        // Alterna menu
        if (Input.GetKeyDown(toggleKey))
        {
            showMenu = !showMenu;
        }

        // Grava posição do player (para rewind)
        if (player != null)
        {
            positionHistory.Add(player.transform.position);
        }

        // Limita histórico
        if (positionHistory.Count > 600)
        {
            positionHistory.RemoveAt(0);
        }

        // Noclip
        if (noclip && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            player.transform.Translate(new Vector3(h, v, 0) * 20f * Time.unscaledDeltaTime);
        }
    }

    void OnGUI()
    {
        if (!showMenu) return;

        float width = 250f;
        float height = 300f;
        float x = Screen.width - width - 20f;
        float y = 20f;

        menuRect = new Rect(x, y, width, height);

        GUILayout.BeginArea(menuRect, "Dev Mode", GUI.skin.window);

        GUILayout.Label("⚙️ Ferramentas de Debug");
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button($"⏪ -{rewindStep}s", GUILayout.Height(25)))
            Rewind(rewindStep);
        if (GUILayout.Button($"⏪ -{rewindSeconds}s", GUILayout.Height(25)))
            Rewind(rewindSeconds);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("❤️ Curar", GUILayout.Height(25)))
            FullHeal();

        GUILayout.Space(5);

        GUILayout.Label("👾 Spawn:");
        if (spawnPrefabs.Count > 0)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<", GUILayout.Width(30)))
                selectedPrefabIndex = (selectedPrefabIndex - 1 + spawnPrefabs.Count) % spawnPrefabs.Count;
            GUILayout.Label(spawnPrefabs[selectedPrefabIndex].name, GUILayout.Width(110));
            if (GUILayout.Button(">", GUILayout.Width(30)))
                selectedPrefabIndex = (selectedPrefabIndex + 1) % spawnPrefabs.Count;
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Spawnar", GUILayout.Height(25)))
                SpawnAtPlayer();
        }

        GUILayout.Space(5);

        if (GUILayout.Button(noclip ? "❌ Desativar Noclip" : "🚀 Ativar Noclip", GUILayout.Height(25)))
            ToggleNoclip();

        if (GUILayout.Button("🔁 Recarregar Cena", GUILayout.Height(25)))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        GUILayout.EndArea();

        if (menuRect.Contains(Event.current.mousePosition))
        {
            if (Event.current.type == EventType.MouseDown)
            {
                Event.current.Use();
            }
        }
    }

    void Rewind(float seconds)
    {
        if (player == null || positionHistory.Count == 0)
            return;

        int framesToRewind = Mathf.RoundToInt(seconds / Time.deltaTime);
        int index = Mathf.Max(0, positionHistory.Count - framesToRewind);

        player.transform.position = positionHistory[index];
        positionHistory.RemoveRange(index, positionHistory.Count - index);
    }


    void FullHeal()
    {
        var health = player?.GetComponent<Damage>();
        if (health != null)
        {
            health.Health = health.maxHealth;
            Debug.Log("Vida recarregada!");
        }
    }

    void SpawnAtPlayer()
    {
        if (spawnPrefabs.Count == 0 || player == null) return;
        Instantiate(spawnPrefabs[selectedPrefabIndex], player.transform.position + Vector3.right * 2, Quaternion.identity);
    }

    void ToggleNoclip()
    {
        noclip = !noclip;
        if (rb != null)
        {
            rb.bodyType = noclip ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        }

        player.GetComponent<PlayerMoviment>().canMove = !noclip;
        player.GetComponent<CapsuleCollider2D>().enabled = !noclip;
    }
}

#endif
