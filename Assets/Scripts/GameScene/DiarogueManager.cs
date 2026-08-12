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

    [Header("選択肢ボタン")]
    [SerializeField]
    private Button[] choiceButtons;

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
        // ノードの範囲チェック
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

        // 話者
        speakerText.text = node.speaker;

        // セリフ
        dialogueText.text = node.text;


        // -----------------------------------------------------
        // 選択肢がある場合
        // -----------------------------------------------------

        if (node.choices != null &&
            node.choices.Count > 0)
        {
            // 次へボタンを非表示
            nextButton.gameObject.SetActive(false);

            // 選択肢を表示
            ShowChoicePanel(node);

            ShowChoices(node);
        }

        // -----------------------------------------------------
        // 選択肢がない場合
        // -----------------------------------------------------

        else
        {
            // 選択肢パネルを全て非表示
            HideAllChoicePanels();

            // 次へボタンを表示
            nextButton.gameObject.SetActive(true);
        }
    }


    // =========================================================
    // 選択肢パネル
    // =========================================================

    // ノードに設定されたパネルを表示
    private void ShowChoicePanel(DialogueNode node)
    {
        // 一旦すべてのパネルを非表示
        HideAllChoicePanels();

        // パネル番号
        int panelIndex = node.choicePanelIndex;

        // 範囲チェック
        if (panelIndex < 0 ||
            panelIndex >= choicePanelList.Count)
        {
            Debug.LogError(
                "指定された選択肢パネルが存在しません。"
                + "PanelIndex : "
                + panelIndex
            );

            return;
        }

        // 指定されたパネルだけ表示
        if (choicePanelList[panelIndex] != null)
        {
            choicePanelList[panelIndex].SetActive(true);
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
    }


    // =========================================================
    // 次へ
    // =========================================================

    private void NextDialogue()
    {
        DialogueNode node = dialogueData.nodes[currentNode];

        // 次のノードが設定されていない
        if (node.nextNode < 0)
        {
            EndDialogue();
            return;
        }

        // 次のノード
        currentNode = node.nextNode;

        // 範囲外チェック
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

        // 次の会話を表示
        ShowDialogue();
    }


    // =========================================================
    // 選択肢表示
    // =========================================================

    private void ShowChoices(DialogueNode node)
    {
        // 全ボタンを非表示
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);

            // 以前のイベントを削除
            choiceButtons[i].onClick.RemoveAllListeners();
        }


        // 選択肢を表示
        for (int i = 0; i < node.choices.Count; i++)
        {
            // ボタン数を超えた場合
            if (i >= choiceButtons.Length)
            {
                Debug.LogWarning(
                    "選択肢の数がボタン数を超えています"
                );

                break;
            }

            DialogueChoice choice = node.choices[i];

            // ボタンを表示
            choiceButtons[i].gameObject.SetActive(true);


            // ボタンのテキストを取得
            TMP_Text buttonText =
                choiceButtons[i].GetComponentInChildren<TMP_Text>();


            // テキストを設定
            if (buttonText != null)
            {
                buttonText.text = choice.text;
            }
            else
            {
                Debug.LogWarning(
                    "選択肢ボタンにTMP_Textがありません。"
                );
            }


            // 選択時のイベント
            choiceButtons[i].onClick.AddListener(() =>
            {
                SelectChoice(choice);
            });
        }
    }


    // =========================================================
    // 選択肢を選択
    // =========================================================

    private void SelectChoice(DialogueChoice choice)
    {
        // 選択時のイベントを実行
        choice.onSelected?.Invoke();


        // 分岐先がない場合
        if (choice.nextNode < 0)
        {
            EndDialogue();
            return;
        }


        // 範囲外チェック
        if (choice.nextNode >= dialogueData.nodes.Count)
        {
            Debug.LogError(
                "選択肢の分岐先が存在しません。Node : "
                + choice.nextNode
            );

            EndDialogue();
            return;
        }


        // 次のノード
        currentNode = choice.nextNode;


        // 次のノードでShowDialogue()が
        // 適切なパネルを表示するので、
        // ここでは全パネルを一旦非表示にする
        HideAllChoicePanels();


        // 次の会話を表示
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