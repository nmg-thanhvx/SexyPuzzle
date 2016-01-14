using UnityEngine;
using System.Collections;
// using System.Text;
using System.IO;

public class CsManager : MonoBehaviour {
	
	public GUISkin skin;
	
	public GUIText txtMove;				// 타일 이동 횟수 
	public GUIText txtStage;			// 스테이지 번호 
	public GUIText txtTime;				// 경과 시간 
	public GUIText txtMsg;				// 메시지 표시 

 	public Transform fanfare;			// 파티클 프리팹 
 	public AudioClip sndFanfare;		// 사운드 
 	public AudioClip sndClick;

	// 타일 수, 타일 크기 
	static public int dir = 0;			// 타일 이동 방향(1~4) 
	static public int tileNum;			// 클릭한 타일 번호 
	
	int countX;							// 타일의 수 
	int countZ;

	static public float sizeX;			// 타일 사이즈 
	static public float sizeZ;

	// 스테이지 처리용 
	static public bool isSound = true;	// 배경 음악 및 사운드 연주 여부 
	static public int stageNum = 1;		// 스테이지 번호 
	float stageTime = 0;				// 스테이지 경과 시간 
	int moveCount = 0;					// 타일 이동 횟수 

	// 이미지 관련  
	static public int picNum = 1;		// 사진 번호 
	static public int picCount = 6;		// 사진 수 

	public enum STATE { START, CLEAR, SAVE, LOAD, CLICK, IDLE, WAIT };
	static public STATE state = STATE.START;

	// 타일의 수에 따른 타일의 크기 
	float[] arSizeX = { 0, 0, 0, 1f, 0.75f, 0.6f, 0.5f };
	float[] arSizeZ = { 0, 0, 0, 1.4f, 1.05f, 0.84f, 0.7f };

	// 스테이지에 따른 타일 수 (사진 6장의 경우) 
	int[] arCountX = { 0, 3, 3, 4, 4, 5, 5, 6 };
	int[] arCountZ = { 0, 3, 4, 4, 5, 5, 6, 6 };

	// 이동할 타일  타일 수, 공백 위치,  타일 번호... 
	int[] arMoveQue = { 0, 0, 0, 0, 0, 0 };   

	// 6x6 타일 번호 
	int[] arTiles = new int[36];	


	//-----------------------------
	// Start
	//-----------------------------
	void Start () {

	}
	
	//-----------------------------
	// 순환 루프 
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
	// 스테이지 만들기	 
	//-----------------------------
	void MakeStage() {
		// 변수 초기화 
		countX = arCountX[stageNum];	
		countZ = arCountZ[stageNum];
	
		sizeX = arSizeX[countX];		// 타일의 크기 
		sizeZ = arSizeZ[countZ];
		
		if (state == STATE.START) {
			ShuffleTile();				// 타일 섞기 
			stageTime = Time.time;			// 스테이지 시작 시각 
		}	
	
		// 텍스처로 사용할 사진 
		Texture2D img = Resources.Load("picture" + picNum) as Texture2D;	
	
		for (int i = 0; i < countX * countZ; i++) {
			int col = i % countX;				// 열번호 
			int row = i / countX;				// 행번호 
	
			float x =  col * sizeX + sizeX / 2.0f;	// 표시 위치 
			float z =  row * sizeZ + sizeZ / 2.0f;
		    
			int num = arTiles[i];				// 섞인 타일 번호 
			if (num == -1) continue;			// 빈 타일 
			
			// 동적으로 타일 만들기 
			GameObject tile = Instantiate(Resources.Load("Prefabs/pfTile")) as GameObject;
			tile.transform.position = new Vector3(x, 0, -z);
			
			// 타일 사이의 간격을 0.01로 설정 
			tile.transform.localScale = new Vector3(sizeX - 0.01f, 0.05f, sizeZ - 0.01f);
			tile.tag = "TILE" + num;
		
			// 섞인 타일 번호로 col, row 계산 
			int tCol = num % countX;		
			int tRow = num / countX;		
			
			// 텍스처 분할 
			float tilingX = 1.0f / countX;		// 텍스처 타일링 수 
			float tilingY = 1.0f / countZ;
	
			float offsetX = tilingX * tCol;	// 텍스처 오프셋 
			float offsetY = 1.0f - (tilingY * tRow) - tilingY;
			
			// 타일 렌더링 
			tile.GetComponent<Renderer>().material.mainTextureScale = new Vector2(tilingX, tilingY);
            tile.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
            tile.GetComponent<Renderer>().material.mainTexture = img;
		}
		
		state = STATE.IDLE;
	}
	
