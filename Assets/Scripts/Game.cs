using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    public int startHayLevel;
    public int startBookLevel;
    public int startButtonsLevel;

    [Header("Parameters")]
    public Transform ListsPanel;

    [SerializeField]
    private RectTransform _canvas;

    public Sprite[] upgradeSprites;

    public Button[] buttons_1;
    public Button[] buttons_2;
    public Button[] buttons_3;
    public Button[] buttons_4;
    public Button[] buttons_5;
    public Sprite[] haySprites;

    public GameObject[] books;
    public Sprite[] emptyBookSprites;
    public Sprite[] halfBookSprites;
    public Sprite[] finishedBookSprites;

    public GameObject hook;
    Vector2 hookPos;
    public GameObject curBook;
    Button[] curButtons;
    public Text lvlText;
    public Slider xpSlider;
    public int[] xpNeeded;

    public GameObject[] hay;
    int xp;
    int maxXp;
    int lvl;

    int curHaylvl;
    int curBooklvl;
    int curButtonslvl;

    public GameObject UpgradeHayButton;
    public GameObject UpgradeButtonsButton;
    public GameObject UpgradeBookButton;

    int curButton;
    int haveUpgradePoints;

    private void Start() {
        hookPos = hook.transform.position;
        curButtons = buttons_1;
        curButton = 0;
        DisEnableButtons();
        curHaylvl = startHayLevel - 1;
        curBooklvl = startBookLevel - 1;
        curButtonslvl = startButtonsLevel - 1;
        xp = 0;
        lvl = 0;
        maxXp = xpNeeded[lvl];
        CheckLvl();

        StartCoroutine(HookHay());
        Upgrade(0);
        Upgrade(1);
        Upgrade(2);

        UpgradeHayButton.SetActive(false);
        UpgradeButtonsButton.SetActive(false);
        UpgradeBookButton.SetActive(false);
        haveUpgradePoints = 0;
    }

    void DropList() {
        GameObject list = Instantiate(curBook, ListsPanel);
        list.transform.SetAsFirstSibling();
        list.GetComponent<Image>().sprite = finishedBookSprites[curBooklvl];

        Rigidbody2D rb = list.GetComponent<Rigidbody2D>();
        rb.simulated = true;

        float forcePower = 10;
        rb.AddTorque(Random.Range(-1f, 1f) * forcePower);
    }

    public void ButtonPressed() {
        curButton++;

        if (curButton == curButtons.Length) {
            curButton = 0;
            DropList();
        }

        DisEnableButtons();

        if (curButton == 0)
            curBook.GetComponent<Image>().sprite = emptyBookSprites[curBooklvl];
        else if (curButton < curButtons.Length / 2)
            curBook.GetComponent<Image>().sprite = halfBookSprites[curBooklvl];
        else
            curBook.GetComponent<Image>().sprite = finishedBookSprites[curBooklvl];
    }

    void DisEnableButtons() {
        for (int i = 0; i < curButtons.Length; i++) {
            curButtons[i].interactable = false;
            curButtons[i].gameObject.GetComponent<Image>().raycastTarget = false;
        }

        curButtons[curButton].interactable = true;
        curButtons[curButton].gameObject.GetComponent<Image>().raycastTarget = true;
    }

    public void GrewNewHay() {
        int toGrew = (curBooklvl + 1);
        for (int i = 0; i < hay.Length; i++) {
            if (!hay[i].activeSelf) {
                hay[i].SetActive(true);

                Vector3 pos = hay[i].transform.localPosition;
                pos.x = Random.Range(-1, 1f) * _canvas.rect.width * 0.45f;
                hay[i].transform.localPosition = pos;
                toGrew--;
                if (toGrew == 0)
                    break;
            }
        }

        if (toGrew > 0 && hook.activeSelf)
            StartCoroutine(HookHay());
    }

    IEnumerator HookHay() {
        float hookSpeed = 3f;
        hook.GetComponent<Rigidbody2D>().simulated = true;

        hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        hook.GetComponent<Rigidbody2D>().AddForce(Vector2.right * hookSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        CollectHay();
        hook.GetComponent<Rigidbody2D>().simulated = false;
        hook.transform.position = hookPos;
    }

    void CheckLvl() {
        if (xp >= maxXp) {
            lvl++;
            xp = 0;
            maxXp = xpNeeded[lvl];
            GetUpgradePoint();
        }

        xpSlider.maxValue = maxXp;
        xpSlider.value = xp;
        lvlText.text = lvl.ToString();

        if (curButtonslvl == 4 && curHaylvl == 4 && curBooklvl == 4)
            lvlText.text = "??";
    }

    void GetUpgradePoint() {
        haveUpgradePoints++;

        if (curHaylvl < 4)
            UpgradeHayButton.SetActive(true);
        if (curBooklvl < 4)
            UpgradeBookButton.SetActive(true);
        if (curButtonslvl < 4)
            UpgradeButtonsButton.SetActive(true);
    }

    public void Upgrade(int what) {
        switch (what) {
            case 0: // hay
                curHaylvl++;
                for (int i = 0; i < hay.Length; i++) {
                    hay[i].GetComponent<Image>().sprite = haySprites[curHaylvl];
                }

                if (curHaylvl == 4) {
                    hook.SetActive(true);
                }

                if (curHaylvl == 4)
                    UpgradeHayButton.SetActive(false);
                else
                    UpgradeHayButton.GetComponent<Image>().sprite = upgradeSprites[curHaylvl];
                break;
            case 1: // book
                curBooklvl++;
                curBook.SetActive(false);
                curBook = books[curBooklvl];
                curBook.SetActive(true);

                if (curBooklvl == 4)
                    UpgradeBookButton.SetActive(false);
                else
                    UpgradeBookButton.GetComponent<Image>().sprite = upgradeSprites[curBooklvl];
                break;
            case 2: // buttons
                curButtonslvl++;
                curButton = 0;
                switch (curButtonslvl) {
                    case 0:
                        for (int i = 0; i < buttons_1.Length; i++) {
                            buttons_1[i].gameObject.SetActive(true);
                        }

                        curButtons = buttons_1;
                        break;

                    case 1:
                        for (int i = 0; i < buttons_1.Length; i++) {
                            buttons_1[i].gameObject.SetActive(false);
                        }

                        for (int i = 0; i < buttons_2.Length; i++) {
                            buttons_2[i].gameObject.SetActive(true);
                        }

                        curButtons = buttons_2;
                        break;
                    case 2:
                        for (int i = 0; i < buttons_2.Length; i++) {
                            buttons_2[i].gameObject.SetActive(false);
                        }

                        for (int i = 0; i < buttons_3.Length; i++) {
                            buttons_3[i].gameObject.SetActive(true);
                        }

                        curButtons = buttons_3;
                        break;
                    case 3:
                        for (int i = 0; i < buttons_3.Length; i++) {
                            buttons_3[i].gameObject.SetActive(false);
                        }

                        for (int i = 0; i < buttons_4.Length; i++) {
                            buttons_4[i].gameObject.SetActive(true);
                        }

                        curButtons = buttons_4;
                        break;
                    case 4:
                        for (int i = 0; i < buttons_4.Length; i++) {
                            buttons_4[i].gameObject.SetActive(false);
                        }

                        for (int i = 0; i < buttons_5.Length; i++) {
                            buttons_5[i].gameObject.SetActive(true);
                        }

                        curButtons = buttons_5;
                        break;
                }

                if (curButtonslvl == 4)
                    UpgradeButtonsButton.SetActive(false);
                else
                    UpgradeButtonsButton.GetComponent<Image>().sprite = upgradeSprites[curButtonslvl];
                break;
        }

        haveUpgradePoints--;
        if (haveUpgradePoints == 0) {
            UpgradeHayButton.SetActive(false);
            UpgradeButtonsButton.SetActive(false);
            UpgradeBookButton.SetActive(false);
        }
    }

    public void CollectHay() {
        for (int i = 0; i < hay.Length; i++) {
            if (hay[i].activeSelf) {
                hay[i].GetComponent<Hay>().StartCollecting();
            }
        }
    }

    public void CollectXP() {
        xp += Mathf.FloorToInt(Mathf.Pow(2, curHaylvl));
        CheckLvl();
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