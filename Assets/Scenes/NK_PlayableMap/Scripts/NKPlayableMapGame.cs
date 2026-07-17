using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Scene tự chạy cho map NK-Map.
// Không cần prefab, không cần tile palette, không cần kéo enemy thủ công.
// Mở scene NK-Map-Playable rồi bấm Play là chơi được.
[ExecuteAlways]
public class NKPlayableMapGame : MonoBehaviour
{
    public int startingCoreHp = 100;
    public int startingGold = 120;
    public int heroCost = 25;

    private int coreHp;
    private int gold;
    private int wave;
    private bool isRunning;
    private bool isGameOver;
    private bool isWin;

    private readonly List<NKPlayableEnemy> enemies = new List<NKPlayableEnemy>();
    private readonly List<NKPlayableHero> heroes = new List<NKPlayableHero>();
    private readonly List<Vector3> pathA = new List<Vector3>();
    private readonly List<Vector3> pathB = new List<Vector3>();
    private readonly List<Vector3> pathBoss = new List<Vector3>();

    private Transform enemyContainer;
    private Transform bossContainer;
    private Transform projectileContainer;
    private Transform heroSlotsContainer;

    private static Sprite squareSprite;
    private static Sprite circleSprite;

    private GUIStyle titleStyle;
    private GUIStyle smallStyle;

    private void OnEnable()
    {
        if (!gameObject.scene.IsValid()) return;

        gameObject.name = "Map03_GameManager";

        EnsureGeneratedMap();
        RefreshReferences();

        if (!Application.isPlaying)
        {
            RefreshSlotLinks();
        }
    }

    private void Awake()
    {
        if (!Application.isPlaying) return;

        gameObject.name = "Map03_GameManager";
        EnsureGeneratedMap();
        RefreshReferences();
        RefreshSlotLinks();
        ResetGameState();
    }

    private void Start()
    {
        if (!Application.isPlaying) return;
        RefreshReferences();
        RefreshSlotLinks();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying) return;

        InitGuiStyles();

        GUI.Box(new Rect(12, 12, 210, 102), "");
        GUI.Label(new Rect(24, 22, 190, 26), "Core HP: " + coreHp + "/" + startingCoreHp, smallStyle);
        GUI.Label(new Rect(24, 47, 190, 26), "Gold: " + gold, smallStyle);
        GUI.Label(new Rect(24, 72, 190, 26), "Wave: " + wave + "/3", smallStyle);

