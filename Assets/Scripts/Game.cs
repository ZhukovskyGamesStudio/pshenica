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

    [SerializeField]
    private int _startHayLevel;

    [SerializeField]
    private int _startBookLevel;

    [SerializeField]
    private int _startButtonsLevel;

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

    int _xp;
    int _maxXp;
    int _lvl;

    int _curHaylvl;
    int _curBooklvl;
    int _curButtonslvl;

    public GameObject UpgradeHayButton;
    public GameObject UpgradeButtonsButton;
    public GameObject UpgradeBookButton;

    int _haveUpgradePoints;

    private void Awake() {
        Instance = this;
        MainGameConfig = _mainGameConfig;
    }

    private void Start() {
        _curHaylvl = _startHayLevel - 1;
        _curBooklvl = _startBookLevel - 1;
        _curButtonslvl = _startButtonsLevel - 1;
        _xp = 0;
        _lvl = 0;
        _maxXp = MainGameConfig.XpNeeded[_lvl];
        CheckLvl();

        //StartCoroutine(HookHay());
        Upgrade(0,true);
        Upgrade(1,true);
        Upgrade(2,true);

        UpgradeHayButton.SetActive(false);
        UpgradeButtonsButton.SetActive(false);
        UpgradeBookButton.SetActive(false);
        _haveUpgradePoints = 0;
        _keyboardButtonsController.SetUpgradeLevel(_curButtonslvl);
        _keyboardButtonsController.OnLastButtonPressed += ButtonPressed;
        _keyboardButtonsController.OnNextButtonPressed += OnButtonPressed;

        _hook.OnHookCollectedHay += CollectHay;
        InvokeRepeating(nameof(TryUpdateLb), 3, 3);
    }

    void DropList() {
        GameObject list = Instantiate(curBook, ListsPanel);
        list.transform.SetAsFirstSibling();
        list.GetComponent<Image>().sprite = finishedBookSprites[_curBooklvl];
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
            curBook.GetComponent<Image>().sprite = emptyBookSprites[_curBooklvl];
        else if (percent < 0.5f)
            curBook.GetComponent<Image>().sprite = halfBookSprites[_curBooklvl];
        else
            curBook.GetComponent<Image>().sprite = finishedBookSprites[_curBooklvl];
        //TODO add writing sound
        //SoundManager.Instance.PlaySound(Sounds.Button);
    }

    public void GrewNewHay() {
        if (_hayFactory.HayAmount > MainGameConfig.MaxHayOnScreen) {
            return;
        }

        int grewAmount = _curBooklvl + 1;

        for (int i = 0; i < grewAmount; i++) {
            Hay h = _hayFactory.GetHay();
            h.Init(_curHaylvl);
            h.Grow();
        }

        SoundManager.Instance.PlaySound(Sounds.Growth);
        if (grewAmount > 0) {
            _hook.TryStartMove();
        }
    }

    void CheckLvl() {
        if (IsAllUpgradesBought) {
            _totalScoreText.gameObject.SetActive(true);
            lvlText.text = "??";
            return;
        }

        if (_xp >= _maxXp) {
            _lvl++;
            _xp = 0;
            _maxXp = MainGameConfig.XpNeeded[_lvl];
            GetUpgradePoint();
        }

        xpSlider.maxValue = _maxXp;
        xpSlider.value = _xp;
        lvlText.text = _lvl.ToString();

        if (IsAllUpgradesBought) {
            _totalScoreText.gameObject.SetActive(true);
            lvlText.text = "??";
            YandexMetrica.Send("allUpgradesBought");
        }
    }

    private bool IsAllUpgradesBought => _curButtonslvl == 4 && _curHaylvl == 4 && _curBooklvl == 4;

    void GetUpgradePoint() {
        _haveUpgradePoints++;

        SoundManager.Instance.PlaySound(Sounds.Collect);
        if (_curHaylvl < 4)
            UpgradeHayButton.SetActive(true);
        if (_curBooklvl < 4)
            UpgradeBookButton.SetActive(true);
        if (_curButtonslvl < 4)
            UpgradeButtonsButton.SetActive(true);
    }

    public void UpgradeButton(int what) {
        Upgrade(what);
    }
    
    private void Upgrade(int what, bool isSkipAnalytics = false) {
        string upgradeAnalyticsName = "";
        var eventParams = new Dictionary<string, string>();
        switch (what) {
            case 0: // hay
               
                _curHaylvl++;
                eventParams.Add("hay", _curHaylvl.ToString());
                if (_curHaylvl == 4) {
                    _hook.Activate();
                }

                if (_curHaylvl == 4)
                    UpgradeHayButton.SetActive(false);
                else
                    UpgradeHayButton.GetComponent<Image>().sprite = upgradeSprites[_curHaylvl];
                break;
            case 1: // book
              
                _curBooklvl++;
                eventParams.Add("book", _curBooklvl.ToString());
                curBook.SetActive(false);
                curBook = books[_curBooklvl];
                curBook.SetActive(true);

                if (_curBooklvl == 4)
                    UpgradeBookButton.SetActive(false);
                else
                    UpgradeBookButton.GetComponent<Image>().sprite = upgradeSprites[_curBooklvl];
                break;
            case 2: // buttons
               
                _curButtonslvl++;
                 eventParams.Add("buttons", _curButtonslvl.ToString());
                _keyboardButtonsController.SetUpgradeLevel(_curButtonslvl);

                if (_curButtonslvl == 4)
                    UpgradeButtonsButton.SetActive(false);
                else
                    UpgradeButtonsButton.GetComponent<Image>().sprite = upgradeSprites[_curButtonslvl];
                break;
        }

        if (!isSkipAnalytics) {
            YandexMetrica.Send("upgradeBought", eventParams);
            SoundManager.Instance.PlaySound(Sounds.Upgrade);
        }
       
        _haveUpgradePoints--;
        if (_haveUpgradePoints == 0) {
            UpgradeHayButton.SetActive(false);
            UpgradeButtonsButton.SetActive(false);
            UpgradeBookButton.SetActive(false);
        }

        
    }

    private void CollectHay() {
        foreach (Hay hay in FindObjectsOfType<Hay>()) {
            if (hay.CanBeCollected) {
                hay.StartCollecting();
            }
        }
    }

    public void CollectXp() {
        _xp += Mathf.FloorToInt(Mathf.Pow(2, _curHaylvl));
        _totalScoreText.text = _xp.ToString();
        CheckLvl();
    }

    private void TryUpdateLb() {
        if (!IsAllUpgradesBought) {
            return;
        }

        int saved = PlayerPrefs.GetInt("totalScore", 0);
        if (_xp > saved) {
            PlayerPrefs.SetInt("totalScore", _xp);
            YandexGame.NewLeaderboardScores("totalScore", _xp);
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
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    void CheckSwipe() {
        if (Input.GetMouseButtonDown(0)) {
            //save began touch 2d point
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0)) {
            //save ended touch 2d point
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //create vector from the two points
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
            float distance = secondPressPos.x - firstPressPos.x;

            //normalize the 2d vector
            currentSwipe.Normalize();

            //swipe right
            if (distance > Screen.width / 1.5f && currentSwipe.x > 0.5f && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
                CollectHay();
            }
        }

        if (Input.touches.Length > 0) {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) {
                //save began touch 2d point
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }

            if (t.phase == TouchPhase.Ended) {
                //save ended touch 2d point
                secondPressPos = new Vector2(t.position.x, t.position.y);

                //create vector from the two points
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                float distance = secondPressPos.x - firstPressPos.x;

                //normalize the 2d vector
                currentSwipe.Normalize();

                //swipe right
                if (distance > Screen.width / 1.5f && currentSwipe.x > 0.5f && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
                    CollectHay();
                }
            }
        }
    }
}