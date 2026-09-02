using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("会話データ")]
    [SerializeField]
    private List<DialogueData> dialogueDataList = new List<DialogueData>();

    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("次へボタン")]
    [SerializeField] private Button nextButton;

    [Header("選択肢パネル")]
    [SerializeField]
    private List<GameObject> choicePanelList = new List<GameObject>();
    private GameObject currentChoicePanel;
    private DialogueData dialogueData;

    private int currentNode = 0;

    public static DialogueManager instance;

    public bool isTalk = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        // 会話パネルを非表示
        dialoguePanel.SetActive(false);

        // 全選択肢パネルを非表示
        HideAllChoicePanels();

        // 次へボタンにイベント登録
        nextButton.onClick.AddListener(NextDialogue);
    }


    // =========================================================
    // 会話開始
    // =========================================================

    // 数字で会話を選択して開始
    public void StartDialogue(int dialogueIndex)
    {
        // インデックスの範囲チェック
        if (dialogueIndex < 0 ||
            dialogueIndex >= dialogueDataList.Count)
        {
            Debug.LogError(
                "指定されたDialogueDataが存在しません。Index : "
                + dialogueIndex
            );

            return;
        }

        // 会話データを取得
        dialogueData = dialogueDataList[dialogueIndex];

        // nullチェック
        if (dialogueData == null)
        {
            Debug.LogError(
                "DialogueDataが設定されていません。Index : "
                + dialogueIndex
            );

            return;
        }

        // 会話が存在するか
        if (dialogueData.nodes == null ||
            dialogueData.nodes.Count == 0)
        {
            Debug.LogError(
                "DialogueDataに会話がありません。Index : "
                + dialogueIndex
            );

            return;
        }

        // 会話開始
        isTalk = true;

        // プレイヤーを停止
        if (PlayerController.instance != null)
        {
            PlayerController.instance.StopPlayer();
        }

        // 最初のノード
        currentNode = 0;

        // 会話パネル表示
        dialoguePanel.SetActive(true);

        // 会話表示
        ShowDialogue();
    }


    // =========================================================
    // 会話表示
    // =========================================================

    // 現在の会話を表示
    private void ShowDialogue()
    {
        if (currentNode < 0 ||
            currentNode >= dialogueData.nodes.Count)
        {
            Debug.LogError(
                "指定されたノードが存在しません。Node : "
                + currentNode
            );

            EndDialogue();
            return;
        }

        DialogueNode node = dialogueData.nodes[currentNode];

        // この会話に入ったときの処理
        node.onNodeEnter?.Invoke();

        // 話者
        speakerText.text = node.speaker;

        // セリフ
        dialogueText.text = node.text;

        if (node.choices != null &&
            node.choices.Count > 0)
        {
            nextButton.gameObject.SetActive(false);

            ShowChoicePanel(node);
            ShowChoices(node);
        }
        else
        {
            HideAllChoicePanels();

            nextButton.gameObject.SetActive(true);
        }
    }


    // =========================================================
    // 選択肢パネル
    // =========================================================

    // ノードに設定されたパネルを表示
    private void ShowChoicePanel(DialogueNode node)
    {
        // すべて非表示
        HideAllChoicePanels();

        int panelIndex = node.choicePanelIndex;

        if (panelIndex < 0 ||
            panelIndex >= choicePanelList.Count)
        {
            Debug.LogError(
                "指定された選択肢パネルが存在しません。"
                + " PanelIndex : "
                + panelIndex
            );

            return;
        }

        // 現在のパネルを保存
        currentChoicePanel = choicePanelList[panelIndex];

        if (currentChoicePanel != null)
        {
            currentChoicePanel.SetActive(true);

            // ★追加修正：表示直後にレイアウトとUI座標を強制再計算して当たりのズレを直す
            RectTransform rect = currentChoicePanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
        }
        else
        {
            Debug.LogError(
                "choicePanelList[" +
                panelIndex +
                "] がnullです。"
            );
        }
    }


    // 全選択肢パネルを非表示
    private void HideAllChoicePanels()
    {
        for (int i = 0; i < choicePanelList.Count; i++)
        {
            if (choicePanelList[i] != null)
            {
                choicePanelList[i].SetActive(false);
            }
        }

        // 現在の選択肢パネルをクリア
        currentChoicePanel = null;
    }


    // =========================================================
    // 次へ
    // =========================================================

    private void NextDialogue()
    {
        DialogueNode node = dialogueData.nodes[currentNode];

        // 現在の会話から出るときの処理
        node.onNodeExit?.Invoke();

        // 次のノードが設定されていない
        if (node.nextNode < 0)
        {
            EndDialogue();
            return;
        }

        currentNode = node.nextNode;

        if (currentNode < 0 ||
            currentNode >= dialogueData.nodes.Count)
        {
            Debug.LogError(
                "次のノード番号が存在しません。Node : "
                + currentNode
            );

            EndDialogue();
            return;
        }

        ShowDialogue();
    }


    // =========================================================
    // 選択肢表示
    // =========================================================

    private void ShowChoices(DialogueNode node)
    {
        if (currentChoicePanel == null)
        {
            Debug.LogError("現在開いている選択肢パネルがありません。");
            return;
        }

        Button[] choiceButtons =
            currentChoicePanel.GetComponentsInChildren<Button>(true);

        // すべてのボタンを初期化
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
            choiceButtons[i].onClick.RemoveAllListeners();

            ChoiceButton choiceButton =
                choiceButtons[i].GetComponent<ChoiceButton>();

            if (choiceButton != null)
            {
                choiceButton.SetChoice(null);
            }
        }

        // 選択肢を設定
        for (int i = 0; i < node.choices.Count; i++)
        {
            if (i >= choiceButtons.Length)
            {
                Debug.LogWarning(
                    "選択肢の数がボタン数を超えています。"
                );

                break;
            }

            Button button = choiceButtons[i];
            DialogueChoice choice = node.choices[i];

            // ChoiceButtonを取得
            ChoiceButton choiceButton =
                button.GetComponent<ChoiceButton>();

            if (choiceButton == null)
            {
                Debug.LogError(
                    button.name +
                    " にChoiceButtonがありません。"
                );

                continue;
            }

            // このボタンにChoiceを登録
            choiceButton.SetChoice(choice);

            // ボタン表示
            button.gameObject.SetActive(true);

            // テキスト設定
            TMP_Text buttonText =
                button.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text = choice.text;
            }

            // ★追加修正：クロージャ（ラムダ式）内で使用する参照をローカル変数に固定
            Button targetButton = button;
            ChoiceButton targetChoiceButton = choiceButton;

            targetButton.onClick.AddListener(() =>
            {
                DialogueChoice selectedChoice =
                    targetChoiceButton.GetChoice();

                Debug.Log(
                    "押されたボタン : " +
                    targetButton.name
                );

                Debug.Log(
                    "選択肢 : " +
                    selectedChoice.text
                );

                Debug.Log(
                    "移動先Node : " +
                    selectedChoice.nextNode
                );

                SelectChoice(selectedChoice);
            });
        }
    }


    // =========================================================
    // 選択肢を選択
    // =========================================================

    private void SelectChoice(DialogueChoice choice)
    {
        if (choice == null)
        {
            Debug.LogError("DialogueChoiceがnullです。");
            return;
        }

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            "選択肢 : " + choice.text
        );

        Debug.Log(
            "分岐先Node : " + choice.nextNode
        );

        Debug.Log(
            "現在Node : " + currentNode
        );

        choice.onSelected?.Invoke();

        if (choice.nextNode < 0)
        {
            EndDialogue();
            return;
        }

        if (choice.nextNode >= dialogueData.nodes.Count)
        {
            Debug.LogError(
                "存在しないNodeです : " +
                choice.nextNode
            );

            EndDialogue();
            return;
        }

        currentNode = choice.nextNode;

        Debug.Log(
            "移動後Node : " + currentNode
        );

        ShowDialogue();
    }


    // =========================================================
    // 会話終了
    // =========================================================

    private void EndDialogue()
    {
        isTalk = false;


        // プレイヤーの操作を再開
        if (PlayerController.instance != null)
        {
            PlayerController.instance.StartPlayer();
        }


        // UIを非表示
        dialoguePanel.SetActive(false);

        // 選択肢パネルを全て非表示
        HideAllChoicePanels();

        // 次へボタンを非表示
        nextButton.gameObject.SetActive(false);


        Debug.Log("会話終了");
    }


    // =========================================================
    // 外部からUIを操作
    // =========================================================

    public void DialoguePanelSetActive(bool active)
    {
        dialoguePanel.SetActive(active);
    }


    public void ChoicePanelSetActive(bool active)
    {
        // 全ての選択肢パネルに対して設定
        for (int i = 0; i < choicePanelList.Count; i++)
        {
            if (choicePanelList[i] != null)
            {
                choicePanelList[i].SetActive(active);
            }
        }
    }
}