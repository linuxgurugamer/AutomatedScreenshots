using System;
using System.IO;
using System.Threading;

namespace AutomatedScreenshots
{
	class SaveFilesHandlers
	{

		private const string saveFileList = "/saveFileList.txt";
		private const int NUMFILES_OFFSET = 0;
		private const int FILESAVECNT_OFFSET = 1;
		private const int FILENAME_OFFSET = 2;

		private const int MAX_OFFSET = 2; // This should equal the highest offset above

		ushort numSaveFiles;
		int fileSaveCnt;
		private static int saveFileCnt;

		/*
		 * BackupWork
		 */
		public void BackupWork (AS asRef)
		{
			SaveFilesHandlers sfh  = new SaveFilesHandlers ();

			// SaveMode is:  OVERWRITE    APPEND   ABORT
			SaveMode s = SaveMode.OVERWRITE;

			saveFileCnt = FileSaveCnt(KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder) + 1;
			string saveFileName = AS.AddInfo (AS.configuration.savePrefix, saveFileCnt,	asRef.isSceneReady(), asRef.isSpecialScene(), asRef.isPreCrash());

			string str = GamePersistence.SaveGame (saveFileName, HighLogic.SaveFolder, s);
			Log.Info ("String: " + str);

			sfh.deleteOldestSaveFile (KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder, AS.configuration.numToRotate, saveFileCnt, saveFileName);

			Log.Info ("backup thread terminated");
		}

		/*
		 * startBackup
		 */
		public void startBackup (AS asRef)
		{
			asRef.backupThread = new Thread (() => BackupWork(asRef));
			asRef.backupThread.Start ();
		}


		/*
		 * FileSaveCnt
		 */
		public int FileSaveCnt(string path)
		{

			string fname = path + saveFileList;

			if (!File.Exists (fname)) {
				return 0;
			}

			// Open the file to read from. 

			StreamReader file = new StreamReader (fname);
			string line = file.ReadLine ();
			// numSaveFiles = Convert.ToUInt16 (line);
			line = file.ReadLine ();
			fileSaveCnt = Convert.ToInt32 (line);
			file.Close (); file.Dispose ();
			return fileSaveCnt;
		}

		/*
		 * 
		 */
		private string[] emptyReadText()
		{
			string[] readText = new string[2];
			readText [NUMFILES_OFFSET] = "0";
			readText [FILESAVECNT_OFFSET] = "0";
			return readText;
		}

		/*
		 * readSaveFileList
		 */
		public string[] readSaveFileList (string path)
		{
			string[] readText;

			string fname = path + saveFileList;
			if (!File.Exists (fname)) {
				Log.Info ("file does not exist: " + fname);
				return emptyReadText();
			}

			// Open the file to read from. 
        
			StreamReader file = new StreamReader (fname);
			string line = file.ReadLine ();
			numSaveFiles = 0;
			try {
				numSaveFiles = Convert.ToUInt16 (line);
			} catch (FormatException ) {
				file.Close (); file.Dispose ();
				return emptyReadText();
			} catch (OverflowException ) {
				file.Close (); file.Dispose ();
				return emptyReadText();
			}


			line = file.ReadLine ();
			try {
				fileSaveCnt = Convert.ToInt32 (line);
			} catch (FormatException ) {
				file.Close (); file.Dispose ();
				return emptyReadText();
			} catch (OverflowException ) {
				file.Close (); file.Dispose ();
				return emptyReadText();
			}

			Log.Info ("fileSaveCnt: " + fileSaveCnt.ToString ());

			ushort cnt = 0;
			readText = new string[numSaveFiles + MAX_OFFSET];
			readText [FILESAVECNT_OFFSET] = fileSaveCnt.ToString ();
			readText [NUMFILES_OFFSET] = numSaveFiles.ToString ();

			while (numSaveFiles > 0) {
				line = file.ReadLine ();
				if (line != null && cnt < numSaveFiles ) {
					cnt++;
					readText [cnt + MAX_OFFSET - 1] = line;
				} else {
					readText [NUMFILES_OFFSET] = cnt.ToString ();
					break;
				}
			}

			file.Close ();
			file.Dispose ();

			return readText;
		}

		/*
		 * writeSaveFileList
		 */
		void writeSaveFileList (string path, string[] writeText)
		{
			string fname = path + saveFileList;

			try{
			File.WriteAllLines (fname, writeText);	
			}
			catch (Exception e) {
				Log.Info ("Exception caught after WriteAllLines: " + e);
			}
				
		}

		/*
		 * deleteOldestSaveFile
		 */
		public void deleteOldestSaveFile (string path, ushort maxSaveFiles, int cnt = -1, string newFile = "")
		{
			string[] fileList = readSaveFileList (path);
		
		//	ushort numSaveFiles = 0;
		//	if (fileList.Length > 0)
		//		numSaveFiles = Convert.ToUInt16 (fileList [NUMFILES_OFFSET]);
		
			ushort x = numSaveFiles;
			if (maxSaveFiles > x)
				x = maxSaveFiles;
			string[] newList = new string[MAX_OFFSET + x + 1];

			for (int i = FILENAME_OFFSET; i <= numSaveFiles + FILENAME_OFFSET - 1; i++) 
			{

				newList [i + 1] = fileList [i];
			}
				
			if (maxSaveFiles <= numSaveFiles) {
				newList [FILENAME_OFFSET + maxSaveFiles] = "";
				for (int i = maxSaveFiles; i <= numSaveFiles; i++) {
					//
					// This will delete all files which are greater than the maxSavefile
					// useful in case the user reduces the number after it has started
					//

					string f = path + "/" + fileList [FILENAME_OFFSET + i - 1] + ".sfs";
					if (File.Exists (f)) { 
						File.Delete (f);
					}
                    f = path + "/" + fileList[FILENAME_OFFSET + i - 1] + ".loadmeta";
                    if (File.Exists(f))
                    {
                        File.Delete(f);
                    }

                }
                numSaveFiles = maxSaveFiles;

			} else {
				if (newFile != "") {
					numSaveFiles++;
				}
			}

			if (newFile != "") {

				newList [FILENAME_OFFSET] = newFile;
			}
			newList [NUMFILES_OFFSET] = numSaveFiles.ToString ();
			if (cnt > 0) {
				fileSaveCnt = cnt;
			}
			newList [FILESAVECNT_OFFSET] = fileSaveCnt.ToString();

			writeSaveFileList (path, newList);
		}

	}
}