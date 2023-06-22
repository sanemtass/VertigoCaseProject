using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WheelGame;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private ButtonHandler buttonHandler;
    [SerializeField] private WheelController wheelController;
    [SerializeField] private IndicatorController indicatorController; // indicatorController'ı ekleyin.
    [SerializeField] public TextMeshProUGUI gainedItemText;

    [SerializeField] private ButtonHandler giveUpButtonHandler;
    [SerializeField] private ButtonHandler reviveButtonHandler;

    public ButtonHandler ButtonHandler { get => buttonHandler; set => buttonHandler = value; }
    public WheelController WheelController { get => wheelController; set => wheelController = value; }
    public IndicatorController IndicatorController { get => indicatorController; set => indicatorController = value; } // Property'yi ekleyin.

    public GameObject bombPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (bombPanel == null)
        {
            Debug.LogError("Grenade Panel is not set in the UIManager.");
        }
    }

    private void Start()
    {
        // Butonun tıklanma işlemine bir listener ekliyoruz
        ButtonHandler.OnButtonClicked += HandleButtonClick;
        giveUpButtonHandler.OnButtonClicked += GiveUpGame;
        reviveButtonHandler.OnButtonClicked += RevivePlayer;
    }

    private void OnDestroy()
    {
        // Listener'ı kaldırıyoruz. Bu önemli çünkü listener'ı kaldırmazsak, bu nesne yok edildiğinde hala bir referans olacağı için bellek sızıntısına neden olabilir.
        ButtonHandler.OnButtonClicked -= HandleButtonClick;
        giveUpButtonHandler.OnButtonClicked -= GiveUpGame;
        reviveButtonHandler.OnButtonClicked -= RevivePlayer;
    }


    private void HandleButtonClick()
    {
        // Butona tıklanıldığında çarkı döndür
        WheelController.RotateWheel();
    }

    public void ShowGainedItemText(string text)
    {
        gainedItemText.text = text;
        gainedItemText.gameObject.SetActive(true); // Make sure the text object is active
                                                   // Animate text
        gainedItemText.transform.localScale = Vector3.zero; // Start size
        gainedItemText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack) // Animate to normal size
            .OnComplete(() => StartCoroutine(HideGainedItemTextAfterDelay())); // Start the hiding coroutine when the scaling animation is complete
    }

    private IEnumerator HideGainedItemTextAfterDelay(float delay = 2f)
    {
        yield return new WaitForSeconds(delay);
        // Hide text
        gainedItemText.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack) // Animate to zero size
            .OnComplete(() => gainedItemText.gameObject.SetActive(false)); // Deactivate the text object when the scaling animation is complete
    }

    public void ShowBombPanel()
    {
        PauseGame(); // Add this
        bombPanel.SetActive(true);
    }

    public void HideBombPanel()
    {
        bombPanel.SetActive(false);
        UnpauseGame(); // Add this
    }

    public void GiveUpGame()
    {
        InventorySystem.Instance.ResetInventory();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        UnpauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Stops the game
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f; // Resumes the game
    }

    public void RevivePlayer()
    {
        const int ReviveCost = 25; // Canlandırma maliyetini belirleyin

        if (PlayerData.Instance.Gold >= ReviveCost)
        {
            // Yeterli altın varsa, canlandırma işlemini gerçekleştirin ve altınları çıkarın
            PlayerData.Instance.RemoveGold(ReviveCost);
            bombPanel.SetActive(false);
        }
        else
        {
            // Yeterli altın yoksa, bir uyarı mesajı gösterin veya başka bir işlem yapın
            Debug.Log("Not enough gold for revive!");
        }

        WheelController.ChangeWheel(wheelController.wheel); // Refresh the wheel
        UnpauseGame(); // Unpause the game after the player revives
    }

}
