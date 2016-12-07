using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	public Agent agent;
	public Door door;
	public Key redKey;
	public Key greenKey;
	public GameObject plane;
	public FileDialog dialog;
	public bool started;

	//public GameObject directionPoint1;
	//public GameObject directionPoint2;

	protected int laps;
	protected int currentLap;
	protected long[] timestamps;
	protected int saveID;

	// Use this for initialization
	void Start () {
		dialog = new FileDialog ();
		saveID = -1;
		reset (1, false, 0, false);
	}
	
	// Update is called once per frame
	void Update () {
		
		checkKeys ();
		
		if (currentLap==0) {
		    if(agent.waitOnce()) {
				newLap ();
				print ("start lap (" + (currentLap) + "/" + laps + ")");
			} else {
				return;
			}
		}

		if (door.isObjectiveComplete() & agent.acknowledgeTrigger () == plane) {
			if(newLap() > laps) {
				agent.stop();
				if(dialog.isActualAttemptMode ()) {
					saveActualAttempt();
				}
			} else {
				door.reset ();
				print ("new lap ("+(currentLap) + "/" + laps+")");
			}
		}
	}
	
	void checkKeys() {
		if (Input.GetKeyUp (KeyCode.A))
			reset (1, false, 0, true);
		else if (Input.GetKeyUp (KeyCode.R))
			reset (3, true, 180, true);
	}

	void reset(int laps, bool actualAttempt, int waitingTime, bool savePrevious) {
		if (savePrevious && !dialog.isActualAttemptMode ()) {
			saveTestAttempt();
		}
		//si tour serieux fini remettre saveID à 0
		dialog.setMode(actualAttempt);
		agent.reset (waitingTime);
		door.reset ();
		currentLap = 0;
		this.laps = laps;
		timestamps = new long[laps + 1];
	}
	
	private string TRUE = "true";
	private string FALSE = "false";
	
	void saveTestAttempt() {
		++saveID;
		string save = saveID.ToString()+";";
		
		for (int i=1; i<=laps; ++i) {
			save += saveLap(i)+";";
		}
		//int s = getTotalTime ();
		//save += s.ToString () + ";" + getTimeString(s).ToString ();
		save += (  redKey.isTaken ()        ? TRUE : FALSE) + ";";
		save += (greenKey.isTaken ()        ? TRUE : FALSE) + ";";
		save += (door.isObjectiveComplete() ? TRUE : FALSE) + ";";
		save += (agent.isGameOver()         ? TRUE : FALSE) + ";";
		
		dialog.write (save);
	}
	
	void saveActualAttempt() {
		string save = "";
		
		for (int i=1; i<=laps; ++i) {
			save += saveLap(i)+";";
		}

		int s = getTotalTime ();
		save += s.ToString () + ";" + getTimeString(s).ToString ();
		
		dialog.write (save);
	}
	
	string saveLap(int lap) {
		int i = getTime (lap);
		return i.ToString () + ";" + getTimeString(i).ToString ();
	}
	
	void saveActualLap() {
		string save = "";
	}

	int newLap() {
		timestamps [currentLap] = System.DateTime.Now.ToFileTime ();
		if(currentLap > 0) {
			print (getTime(currentLap));
		}
		++currentLap;
		return currentLap;
	}

	int getExactTime(int lap) {

		long end = (timestamps [lap] == 0) //si tour non fini
			? System.DateTime.Now.ToFileTime ()
			: timestamps [lap];

		return (int)(end - timestamps [lap - 1]);
	}

	int getTime(int lap) {
		return (getExactTime (lap) / 10000) * 10000;
	}

	void printTimes() {
		for (int i=1; i<=laps; ++i) {
			print (getTime(i));
		}
	}
	
	Rect GUIBelow(Rect rect) {
		rect.y += rect.height;
		return rect;
		
		//return new Rect(rect.x, rect.y+rect.height, rect.width, rect.height);
	}
	
	Rect GUIRight(Rect rect, int right) {
		rect.x += right;
		return rect;
		
		//return new Rect(rect.x, rect.y+rect.height, rect.width, rect.height);
	}

	string getTimeString(int time) {
		int t;

		t = (time / 600000000);
		string min = t.ToString();
		if (t < 10)
			min = "0" + min;

		t = ((time % 600000000) / 10000000);
		string sec = t.ToString();
		if (t < 10)
			sec = "0" + sec;

		t = ((time % 10000000) / 10000);
		string mil = t.ToString();
		if (t < 100)
			mil = "0" + mil;
		if (t < 10)
			mil = "0" + mil;
		
		return min + ":" + sec + ":" + mil;
		//return min + ":" + sec + ":" + mil+" ("+time+")";
	}

	string timeLeft(int lap) {
		int w = agent.getWaitingTime();
		if (currentLap == 0) {
			return "<color=red>Starting in "+((w/60)+1)+" seconds ...</color>";
		}
		return "Current : " + getTimeString ((int)((long)System.DateTime.Now.ToFileTime () - timestamps [lap - 1]));
	}

	int getTotalTime() {
		int totalTime = 0;
		for (int i=1; i<=laps; ++i) {
			totalTime += getTime(i);
		}
		return totalTime;
	}
	
	void OnGUI() {
		int lineHeight = 20;
		int right = 80;
		Rect r = new Rect (10, 0, 300, lineHeight);
		Rect l;

		GUIStyle style = new GUIStyle();
		style.fontSize = 15;
		style.normal.textColor = Color.white;

		if (!dialog.isActualAttemptMode ()) {
			GUI.Label(new Rect (10, 0, 300, lineHeight*3), 
			          "FREE ROAMING MODE : \n" +
			          "PRESS A TO RESET TURN\n" +
			          "PRESS R WHEN READY FOR A REAL 3 LAPS CIRCUIT"
			          , style);
			return;
		}

		int lap = Mathf.Min (currentLap, laps);

		string line = "________________________";

		string greenKey   = door.green.isTaken()         ? "<color=green>Green Key</color>" : "";
		string redKey     = door.red  .isTaken()         ? "<color=red>Red Key </color>" : "";
		string doorOpen   = door.isObjectiveComplete()   ? "<color=blue>Door open</color>" : "";
		string current    = currentLap < laps + 1        ? timeLeft(lap) : "";
		string firstLine  = currentLap > 1               ? line : "";
		string secondLine = currentLap == laps + 1       ? line : "";

		GUI.Label(r=GUIBelow(r), "Lap : "+lap+'/'+laps, style);
		GUI.Label(r=GUIBelow(r)       ,   redKey      , style);
		GUI.Label(l=GUIRight(r, right), greenKey      , style);
		GUI.Label(l=GUIRight(l, right), doorOpen      , style);
		GUI.Label(r=GUIBelow(r), current              , style);
		GUI.Label(r=GUIBelow(r), line                 , style);

		for (int i=1; i<=laps; ++i) {
			string lapstring = i<currentLap ? " : "+getTimeString(getTime(i)) : "";
			GUI.Label(r=GUIBelow(r), "|Lap "+i+"|"+lapstring, style);
		}


		if(currentLap == laps + 1) {

			string total = currentLap == laps + 1 ? "Total time : "+getTimeString(getTotalTime()) : "";
			GUI.Label(r=GUIBelow(r), line , style);
			GUI.Label(r=GUIBelow(r), total, style);
		}


	}

	/*void OnTriggerEnter(Collider col) {
		print(col.name);
	}
	
	void OnCollisionEnter(Collision col) {
		print(col.collider.name);
	}*/

}