        if (!isRunning)
        {
            string title = isGameOver ? "DEFEAT" : (isWin ? "VICTORY" : "Map 3");
            GUI.Label(new Rect(Screen.width / 2f - 140, Screen.height / 2f - 88, 280, 40), title, titleStyle);

            string buttonText = isGameOver || isWin ? "Restart" : "Start";
            if (GUI.Button(new Rect(Screen.width / 2f - 70, Screen.height / 2f - 38, 140, 38), buttonText))
            {
                StartGame();
            }

            GUI.Label(new Rect(Screen.width / 2f - 220, Screen.height / 2f + 10, 440, 80),
                "Click vào các ô xanh để đặt hero.\nHero sẽ tự bắn enemy. Không để enemy tới Core bên phải.", smallStyle);
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width - 92, 14, 80, 32), "Pause"))
            {
                Time.timeScale = Time.timeScale > 0 ? 0 : 1;
            }
        }
    }

    public void StartGame()
    {
        StopAllCoroutines();
        ClearDynamicObjects();
        ResetGameState();
        isRunning = true;
        StartCoroutine(WaveRoutine());
    }

    public bool TryBuildHero(NKPlayableHeroSlot slot)
    {
        if (!Application.isPlaying) return false;
        if (!isRunning || isGameOver || isWin) return false;
        if (slot == null || slot.HasHero) return false;
        if (gold < heroCost) return false;

        gold -= heroCost;
        slot.HasHero = true;

        GameObject heroObj = CreateVisualObject("Hero", slot.transform.position + new Vector3(0, 0, -0.05f), new Vector3(0.55f, 0.55f, 1), new Color(0.2f, 0.65f, 1f, 1f), heroSlotsContainer, 30, true);
        var hero = heroObj.AddComponent<NKPlayableHero>();
        hero.Setup(this, 3.1f, 0.65f, 15);
        heroes.Add(hero);

        return true;
    }

    public void RegisterEnemy(NKPlayableEnemy enemy)
    {
        if (!enemies.Contains(enemy)) enemies.Add(enemy);
    }

    public void RemoveEnemy(NKPlayableEnemy enemy, bool killedByHero)
    {
        enemies.Remove(enemy);
        if (killedByHero)
        {
            gold += enemy.goldReward;
        }
    }

    public List<NKPlayableEnemy> GetEnemies()
    {
        return enemies;
    }

    public void DamageCore(int damage)
    {
        if (!isRunning || isGameOver || isWin) return;

        coreHp -= damage;
        if (coreHp <= 0)
        {
            coreHp = 0;
            isGameOver = true;
            isRunning = false;
            StopAllCoroutines();
        }
    }

    public NKPlayableProjectile SpawnProjectile(Vector3 start, NKPlayableEnemy target, float damage)
    {
        if (target == null) return null;

        GameObject bullet = CreateVisualObject("Projectile", start, new Vector3(0.18f, 0.18f, 1), new Color(1f, 0.86f, 0.25f, 1f), projectileContainer, 40, true);
        var projectile = bullet.AddComponent<NKPlayableProjectile>();
        projectile.Setup(target, damage, 9f);
        return projectile;
    }

    private IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(0.4f);

        for (wave = 1; wave <= 3; wave++)
        {
            int count = 6 + wave * 2;
            for (int i = 0; i < count; i++)
            {
                List<Vector3> selectedPath = (i % 2 == 0) ? pathA : pathB;
                SpawnEnemy(selectedPath, false, wave);
                yield return new WaitForSeconds(Mathf.Max(0.45f, 0.9f - wave * 0.12f));
            }

            if (wave == 3)
            {
                yield return new WaitForSeconds(1.0f);
                SpawnEnemy(pathBoss, true, wave);
            }

            while (enemies.Count > 0 && isRunning)
            {
                yield return null;
            }

            if (!isRunning) yield break;
            gold += 35;
            yield return new WaitForSeconds(1.0f);
        }

        isWin = true;
        isRunning = false;
    }

    private void SpawnEnemy(List<Vector3> path, bool isBossEnemy, int waveNumber)
    {
        if (path == null || path.Count == 0) return;

        Color color = isBossEnemy ? new Color(0.85f, 0.2f, 0.18f, 1f) : new Color(0.35f, 0.95f, 0.42f, 1f);
        Vector3 scale = isBossEnemy ? new Vector3(0.72f, 0.72f, 1) : new Vector3(0.42f, 0.42f, 1);
        Transform parent = isBossEnemy ? bossContainer : enemyContainer;
        GameObject obj = CreateVisualObject(isBossEnemy ? "Boss_SkeletonKing" : "Enemy_Slime", path[0], scale, color, parent, 25, true);

        var enemy = obj.AddComponent<NKPlayableEnemy>();
        float hp = isBossEnemy ? 230f : 35f + waveNumber * 12f;
        float speed = isBossEnemy ? 1.0f : 1.45f + waveNumber * 0.13f;
        int coreDamage = isBossEnemy ? 35 : 10;
        int reward = isBossEnemy ? 80 : 10;
        enemy.Setup(this, path, hp, speed, coreDamage, reward);
        RegisterEnemy(enemy);
    }

    private void ResetGameState()
    {
        coreHp = startingCoreHp;
        gold = startingGold;
        wave = 0;
        isRunning = false;
        isGameOver = false;
        isWin = false;
        enemies.Clear();
        heroes.Clear();
        Time.timeScale = 1;

        RefreshSlotLinks();
        if (heroSlotsContainer != null)
        {
            var slots = heroSlotsContainer.GetComponentsInChildren<NKPlayableHeroSlot>(true);
            foreach (var slot in slots)
            {
                slot.HasHero = false;
            }
        }
    }

    private void ClearDynamicObjects()
    {
        enemies.Clear();
        heroes.Clear();

        ClearChildren(enemyContainer);
        ClearChildren(bossContainer);
        ClearChildren(projectileContainer);

        if (heroSlotsContainer != null)
        {
            var oldHeroes = new List<GameObject>();
            for (int i = 0; i < heroSlotsContainer.childCount; i++)
            {
                Transform child = heroSlotsContainer.GetChild(i);
                if (child.name.StartsWith("Hero")) oldHeroes.Add(child.gameObject);
            }
            foreach (var obj in oldHeroes) DestroySafe(obj);
        }
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null) return;
        var list = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++) list.Add(parent.GetChild(i).gameObject);
        foreach (var obj in list) DestroySafe(obj);
    }

    private void RefreshReferences()
    {
        Transform gameplay = FindOrCreateRoot("GameplayObjects_Map03").transform;
        Transform slotsRoot = FindOrCreateChild(gameplay, "HeroSlots").transform;
        enemyContainer = FindOrCreateChild(gameplay, "EnemyContainer").transform;
        bossContainer = FindOrCreateChild(gameplay, "BossContainer").transform;
        projectileContainer = FindOrCreateChild(gameplay, "ProjectileContainer").transform;
        heroSlotsContainer = slotsRoot;

        pathA.Clear();
        pathB.Clear();
        pathBoss.Clear();

        ReadPath("Path_A_LeftToCore", pathA);
        ReadPath("Path_B_TopToCore", pathB);
        ReadPath("BossPath_BottomLeftToCore", pathBoss);

        if (pathA.Count == 0)
        {
            pathA.Add(new Vector3(-7.2f, 0f, 0));
            pathA.Add(new Vector3(-3.2f, 0f, 0));
            pathA.Add(new Vector3(0f, 0f, 0));
            pathA.Add(new Vector3(3.7f, 0f, 0));
            pathA.Add(new Vector3(6.55f, 0f, 0));
        }
        if (pathB.Count == 0)
        {
            pathB.Add(new Vector3(0f, 3.7f, 0));
            pathB.Add(new Vector3(0f, 1.2f, 0));
            pathB.Add(new Vector3(0f, 0f, 0));
            pathB.Add(new Vector3(3.4f, 0f, 0));
            pathB.Add(new Vector3(6.55f, 0f, 0));
        }
        if (pathBoss.Count == 0)
        {
            pathBoss.Add(new Vector3(-4.7f, -3.35f, 0));
            pathBoss.Add(new Vector3(-3.6f, -2.1f, 0));
            pathBoss.Add(new Vector3(-1.5f, -0.6f, 0));
            pathBoss.Add(new Vector3(0f, 0f, 0));
            pathBoss.Add(new Vector3(6.55f, 0f, 0));
        }
    }

    private void ReadPath(string pathName, List<Vector3> target)
    {
        GameObject pathObj = GameObject.Find(pathName);
        if (pathObj == null) return;

        for (int i = 0; i < pathObj.transform.childCount; i++)
        {
            target.Add(pathObj.transform.GetChild(i).position);
        }
    }

    private void RefreshSlotLinks()
    {
        if (heroSlotsContainer == null) return;

        var slots = heroSlotsContainer.GetComponentsInChildren<NKPlayableHeroSlot>(true);
        foreach (var slot in slots)
        {
            slot.SetGame(this);
        }
    }

    private void EnsureGeneratedMap()
    {
        EnsureCamera();

        Transform grid = FindOrCreateRoot("Grid_Dungeon").transform;
        Transform gameplay = FindOrCreateRoot("GameplayObjects_Map03").transform;
        FindOrCreateRoot("EventSystem");
        FindOrCreateRoot("UIController");

        BuildGrid(grid);
        BuildGameplayObjects(gameplay);
    }

    private void EnsureCamera()
    {
        Camera cam = Camera.main;
        GameObject cameraObject;

        if (cam == null)
        {
            cameraObject = GameObject.Find("Main Camera");
            if (cameraObject == null) cameraObject = new GameObject("Main Camera");
            cam = cameraObject.GetComponent<Camera>();
            if (cam == null) cam = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }
        else
        {
            cameraObject = cam.gameObject;
            cameraObject.name = "Main Camera";
        }

        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        cameraObject.transform.rotation = Quaternion.identity;
        cam.orthographic = true;
        cam.orthographicSize = 4.8f;
        cam.backgroundColor = new Color(0.11f, 0.12f, 0.14f, 1f);
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    private void BuildGrid(Transform grid)
    {
        Transform background = FindOrCreateChild(grid, "MapImage_Background").transform;
        Transform ground = FindOrCreateChild(grid, "Ground_Tilemap").transform;
        Transform path = FindOrCreateChild(grid, "Path_Tilemap").transform;
        Transform wall = FindOrCreateChild(grid, "Wall_Tilemap").transform;
        Transform obstacle = FindOrCreateChild(grid, "Obstacle_Tilemap").transform;
        Transform decoration = FindOrCreateChild(grid, "Decoration_Tilemap").transform;
        Transform trap = FindOrCreateChild(grid, "Trap_Tilemap").transform;
        Transform buildSlot = FindOrCreateChild(grid, "BuildSlot_Tilemap").transform;

        CreateOrUpdateRect(background, "Dungeon_Background", Vector3.zero, new Vector3(15.6f, 8.2f, 1), new Color(0.23f, 0.27f, 0.28f, 1), 0);

        // Nền gạch dungeon.
        int idx = 0;
        for (int x = -7; x <= 7; x++)
        {
            for (int y = -4; y <= 4; y++)
            {
                Color c = ((x + y) % 2 == 0) ? new Color(0.29f, 0.32f, 0.36f, 1) : new Color(0.25f, 0.28f, 0.33f, 1);
                CreateOrUpdateRect(ground, "GroundTile_" + idx++, new Vector3(x, y, 0), new Vector3(0.96f, 0.96f, 1), c, 1);
            }
        }

        // Đường đi chính.
        CreateOrUpdateRect(path, "Path_Horizontal", new Vector3(0f, 0f, -0.02f), new Vector3(14.8f, 0.72f, 1), new Color(0.72f, 0.64f, 0.52f, 1), 5);
        CreateOrUpdateRect(path, "Path_Vertical_Top", new Vector3(0f, 2.05f, -0.02f), new Vector3(0.72f, 4.1f, 1), new Color(0.72f, 0.64f, 0.52f, 1), 5);
        CreateOrUpdateRect(path, "Path_Boss_Diagonal_01", new Vector3(-3.55f, -2.25f, -0.02f), new Vector3(0.95f, 2.0f, 1), new Color(0.64f, 0.56f, 0.47f, 1), 4, -32f);
        CreateOrUpdateRect(path, "Path_Boss_Diagonal_02", new Vector3(-1.8f, -1.08f, -0.02f), new Vector3(0.9f, 2.2f, 1), new Color(0.64f, 0.56f, 0.47f, 1), 4, -72f);

        // Tường ngoài.
        CreateOrUpdateRect(wall, "Wall_Top", new Vector3(0f, 4.2f, -0.01f), new Vector3(15.9f, 0.32f, 1), new Color(0.16f, 0.17f, 0.22f, 1), 10);
        CreateOrUpdateRect(wall, "Wall_Bottom", new Vector3(0f, -4.2f, -0.01f), new Vector3(15.9f, 0.32f, 1), new Color(0.16f, 0.17f, 0.22f, 1), 10);
        CreateOrUpdateRect(wall, "Wall_Left", new Vector3(-7.85f, 0f, -0.01f), new Vector3(0.32f, 8.2f, 1), new Color(0.16f, 0.17f, 0.22f, 1), 10);
        CreateOrUpdateRect(wall, "Wall_Right", new Vector3(7.85f, 0f, -0.01f), new Vector3(0.32f, 8.2f, 1), new Color(0.16f, 0.17f, 0.22f, 1), 10);

        // Chướng ngại / trang trí / bẫy.
        Vector3[] rocks = { new Vector3(-3.0f, 1.35f, 0), new Vector3(2.0f, 1.25f, 0), new Vector3(3.2f, -1.45f, 0), new Vector3(-5.1f, -1.7f, 0), new Vector3(4.7f, 1.7f, 0) };
        for (int i = 0; i < rocks.Length; i++)
        {
            CreateOrUpdateRect(obstacle, "Obstacle_Rock_" + (i + 1), rocks[i], new Vector3(0.45f, 0.45f, 1), new Color(0.42f, 0.45f, 0.50f, 1), 12);
        }

        Vector3[] decor = { new Vector3(-6.1f, 2.5f, 0), new Vector3(-5.3f, 3.05f, 0), new Vector3(5.8f, -2.7f, 0), new Vector3(1.4f, 2.75f, 0), new Vector3(-1.0f, -2.95f, 0), new Vector3(6.5f, 2.9f, 0) };
        for (int i = 0; i < decor.Length; i++)
        {
            CreateOrUpdateCircle(decoration, "Decoration_Crystal_" + (i + 1), decor[i], new Vector3(0.18f, 0.18f, 1), new Color(0.36f, 0.52f, 0.68f, 1), 11);
        }

        Vector3[] traps = { new Vector3(-2.0f, 0f, 0), new Vector3(2.7f, 0f, 0), new Vector3(0f, 1.65f, 0) };
        for (int i = 0; i < traps.Length; i++)
        {
            CreateOrUpdateRect(trap, "Trap_Spike_" + (i + 1), traps[i], new Vector3(0.22f, 0.22f, 1), new Color(0.84f, 0.30f, 0.25f, 1), 13, 45f);
        }

        // Ô đặt hero hiển thị trên layer map.
        Vector3[] slots = GetSlotPositions();
        for (int i = 0; i < slots.Length; i++)
        {
            CreateOrUpdateRect(buildSlot, "BuildSlotTile_" + (i + 1), slots[i], new Vector3(0.72f, 0.72f, 1), new Color(0.42f, 0.54f, 0.63f, 0.9f), 8);
        }
    }

    private void BuildGameplayObjects(Transform gameplay)
    {
        Transform spawnPoints = FindOrCreateChild(gameplay, "SpawnPoints").transform;
        Transform waypoints = FindOrCreateChild(gameplay, "Waypoints").transform;
        Transform baseRoot = FindOrCreateChild(gameplay, "Base").transform;
        Transform heroArea = FindOrCreateChild(gameplay, "HeroArea").transform;
        Transform heroSlots = FindOrCreateChild(gameplay, "HeroSlots").transform;
        FindOrCreateChild(gameplay, "EnemyContainer");
        FindOrCreateChild(gameplay, "BossContainer");
        FindOrCreateChild(gameplay, "ProjectileContainer");
        FindOrCreateChild(gameplay, "ItemDropContainer");
        FindOrCreateChild(gameplay, "TrapObjects");
        FindOrCreateChild(gameplay, "UI");

        CreateOrUpdateCircle(spawnPoints, "SpawnPoint_Left", new Vector3(-7.2f, 0f, -0.05f), new Vector3(0.55f, 0.55f, 1), new Color(0.35f, 0.6f, 1f, 0.9f), 20);
        CreateOrUpdateCircle(spawnPoints, "SpawnPoint_Top", new Vector3(0f, 3.7f, -0.05f), new Vector3(0.55f, 0.55f, 1), new Color(0.52f, 0.42f, 1f, 0.9f), 20);
        CreateOrUpdateCircle(spawnPoints, "BossSpawnPoint_BottomLeft", new Vector3(-4.7f, -3.35f, -0.05f), new Vector3(0.62f, 0.62f, 1), new Color(0.75f, 0.35f, 1f, 0.9f), 20);

        CreateOrUpdateRect(baseRoot, "Core_LastFlame", new Vector3(6.65f, 0f, -0.05f), new Vector3(0.95f, 0.95f, 1), new Color(1f, 0.78f, 0.42f, 1f), 22);
        CreateOrUpdateCircle(baseRoot, "Core_Flame", new Vector3(6.65f, 0f, -0.06f), new Vector3(0.45f, 0.45f, 1), new Color(1f, 0.33f, 0.26f, 1f), 23);

        CreateOrUpdateRect(heroArea, "HeroArea_Top", new Vector3(0f, 2.1f, -0.04f), new Vector3(12.5f, 2.7f, 1), new Color(0.22f, 0.34f, 0.30f, 0.18f), 7);
        CreateOrUpdateRect(heroArea, "HeroArea_Bottom", new Vector3(0f, -2.3f, -0.04f), new Vector3(12.5f, 2.6f, 1), new Color(0.22f, 0.34f, 0.30f, 0.18f), 7);

        BuildPathObject(waypoints, "Path_A_LeftToCore", new Vector3[]
        {
            new Vector3(-7.2f, 0f, 0), new Vector3(-3.2f, 0f, 0), new Vector3(0f, 0f, 0), new Vector3(3.7f, 0f, 0), new Vector3(6.55f, 0f, 0)
        }, new Color(1f, 0.92f, 0.2f, 1f));

        BuildPathObject(waypoints, "Path_B_TopToCore", new Vector3[]
        {
            new Vector3(0f, 3.7f, 0), new Vector3(0f, 1.2f, 0), new Vector3(0f, 0f, 0), new Vector3(3.4f, 0f, 0), new Vector3(6.55f, 0f, 0)
        }, new Color(0.35f, 0.55f, 1f, 1f));

        BuildPathObject(waypoints, "BossPath_BottomLeftToCore", new Vector3[]
        {
            new Vector3(-4.7f, -3.35f, 0), new Vector3(-3.6f, -2.1f, 0), new Vector3(-1.5f, -0.6f, 0), new Vector3(0f, 0f, 0), new Vector3(6.55f, 0f, 0)
        }, new Color(0.85f, 0.45f, 1f, 1f));

        Vector3[] slots = GetSlotPositions();
        for (int i = 0; i < slots.Length; i++)
        {
            GameObject slot = CreateOrUpdateRect(heroSlots, "HeroSlot_" + (i + 1).ToString("00"), slots[i], new Vector3(0.62f, 0.62f, 1), new Color(0.36f, 0.72f, 1f, 0.75f), 16);
            var col = slot.GetComponent<BoxCollider2D>();
            if (col == null) col = slot.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;

            var slotScript = slot.GetComponent<NKPlayableHeroSlot>();
            if (slotScript == null) slotScript = slot.AddComponent<NKPlayableHeroSlot>();
            slotScript.SetGame(this);
        }
    }

    private void BuildPathObject(Transform parent, string name, Vector3[] points, Color color)
    {
        Transform path = FindOrCreateChild(parent, name).transform;
        for (int i = 0; i < points.Length; i++)
        {
            GameObject wp = FindOrCreateChild(path, "Waypoint_" + (i + 1).ToString("00"));
            wp.transform.position = points[i];
            wp.transform.localScale = Vector3.one;
            CreateOrUpdateCircle(wp.transform, "Marker", points[i], new Vector3(0.12f, 0.12f, 1), color, 24);
        }
    }

    private Vector3[] GetSlotPositions()
    {
        return new Vector3[]
        {
            new Vector3(-4.8f, 1.6f, -0.05f),
            new Vector3(-2.1f, 1.65f, -0.05f),
            new Vector3(2.0f, 1.65f, -0.05f),
            new Vector3(4.4f, 1.35f, -0.05f),
            new Vector3(-5.4f, -1.55f, -0.05f),
            new Vector3(-2.4f, -1.9f, -0.05f),
            new Vector3(1.5f, -1.75f, -0.05f),
            new Vector3(4.0f, -1.35f, -0.05f)
        };
    }

    private GameObject CreateOrUpdateRect(Transform parent, string name, Vector3 pos, Vector3 scale, Color color, int order, float zRotation = 0f)
    {
        GameObject obj = FindOrCreateChild(parent, name);
        obj.transform.position = pos;
        obj.transform.rotation = Quaternion.Euler(0, 0, zRotation);
        obj.transform.localScale = scale;
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = GetSquareSprite();
        sr.color = color;
        sr.sortingOrder = order;
        return obj;
    }

    private GameObject CreateOrUpdateCircle(Transform parent, string name, Vector3 pos, Vector3 scale, Color color, int order)
    {
        GameObject obj = FindOrCreateChild(parent, name);
        obj.transform.position = pos;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.localScale = scale;
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = GetCircleSprite();
        sr.color = color;
        sr.sortingOrder = order;
        return obj;
    }

    private GameObject CreateVisualObject(string name, Vector3 pos, Vector3 scale, Color color, Transform parent, int order, bool circle)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, true);
        obj.transform.position = pos;
        obj.transform.localScale = scale;
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = circle ? GetCircleSprite() : GetSquareSprite();
        sr.color = color;
        sr.sortingOrder = order;
        return obj;
    }

    private GameObject FindOrCreateRoot(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj == null) obj = new GameObject(name);
        return obj;
    }

    private GameObject FindOrCreateChild(Transform parent, string name)
    {
        Transform found = parent.Find(name);
        if (found != null) return found.gameObject;

        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = Vector3.zero;
        return obj;
    }

    private void DestroySafe(GameObject obj)
    {
        if (obj == null) return;
        if (Application.isPlaying) Destroy(obj);
        else DestroyImmediate(obj);
    }

    private Sprite GetSquareSprite()
    {
        if (squareSprite != null) return squareSprite;
        Texture2D tex = new Texture2D(16, 16);
        tex.name = "Generated_SquareSprite";
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++) tex.SetPixel(x, y, Color.white);
        }
        tex.Apply();
        squareSprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        return squareSprite;
    }

    private Sprite GetCircleSprite()
    {
        if (circleSprite != null) return circleSprite;
        Texture2D tex = new Texture2D(32, 32);
        tex.name = "Generated_CircleSprite";
        Vector2 center = new Vector2(15.5f, 15.5f);
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center);
                Color c = d <= 15f ? Color.white : new Color(1, 1, 1, 0);
                tex.SetPixel(x, y, c);
            }
        }
        tex.Apply();
        circleSprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
        return circleSprite;
    }

    private void InitGuiStyles()
    {
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontSize = 30;
            titleStyle.normal.textColor = Color.white;
        }
        if (smallStyle == null)
        {
            smallStyle = new GUIStyle(GUI.skin.label);
            smallStyle.fontSize = 18;
            smallStyle.normal.textColor = Color.white;
            smallStyle.alignment = TextAnchor.UpperLeft;
        }
    }
}

