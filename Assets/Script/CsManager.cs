using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class CsManager : MonoBehaviour {
	
	public GUISkin skin;
	
	public GUIText txtMove;				
	public GUIText txtStage;			
	public GUIText txtTime;				
	public GUIText txtMsg;				

 	public Transform fanfare;			
 	public AudioClip sndFanfare;		
 	public AudioClip sndClick;

	
	static public int dir = 0;		
	static public int tileNum;	
    		
	int countX;							
	int countZ;

	static public float sizeX;			
	static public float sizeZ;

	
	static public bool isSound = true;
    static public string picture;
	static public int stageNum = 1;		
	float stageTime = 0;				
	int moveCount = 0;					

	
	//static public int picNum = 1;		
	static public int picCount = 6;		 

	public enum STATE { START, CLEAR, SAVE, LOAD, CLICK, IDLE, WAIT };
	static public STATE state = STATE.START;

	
	float[] arSizeX = { 0, 0, 0, 1f, 0.75f, 0.6f, 0.5f };
	float[] arSizeZ = { 0, 0, 0, 1.4f, 1.05f, 0.84f, 0.7f };


	int[] arCountX = { 0, 3, 3, 4, 4, 5, 5, 6 };
	int[] arCountZ = { 0, 3, 4, 4, 5, 5, 6, 6 };


	int[] arMoveQue = { 0, 0, 0, 0, 0, 0 };   

	
	int[] arTiles = new int[36];	


	//-----------------------------
	// Start
	//-----------------------------
	void Start () {
     //   StartCoroutine("StageClear");
    }

    //-----------------------------
    // Update
    //-----------------------------
    void Update () {
		// print (state);
		switch (state) {
		case STATE.START :
			MakeStage();
			break;
		case STATE.CLICK :
			CheckMove();
			if (dir > 0) {
				MoveTile();
				CheckClear();
			}	
			break;
		case STATE.CLEAR :
			StartCoroutine("StageClear");
			break;
		case STATE.SAVE :
			StartCoroutine("SaveStage");
			break;
		case STATE.LOAD :
			StartCoroutine("LoadStage");
			break;
		}	
	}

    //-----------------------------
    // MakeStage 
    //-----------------------------
    void MakeStage() {
		countX = arCountX[stageNum];	
		countZ = arCountZ[stageNum];
	
		sizeX = arSizeX[countX];		
		sizeZ = arSizeZ[countZ];
		
		if (state == STATE.START) {
			ShuffleTile();				
			stageTime = Time.time;			
		}

        string picturea = "Pictures/" + picture + "/" + picture + stageNum;
        Texture2D img = Resources.Load(picturea) as Texture2D;	
	
		for (int i = 0; i < countX * countZ; i++) {
			int col = i % countX;				
			int row = i / countX;				
	
			float x =  col * sizeX + sizeX / 2.0f;
			float z =  row * sizeZ + sizeZ / 2.0f;
		    
			int num = arTiles[i];				
			if (num == -1) continue;		
			
			
			GameObject tile = Instantiate(Resources.Load("Prefabs/pfTile")) as GameObject;
			tile.transform.position = new Vector3(x, 0, -z);
			
		
			tile.transform.localScale = new Vector3(sizeX - 0.01f, 0.05f, sizeZ - 0.01f);
			tile.tag = "TILE" + num;
		
			
			int tCol = num % countX;		
			int tRow = num / countX;		
			
			
			float tilingX = 1.0f / countX;		
			float tilingY = 1.0f / countZ;
	
			float offsetX = tilingX * tCol;	
			float offsetY = 1.0f - (tilingY * tRow) - tilingY;
			
			
			tile.GetComponent<Renderer>().material.mainTextureScale = new Vector2(tilingX, tilingY);
            tile.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
            tile.GetComponent<Renderer>().material.mainTexture = img;
		}
		
		state = STATE.IDLE;
	}

    //-----------------------------
    // ShuffleTile
    //-----------------------------
    void ShuffleTile() {
		int loop = countX * countZ - 1;
		
		
		for (int i = 0; i <= loop; i++) {
			arTiles[i] = i;
		}
		
		arTiles[loop] = -1;		
		
		
		for (int i = 0; i <= loop; i++) {
			int n1 = Random.Range(0, loop + 1);
			int n2 = Random.Range(0, loop + 1);
		
			int tmp = arTiles[n1];
			arTiles[n1] = arTiles[n2];
			arTiles[n2] = tmp;
		}
		
		CheckShuffle();			
	}

    //-----------------------------
    // CheckShuffle
    //-----------------------------
    void CheckShuffle() {
		int loop = countX * countZ;
	
		int cnt = 0;				
		
		do {
			cnt = 0;			
			int n1 = 0;
			int n2 = 0;
			for (int i = 0; i < loop - 1; i++) {
				if (arTiles[i] == -1) continue;
				
				for (int j = i + 1; j < loop; j++) {
					if (arTiles[j] == -1) continue;
					
					if (arTiles[i] > arTiles[j]) {
						cnt++;
						n1 = i;		
						n2 = j;
					}
				} 
			} 
			
			if (cnt % 2 == 0) break;
			int tmp = arTiles[n1];
			arTiles[n1] = arTiles[n2];
			arTiles[n2] = tmp;
		} while (cnt % 2 == 1);			 
	}

    //-----------------------------
    // CheckMove 
    //-----------------------------
    void CheckMove() {
		int pos = CalcPoint(tileNum);	
		int target = CalcPoint(-1);		
		
		int posCol = pos % countX;		
		int posRow = pos / countX;
		
		int tarCol = target % countX;
		int tarRow = target / countX;
		
		if (posCol != tarCol && posRow != tarRow) return;	
		
		
		if (posRow == tarRow)
			CalcHorizontal(pos, posCol, tarCol);
		else	
			CalcVertical(pos, posRow, tarRow);
			
		arMoveQue[1] = target; 			
		
	
	}

    //-----------------------------
    // Calculate Point
    //-----------------------------
    int CalcPoint(int num) {
		int result = -9;
		
		for (int i = 0; i < countX * countZ; i++) {
			if (arTiles[i] == num) {
			 	result = i;
			 	break;
			 }
		}
		return result;	 	
	}

    //-----------------------------
    // Calculate Horizontal
    //-----------------------------
    void CalcHorizontal(int pos, int posCol, int tarCol) {
		int cnt = posCol - tarCol;			
		arMoveQue[0] = Mathf.Abs(cnt);		
	
		if (posCol < tarCol) {				
			dir = 2;
			for (int i = 0; i < -cnt; i++) {
				arMoveQue[i + 2] = pos + i;	
			}	
		} else {							
			dir = 4;
			for (int i = 0; i < cnt; i++) {
				arMoveQue[i + 2] = pos - i;
			}	
		}
	}

    //-----------------------------
    // Calculate Vertical
    //-----------------------------
    void CalcVertical(int pos, int posRow, int tarRow) {
		int cnt = posRow - tarRow;			
		arMoveQue[0] = Mathf.Abs(cnt);		
	
		if (posRow < tarRow) {				
			dir = 3;
			for (int i = 0; i < -cnt; i++) {
				arMoveQue[i + 2] = pos + i * countX;		
			}	
		} else {						
			dir = 1;
			for (int i = 0; i < cnt; i++) {
				arMoveQue[i + 2] = pos - i * countX;
			}	
		}
	}

    //-----------------------------
    // MoveTile
    //-----------------------------
    void MoveTile() {
		state = STATE.WAIT;
		moveCount++;
		
		if (isSound)
			AudioSource.PlayClipAtPoint(sndClick, new Vector3(1.5f, 0, -2));
	
		int cnt = arMoveQue[0];				
		for (int i = 2; i <= cnt + 1; i++) {
			int n = arMoveQue[i];	
			GameObject obj = GameObject.FindGameObjectWithTag("TILE" + arTiles[n]);
			obj.SendMessage("MoveTile", SendMessageOptions.DontRequireReceiver);
		}
	
	
		int blk = arMoveQue[1];				
		int last = arMoveQue[cnt + 1];		
		arTiles[blk] = arTiles[last];		
		
		for (int i = cnt + 1; i >= 3; i--) {
			int n = arMoveQue[i];
			switch (dir) {
				case 1 : arTiles[n] = arTiles[n + countX]; break;	
				case 2 : arTiles[n] = arTiles[n - 1]; break;	
				case 3 : arTiles[n] = arTiles[n - countX]; break;
				case 4 : arTiles[n] = arTiles[n + 1]; break;
			}	
		}
	
		int k = arMoveQue[2];			
		arTiles[k] = -1;				
		
		dir = 0;
		state = STATE.IDLE;
	}

    //-----------------------------
    // CheckClear
    //-----------------------------
    void CheckClear() {
		bool isDone = true;
		int loop = countX * countZ;
		
		for (int i = 0; i < loop - 1; i++) {
			if (arTiles[i] != i) {
				isDone = false;
				break;
			}
		}
		
		if (isDone) {
			state = STATE.CLEAR;
		
		}	
	}
	
	//-----------------------------
	// StageClear
	//-----------------------------
	IEnumerator StageClear() {
		state = STATE.WAIT;
		Instantiate(fanfare, new Vector3(1.5f, 0.5f, -2.5f), Quaternion.identity);
		if (isSound)
			AudioSource.PlayClipAtPoint(sndFanfare, new Vector3(1.5f, 0, -2));
		
	
		for (int i = 1; i < 5; i++) {
			txtMsg.text = "Stage Clear!";
			yield return new WaitForSeconds(0.5f);
			
			txtMsg.text = "";
			yield return new WaitForSeconds(0.5f);
		}
		
		DeleteTiles();				
		stageNum++;					
		if (stageNum > picCount + 1)
			stageNum = 1;
			
		/*picNum++;
		if (picNum > picCount)
			picNum = 1;	*/
			
		
		moveCount = 0;				
		stageTime = 0;       
        PlayerPrefs.SetInt(picture, stageNum);
        state = STATE.START;
       
	}
	
	//-----------------------------
	// Delete Tiles
	//-----------------------------
	void DeleteTiles() {
		for (int i = 0; i < countX * countZ - 1; i++) {
			Destroy(GameObject.FindWithTag("TILE" + i));
		}
	}
	
	//-----------------------------
	// Save Stage
	//-----------------------------
	IEnumerator SaveStage() {
		state = STATE.WAIT;
		string msg = "Game Saved...";
		
		// File Create 
		string dir = Application.persistentDataPath;
		string fileName = dir + "/gamesave.txt";			
		try {
			StreamWriter stream = new StreamWriter(fileName);
	
		
			int nTime = (int) (Time.time - stageTime);	
			stream.WriteLine(stageNum);					
			stream.WriteLine(nTime); 					
			stream.WriteLine(moveCount);			
			stream.WriteLine(picture);					
	
			
			for (int i = 0; i < countX * countZ; i++) {
				stream.WriteLine(arTiles[i]);	
			}
			stream.Flush();
			stream.Close();
		} catch {
			msg = "File save failed...";
		}
		
		state = STATE.IDLE;
		
		
		for (int i = 1; i < 3; i++) {
			txtMsg.text = msg;
			yield return new WaitForSeconds(0.5f);
			txtMsg.text = "";
			yield  return new WaitForSeconds(0.5f);
		}
	}
	
	//-----------------------------
	// Load Stage
	//-----------------------------
	IEnumerator LoadStage() {
		state = STATE.IDLE;
		string msg = "Game Loaded...";
		
		// File Open 
		string dir = Application.persistentDataPath;
		string fileName = dir + "/gamesave.txt";		
		try {
			FileStream FS = new FileStream(fileName, FileMode.Open);
			StreamReader stream = new StreamReader(FS);
			
			stageNum = int.Parse(stream.ReadLine());
			stageTime = Time.time - int.Parse(stream.ReadLine());
			moveCount = int.Parse(stream.ReadLine());	
			picture = stream.ReadLine().ToString();		
			
			DeleteTiles();		
			
			int cnt = arCountX[stageNum] * arCountZ[stageNum];
			for (int i = 0; i < cnt; i++) {
				arTiles[i] = int.Parse(stream.ReadLine());		
			}
			stream.Close();
			MakeStage();
		} catch {
		
			if (CalcPoint(-1) == -9) {		
				state = STATE.START;
				yield break;
			}
			
			msg = "File not found...";
		}		
		
		state = STATE.IDLE;
	
	
		for (int i = 1; i < 3; i++) {
			txtMsg.text = msg;
			yield return new WaitForSeconds(0.5f);
			txtMsg.text = "";
			yield return new WaitForSeconds(0.5f);
		}
	}
	
	//-----------------------------
	// OnGUI 
	//-----------------------------
	void OnGUI() {
		GUI.skin = skin;
	
		int w = Screen.width;		
		int h = Screen.height;

        string picturea =  "Pictures/" + picture + "/" + picture + stageNum;
        Texture2D img = Resources.Load(picturea) as Texture2D;	
		GUI.DrawTexture(new Rect(8, h - 115, 90, 110), img);
		
	
		int sec = (int) (Time.time - stageTime);
		int hour = sec / 3600;
		int min = (sec % 3600) / 60;
		sec = sec % 60; 
		
		txtMove.text = "Move = " + moveCount;
		txtStage.text = "Stage = " + stageNum;
		txtTime.text = "Time = " + hour + ":" + min + ":" + sec;
						
		string OnOff = "On";
		if (isSound) 
			OnOff = "Off";
			
	    // On/Off  Sound
		if (GUI.Button(new Rect(w - 120, h - 165, 110, 35), "Sound " + OnOff)) {
			isSound = !isSound;

		}	
		
		// Save & Load 
		if (GUI.Button(new Rect(w - 120, h - 125, 110, 35), "Save Game"))
			state = STATE.SAVE;
		if (GUI.Button(new Rect(w - 120, h - 85, 110, 35), "Load Game"))
			state = STATE.LOAD;
	
		// Quit	
		if (GUI.Button(new Rect(w - 120, h - 45, 110, 35), "Quit Game")) {
			DeleteTiles();		
            SceneManager.LoadScene("GameTitle");
        }	
	}
	
} // class
