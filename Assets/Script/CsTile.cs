using UnityEngine;
using System.Collections;

public class CsTile : MonoBehaviour {
	
	int speed = 8;
	
	// 타일 이동 방향 
	int[] dirX = { 0, 0, -1, 0, 1 };
	int[] dirZ = { 0, -1, 0, 1, 0 };

	//-----------------------------
	// Game Loop
	//-----------------------------
	void Update () {
		if (CsManager.state == CsManager.STATE.WAIT) return;
		
		if (Input.GetButtonDown("Fire1")) {
			CheckTile();	
		}
	}
	
	//-----------------------------
	// Check Tile
	//-----------------------------
	void CheckTile () {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
			if (hit.transform.tag.Contains("TILE")) {
				CsManager.tileNum = int.Parse(hit.transform.tag.Substring(4));
				CsManager.state = CsManager.STATE.CLICK;
			}
		}	
	}
	
	//-----------------------------
	// Move
	//-----------------------------
	IEnumerator MoveTile() {
		Vector3 pos = transform.position;	// 타일의 현재 위치 
	
		int dir = CsManager.dir;			// 이동 방향 
		float dist = 0.0f;					// 이동할 거리 
	
		if (dir == 1 || dir == 3)			// 수직 방향 이동 
			dist = CsManager.sizeZ;
		if (dir == 2 || dir == 4)			// 수평 방향 이동 
			dist = CsManager.sizeX;
		
		while (dist > 0.1f) {
			float amtToMoveX = speed * Time.deltaTime * dirX[dir];
			float amtToMoveZ = speed * Time.deltaTime * dirZ[dir];
			transform.Translate(new Vector3(amtToMoveX, 0, amtToMoveZ));
		
			// 이동한 거리 누적 
			if (dir == 1 || dir == 3)
				dist -= Mathf.Abs(amtToMoveZ);
			if (dir == 2 || dir == 4)
				dist -= Mathf.Abs(amtToMoveX);
			yield return 0;
		}
	
		// 이동 후 핀트 맞춤 
		switch (dir) {
			case 1 : pos.z += CsManager.sizeZ; break;	
			case 2 : pos.x += CsManager.sizeX; break;	
			case 3 : pos.z -= CsManager.sizeZ; break;	
			case 4 : pos.x -= CsManager.sizeX; break;	
		}
		
		transform.position = pos;
	}
}
