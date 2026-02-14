using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MafiaCatController : MonoBehaviour
{
    [Header("Percentuale rubata per livello (0..1)")]
    [Range(0f, 1f)] public float stealPercentLevel1 = 0.10f;
    [Range(0f, 1f)] public float stealPercentLevel2 = 0.20f;
    [Range(0f, 1f)] public float stealPercentLevel3 = 0.30f;

    [Header("Movimento")]
    public Transform waitPoint;
    public Transform exitPoint;
    public float moveSpeed = 3.5f;
    public float waitSecondsBeforeSteal = 1f;
    public float waitSecondsAfterSteal = 0.8f;

    // 🔹 Tiene memoria dei livelli in cui ha già rubato (durante questa run)
    private static HashSet<int> stolenLevels = new HashSet<int>();

    private void OnEnable()
    {
        // Se non siamo in modalità storia, il gatto non deve esistere
        if (!GameSettings.IsStoryMode)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (waitPoint == null || exitPoint == null)
        {
            Debug.LogError("[MafiaCat] waitPoint o exitPoint non assegnati!");
            return;
        }

        int currentLevel = Mathf.Clamp(GameSettings.SelectedLevel, 1, 3);

        // Se ha già rubato in questo livello, esce subito
        if (stolenLevels.Contains(currentLevel))
        {
            StartCoroutine(LeaveImmediately());
            return;
        }

        stolenLevels.Add(currentLevel);
        StartCoroutine(RobRoutine(currentLevel));
    }

    private IEnumerator RobRoutine(int level)
    {
        // 1️⃣ Vai al centro
        yield return MoveTo(waitPoint.position);

        // 2️⃣ Attesa "minacciosa"
        if (waitSecondsBeforeSteal > 0f)
            yield return new WaitForSeconds(waitSecondsBeforeSteal);

        // 3️⃣ Ruba
        float percent = GetPercentForLevel(level);
        StealPercent(percent);

        // 4️⃣ Attesa post furto
        if (waitSecondsAfterSteal > 0f)
            yield return new WaitForSeconds(waitSecondsAfterSteal);

        // 5️⃣ Esci
        yield return MoveTo(exitPoint.position);

        Destroy(gameObject);
    }

    private IEnumerator LeaveImmediately()
    {
        yield return MoveTo(exitPoint.position);
        Destroy(gameObject);
    }

    private float GetPercentForLevel(int level)
    {
        switch (level)
        {
            case 1: return stealPercentLevel1;
            case 2: return stealPercentLevel2;
            case 3: return stealPercentLevel3;
            default: return stealPercentLevel1;
        }
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while (Vector2.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    private void StealPercent(float percent)
    {
        if (EconomyManager.instance == null)
        {
            Debug.LogWarning("[MafiaCat] EconomyManager non trovato!");
            return;
        }

        percent = Mathf.Clamp01(percent);

        int currentCoins = EconomyManager.instance.CurrentCoins;
        int amountToSteal = Mathf.FloorToInt(currentCoins * percent);

        if (amountToSteal <= 0)
        {
            Debug.Log("[MafiaCat] Non c'è niente da rubare.");
            return;
        }

        EconomyManager.instance.SpendCoins(amountToSteal);

        Debug.Log($"[MafiaCat] Ha rubato {amountToSteal} monete ({percent * 100f:0}% del totale).");
    }

    // 🔹 Se vuoi resettare manualmente tra scene
    public static void ResetRunMemory()
    {
        stolenLevels.Clear();
    }
}
