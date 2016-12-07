using UnityEngine;
using System.Collections;

public class FileDialog {
	
	string freeTryFile = "Results/test.csv";
	string actualAttemptFile = "Results/times.csv";
	string bestTimesFile = "Results/best.csv";

	bool isActualAttempt;

	public bool isActualAttemptMode() {
		return isActualAttempt;
	}


	public void readBestTimes() {
		/*if (File.Exists(fileName)) {
			Debug.Log(fileName+" already exists.");
			return;
		}
		var sr = File.CreateText(fileName);
		sr.WriteLine ("This is my file.");
		sr.WriteLine ("I can write ints {0} or floats {1}, and so on.",
		              1, 4.2);
		sr.Close();*/
	}

	public void writeBestTimes() {

	}

	string getHeader() {
		return isActualAttempt 
			? "Lap 1;Lap text 1;Lap 2;Lap text 2; Lap 3;Lap text 3;Total time;Total text time"
			: "save ID;Time;Time text;Red key;Green key;Open Door;Completed";
	}

	public void write(string text) {
		string file = getFile ();

		if (!System.IO.Directory.Exists ("Results")) {
			System.IO.Directory.CreateDirectory ("Results");
			Debug.Log ("Result folder created.");
		}


		if (!System.IO.File.Exists(file)) {
			Debug.Log ("file created : "+file);
			System.IO.File.WriteAllText(
				file, 
				getHeader()+System.Environment.NewLine);
		}
		System.IO.File.AppendAllText(
			file, 
			text+System.Environment.NewLine);
		Debug.Log ("Attempt saved : "+file);
	}
	
	public void setMode(bool actualAttemptMode) {
		this.isActualAttempt = actualAttemptMode;
	}
	
	public void setActualAttemptMode() {
		this.isActualAttempt = true;
	}
	
	public void setFreeTryMode() {
		this.isActualAttempt = false;
	}

	public string getFile() {
		return isActualAttempt ? actualAttemptFile : freeTryFile;
	}

}

