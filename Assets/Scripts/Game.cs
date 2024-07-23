using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour {
    public static Game Instance;
    public static MainGameConfig MainGameConfig;

    [SerializeField]
    private MainGameConfig _mainGameConfig;

    [Header("Parameters")]
    public Transform ListsPanel;

    [SerializeField]
    private HayFactory _hayFactory;

    [SerializeField]
    private Text _totalScoreText;

    [SerializeField]
    private KeyboardButtonsController _keyboardButtonsController;

    [SerializeField]
    private Hook _hook;

    public Sprite[] upgradeSprites;

    public GameObject[] books;
    public Sprite[] emptyBookSprites;
    public Sprite[] halfBookSprites;
    public Sprite[] finishedBookSprites;

    public GameObject curBook;
    public Text lvlText;
    public Slider xpSlider;

    int _maxXp;

    public GameObject UpgradeHayButton;
    public GameObject UpgradeButtonsButton;
    public GameObject UpgradeBookButton;

    private void Awake() {
        Instance = this;
        MainGameConfig = _mainGameConfig;
    }

    private void Start() {
        _maxXp = MainGameConfig.XpNeeded[PshenicaSaveLoadManager.Profile.Lvl];
        CheckLvl();

        //StartCoroutine(HookHay());
        SetHayLvl();
        SetBookLvl();
        SetButtonsLvl();

        _keyboardButtonsController.OnLastButtonPressed += ButtonPressed;
        _keyboardButtonsController.OnNextButtonPressed += OnButtonPressed;

        _hook.OnHookCollectedHay += CollectHay;
        InvokeRepeating(nameof(TryUpdateLb), 3, 3);
        SetTotalScore();
        UpdateUpgradeButtonsActive();
    }

    void DropList() {
        GameObject list = Instantiate(curBook, ListsPanel);
        list.transform.SetAsFirstSibling();
        list.GetComponent<Image>().sprite = finishedBookSprites[PshenicaSaveLoadManager.Profile.BookUpgrade];
        SoundManager.Instance.PlaySound(Sounds.WriteProgress);
        Rigidbody2D rb = list.GetComponent<Rigidbody2D>();
        rb.simulated = true;

        float forcePower = 10;
        rb.AddTorque(Random.Range(-1f, 1f) * forcePower);
    }

    private void ButtonPressed() {
        DropList();
    }

    private void OnButtonPressed(float percent) {
        if (percent == 0)
            curBook.GetComponent<Image>().sprite = emptyBookSprites[PshenicaSaveLoadManager.Profile.BookUpgrade];
        else if (percent < 0.5f)
            curBook.GetComponent<Image>().sprite = halfBookSprites[PshenicaSaveLoadManager.Profile.BookUpgrade];
        else
            curBook.GetComponent<Image>().sprite = finishedBookSprites[PshenicaSaveLoadManager.Profile.BookUpgrade];
        //TODO add writing sound
        //SoundManager.Instance.PlaySound(Sounds.Button);
    }

    public void GrewNewHay() {
        if (_hayFactory.HayAmount > MainGameConfig.MaxHayOnScreen) {
            return;
        }

        int grewAmount = PshenicaSaveLoadManager.Profile.BookUpgrade + 1;

        for (int i = 0; i < grewAmount; i++) {
            Hay h = _hayFactory.GetHay();
            h.Init(PshenicaSaveLoadManager.Profile.HayUpgrade);
            h.Grow();
        }

        SoundManager.Instance.PlaySound(Sounds.Growth);
        if (grewAmount > 0) {
            _hook.TryStartMove();
        }
    }

    void CheckLvl() {
        if (IsAllUpgradesBought) {
            SetTotalScore();
            return;
        }

        if (PshenicaSaveLoadManager.Profile.Xp >= _maxXp) {
            PshenicaSaveLoadManager.Profile.Lvl++;
            PshenicaSaveLoadManager.Profile.Xp = 0;
            _maxXp = MainGameConfig.XpNeeded[PshenicaSaveLoadManager.Profile.Lvl];
            GetUpgradePoint();
        }

        xpSlider.maxValue = _maxXp;
        xpSlider.value = PshenicaSaveLoadManager.Profile.Xp;
        lvlText.text = PshenicaSaveLoadManager.Profile.Lvl.ToString();

        if (IsAllUpgradesBought) {
            SetTotalScore();
            YandexMetrica.Send("allUpgradesBought");
        }
    }

    private void SetTotalScore() {
        if (IsAllUpgradesBought) {
            _totalScoreText.gameObject.SetActive(true);
            lvlText.text = "??";
            _totalScoreText.text = PshenicaSaveLoadManager.Profile.Xp.ToString();
        } else {
            _totalScoreText.gameObject.SetActive(false);
        }
    }

    private bool IsAllUpgradesBought => PshenicaSaveLoadManager.Profile.HayUpgrade == 4 && PshenicaSaveLoadManager.Profile.BookUpgrade == 4 &&
                                        PshenicaSaveLoadManager.Profile.ButtonsUpgrade == 4;

    void GetUpgradePoint() {
        PshenicaSaveLoadManager.Profile.UpgradePoints++;

        SoundManager.Instance.PlaySound(Sounds.Collect);
        if (PshenicaSaveLoadManager.Profile.HayUpgrade < 4)
            UpgradeHayButton.SetActive(true);
        if (PshenicaSaveLoadManager.Profile.BookUpgrade < 4)
            UpgradeBookButton.SetActive(true);
        if (PshenicaSaveLoadManager.Profile.ButtonsUpgrade < 4)
            UpgradeButtonsButton.SetActive(true);

        PshenicaSaveLoadManager.Save();
    }

    public void UpgradeButton(int what) {
        Upgrade(what);
    }

    private void Upgrade(int what, bool isSkipAnalytics = false) {
        Dictionary<string, string> eventParams = new Dictionary<string, string>();
        switch (what) {
            case 0: // hay

                PshenicaSaveLoadManager.Profile.HayUpgrade++;
                eventParams.Add("hay", PshenicaSaveLoadManager.Profile.HayUpgrade.ToString());

                SetHayLvl();
                break;
            case 1: // book

                PshenicaSaveLoadManager.Profile.BookUpgrade++;
                eventParams.Add("book", PshenicaSaveLoadManager.Profile.BookUpgrade.ToString());

                SetBookLvl();
                break;
            case 2: // buttons

                PshenicaSaveLoadManager.Profile.ButtonsUpgrade++;
                eventParams.Add("buttons", PshenicaSaveLoadManager.Profile.ButtonsUpgrade.ToString());
                SetButtonsLvl();
                break;
        }

        PshenicaSaveLoadManager.Save();

        if (!isSkipAnalytics) {
            YandexMetrica.Send("upgradeBought", eventParams);
            SoundManager.Instance.PlaySound(Sounds.Upgrade);
        }

        PshenicaSaveLoadManager.Profile.UpgradePoints--;
        UpdateUpgradeButtonsActive();

        PshenicaSaveLoadManager.Save();
    }

    private void UpdateUpgradeButtonsActive() {
        if (PshenicaSaveLoadManager.Profile.UpgradePoints <= 0) {
            UpgradeHayButton.SetActive(false);
            UpgradeButtonsButton.SetActive(false);
            UpgradeBookButton.SetActive(false);
        }
    }

    private void SetButtonsLvl() {
        _keyboardButtonsController.SetUpgradeLevel(PshenicaSaveLoadManager.Profile.ButtonsUpgrade);

        if (PshenicaSaveLoadManager.Profile.ButtonsUpgrade == 4)
            UpgradeButtonsButton.SetActive(false);
        else
            UpgradeButtonsButton.GetComponent<Image>().sprite = upgradeSprites[PshenicaSaveLoadManager.Profile.ButtonsUpgrade];
    }

    private void SetBookLvl() {
        curBook.SetActive(false);
        curBook = books[PshenicaSaveLoadManager.Profile.BookUpgrade];
        curBook.SetActive(true);

        if (PshenicaSaveLoadManager.Profile.BookUpgrade == 4)
            UpgradeBookButton.SetActive(false);
        else
            UpgradeBookButton.GetComponent<Image>().sprite = upgradeSprites[PshenicaSaveLoadManager.Profile.BookUpgrade];
    }

    private void SetHayLvl() {
        if (PshenicaSaveLoadManager.Profile.HayUpgrade == 4) {
            _hook.Activate();
        }

        if (PshenicaSaveLoadManager.Profile.HayUpgrade == 4)
            UpgradeHayButton.SetActive(false);
        else
            UpgradeHayButton.GetComponent<Image>().sprite = upgradeSprites[PshenicaSaveLoadManager.Profile.HayUpgrade];
    }

    private void CollectHay() {
        foreach (Hay hay in FindObjectsOfType<Hay>()) {
            if (hay.CanBeCollected) {
                hay.StartCollecting();
            }
        }
    }

    public void CollectXp() {
        PshenicaSaveLoadManager.Profile.Xp += Mathf.FloorToInt(Mathf.Pow(2, PshenicaSaveLoadManager.Profile.HayUpgrade));
        _totalScoreText.text = PshenicaSaveLoadManager.Profile.Xp.ToString();
        CheckLvl();
        PshenicaSaveLoadManager.Save();
    }

    private void TryUpdateLb() {
        if (!IsAllUpgradesBought) {
            return;
        }

        int saved = PlayerPrefs.GetInt("totalScore", 0);
        if (PshenicaSaveLoadManager.Profile.Xp > saved) {
            PlayerPrefs.SetInt("totalScore", PshenicaSaveLoadManager.Profile.Xp);
            YandexGame.NewLeaderboardScores("totalScore", PshenicaSaveLoadManager.Profile.Xp);
        }
    }

    private void OnTriggerEnter2D(Collider2D coll) {
        Destroy(coll.gameObject, 0.3f);
        GrewNewHay();
    }

    public void Update() {
        CheckSwipe();
    }

    //inside class
    Vector2 _firstPressPos;
    Vector2 _secondPressPos;
    Vector2 _currentSwipe;

    void CheckSwipe() {
        if (Input.GetMouseButtonDown(0)) {
            //save began touch 2d point
            _firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0)) {
            //save ended touch 2d point
            _secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //create vector from the two points
            _currentSwipe = new Vector2(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);
            float distance = _secondPressPos.x - _firstPressPos.x;

            //normalize the 2d vector
            _currentSwipe.Normalize();

            //swipe right
            if (distance > Screen.width / 1.5f && _currentSwipe.x > 0.5f && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f) {
                CollectHay();
            }
        }

        if (Input.touches.Length > 0) {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) {
                //save began touch 2d point
                _firstPressPos = new Vector2(t.position.x, t.position.y);
            }

            if (t.phase == TouchPhase.Ended) {
                //save ended touch 2d point
                _secondPressPos = new Vector2(t.position.x, t.position.y);

                //create vector from the two points
                _currentSwipe = new Vector3(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);

                float distance = _secondPressPos.x - _firstPressPos.x;

                //normalize the 2d vector
                _currentSwipe.Normalize();

                //swipe right
                if (distance > Screen.width / 1.5f && _currentSwipe.x > 0.5f && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f) {
                    CollectHay();
                }
            }
        }
    }
}