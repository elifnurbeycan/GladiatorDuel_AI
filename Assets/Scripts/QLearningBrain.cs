using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QLearningBrain : MonoBehaviour
{
    [Header("General Settings")]
    public float learningRate = 0.5f; 
    public float discount = 0.9f;
    public float exploration = 0.3f;  

    [Header("File Settings")]
    public string saveFileName = "RLBrain.json";
    
    // DOSYA İŞLEMLERİNİ AÇ/KAPA
    public bool enableFileIO = true; 

    private List<float> currentInputs = new();
    private List<ActionDefinition> actions = new();
    private Dictionary<string, float[]> Q = new();
    
    private string GetSavePath() 
    {
        return Path.Combine(Application.dataPath, saveFileName);
    }

    [Serializable]
    public class ActionDefinition
    {
        public string actionName;
        public Action<object[]> method;
        public int parameterCount;

        public ActionDefinition(string name, Action<object[]> func, int paramCount)
        {
            actionName = name;
            method = func;
            parameterCount = paramCount;
        }
    }

    [Serializable]
    private class SaveModel
    {
        public int inputCount;
        public int actionCount;
        public List<string> keys = new List<string>();
        public List<FloatArrayWrapper> values = new List<FloatArrayWrapper>();
    }

    [Serializable]
    public class FloatArrayWrapper
    {
        public float[] array;
    }

    void Awake()
    {
        // Eğer dosya işlemleri açıksa yükle, yoksa boş başlat
        if (enableFileIO) LoadOrCreateModel();
        else Q = new Dictionary<string, float[]>();
    }

    // --- PUBLIC API ---

    public void SetInputs(List<float> inputs)
    {
        currentInputs = inputs;
    }

    public void RegisterAction(string name, Action<object[]> method, int parameterCount)
    {
        actions.Add(new ActionDefinition(name, method, parameterCount));
    }

    // MOD DEĞİŞTİRME FONKSİYONU
    public void ConfigureBrain(bool useFile, string filename)
    {
        enableFileIO = useFile;

        if (useFile)
        {
            // AKILLI MOD: Dosya ismini ayarla ve YÜKLE
            saveFileName = filename;
            LoadOrCreateModel();
            Debug.Log($"RLBrain: Dosya Modu AÇIK -> {filename} yüklendi.");
        }
        else
        {
            // RASTGELE MOD: Hafızayı sil ve dosya işlemlerini UNUT
            Q.Clear();
            Debug.Log("RLBrain: Dosya Modu KAPALI -> Tamamen Rastgele.");
        }
    }

    public int DecideAction()
    {
        string state = EncodeState(currentInputs);
        EnsureStateExists(state);

        if (UnityEngine.Random.value < exploration)
        {
            return UnityEngine.Random.Range(0, actions.Count);
        }

        float[] qRow = Q[state];
        
        // Eğer hafıza boşsa (veya hepsi 0 ise) rastgele seç
        bool allZero = true;
        for(int i=0; i<qRow.Length; i++) { if(qRow[i] != 0) { allZero = false; break; } }
        
        if(allZero) return UnityEngine.Random.Range(0, actions.Count);

        int bestIndex = 0;
        float bestVal = qRow[0];

        for (int i = 1; i < qRow.Length; i++)
        {
            if (qRow[i] > bestVal)
            {
                bestVal = qRow[i];
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    public void ExecuteAction(int actionIndex, params object[] parameters)
    {
        if (actionIndex >= 0 && actionIndex < actions.Count)
        {
            actions[actionIndex].method.Invoke(parameters);
        }
    }

    public void Reward(float value) => ApplyReward(value);
    public void Punish(float value) => ApplyReward(-Mathf.Abs(value));

    // --- INTERNAL LOGIC ---

    private void ApplyReward(float reward)
    {
        // Rastgele moddaysak ve öğrenme kapalıysa işlem yapma (Opsiyonel)
        
        string oldState = EncodeState(currentInputs);
        EnsureStateExists(oldState);

        float[] qRow = Q[oldState];
        int bestAction = GetBestActionIndex(qRow);
        
        qRow[bestAction] += learningRate * (reward - qRow[bestAction]);
    }

    private int GetBestActionIndex(float[] qRow)
    {
        int bestIndex = 0;
        float bestVal = qRow[0];
        for (int i = 1; i < qRow.Length; i++)
        {
            if (qRow[i] > bestVal) { bestVal = qRow[i]; bestIndex = i; }
        }
        return bestIndex;
    }

    private void EnsureStateExists(string state)
    {
        if (Q == null) Q = new Dictionary<string, float[]>(); 

        if (!Q.ContainsKey(state))
        {
            Q[state] = new float[actions.Count];
        }
    }

    private string EncodeState(List<float> inputs)
    {
        return string.Join("_", inputs);
    }

    // --- SERIALIZATION ---

    private void LoadOrCreateModel()
    {
        if (!enableFileIO) return; // Dosya kapalıysa yükleme yapma

        string path = GetSavePath(); 
        if (!File.Exists(path))
        {
            CreateNewModel();
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            SaveModel model = JsonUtility.FromJson<SaveModel>(json);
            if (model == null || model.keys == null) { CreateNewModel(); return; }

            Q = new Dictionary<string, float[]>();
            for (int i = 0; i < model.keys.Count; i++)
            {
                Q[model.keys[i]] = model.values[i].array;
            }
        }
        catch (Exception) { CreateNewModel(); }
    }

    private void CreateNewModel()
    {
        Q = new Dictionary<string, float[]>();
        if (enableFileIO) SaveState(); 
    }

    public void SaveState()
    {
        // Dosya işlemleri kapalıysa KAYDETME
        if (!enableFileIO) return;

        SaveModel model = new SaveModel();
        model.inputCount = (currentInputs != null) ? currentInputs.Count : 5; 
        model.actionCount = actions.Count;
        
        foreach (var kvp in Q)
        {
            model.keys.Add(kvp.Key);
            model.values.Add(new FloatArrayWrapper { array = kvp.Value });
        }

        string json = JsonUtility.ToJson(model, true);
        File.WriteAllText(GetSavePath(), json);
    }
}