using UnityEngine;
using TMPro;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    public TMP_Text comboText;
    private int comboMultiplier = 1;
    private int comboCount = 0;
    private float comboTimer;
    public float maxComboTime = 2f;
    public AnimasiText Anim;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Update()
    {
        if (comboMultiplier >= 1)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0.1)
            {
                ResetCombo();
            }
        }
    }

    public void IncreaseCombo()
    {
        comboCount++;
        comboMultiplier = Mathf.Min(comboCount, 5); // Batas x5 multiplier
        comboTimer = maxComboTime; // Reset timer

        comboText.text = $"Combo x{comboMultiplier}!";
        Anim.Show();
    }

    public void ResetCombo()
    {
        comboCount = 0;
        comboMultiplier = 1;
        Anim.Hide();
    }

    public int GetComboMultiplier()
    {
        return comboMultiplier;
    }
}
