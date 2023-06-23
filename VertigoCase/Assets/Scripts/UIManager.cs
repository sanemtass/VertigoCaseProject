using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WheelGame;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private ButtonHandler buttonHandler;
    [SerializeField] private WheelController wheelController;
    [SerializeField] private IndicatorController indicatorController;
    [SerializeField] public TextMeshProUGUI gainedItemText;

    [SerializeField] private ButtonHandler giveUpButtonHandler;
    [SerializeField] private ButtonHandler reviveButtonHandler;
    [SerializeField] private ButtonHandler exitButtonHandler;
    [SerializeField] private Button exitButton;

    public ButtonHandler ButtonHandler { get => buttonHandler; set => buttonHandler = value; }
    public WheelController WheelController { get => wheelController; set => wheelController = value; }
    public IndicatorController IndicatorController { get => indicatorController; set => indicatorController = value; }

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
        ButtonHandler.OnButtonClicked += HandleButtonClick;
        giveUpButtonHandler.OnButtonClicked += GiveUpGame;
        reviveButtonHandler.OnButtonClicked += RevivePlayer;
        exitButtonHandler.OnButtonClicked += ExitGame;
    }

    void Update()
    {
        if (GameManager.Instance.isSafeZone && !wheelController.isRotating)
        {
            exitButton.interactable = true;
        }
        else
        {
            exitButton.interactable = false;
        }
    }

    private void OnDestroy()
    {
        ButtonHandler.OnButtonClicked -= HandleButtonClick;
        giveUpButtonHandler.OnButtonClicked -= GiveUpGame;
        reviveButtonHandler.OnButtonClicked -= RevivePlayer;
        exitButtonHandler.OnButtonClicked -= ExitGame;
    }

    private void HandleButtonClick()
    {
        WheelController.RotateWheel();
    }

    public void ShowGainedItemText(string text)
    {
        gainedItemText.text = text;
        gainedItemText.gameObject.SetActive(true);
        gainedItemText.transform.localScale = Vector3.zero;
        gainedItemText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack)
            .OnComplete(() => StartCoroutine(HideGainedItemTextAfterDelay())); //Start the hiding coroutine when the scaling animation is complete
    }

    private IEnumerator HideGainedItemTextAfterDelay(float delay = 2f)
    {
        yield return new WaitForSeconds(delay);
        // Hide text
        gainedItemText.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => gainedItemText.gameObject.SetActive(false));
    }

    public void ShowBombPanel()
    {
        PauseGame();
        bombPanel.SetActive(true);
    }

    public void HideBombPanel()
    {
        bombPanel.SetActive(false);
        UnpauseGame();
    }

    public void GiveUpGame()
    {
        InventorySystem.Instance.ResetInventory();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        UnpauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
    }

    public void RevivePlayer()
    {
        const int ReviveCost = 25;

        if (PlayerData.Instance.Gold >= ReviveCost)
        {
            //Perform the animation and remove the gold if there is enough gold.
            PlayerData.Instance.RemoveGold(ReviveCost);
            bombPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Not enough gold for revive!");
        }

        WheelController.ChangeWheel(wheelController.wheel_value); //Refresh the wheel
        UnpauseGame(); //Unpause the game after the player revives
    }

    public void ExitGame()
    {
        if (GameManager.Instance.isSafeZone && !WheelController.isRotating)
        {
            InventorySystem.Instance.ResetInventory();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
