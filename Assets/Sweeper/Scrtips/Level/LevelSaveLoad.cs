using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Level
{
    public class LevelSaveLoad : SingletonBase<LevelSaveLoad>
    {
        private List<LevelObjectSaveData> _levelObjectDatas = new List<LevelObjectSaveData>();

        public const string FolderName = "LevelData";
        public string FileName = "Level";

        #region Main Methods

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Save("0.dat");
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load("0.dat");
            }
        }

        public void Save(string levelName)
        {
            ClearLocalSaveDatas();
            string savePath = GetFullPath(levelName);

            LevelSaveData toSave = new LevelSaveData();

            {
                LevelObject[] levelObjects = GameObject.FindObjectsOfType<LevelObject>();
                if (levelObjects.Length > 0)
                {
                    foreach (var l in levelObjects)
                    {
                        _levelObjectDatas.Add(l.ToSaveData());
                    }
                }
            }

            toSave._levelObjectDatas = _levelObjectDatas;

            //Actual Save
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, toSave);
            stream.Close();
        }

        public bool Load(string levelName)
        {
            ClearLocalSaveDatas();

            string loadPath = GetFullPath(levelName);
            if (!File.Exists(loadPath))
            {
                //File doesn't exists
                return false;
            }
            else
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(loadPath, FileMode.Open, FileAccess.Read);

                LevelSaveData toLoad = formatter.Deserialize(stream) as LevelSaveData;
                stream.Close();
                CreateObjectsFromSaveData(toLoad);
                return true;
            }
        }

        private void CreateObjectsFromSaveData(LevelSaveData data)
        {
            foreach (var l in data._levelObjectDatas)
            {
                GameObject go = Instantiate(LevelCreator.Instance.InstallObjects[l.PrefabIndex]);
                LevelCreator.Instance.InstallObjectAtBoardPosition(l.BoardX, l.BoardY, l.BoardZ, l.PrefabIndex, l.InstalledSide);
            }
        }

        #endregion

        #region  Utils
        private void ClearLocalSaveDatas()
        {
            _levelObjectDatas.Clear();
        }

        private string GetFullPath(string levelName)
        {
            string saveLocation = Application.persistentDataPath + "/" + FolderName + "/";
            if (!Directory.Exists(saveLocation))
            {
                Directory.CreateDirectory(saveLocation);
            }
            return saveLocation + levelName;
        }
        #endregion

        [System.Serializable]
        private class LevelSaveData
        {
            public List<LevelObjectSaveData> _levelObjectDatas;
        }

    }
}
