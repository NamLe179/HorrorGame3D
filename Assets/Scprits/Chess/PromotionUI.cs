using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PromotionUI : MonoBehaviour
{
    public static PromotionUI Instance;

    private Chessman pawnToPromote;

    void Awake()
    {
        // Singleton đơn giản
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        gameObject.SetActive(false); // ẩn UI khi bắt đầu
    }

    // Gọi khi cần phong cấp — lưu lại pawn cần promote, hiện UI
    public void Open(Chessman pawn)
    {
        pawnToPromote = pawn;
        gameObject.SetActive(true);
    }

    // Gọi khi người chơi chọn 1 trong 4 quân
    public void PromoteTo(string piece)
    {
        if (pawnToPromote == null) return;

        string newName = pawnToPromote.player + "_" + piece;

        pawnToPromote.name = newName;
        pawnToPromote.Activate();

        GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<Game>()
            .SetPosition(pawnToPromote.gameObject);

        GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<Game>()
            .NextTurn();

        pawnToPromote = null;
        gameObject.SetActive(false);
    }



}