public class NKPlayableHeroSlot : MonoBehaviour
{
    public bool HasHero { get; set; }
    private NKPlayableMapGame game;

    public void SetGame(NKPlayableMapGame mapGame)
    {
        game = mapGame;
    }

    private void OnMouseDown()
    {
        if (game != null)
        {
            game.TryBuildHero(this);
        }
    }
}

public class NKPlayableEnemy : MonoBehaviour
{
    public int goldReward;

    private NKPlayableMapGame game;
    private List<Vector3> path;
    private int index;
    private float hp;
    private float maxHp;
    private float speed;
    private int coreDamage;
    private bool removed;

    private Transform hpBar;

    public void Setup(NKPlayableMapGame mapGame, List<Vector3> waypoints, float health, float moveSpeed, int damageToCore, int reward)
    {
        game = mapGame;
        path = new List<Vector3>(waypoints);
        hp = health;
        maxHp = health;
        speed = moveSpeed;
        coreDamage = damageToCore;
        goldReward = reward;
        index = 1;
        removed = false;
        transform.position = path[0];
        CreateHpBar();
    }

    private void Update()
    {
        if (path == null || path.Count == 0 || removed) return;
        if (index >= path.Count)
        {
            ReachCore();
            return;
        }

        Vector3 target = path[index];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) <= 0.03f)
        {
            index++;
        }
    }

    public void TakeDamage(float damage)
    {
        if (removed) return;
        hp -= damage;
        UpdateHpBar();
        if (hp <= 0)
        {
            Die(true);
        }
    }

    private void ReachCore()
    {
        if (removed) return;
        removed = true;
        if (game != null)
        {
            game.DamageCore(coreDamage);
            game.RemoveEnemy(this, false);
        }
        Destroy(gameObject);
    }

    private void Die(bool killedByHero)
    {
        if (removed) return;
        removed = true;
        if (game != null) game.RemoveEnemy(this, killedByHero);
        Destroy(gameObject);
    }

    private void CreateHpBar()
    {
        GameObject bg = new GameObject("HP_Background");
        bg.transform.SetParent(transform, false);
        bg.transform.localPosition = new Vector3(0, 0.7f, 0);
        bg.transform.localScale = new Vector3(1.15f, 0.16f, 1);
        var bgSr = bg.AddComponent<SpriteRenderer>();
        bgSr.sprite = CreateSmallSquare();
        bgSr.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        bgSr.sortingOrder = 50;

        GameObject fill = new GameObject("HP_Fill");
        fill.transform.SetParent(bg.transform, false);
        fill.transform.localPosition = Vector3.zero;
        fill.transform.localScale = Vector3.one;
        var fillSr = fill.AddComponent<SpriteRenderer>();
        fillSr.sprite = CreateSmallSquare();
        fillSr.color = new Color(1f, 0.1f, 0.1f, 1f);
        fillSr.sortingOrder = 51;
        hpBar = fill.transform;
    }

    private void UpdateHpBar()
    {
        if (hpBar == null || maxHp <= 0) return;
        float ratio = Mathf.Clamp01(hp / maxHp);
        hpBar.localScale = new Vector3(ratio, 1, 1);
        hpBar.localPosition = new Vector3((ratio - 1f) * 0.5f, 0, 0);
    }

    private Sprite CreateSmallSquare()
    {
        Texture2D tex = new Texture2D(4, 4);
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++) tex.SetPixel(x, y, Color.white);
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
    }
}

