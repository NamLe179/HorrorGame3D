using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    //Some functions will need reference to the controller
    public GameObject controller;

    //The Chesspiece that was tapped to create this MovePlate
    GameObject reference = null;

    //Location on the board
    int matrixX;
    int matrixY;

    //false: movement, true: attacking
    public bool attack = false;

    public void Start()
    {
        if (attack)
        {
            //Set to red
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        //Destroy the victim Chesspiece
        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

            if (cp.name == "white_king") controller.GetComponent<Game>().Winner("black");
            if (cp.name == "black_king") controller.GetComponent<Game>().Winner("white");

            // Sửa: Thay Destroy bằng SetActive(false) + remove from array
            cp.SetActive(false);
            controller.GetComponent<Game>().RemovePieceFromArray(cp); // Gọi hàm mới từ Game.cs
        }

        //Set the Chesspiece's original location to be empty
        controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Chessman>().GetXBoard(),
            reference.GetComponent<Chessman>().GetYBoard());

        //Move reference chess piece to this position
        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();

        //Update the matrix
        controller.GetComponent<Game>().SetPosition(reference);



        // Kiểm tra Promotion sau khi di chuyển tốt
        Chessman cm = reference.GetComponent<Chessman>();



        // Promotion người chơi (white)


        if (cm.player == "white" && cm.name == "white_pawn" && cm.GetYBoard() == 7)
        {

            // Cập nhật vị trí pawn đã đi đến trước khi phong
            controller.GetComponent<Game>().SetPosition(cm.gameObject);

            // Xóa hết moveplates  để không click lại vào ô cũ
            reference.GetComponent<Chessman>().DestroyMovePlates();

            // Mở UI phong quân
            PromotionUI.Instance.Open(cm);

            // STOP không cho NextTurn
            return;


        }

        if (cm.player == "black" && cm.name == "black_pawn" && cm.GetYBoard() == 0)
        {
            PromotePawn(cm, "black");
        }


        //Destroy move plates
        reference.GetComponent<Chessman>().DestroyMovePlates();

        //Switch Current Player
        controller.GetComponent<Game>().NextTurn();

    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    private void PromotePawn(Chessman pawn, string player)
    {
        // Các quân có thể phong
        string[] choices = { "queen", "rook", "bishop", "knight" };

        // Random 1 quân
        int r = Random.Range(0, choices.Length);
        string newName = player + "_" + choices[r];

        pawn.name = newName;

        pawn.Activate();
    }



    public GameObject GetReference()
    {
        return reference;
    }
}