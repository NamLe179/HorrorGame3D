using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Game : MonoBehaviour
{
    public Text winnerText;        
    public Text restartText;      
    public GameObject chesspiece;

    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    private string currentPlayer = "white";
    private bool gameOver = false;
    private bool aiIsThinking = false;

    private List<(int fromX, int fromY, int toX, int toY)> aiMoveHistory
    = new List<(int, int, int, int)>();


    // Thêm struct để lưu nước đi của AI
    [System.Serializable]
    public struct ChessMove
    {
        public Chessman piece;
        public int toX;
        public int toY;
        public bool isAttack;
    }


    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        if (x < 0 || y < 0 || x >= 8 || y >= 8) return null;
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        return x >= 0 && y >= 0 && x < 8 && y < 8;
    }

    public string GetCurrentPlayer() => currentPlayer;
    public bool IsGameOver() => gameOver;





    //   UNITY

    public void Start()
    {
        HideEndGameTexts();
        if (winnerText != null) winnerText.gameObject.SetActive(false);
        if (restartText != null) restartText.gameObject.SetActive(false);

        playerWhite = new GameObject[] {
            Create("white_rook", 0, 0), Create("white_knight", 1, 0),
            Create("white_bishop", 2, 0), Create("white_queen", 3, 0), Create("white_king", 4, 0),
            Create("white_bishop", 5, 0), Create("white_knight", 6, 0), Create("white_rook", 7, 0),
            Create("white_pawn", 0, 1), Create("white_pawn", 1, 1), Create("white_pawn", 2, 1),
            Create("white_pawn", 3, 1), Create("white_pawn", 4, 1), Create("white_pawn", 5, 1),
            Create("white_pawn", 6, 1), Create("white_pawn", 7, 1)
        };

        playerBlack = new GameObject[] {
            Create("black_rook", 0, 7), Create("black_knight", 1, 7),
            Create("black_bishop", 2, 7), Create("black_queen", 3, 7), Create("black_king", 4, 7),
            Create("black_bishop", 5, 7), Create("black_knight", 6, 7), Create("black_rook", 7, 7),
            Create("black_pawn", 0, 6), Create("black_pawn", 1, 6), Create("black_pawn", 2, 6),
            Create("black_pawn", 3, 6), Create("black_pawn", 4, 6), Create("black_pawn", 5, 6),
            Create("black_pawn", 6, 6), Create("black_pawn", 7, 6)
        };

        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }


    public void Update()
    {
        if (gameOver && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("Game");
        }

        // Chỉ gọi AI khi đến lượt đen VÀ chưa đang nghĩ
        if (!gameOver && currentPlayer == "black" && !aiIsThinking)
        {
            aiIsThinking = true;
            StartCoroutine(AiThinkAndMove());
        }
        //  KIỂM TRA KHI NGƯỜI CHƠI (TRẮNG) HẾT NƯỚC ĐI
        if (!gameOver && currentPlayer == "white")
        {
            List<ChessMove> playerMoves = GetAllValidMoves("white");  // lấy tất cả nước đi 
            playerMoves = FilterMovesSafe("white", playerMoves);

            if (playerMoves.Count == 0)
            {
                if (IsInCheck("white"))
                    Winner("black");     // trắng bị chiếu hết → đen thắng
                else
                    Winner("draw");       // hòa do bí
            }
        }
    }


    private void HideEndGameTexts()
    {
        var winner = GameObject.FindGameObjectWithTag("WinnerText");
        if (winner != null) winner.SetActive(false);

        var restart = GameObject.FindGameObjectWithTag("RestartText");
        if (restart != null) restart.SetActive(false);
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }



    // ================== GAME FLOW ==================


    public void NextTurn()
    {
        currentPlayer = (currentPlayer == "white") ? "black" : "white";
    }

    public void Winner(string playerWinner)
    {
        gameOver = true;

        if (winnerText != null)
        {
            winnerText.text = (playerWinner == "white" ? "Trắng" : "Đen") + " thắng!";
            winnerText.gameObject.SetActive(true);
        }

        if (restartText != null)
        {
            restartText.gameObject.SetActive(true);
        }
    }



    // ================== AI ==================




    private IEnumerator AiThinkAndMove()
    {
        yield return new WaitForSeconds(0.8f);

        List<ChessMove> validMoves = GetAllValidMoves("black");
        validMoves = FilterMovesSafe("black", validMoves);

        // --- Checkmate / Stalemate ---
        if (validMoves.Count == 0)
        {
            if (IsInCheck("black"))
                Winner("white");   // checkmate
            else
                Winner("draw");    // stalemate

            aiIsThinking = false;
            yield break;
        }

        // --- Nếu còn nước 
        ChessMove bestMove = validMoves[0];
        int bestScore = int.MaxValue;

        foreach (var move in validMoves)
        {
            var backup = FakeMove(move);

            int score = Minimax(2, true);

            int repetition = CountMoveRepetition(move);
            if (repetition >= 2)   // tức là đã đi 2 lần rồi → tránh lặp lần 3
            {
                score += 500;  // đen minimax là MIN, score lớn → AI sẽ ghét nước đó
            }

            UndoFakeMove(move, backup);

            if (score < bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        ExecuteMove(bestMove);
        aiIsThinking = false;

    }




    public int Minimax(int depth, bool maximizingPlayer)
    {
        if (depth == 0)
            return EvaluateBoard();

        string player = maximizingPlayer ? "white" : "black";

        List<ChessMove> moves = GetAllValidMoves(player);
        moves = FilterMovesSafe(player, moves);

        if (moves.Count == 0)
            return EvaluateBoard();

        if (maximizingPlayer)
        {
            int best = int.MinValue;

            foreach (var move in moves)
            {
                var backup = FakeMove(move);

                int eval = Minimax(depth - 1, false);

                UndoFakeMove(move, backup);

                best = Mathf.Max(best, eval);
            }

            return best;
        }
        else
        {
            int best = int.MaxValue;

            foreach (var move in moves)
            {
                var backup = FakeMove(move);

                int eval = Minimax(depth - 1, true);

                UndoFakeMove(move, backup);

                best = Mathf.Min(best, eval);
            }

            return best;
        }
    
    }



    public int EvaluateBoard()
    {
        if (IsEndGame())
            return EvaluateEndGame();

        int score = 0;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                GameObject obj = positions[x, y];
                if (obj == null) continue;

                Chessman cm = obj.GetComponent<Chessman>();
                int v = GetPieceValue(cm.name);

                score += (cm.player == "white") ? v : -v;
            }
        }

        return score;
    }



    int EvaluateEndGame()
    {
        Chessman whiteKing = GetKing("white");
        Chessman blackKing = GetKing("black");

        int wkx = whiteKing.GetXBoard();
        int wky = whiteKing.GetYBoard();
        int bkx = blackKing.GetXBoard();
        int bky = blackKing.GetYBoard();

        int score = 0;

        // 1️⃣ Dồn vua trắng vào biên → TỐT CHO ĐEN → score ÂM
        int distToEdge = Mathf.Min(
            Mathf.Min(wkx, 7 - wkx),
            Mathf.Min(wky, 7 - wky)
        );
        score -= (3 - distToEdge) * 500;

        // 2️⃣ Giảm số nước đi vua trắng → tốt cho đen
        int mobility = CountKingMobility("white");
        score -= (8 - mobility) * 400;

        // 3️⃣ Vua đen tiến gần → tốt cho đen
        int kingDist = Mathf.Abs(wkx - bkx) + Mathf.Abs(wky - bky);
        score -= (14 - kingDist) * 200;

        // 4️⃣ Đang check trắng → rất tốt cho đen
        if (IsInCheck("white"))
            score -= 3000;

        return score;
    }


    bool IsEndGame()
    {
        int whitePieces = 0;

        foreach (var p in playerWhite)
            if (p != null && p.activeSelf)
                whitePieces++;

        return whitePieces <= 1; // chỉ còn vua
    }







    // ================== MOVE EXECUTION ==================


    public void ExecuteMove(ChessMove move)
    {
        Chessman cm = move.piece;

        int oldX = cm.GetXBoard();
        int oldY = cm.GetYBoard();




        // 🔥 LƯU NƯỚC ĐI CỦA AI TRƯỚC KHI DI CHUYỂN
        if (cm.player == "black")
        {
            aiMoveHistory.Add((oldX, oldY, move.toX, move.toY));

            if (aiMoveHistory.Count > 20)
                aiMoveHistory.RemoveAt(0);
        }

        // Xử lý ăn quân
        if (move.isAttack)
        {
            GameObject victim = GetPosition(move.toX, move.toY);
            if (victim.name.Contains("king"))
            {
                Winner("black");
                return;
            }
            // Sửa: Thay Destroy bằng SetActive(false) và nullify trong array để tránh exception
            victim.SetActive(false);
            RemovePieceFromArray(victim);
        }

        // Dọn ô cũ
        SetPositionEmpty(cm.GetXBoard(), cm.GetYBoard());

        // Di chuyển quân
        cm.SetXBoard(move.toX);
        cm.SetYBoard(move.toY);
        cm.SetCoords();

        // Cập nhật vị trí mới
        SetPosition(cm.gameObject);

        // PROMOTION CHO AI
        if (cm.name == "black_pawn" && cm.GetYBoard() == 0)
        {
            cm.name = "black_queen";
            cm.Activate();
        }

        if (cm.name == "white_pawn" && cm.GetYBoard() == 7)
        {
            cm.name = "white_queen";
            cm.Activate();
        }


        // Xóa hết MovePlate cũ (nếu có)
        cm.DestroyMovePlates();

        // Đổi lượt
        NextTurn();
    }




    private (int, int, GameObject) FakeMove(ChessMove move)
    {
        Chessman piece = move.piece;

        int oldX = piece.GetXBoard();
        int oldY = piece.GetYBoard();
        GameObject killed = positions[move.toX, move.toY];

        // simulate
        positions[oldX, oldY] = null;
        positions[move.toX, move.toY] = piece.gameObject;

        piece.SetXBoard(move.toX);
        piece.SetYBoard(move.toY);

        if (killed != null) killed.SetActive(false);

        return (oldX, oldY, killed);
    }



    private void UndoFakeMove(ChessMove move, (int oldX, int oldY, GameObject killed) backup)
    {
        Chessman piece = move.piece;

        positions[backup.oldX, backup.oldY] = piece.gameObject;
        positions[move.toX, move.toY] = backup.killed;

        piece.SetXBoard(backup.oldX);
        piece.SetYBoard(backup.oldY);

        if (backup.killed != null)
            backup.killed.SetActive(true);
    }



    // Hàm mới: Xóa piece khỏi array sau khi "destroy"
    public void RemovePieceFromArray(GameObject piece)
    {
        for (int i = 0; i < playerWhite.Length; i++)
        {
            if (playerWhite[i] == piece)
            {
                playerWhite[i] = null;
                return;
            }
        }
        for (int i = 0; i < playerBlack.Length; i++)
        {
            if (playerBlack[i] == piece)
            {
                playerBlack[i] = null;
                return;
            }
        }
    }




    // ================== MOVE GENERATION ==================


    // ================== LẤY TẤT CẢ NƯỚC ĐI HỢP LỆ CHO AI ==================
    public List<ChessMove> GetAllValidMoves(string player)
    {
        List<ChessMove> moves = new List<ChessMove>();
        GameObject[] pieces = player == "white" ? playerWhite : playerBlack;

        foreach (GameObject pieceObj in pieces)
        {
            if (pieceObj == null || !pieceObj.activeSelf) continue;
            Chessman cm = pieceObj.GetComponent<Chessman>();
            if (cm == null) continue;

            CollectMovesForPiece(cm, moves);
        }

        return moves;
    }



    private void CollectMovesForPiece(Chessman cm, List<ChessMove> moves)
    {
        string name = cm.name;
        int x = cm.GetXBoard();
        int y = cm.GetYBoard();

        switch (name)
        {
            case "black_queen":
            case "white_queen":
                AddLineMoves(cm, moves, 1, 0); AddLineMoves(cm, moves, -1, 0);
                AddLineMoves(cm, moves, 0, 1); AddLineMoves(cm, moves, 0, -1);
                AddLineMoves(cm, moves, 1, 1); AddLineMoves(cm, moves, 1, -1);
                AddLineMoves(cm, moves, -1, 1); AddLineMoves(cm, moves, -1, -1);
                break;

            case "black_rook":
            case "white_rook":
                AddLineMoves(cm, moves, 1, 0); AddLineMoves(cm, moves, -1, 0);
                AddLineMoves(cm, moves, 0, 1); AddLineMoves(cm, moves, 0, -1);
                break;

            case "black_bishop":
            case "white_bishop":
                AddLineMoves(cm, moves, 1, 1); AddLineMoves(cm, moves, 1, -1);
                AddLineMoves(cm, moves, -1, 1); AddLineMoves(cm, moves, -1, -1);
                break;

            case "black_knight":
            case "white_knight":
                TryAddMove(cm, moves, x + 1, y + 2); TryAddMove(cm, moves, x + 1, y - 2);
                TryAddMove(cm, moves, x - 1, y + 2); TryAddMove(cm, moves, x - 1, y - 2);
                TryAddMove(cm, moves, x + 2, y + 1); TryAddMove(cm, moves, x + 2, y - 1);
                TryAddMove(cm, moves, x - 2, y + 1); TryAddMove(cm, moves, x - 2, y - 1);
                break;

            case "black_king":
            case "white_king":
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (dx != 0 || dy != 0)
                            TryAddMove(cm, moves, x + dx, y + dy);
                break;

            case "black_pawn":
                TryAddPawnMove(cm, moves, x, y - 1);
                TryAddPawnAttack(cm, moves, x + 1, y - 1);
                TryAddPawnAttack(cm, moves, x - 1, y - 1);

                if (y == 6 && GetPosition(x, 5) == null && GetPosition(x, 4) == null)
                    TryAddPawnMove(cm, moves, x, y - 2);
                break;

            case "white_pawn":
                TryAddPawnMove(cm, moves, x, y + 1);
                TryAddPawnAttack(cm, moves, x + 1, y + 1);
                TryAddPawnAttack(cm, moves, x - 1, y + 1);

                if (y == 1 && GetPosition(x, 2) == null && GetPosition(x, 3) == null)
                    TryAddPawnMove(cm, moves, x, y + 2);

                break;
        }
    }




    private void AddLineMoves(Chessman cm, List<ChessMove> moves, int dx, int dy)
    {
        int x = cm.GetXBoard() + dx;
        int y = cm.GetYBoard() + dy;
        while (PositionOnBoard(x, y) && GetPosition(x, y) == null)
        {
            moves.Add(new ChessMove { piece = cm, toX = x, toY = y, isAttack = false });
            x += dx; y += dy;
        }
        if (PositionOnBoard(x, y) && GetPosition(x, y) != null &&
            GetPosition(x, y).GetComponent<Chessman>().player != cm.player)
        {
            moves.Add(new ChessMove { piece = cm, toX = x, toY = y, isAttack = true });
        }
    }




    private void TryAddMove(Chessman cm, List<ChessMove> moves, int targetX, int targetY)
    {
        if (!PositionOnBoard(targetX, targetY)) return;
        GameObject target = GetPosition(targetX, targetY);
        if (target == null)
            moves.Add(new ChessMove { piece = cm, toX = targetX, toY = targetY, isAttack = false });
        else if (target.GetComponent<Chessman>().player != cm.player)
            moves.Add(new ChessMove { piece = cm, toX = targetX, toY = targetY, isAttack = true });
    }



    private void TryAddPawnMove(Chessman cm, List<ChessMove> moves, int x, int y)
    {
        if (PositionOnBoard(x, y) && GetPosition(x, y) == null)
            moves.Add(new ChessMove { piece = cm, toX = x, toY = y, isAttack = false });
    }



    private void TryAddPawnAttack(Chessman cm, List<ChessMove> moves, int x, int y)
    {
        if (PositionOnBoard(x, y) && GetPosition(x, y) != null &&
            GetPosition(x, y).GetComponent<Chessman>().player != cm.player)
            moves.Add(new ChessMove { piece = cm, toX = x, toY = y, isAttack = true });
    }




    // ================== RULES ==================




    public bool IsInCheck(string player)
    {
        Chessman king = GetKing(player);
        return IsSquareAttacked(king.GetXBoard(), king.GetYBoard(),
                                (player == "white") ? "black" : "white");
    }



    public bool IsSquareAttacked(int x, int y, string byPlayer)
    {
        GameObject[] pieces = (byPlayer == "white") ? playerWhite : playerBlack;

        foreach (var obj in pieces)
        {
            if (obj == null || !obj.activeSelf) continue; // Sửa: Skip nếu null hoặc inactive (quân bị capture)
            Chessman cm = obj.GetComponent<Chessman>();
            if (cm == null) continue; // An toàn thêm



            // sử dụng chính logic Move của AI để kiểm tra ô bị tấn công
            List<ChessMove> moves = new List<ChessMove>();
            CollectMovesForPiece(cm, moves);

            foreach (var m in moves)
            {
                if (m.toX == x && m.toY == y)
                    return true;
            }
        }

        return false;
    }



    public List<ChessMove> FilterMovesSafe(string player, List<ChessMove> moves)
    {
        List<ChessMove> safe = new List<ChessMove>();

        foreach (var m in moves)
        {
            Chessman piece = m.piece;

            int oldX = piece.GetXBoard();
            int oldY = piece.GetYBoard();

            GameObject killed = positions[m.toX, m.toY];

            // ---- SIMULATE MOVE ----
            positions[oldX, oldY] = null;
            positions[m.toX, m.toY] = piece.gameObject;

            piece.SetXBoard(m.toX);
            piece.SetYBoard(m.toY);

            // tạm ẩn quân bị ăn 
            if (killed != null) killed.SetActive(false);

            bool inCheck = IsInCheck(player);

            // ---- REVERT ----
            piece.SetXBoard(oldX);
            piece.SetYBoard(oldY);

            positions[oldX, oldY] = piece.gameObject;
            positions[m.toX, m.toY] = killed;

            if (killed != null) killed.SetActive(true);

            if (!inCheck) safe.Add(m);
        }

        return safe;
    }



    int CountKingMobility(string player)
    {
        Chessman king = GetKing(player);
        int x = king.GetXBoard();
        int y = king.GetYBoard();

        int moves = 0;
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx;
                int ny = y + dy;
                if (PositionOnBoard(nx, ny) &&
                    !IsSquareAttacked(nx, ny, "black"))
                    moves++;
            }
        return moves;
    }




    // ================== UTIL ==================




    public Chessman GetKing(string player)
    {
        GameObject[] pieces = (player == "white") ? playerWhite : playerBlack;

        foreach (var p in pieces)
        {
            if (p != null && p.activeSelf && p.GetComponent<Chessman>().name == player + "_king")
                return p.GetComponent<Chessman>();
        }
        return null;
    }




    public int GetPieceValue(string name)
    {
        if (name.Contains("pawn")) return 10;
        if (name.Contains("knight") || name.Contains("bishop")) return 30;
        if (name.Contains("rook")) return 50;
        if (name.Contains("queen")) return 90;
        if (name.Contains("king")) return 900; // không quá lớn để minimax không bị overload
        return 0;
    }


    private int CountMoveRepetition(ChessMove move)
    {
        int count = 0;

        foreach (var m in aiMoveHistory)
        {
            if (m.fromX == move.piece.GetXBoard() &&
                m.fromY == move.piece.GetYBoard() &&
                m.toX == move.toX &&
                m.toY == move.toY)
            {
                count++;
            }
        }

        return count;
    }





    

    

    




   



   


    

    

   


   

    


    

    

    


    










   


    

    

   

    

    

    


    

    


    



}