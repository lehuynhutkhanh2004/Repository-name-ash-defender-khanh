using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LHNK-Map: scene tự tạo map khi mở/chạy.
// Không cần prefab, không cần tile palette, không cần asset ngoài.
// Mở scene Assets/Scenes/LHNK-Map-Playable.unity rồi bấm Play là chạy được.
[ExecuteAlways]
public class LHNKPlayableMapGame : MonoBehaviour
{
    [Header("Game Settings")]
    public int startingCoreHp = 120;
    public int startingGold = 140;
    public int heroCost = 25;

    private int coreHp;
    private int gold;
    private int wave;
    private bool isRunning;
    private bool isGameOver;
    private bool isWin;

    private readonly List<LHNKEnemy> enemies = new List<LHNKEnemy>();
    private readonly List<LHNKHero> heroes = new List<LHNKHero>();
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

        gameObject.name = "LHNK_Map_GameManager";
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

        gameObject.name = "LHNK_Map_GameManager";
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

        GUI.Box(new Rect(12, 12, 225, 108), "");
        GUI.Label(new Rect(24, 22, 200, 26), "Core HP: " + coreHp + "/" + startingCoreHp, smallStyle);
        GUI.Label(new Rect(24, 47, 200, 26), "Gold: " + gold, smallStyle);
        GUI.Label(new Rect(24, 72, 200, 26), "Wave: " + wave + "/3", smallStyle);