public class NKPlayableHero : MonoBehaviour
{
    private NKPlayableMapGame game;
    private float range;
    private float attackInterval;
    private float damage;
    private float timer;

    public void Setup(NKPlayableMapGame mapGame, float attackRange, float interval, float attackDamage)
    {
        game = mapGame;
        range = attackRange;
        attackInterval = interval;
        damage = attackDamage;
        timer = Random.Range(0f, attackInterval);
    }

    private void Update()
    {
        if (game == null) return;
        timer -= Time.deltaTime;
        if (timer > 0) return;
        timer = attackInterval;

        NKPlayableEnemy target = FindTarget();
        if (target != null)
        {
            game.SpawnProjectile(transform.position, target, damage);
        }
    }

    private NKPlayableEnemy FindTarget()
    {
        List<NKPlayableEnemy> enemies = game.GetEnemies();
        NKPlayableEnemy best = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < enemies.Count; i++)
        {
            NKPlayableEnemy enemy = enemies[i];
            if (enemy == null) continue;
            float d = Vector3.Distance(transform.position, enemy.transform.position);
            if (d <= range && d < bestDistance)
            {
                best = enemy;
                bestDistance = d;
            }
        }
        return best;
    }
}

public class NKPlayableProjectile : MonoBehaviour
{
    private NKPlayableEnemy target;
    private float damage;
    private float speed;

    public void Setup(NKPlayableEnemy enemyTarget, float projectileDamage, float projectileSpeed)
    {
        target = enemyTarget;
        damage = projectileDamage;
        speed = projectileSpeed;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.transform.position) <= 0.12f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