	//-----------------------------
	// 타일 섞기 
	//-----------------------------
	void ShuffleTile() {
		int loop = countX * countZ - 1;
		
		// 초기값 
		for (int i = 0; i <= loop; i++) {
			arTiles[i] = i;
		}
		
		arTiles[loop] = -1;		// 빈 타일은 -1 
		
		// return;
		
		// 섞기 	
		for (int i = 0; i <= loop; i++) {
			int n1 = Random.Range(0, loop + 1);
			int n2 = Random.Range(0, loop + 1);
			
			// 서로 교환 
			int tmp = arTiles[n1];
			arTiles[n1] = arTiles[n2];
			arTiles[n2] = tmp;
		}
		
		CheckShuffle();			// 무결성 조사 
	}

	//-----------------------------
	// 배열의 무결성 조사
	//-----------------------------
	void CheckShuffle() {
		int loop = countX * countZ;
	
		int cnt = 0;				// 치환의 수 
		
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
						n1 = i;		// 임시 저장 
						n2 = j;
					}
				} // for j 
			} // for i	 
			
			if (cnt % 2 == 0) break;	// 완료 
			int tmp = arTiles[n1];
			arTiles[n1] = arTiles[n2];
			arTiles[n2] = tmp;
		} while (cnt % 2 == 1);			 
	}
	
	//-----------------------------
	// 타일이 이동가능한가? 
	//-----------------------------
	void CheckMove() {
		int pos = CalcPoint(tileNum);	// 클릭한 타일 번호 
		int target = CalcPoint(-1);		// 빈 타일 번호 
		
		int posCol = pos % countX;		// 클릭한 타일의 col  
		int posRow = pos / countX;
		
		int tarCol = target % countX;	// 빈 타일의 col 
		int tarRow = target / countX;
		
		if (posCol != tarCol && posRow != tarRow) return;	// 움직일 타일 없음 
		
		// 수평 방향 계산 
		if (posRow == tarRow)
			CalcHorizontal(pos, posCol, tarCol);
		else	
			CalcVertical(pos, posRow, tarRow);
			
		arMoveQue[1] = target; 			// 공백 위치 
		
		// 디버그용 
		// print("Dir = " + dir + ", MoveCount = " + arMoveQue[0]);		
	}

	//-----------------------------
	// 지정한 타일 위치 찾기 
	//-----------------------------
	int CalcPoint(int num) {
		int result = -9;
		
		// 배열을 순차적으로 검색 
		for (int i = 0; i < countX * countZ; i++) {
			if (arTiles[i] == num) {
			 	result = i;
			 	break;
			 }
		}
		return result;	 	
	}

	//-----------------------------
	// 수평 방향 이동할 타일 찾기 
	//-----------------------------
	void CalcHorizontal(int pos, int posCol, int tarCol) {
		int cnt = posCol - tarCol;			// 이동할 타일 수 
		arMoveQue[0] = Mathf.Abs(cnt);		// 이동할 타일 수 
	
		if (posCol < tarCol) {				// 오른쪽으로 이동 
			dir = 2;
			for (int i = 0; i < -cnt; i++) {
				arMoveQue[i + 2] = pos + i;	// Click한 카드부터 저장 
			}	
		} else {							// 왼쪽으로 이동	
			dir = 4;
			for (int i = 0; i < cnt; i++) {
				arMoveQue[i + 2] = pos - i;
			}	
		}
	}
	
	//-----------------------------
	// 수직 방향 이동할 타일 찾기
	//-----------------------------
	void CalcVertical(int pos, int posRow, int tarRow) {
		int cnt = posRow - tarRow;			// 이동할 타일 수 
		arMoveQue[0] = Mathf.Abs(cnt);		// 이동할 타일 수 
	
		if (posRow < tarRow) {				// 아래로 이동 
			dir = 3;
			for (int i = 0; i < -cnt; i++) {
				arMoveQue[i + 2] = pos + i * countX;		// Click한 카드부터 이동 
			}	
		} else {							// 위로 이동 
			dir = 1;
			for (int i = 0; i < cnt; i++) {
				arMoveQue[i + 2] = pos - i * countX;
			}	
		}
	}
	
	//-----------------------------
	// 타일 이동
	//-----------------------------
	void MoveTile() {
		state = STATE.WAIT;
		moveCount++;
		
		if (isSound)
			AudioSource.PlayClipAtPoint(sndClick, new Vector3(1.5f, 0, -2));
	
		int cnt = arMoveQue[0];				// 이동할 타일 수 
		for (int i = 2; i <= cnt + 1; i++) {
			int n = arMoveQue[i];	
			GameObject obj = GameObject.FindGameObjectWithTag("TILE" + arTiles[n]);
			obj.SendMessage("MoveTile", SendMessageOptions.DontRequireReceiver);
		}
	
		// 타일 이동 후 배열 정리 
		int blk = arMoveQue[1];				// 공백 타일 위치 
		int last = arMoveQue[cnt + 1];		// 마지막 타일 번호 
		arTiles[blk] = arTiles[last];		// 공백 위치에 마지막 타일 번호 기록  
		
		for (int i = cnt + 1; i >= 3; i--) {
			int n = arMoveQue[i];
			switch (dir) {
				case 1 : arTiles[n] = arTiles[n + countX]; break;	// 배열 이동 
				case 2 : arTiles[n] = arTiles[n - 1]; break;	
				case 3 : arTiles[n] = arTiles[n - countX]; break;
				case 4 : arTiles[n] = arTiles[n + 1]; break;
			}	
		}
	
		int k = arMoveQue[2];			// 클릭한 타일 번호 
		arTiles[k] = -1;				// 클릭한 타일을 새로운 공백 
		
		dir = 0;
		state = STATE.IDLE;
	}

	//-----------------------------
	// 결과 판정
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
			// print ("done");
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
		
		// 메시지 표시
		for (int i = 1; i < 5; i++) {
			txtMsg.text = "Stage Clear!";
			yield return new WaitForSeconds(0.5f);
			
			txtMsg.text = "";
			yield return new WaitForSeconds(0.5f);
		}
		
		DeleteTiles();				// 현재의 타일 제거	
		stageNum++;					// 스테이지 증가	
		if (stageNum > picCount + 1)
			stageNum = 1;
			
		picNum++;
		if (picNum > picCount)
			picNum = 1;	
			
		// 변수 초기화	
		moveCount = 0;				// 타일 이동 횟수 
		stageTime = 0;				// 스테이지 경과 시간 
		
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
		string fileName = dir + "/gamesave.txt";			// 파일명  
		try {
			StreamWriter stream = new StreamWriter(fileName);
	
			// 데이터 기록 
			int nTime = (int) (Time.time - stageTime);	// 현재까지 경과 시간 
			stream.WriteLine(stageNum);					// 스테이지 번호 
			stream.WriteLine(nTime); 					// 스테이지 경과 시간 
			stream.WriteLine(moveCount);				// 타일 이동 횟수 
			stream.WriteLine(picNum);					// 이미지 번호 
	
			// 배열 저장 
			for (int i = 0; i < countX * countZ; i++) {
				stream.WriteLine(arTiles[i]);	
			}
			stream.Flush();
			stream.Close();
		} catch {
			msg = "File save failed...";
		}
		
		state = STATE.IDLE;
		
		// 메시지 표시 
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
		string fileName = dir + "/gamesave.txt";		// 파일명 
		try {
			FileStream FS = new FileStream(fileName, FileMode.Open);
			StreamReader stream = new StreamReader(FS);
			
			stageNum = int.Parse(stream.ReadLine());
			stageTime = Time.time - int.Parse(stream.ReadLine());
			moveCount = int.Parse(stream.ReadLine());	// 타일 이동 횟수 
			picNum = int.Parse(stream.ReadLine());		// 이미지 번호 
			
			DeleteTiles();		// 현재 타일 제거 
			
			int cnt = arCountX[stageNum] * arCountZ[stageNum];
			for (int i = 0; i < cnt; i++) {
				arTiles[i] = int.Parse(stream.ReadLine());		// 배열 읽기	 
			}
			stream.Close();
			MakeStage();
		} catch {
			// 타일이 없으면 새로 시작 
			if (CalcPoint(-1) == -9) {		
				state = STATE.START;
				yield break;
			}
			
			msg = "File not found...";
		}		
		
		state = STATE.IDLE;
	
		// 메시지 표시
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
	
		int w = Screen.width;		// 화면의 폭 
		int h = Screen.height;		// 화면의 높이 
		
		// 썸네일 표시 
		Texture2D img = Resources.Load("picture" + picNum) as Texture2D;	
		GUI.DrawTexture(new Rect(5, h - 87, 60, 82), img);
		
		// 시간 계산 
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
			
		// 사운드 On/Off  
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
			DeleteTiles();		// 현재 타일 제거 
			Application.LoadLevel("GameTitle");
		}	
	}
	
} // class