        if (!isRunning)
        {
            string title = isGameOver ? "DEFEAT" : (isWin ? "VICTORY" : "LHNK Forest Map");
            GUI.Label(new Rect(Screen.width / 2f - 190, Screen.height / 2f - 92, 380, 42), title, titleStyle);

            string buttonText = isGameOver || isWin ? "Restart" : "Start";
            if (GUI.Button(new Rect(Screen.width / 2f - 70, Screen.height / 2f - 42, 140, 38), buttonText))
            {
                StartGame();
            }

            GUI.Label(new Rect(Screen.width / 2f - 265, Screen.height / 2f + 8, 530, 86),
                "Click vào các ô xanh để đặt hero/tower.\nEnemy sẽ đi từ 3 cổng vào về Core bên phải.\nMap này dùng layout giống mẫu nhưng hình ảnh/trang trí khác.", smallStyle);
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width - 96, 14, 84, 32), Time.timeScale > 0 ? "Pause" : "Resume"))
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

    public bool TryBuildHero(LHNKHeroSlot slot)
    {
        if (!Application.isPlaying) return false;
        if (!isRunning || isGameOver || isWin) return false;
        if (slot == null || slot.HasHero) return false;
        if (gold < heroCost) return false;

        gold -= heroCost;
        slot.HasHero = true;

        GameObject baseObj = CreateVisualObject("Hero_TowerBase", slot.transform.position + new Vector3(0, 0, -0.04f), new Vector3(0.58f, 0.58f, 1), new Color(0.12f, 0.44f, 0.92f, 1f), heroSlotsContainer, 34, false);
        GameObject orb = CreateVisualObject("Hero_Orb", slot.transform.position + new Vector3(0, 0.06f, -0.06f), new Vector3(0.33f, 0.33f, 1), new Color(0.25f, 0.88f, 1f, 1f), heroSlotsContainer, 35, true);
        orb.transform.SetParent(baseObj.transform, true);

        var hero = baseObj.AddComponent<LHNKHero>();
        hero.Setup(this, 3.25f, 0.62f, 16);
        heroes.Add(hero);
        return true;
    }

    public void RegisterEnemy(LHNKEnemy enemy)
    {
        if (!enemies.Contains(enemy)) enemies.Add(enemy);
    }

    public void RemoveEnemy(LHNKEnemy enemy, bool killedByHero)
    {
        enemies.Remove(enemy);
        if (killedByHero)
        {
            gold += enemy.goldReward;
        }
    }

    public List<LHNKEnemy> GetEnemies()
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

    public LHNKProjectile SpawnProjectile(Vector3 start, LHNKEnemy target, float damage)
    {
        if (target == null) return null;

        GameObject bullet = CreateVisualObject("Projectile_GlowSeed", start, new Vector3(0.17f, 0.17f, 1), new Color(1f, 0.93f, 0.25f, 1f), projectileContainer, 45, true);
        var projectile = bullet.AddComponent<LHNKProjectile>();
        projectile.Setup(target, damage, 9.5f);
        return projectile;
    }

    private IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(0.45f);

        for (wave = 1; wave <= 3; wave++)
        {
            int count = 7 + wave * 2;
            for (int i = 0; i < count; i++)
            {
                List<Vector3> selectedPath = (i % 2 == 0) ? pathA : pathB;
                SpawnEnemy(selectedPath, false, wave);
                yield return new WaitForSeconds(Mathf.Max(0.42f, 0.88f - wave * 0.12f));
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
            gold += 40;
            yield return new WaitForSeconds(0.8f);
        }

        isWin = true;
        isRunning = false;
    }

    private void SpawnEnemy(List<Vector3> path, bool isBossEnemy, int waveNumber)
    {
        if (path == null || path.Count == 0) return;

        Color color = isBossEnemy ? new Color(0.93f, 0.30f, 0.20f, 1f) : new Color(0.30f, 0.95f, 0.48f, 1f);
        Vector3 scale = isBossEnemy ? new Vector3(0.76f, 0.76f, 1) : new Vector3(0.44f, 0.44f, 1);
        Transform parent = isBossEnemy ? bossContainer : enemyContainer;
        GameObject obj = CreateVisualObject(isBossEnemy ? "Boss_AncientGolem" : "Enemy_ForestSlime", path[0], scale, color, parent, 30, true);

        if (isBossEnemy)
        {
            CreateVisualObject("Boss_Core", path[0] + new Vector3(0, 0, -0.02f), new Vector3(0.38f, 0.38f, 1), new Color(0.55f, 0.12f, 0.12f, 1), obj.transform, 31, false);
        }

        var enemy = obj.AddComponent<LHNKEnemy>();
        float hp = isBossEnemy ? 250f : 38f + waveNumber * 13f;
        float speed = isBossEnemy ? 1.0f : 1.48f + waveNumber * 0.12f;
        int coreDamage = isBossEnemy ? 38 : 10;
        int reward = isBossEnemy ? 90 : 10;
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
            var slots = heroSlotsContainer.GetComponentsInChildren<LHNKHeroSlot>(true);
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
                if (child.name.StartsWith("Hero_")) oldHeroes.Add(child.gameObject);
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
        Transform gameplay = FindOrCreateRoot("GameplayObjects_LHNK").transform;
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
            pathA.Add(new Vector3(-7.25f, 0f, 0));
            pathA.Add(new Vector3(-3.2f, 0f, 0));
            pathA.Add(new Vector3(0f, 0f, 0));
            pathA.Add(new Vector3(3.75f, 0f, 0));
            pathA.Add(new Vector3(6.55f, 0f, 0));
        }
        if (pathB.Count == 0)
        {
            pathB.Add(new Vector3(0f, 3.72f, 0));
            pathB.Add(new Vector3(0f, 1.18f, 0));
            pathB.Add(new Vector3(0f, 0f, 0));
            pathB.Add(new Vector3(3.75f, 0f, 0));
            pathB.Add(new Vector3(6.55f, 0f, 0));
        }
        if (pathBoss.Count == 0)
        {
            pathBoss.Add(new Vector3(-4.75f, -3.35f, 0));
            pathBoss.Add(new Vector3(-3.7f, -2.25f, 0));
            pathBoss.Add(new Vector3(-1.55f, -0.65f, 0));
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
        var slots = heroSlotsContainer.GetComponentsInChildren<LHNKHeroSlot>(true);
        foreach (var slot in slots)
        {
            slot.SetGame(this);
        }
    }

    private void EnsureGeneratedMap()
    {
        EnsureCamera();
        Transform grid = FindOrCreateRoot("Grid_LHNK_ForestRuin").transform;
        Transform gameplay = FindOrCreateRoot("GameplayObjects_LHNK").transform;
        FindOrCreateRoot("UIController_LHNK");
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
        cam.orthographicSize = 4.85f;
        cam.backgroundColor = new Color(0.06f, 0.08f, 0.09f, 1f);
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    private void BuildGrid(Transform grid)
    {
        Transform background = FindOrCreateChild(grid, "Forest_Background").transform;
        Transform ground = FindOrCreateChild(grid, "Ground_Details").transform;
        Transform path = FindOrCreateChild(grid, "Stone_Roads").transform;
        Transform walls = FindOrCreateChild(grid, "Ruined_Walls").transform;
        Transform decorations = FindOrCreateChild(grid, "Forest_Decorations").transform;
        Transform buildSlot = FindOrCreateChild(grid, "BuildSlot_Visuals").transform;
        Transform portals = FindOrCreateChild(grid, "Magic_Portals").transform;
        Transform coreDecor = FindOrCreateChild(grid, "Core_Decoration").transform;

        CreateOrUpdateRect(background, "DeepForest_Background", Vector3.zero, new Vector3(15.8f, 8.35f, 1), new Color(0.08f, 0.13f, 0.12f, 1), 0);

        int idx = 0;
        for (int x = -7; x <= 7; x++)
        {
            for (int y = -4; y <= 4; y++)
            {
                float v = ((x * 17 + y * 31) % 5) * 0.012f;
                Color c = ((x + y) % 2 == 0) ? new Color(0.16f + v, 0.32f + v, 0.17f, 1) : new Color(0.12f + v, 0.27f + v, 0.14f, 1);
                CreateOrUpdateRect(ground, "GrassTile_" + idx++, new Vector3(x, y, 0), new Vector3(0.98f, 0.98f, 1), c, 1);
            }
        }

        BuildPathSegment(path, "Road_Main", new Vector3(-7.35f, 0f, -0.025f), new Vector3(6.75f, 0f, -0.025f), 0.82f, new Color(0.72f, 0.70f, 0.62f, 1), 6);
        BuildPathSegment(path, "Road_Top", new Vector3(0f, 0f, -0.025f), new Vector3(0f, 3.75f, -0.025f), 0.82f, new Color(0.73f, 0.71f, 0.63f, 1), 6);
        BuildPathSegment(path, "Road_Boss", new Vector3(-4.75f, -3.35f, -0.025f), new Vector3(0f, 0f, -0.025f), 0.86f, new Color(0.67f, 0.63f, 0.54f, 1), 6);
        CreateOrUpdateCircle(path, "Road_Center_Round", new Vector3(0f, 0f, -0.03f), new Vector3(1.18f, 1.18f, 1), new Color(0.69f, 0.67f, 0.58f, 1), 7);
        CreateOrUpdateCircle(path, "Road_Center_Moss", new Vector3(0.03f, -0.02f, -0.035f), new Vector3(0.75f, 0.75f, 1), new Color(0.45f, 0.54f, 0.34f, 0.35f), 8);

        BuildOuterRuins(walls);
        BuildPortals(portals);
        BuildCoreDecoration(coreDecor);
        BuildForestDecoration(decorations);
        BuildBuildSlots(buildSlot);
    }

    private void BuildPathSegment(Transform parent, string prefix, Vector3 start, Vector3 end, float width, Color baseColor, int order)
    {
        Vector3 mid = (start + end) * 0.5f;
        float length = Vector3.Distance(start, end);
        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
        CreateOrUpdateRect(parent, prefix + "_Base", mid, new Vector3(length, width, 1), baseColor, order, angle);
        CreateOrUpdateRect(parent, prefix + "_MossEdgeA", mid + Perp(start, end) * (width * 0.58f), new Vector3(length, 0.16f, 1), new Color(0.18f, 0.38f, 0.17f, 1), order + 1, angle);
        CreateOrUpdateRect(parent, prefix + "_MossEdgeB", mid - Perp(start, end) * (width * 0.58f), new Vector3(length, 0.16f, 1), new Color(0.18f, 0.38f, 0.17f, 1), order + 1, angle);

        int count = Mathf.Max(4, Mathf.RoundToInt(length * 2.05f));
        Vector3 dir = (end - start).normalized;
        Vector3 perp = Perp(start, end);
        for (int i = 0; i <= count; i++)
        {
            float t = (count == 0) ? 0 : i / (float)count;
            Vector3 p = Vector3.Lerp(start, end, t);
            float off = (i % 2 == 0) ? -width * 0.18f : width * 0.18f;
            float lenMul = (i % 3 == 0) ? 0.34f : 0.28f;
            Color stone = (i % 2 == 0) ? new Color(0.82f, 0.80f, 0.70f, 1) : new Color(0.64f, 0.63f, 0.56f, 1);
            CreateOrUpdateRect(parent, prefix + "_Stone_" + i.ToString("00"), p + perp * off + new Vector3(0, 0, -0.01f), new Vector3(lenMul, 0.22f, 1), stone, order + 2, angle);
        }
    }

    private Vector3 Perp(Vector3 a, Vector3 b)
    {
        Vector3 dir = (b - a).normalized;
        return new Vector3(-dir.y, dir.x, 0);
    }

    private void BuildOuterRuins(Transform walls)
    {
        Color wallDark = new Color(0.20f, 0.24f, 0.23f, 1);
        Color wallLight = new Color(0.39f, 0.45f, 0.41f, 1);
        CreateOrUpdateRect(walls, "Boundary_Top_Dark", new Vector3(0f, 4.18f, -0.01f), new Vector3(15.9f, 0.35f, 1), wallDark, 12);
        CreateOrUpdateRect(walls, "Boundary_Bottom_Dark", new Vector3(0f, -4.18f, -0.01f), new Vector3(15.9f, 0.35f, 1), wallDark, 12);
        CreateOrUpdateRect(walls, "Boundary_Left_Dark", new Vector3(-7.86f, 0f, -0.01f), new Vector3(0.34f, 8.25f, 1), wallDark, 12);
        CreateOrUpdateRect(walls, "Boundary_Right_Dark", new Vector3(7.86f, 0f, -0.01f), new Vector3(0.34f, 8.25f, 1), wallDark, 12);

        for (int i = 0; i < 17; i++)
        {
            float x = -7.2f + i * 0.9f;
            float y = (i % 3 == 0) ? 3.92f : 4.0f;
            CreateOrUpdateRect(walls, "Top_RuinStone_" + i, new Vector3(x, y, -0.02f), new Vector3(0.45f, 0.42f, 1), (i % 2 == 0) ? wallLight : wallDark, 14);
        }
        for (int i = 0; i < 13; i++)
        {
            float x = -6.9f + i * 1.1f;
            CreateOrUpdateRect(walls, "Bottom_RuinStone_" + i, new Vector3(x, -3.95f, -0.02f), new Vector3(0.50f, 0.34f, 1), (i % 2 == 0) ? wallLight : wallDark, 14);
        }

        BuildPillar(walls, "Pillar_TopLeft", new Vector3(-6.1f, 3.35f, 0));
        BuildPillar(walls, "Pillar_TopRight", new Vector3(5.9f, 3.25f, 0));
        BuildPillar(walls, "Pillar_BottomRight", new Vector3(5.2f, -3.35f, 0));
        BuildPillar(walls, "Pillar_BottomLeft", new Vector3(-6.2f, -3.25f, 0));
    }

    private void BuildPillar(Transform parent, string prefix, Vector3 pos)
    {
        CreateOrUpdateRect(parent, prefix + "_Base", pos, new Vector3(0.45f, 0.28f, 1), new Color(0.30f, 0.34f, 0.32f, 1), 16);
        CreateOrUpdateRect(parent, prefix + "_Column", pos + new Vector3(0, 0.22f, -0.01f), new Vector3(0.32f, 0.55f, 1), new Color(0.42f, 0.48f, 0.44f, 1), 17);
        CreateOrUpdateCircle(parent, prefix + "_Moss", pos + new Vector3(0.12f, 0.47f, -0.02f), new Vector3(0.16f, 0.12f, 1), new Color(0.25f, 0.55f, 0.22f, 1), 18);
    }

    private void BuildPortals(Transform portals)
    {
        BuildPortal(portals, "Portal_Left", new Vector3(-7.25f, 0f, -0.06f), 0f);
        BuildPortal(portals, "Portal_Top", new Vector3(0f, 3.75f, -0.06f), 90f);
        BuildPortal(portals, "Portal_BottomLeft", new Vector3(-4.75f, -3.35f, -0.06f), -36f);

        BuildArrow(portals, "Arrow_01", new Vector3(-6.2f, 0f, -0.07f), 0f);
        BuildArrow(portals, "Arrow_02", new Vector3(-5.25f, 0f, -0.07f), 0f);
        BuildArrow(portals, "Arrow_03", new Vector3(-4.3f, 0f, -0.07f), 0f);
    }

    private void BuildPortal(Transform parent, string prefix, Vector3 pos, float angle)
    {
        CreateOrUpdateCircle(parent, prefix + "_Glow", pos, new Vector3(0.92f, 0.92f, 1), new Color(0.54f, 0.24f, 1f, 0.55f), 24);
        CreateOrUpdateCircle(parent, prefix + "_Inside", pos, new Vector3(0.58f, 0.58f, 1), new Color(0.12f, 0.04f, 0.25f, 0.95f), 25);
        CreateOrUpdateCircle(parent, prefix + "_Rune", pos, new Vector3(0.35f, 0.35f, 1), new Color(0.76f, 0.54f, 1f, 0.85f), 26);
        CreateOrUpdateRect(parent, prefix + "_StoneA", pos + new Vector3(-0.36f, -0.35f, -0.01f), new Vector3(0.24f, 0.56f, 1), new Color(0.39f, 0.43f, 0.40f, 1), 27, angle);
        CreateOrUpdateRect(parent, prefix + "_StoneB", pos + new Vector3(0.36f, -0.35f, -0.01f), new Vector3(0.24f, 0.56f, 1), new Color(0.39f, 0.43f, 0.40f, 1), 27, angle);
    }

    private void BuildArrow(Transform parent, string prefix, Vector3 pos, float angle)
    {
        CreateOrUpdateRect(parent, prefix + "_A", pos + new Vector3(-0.06f, 0.10f, 0), new Vector3(0.30f, 0.12f, 1), new Color(0.23f, 1f, 0.37f, 0.9f), 28, angle + 45f);
        CreateOrUpdateRect(parent, prefix + "_B", pos + new Vector3(-0.06f, -0.10f, 0), new Vector3(0.30f, 0.12f, 1), new Color(0.23f, 1f, 0.37f, 0.9f), 28, angle - 45f);
    }

    private void BuildCoreDecoration(Transform parent)
    {
        CreateOrUpdateCircle(parent, "Core_OuterRing", new Vector3(6.65f, 0f, -0.05f), new Vector3(1.22f, 1.22f, 1), new Color(0.24f, 0.24f, 0.24f, 1), 22);
        CreateOrUpdateCircle(parent, "Core_InnerStone", new Vector3(6.65f, 0f, -0.06f), new Vector3(0.92f, 0.92f, 1), new Color(0.45f, 0.42f, 0.36f, 1), 23);
        CreateOrUpdateRect(parent, "Core_GoldDiamond", new Vector3(6.65f, 0f, -0.07f), new Vector3(0.58f, 0.58f, 1), new Color(1f, 0.66f, 0.10f, 1), 25, 45f);
        CreateOrUpdateCircle(parent, "Core_Glow", new Vector3(6.65f, 0f, -0.08f), new Vector3(0.35f, 0.35f, 1), new Color(1f, 0.88f, 0.30f, 0.9f), 26);
        CreateOrUpdateRect(parent, "Core_BannerTop", new Vector3(6.05f, 0.72f, -0.04f), new Vector3(0.22f, 0.70f, 1), new Color(0.05f, 0.23f, 0.57f, 1), 24);
        CreateOrUpdateRect(parent, "Core_BannerBottom", new Vector3(6.05f, -0.72f, -0.04f), new Vector3(0.22f, 0.70f, 1), new Color(0.05f, 0.23f, 0.57f, 1), 24);
        CreateOrUpdateRect(parent, "Core_BannerRight", new Vector3(7.24f, 0f, -0.04f), new Vector3(0.20f, 0.72f, 1), new Color(0.05f, 0.23f, 0.57f, 1), 24);
    }

    private void BuildForestDecoration(Transform deco)
    {
        Vector3[] trees =
        {
            new Vector3(-6.7f, 3.1f, 0), new Vector3(-5.9f, 2.45f, 0), new Vector3(-6.7f, -2.9f, 0), new Vector3(-5.8f, -3.3f, 0),
            new Vector3(5.7f, 2.7f, 0), new Vector3(6.8f, 2.9f, 0), new Vector3(6.7f, -2.8f, 0), new Vector3(5.8f, -3.2f, 0),
            new Vector3(-1.4f, 3.05f, 0), new Vector3(2.6f, 3.15f, 0), new Vector3(-0.8f, -3.1f, 0), new Vector3(2.8f, -3.2f, 0)
        };
        for (int i = 0; i < trees.Length; i++) BuildTree(deco, "Tree_" + i.ToString("00"), trees[i], (i % 3 == 0) ? 1.05f : 0.9f);

        Vector3[] crystals =
        {
            new Vector3(-6.35f, 2.35f, 0), new Vector3(5.25f, 2.55f, 0), new Vector3(-6.35f, -2.25f, 0), new Vector3(5.35f, -2.55f, 0)
        };
        for (int i = 0; i < crystals.Length; i++) BuildCrystal(deco, "Crystal_" + i, crystals[i]);

        Vector3[] ruins =
        {
            new Vector3(-3.5f, 2.55f, 0), new Vector3(3.15f, 2.6f, 0), new Vector3(-3.2f, -2.6f, 0), new Vector3(3.2f, -2.45f, 0), new Vector3(1.4f, 2.85f, 0), new Vector3(4.4f, -2.9f, 0)
        };
        for (int i = 0; i < ruins.Length; i++) BuildRuin(deco, "SmallRuin_" + i, ruins[i]);

        Vector3[] flowers =
        {
            new Vector3(-5.4f, 1.15f, 0), new Vector3(-3.9f, 2.0f, 0), new Vector3(-1.2f, 1.35f, 0), new Vector3(1.0f, 1.2f, 0),
            new Vector3(3.8f, 1.0f, 0), new Vector3(5.2f, 1.7f, 0), new Vector3(-5.8f, -0.9f, 0), new Vector3(-2.0f, -2.9f, 0),
            new Vector3(0.7f, -2.5f, 0), new Vector3(3.6f, -2.8f, 0), new Vector3(5.3f, -1.2f, 0)
        };
        for (int i = 0; i < flowers.Length; i++) BuildFlowerPatch(deco, "FlowerPatch_" + i, flowers[i]);

        BuildPond(deco, "Pond_BottomRight", new Vector3(4.9f, -3.3f, 0));
    }

    private void BuildTree(Transform parent, string prefix, Vector3 pos, float scale)
    {
        CreateOrUpdateRect(parent, prefix + "_Trunk", pos + new Vector3(0, -0.18f, -0.01f), new Vector3(0.16f * scale, 0.36f * scale, 1), new Color(0.26f, 0.15f, 0.08f, 1), 18);
        CreateOrUpdateCircle(parent, prefix + "_LeafA", pos + new Vector3(-0.16f * scale, 0.12f * scale, -0.02f), new Vector3(0.45f * scale, 0.42f * scale, 1), new Color(0.07f, 0.28f, 0.12f, 1), 19);
        CreateOrUpdateCircle(parent, prefix + "_LeafB", pos + new Vector3(0.15f * scale, 0.10f * scale, -0.02f), new Vector3(0.47f * scale, 0.43f * scale, 1), new Color(0.09f, 0.36f, 0.14f, 1), 20);
        CreateOrUpdateCircle(parent, prefix + "_LeafC", pos + new Vector3(0, 0.34f * scale, -0.03f), new Vector3(0.42f * scale, 0.42f * scale, 1), new Color(0.12f, 0.42f, 0.18f, 1), 21);
    }

    private void BuildCrystal(Transform parent, string prefix, Vector3 pos)
    {
        CreateOrUpdateCircle(parent, prefix + "_Glow", pos, new Vector3(0.62f, 0.62f, 1), new Color(0.08f, 0.78f, 1f, 0.28f), 17);
        CreateOrUpdateRect(parent, prefix + "_Main", pos + new Vector3(0, 0.05f, -0.01f), new Vector3(0.28f, 0.58f, 1), new Color(0.15f, 0.84f, 1f, 1), 22, 45f);
        CreateOrUpdateRect(parent, prefix + "_SideA", pos + new Vector3(-0.23f, -0.05f, -0.01f), new Vector3(0.18f, 0.36f, 1), new Color(0.06f, 0.55f, 0.85f, 1), 21, 35f);
        CreateOrUpdateRect(parent, prefix + "_SideB", pos + new Vector3(0.23f, -0.04f, -0.01f), new Vector3(0.18f, 0.34f, 1), new Color(0.08f, 0.65f, 0.95f, 1), 21, -35f);
    }

    private void BuildRuin(Transform parent, string prefix, Vector3 pos)
    {
        CreateOrUpdateRect(parent, prefix + "_StoneA", pos + new Vector3(-0.22f, 0, 0), new Vector3(0.35f, 0.30f, 1), new Color(0.35f, 0.40f, 0.37f, 1), 15);
        CreateOrUpdateRect(parent, prefix + "_StoneB", pos + new Vector3(0.18f, 0.05f, 0), new Vector3(0.42f, 0.24f, 1), new Color(0.44f, 0.48f, 0.44f, 1), 16, 8f);
        CreateOrUpdateCircle(parent, prefix + "_Moss", pos + new Vector3(0.05f, 0.22f, -0.01f), new Vector3(0.20f, 0.12f, 1), new Color(0.21f, 0.50f, 0.17f, 1), 17);
    }

    private void BuildFlowerPatch(Transform parent, string prefix, Vector3 pos)
    {
        Color a = new Color(0.78f, 0.42f, 1f, 1);
        Color b = new Color(0.20f, 0.75f, 1f, 1);
        Color c = new Color(1f, 0.86f, 0.35f, 1);
        CreateOrUpdateCircle(parent, prefix + "_A", pos + new Vector3(-0.08f, 0.02f, -0.01f), new Vector3(0.08f, 0.08f, 1), a, 18);
        CreateOrUpdateCircle(parent, prefix + "_B", pos + new Vector3(0.08f, -0.02f, -0.01f), new Vector3(0.07f, 0.07f, 1), b, 18);
        CreateOrUpdateCircle(parent, prefix + "_C", pos + new Vector3(0.02f, 0.11f, -0.01f), new Vector3(0.06f, 0.06f, 1), c, 18);
    }

    private void BuildPond(Transform parent, string prefix, Vector3 pos)
    {
        CreateOrUpdateCircle(parent, prefix + "_Water", pos, new Vector3(0.95f, 0.45f, 1), new Color(0.05f, 0.30f, 0.34f, 0.92f), 16);
        CreateOrUpdateCircle(parent, prefix + "_Shine", pos + new Vector3(-0.2f, 0.07f, -0.01f), new Vector3(0.32f, 0.12f, 1), new Color(0.35f, 0.78f, 0.82f, 0.65f), 17);
        CreateOrUpdateCircle(parent, prefix + "_LilyA", pos + new Vector3(0.18f, -0.05f, -0.02f), new Vector3(0.14f, 0.08f, 1), new Color(0.19f, 0.58f, 0.19f, 1), 18);
        CreateOrUpdateCircle(parent, prefix + "_LilyB", pos + new Vector3(-0.25f, -0.10f, -0.02f), new Vector3(0.12f, 0.07f, 1), new Color(0.24f, 0.64f, 0.22f, 1), 18);
    }

    private void BuildBuildSlots(Transform buildSlot)
    {
        Vector3[] slots = GetSlotPositions();
        for (int i = 0; i < slots.Length; i++)
        {
            Vector3 p = slots[i];
            CreateOrUpdateRect(buildSlot, "BuildSlotGlow_" + (i + 1).ToString("00"), p, new Vector3(0.86f, 0.86f, 1), new Color(0.04f, 0.70f, 1f, 0.25f), 20);
            CreateOrUpdateRect(buildSlot, "BuildSlotPlate_" + (i + 1).ToString("00"), p + new Vector3(0, 0, -0.01f), new Vector3(0.70f, 0.70f, 1), new Color(0.21f, 0.48f, 0.54f, 0.80f), 21);
            CreateOrUpdateRect(buildSlot, "BuildSlotRune_" + (i + 1).ToString("00"), p + new Vector3(0, 0, -0.02f), new Vector3(0.27f, 0.27f, 1), new Color(0.40f, 0.90f, 1f, 0.75f), 22, 45f);
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

        CreateOrUpdateCircle(spawnPoints, "SpawnPoint_Left", new Vector3(-7.25f, 0f, -0.08f), new Vector3(0.42f, 0.42f, 1), new Color(0.60f, 0.25f, 1f, 0.55f), 32);
        CreateOrUpdateCircle(spawnPoints, "SpawnPoint_Top", new Vector3(0f, 3.72f, -0.08f), new Vector3(0.42f, 0.42f, 1), new Color(0.60f, 0.25f, 1f, 0.55f), 32);
        CreateOrUpdateCircle(spawnPoints, "BossSpawnPoint_BottomLeft", new Vector3(-4.75f, -3.35f, -0.08f), new Vector3(0.48f, 0.48f, 1), new Color(0.72f, 0.25f, 1f, 0.58f), 32);

        CreateOrUpdateCircle(baseRoot, "Core_ColliderVisual", new Vector3(6.65f, 0f, -0.08f), new Vector3(0.55f, 0.55f, 1), new Color(1f, 0.72f, 0.12f, 0.92f), 33);

        CreateOrUpdateRect(heroArea, "AllowedBuildArea_Top", new Vector3(0f, 2.08f, -0.04f), new Vector3(12.4f, 2.75f, 1), new Color(0.05f, 0.18f, 0.08f, 0.08f), 4);
        CreateOrUpdateRect(heroArea, "AllowedBuildArea_Bottom", new Vector3(0f, -2.28f, -0.04f), new Vector3(12.4f, 2.65f, 1), new Color(0.05f, 0.18f, 0.08f, 0.08f), 4);

        BuildPathObject(waypoints, "Path_A_LeftToCore", new Vector3[]
        {
            new Vector3(-7.25f, 0f, 0), new Vector3(-3.2f, 0f, 0), new Vector3(0f, 0f, 0), new Vector3(3.75f, 0f, 0), new Vector3(6.55f, 0f, 0)
        });

        BuildPathObject(waypoints, "Path_B_TopToCore", new Vector3[]
        {
            new Vector3(0f, 3.72f, 0), new Vector3(0f, 1.18f, 0), new Vector3(0f, 0f, 0), new Vector3(3.75f, 0f, 0), new Vector3(6.55f, 0f, 0)
        });

        BuildPathObject(waypoints, "BossPath_BottomLeftToCore", new Vector3[]
        {
            new Vector3(-4.75f, -3.35f, 0), new Vector3(-3.7f, -2.25f, 0), new Vector3(-1.55f, -0.65f, 0), new Vector3(0f, 0f, 0), new Vector3(6.55f, 0f, 0)
        });

        Vector3[] slots = GetSlotPositions();
        for (int i = 0; i < slots.Length; i++)
        {
            GameObject slot = CreateOrUpdateRect(heroSlots, "HeroSlot_" + (i + 1).ToString("00"), slots[i], new Vector3(0.76f, 0.76f, 1), new Color(0.30f, 0.82f, 1f, 0.33f), 30);
            var col = slot.GetComponent<BoxCollider2D>();
            if (col == null) col = slot.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;
            var slotScript = slot.GetComponent<LHNKHeroSlot>();
            if (slotScript == null) slotScript = slot.AddComponent<LHNKHeroSlot>();
            slotScript.SetGame(this);
        }
    }

    private void BuildPathObject(Transform parent, string name, Vector3[] points)
    {
        Transform path = FindOrCreateChild(parent, name).transform;
        for (int i = 0; i < points.Length; i++)
        {
            GameObject wp = FindOrCreateChild(path, "Waypoint_" + (i + 1).ToString("00"));
            wp.transform.position = points[i];
            wp.transform.localScale = Vector3.one;
        }
    }

    private Vector3[] GetSlotPositions()
    {
        return new Vector3[]
        {
            new Vector3(-4.85f, 1.55f, -0.05f),
            new Vector3(-2.10f, 1.62f, -0.05f),
            new Vector3(2.00f, 1.62f, -0.05f),
            new Vector3(4.35f, 1.32f, -0.05f),
            new Vector3(-5.35f, -1.55f, -0.05f),
            new Vector3(-2.35f, -1.90f, -0.05f),
            new Vector3(1.55f, -1.75f, -0.05f),
            new Vector3(4.05f, -1.35f, -0.05f)
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
        tex.name = "LHNK_Generated_SquareSprite";
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
        tex.name = "LHNK_Generated_CircleSprite";
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
            smallStyle.fontSize = 17;
            smallStyle.normal.textColor = Color.white;
            smallStyle.alignment = TextAnchor.UpperLeft;
        }
    }
}

public class LHNKHeroSlot : MonoBehaviour
{
    public bool HasHero { get; set; }
    private LHNKPlayableMapGame game;

    public void SetGame(LHNKPlayableMapGame mapGame)
    {
        game = mapGame;
    }

    private void OnMouseDown()
    {
        if (game != null) game.TryBuildHero(this);
    }
}

public class LHNKEnemy : MonoBehaviour
{
    public int goldReward;

    private LHNKPlayableMapGame game;
    private List<Vector3> path;
    private int index;
    private float hp;
    private float maxHp;
    private float speed;
    private int coreDamage;
    private bool removed;
    private Transform hpBar;

    public void Setup(LHNKPlayableMapGame mapGame, List<Vector3> waypoints, float health, float moveSpeed, int damageToCore, int reward)
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
        if (hp <= 0) Die(true);
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
        bg.transform.localScale = new Vector3(1.10f, 0.15f, 1);
        var bgSr = bg.AddComponent<SpriteRenderer>();
        bgSr.sprite = CreateSmallSquare();
        bgSr.color = new Color(0.05f, 0.05f, 0.05f, 0.9f);
        bgSr.sortingOrder = 55;

        GameObject fill = new GameObject("HP_Fill");
        fill.transform.SetParent(bg.transform, false);
        fill.transform.localPosition = Vector3.zero;
        fill.transform.localScale = Vector3.one;
        var fillSr = fill.AddComponent<SpriteRenderer>();
        fillSr.sprite = CreateSmallSquare();
        fillSr.color = new Color(1f, 0.16f, 0.10f, 1f);
        fillSr.sortingOrder = 56;
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

public class LHNKHero : MonoBehaviour
{
    private LHNKPlayableMapGame game;
    private float range;
    private float attackInterval;
    private float damage;
    private float timer;

    public void Setup(LHNKPlayableMapGame mapGame, float attackRange, float interval, float attackDamage)
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

        LHNKEnemy target = FindTarget();
        if (target != null)
        {
            game.SpawnProjectile(transform.position, target, damage);
        }
    }

    private LHNKEnemy FindTarget()
    {
        List<LHNKEnemy> enemies = game.GetEnemies();
        LHNKEnemy best = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < enemies.Count; i++)
        {
            LHNKEnemy enemy = enemies[i];
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

public class LHNKProjectile : MonoBehaviour
{
    private LHNKEnemy target;
    private float damage;
    private float speed;

    public void Setup(LHNKEnemy enemyTarget, float projectileDamage, float projectileSpeed)
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
